using CoreMysql;
using EFMigrate.Models;
using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMigrate
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class StudentSystemContext : DbContext
    {
        const string connStr = "";
        public StudentSystemContext()
            :base(connStr)
        {

        }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<ClassGrade> ClassGrades { get; set; }
        public DbSet<Student> Students { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            DbInterception.Add(new CommandInterceptor());
            Database.SetInitializer<StudentSystemContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}
