// Copyright © 2013 Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Resources;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Tests;
using MySql.Web.Common;
using MySql.Web.Security;

namespace MySql.Web.Tests
{
  public class WebTestBase : TestBase
  {
    public WebTestBase(TestFixture fixture) : base(fixture, true)
    {
      for (int ver = 1; ver <= SchemaManager.Version; ver++)
        LoadSchema(ver);
    }

    public override void Cleanup()
    {
      executeSQL($"DROP DATABASE `{Connection.Database}`", true);
    }

    internal protected void LoadSchema(int version)
    {
      if (version < 1) return;

      MySQLMembershipProvider provider = new MySQLMembershipProvider();

      ResourceManager r = new ResourceManager("MySql.Web.Properties.Resources", typeof(MySQLMembershipProvider).Assembly);
      string schema = r.GetString(String.Format("schema{0}", version));
      MySqlScript script = new MySqlScript(Connection);
      script.Query = schema;

      try
      {
        script.Execute();
      }
      catch (MySqlException ex)
      {
        if (ex.Number == 1050 && version == 7)
        {
          // Schema7 performs several renames of tables to their lowercase representation. 
          // If the current server OS does not support renaming to lowercase, then let's just continue.          
          script.Query = "UPDATE my_aspnet_schemaversion SET version=7";
          script.Execute();
        }
      }
    }
  }
}

