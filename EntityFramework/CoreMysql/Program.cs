using EFMigrate;
using EFMigrate.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMysql
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (StudentSystemContext studentContext = new StudentSystemContext())
            {
                studentContext.Database.Log = Console.Write;

                //Grade grade = new Grade
                //{
                //    Name = "一年级"
                //};
                //studentContext.Grades.Add(grade);
                //ClassGrade classGrade = new ClassGrade 
                //{ 
                //    Name = "一年级一班",
                //    GradeId = 1
                //};
                //studentContext.ClassGrades.Add(classGrade);
                //Student student = new Student
                //{
                //    Name = "小hi",
                //    ClassId = 1,
                //    GradeId = 1,
                //    Age = 8,
                //    Sex = 0
                //};
                //studentContext.Students.Add(student);
                //studentContext.SaveChanges();

                //多表关联
                var studentInfo = studentContext.Students
                    .Join(studentContext.ClassGrades, s => s.GradeId, cg => cg.GradeId, (s, cg) => new { s.Id, s.Name, s.ClassId, GradeClassName = cg.Name })
                    .Join(studentContext.Grades, s => s.ClassId, g => g.Id, (s, g) => new { s.Id, s.Name, s.GradeClassName, GradeName = g.Name })
                    .FirstOrDefault();
                Console.WriteLine($"{studentInfo.Id}  {studentInfo.Name}  {studentInfo.GradeClassName} {studentInfo.GradeName}");

                //多表关联排序
                var studentInfo2 = studentContext.Students
                     .Join(studentContext.ClassGrades, s => s.GradeId, cg => cg.GradeId, (s, cg) => new { s.Id, s.Name, s.ClassId, GradeClassName = cg.Name })
                     .Join(studentContext.Grades, s => s.ClassId, g => g.Id, (s, g) => new { s.Id, s.Name, s.ClassId, s.GradeClassName, GradeName = g.Name })
                     .OrderBy(i => i.ClassId).ThenBy(i => i.Id)
                     .FirstOrDefault();
                Console.WriteLine($"{studentInfo2.Id}  {studentInfo2.Name}  {studentInfo2.GradeClassName} {studentInfo2.GradeName}");


                //group复杂分组
                var studentInfo3 = studentContext.Students
                .Join(studentContext.ClassGrades, s => s.GradeId, cg => cg.GradeId, (s, cg) => new { s.Id, s.Name, s.ClassId, GradeClassName = cg.Name })
                .Join(studentContext.Grades, s => s.ClassId, g => g.Id, (s, g) => new { s.Id, s.Name, s.ClassId, s.GradeClassName, GradeName = g.Name })
                .GroupBy(i => new { i.ClassId, i.GradeClassName })
                .Where(i => i.Key.ClassId > 0);
                foreach (var item in studentInfo3)
                {
                    Console.WriteLine($"{item.Key.GradeClassName}");
                    foreach (var student in item)
                    {
                        Console.WriteLine($"     {student.Id}  {student.Name}");
                    }
                }

                //简单group
                var studentInfo4 = studentContext.Students
                .GroupBy(i => i.ClassId)
                .Where(i => i.Key > 0);
                foreach (var item in studentInfo4)
                {
                    Console.WriteLine($"{item.Key}");
                    foreach (var student in item)
                    {
                        Console.WriteLine($"     {student.Id}  {student.Name}");
                    }
                }

                //自定义查询
                string sql = "select Id,Name from Student where Deleted=0 group by Id,Name";
                var groupResult = studentContext.Database.SqlQuery<StudentGroupDo>(sql);
                foreach (var item in groupResult)
                {
                    Console.WriteLine($"{item.Id}  {item.Name}");
                }

                var count = studentContext.Students.Count(i => i.Sex == 1);
                Console.WriteLine("男同学人数:{0}", count);

                var sum = studentContext.Students.Sum(i => i.Id);
                Console.WriteLine("同学Id总和:{0}", sum);

                var avg = studentContext.Students.Where(i => !i.Deleted).Average(i => i.Age);
                Console.WriteLine("同学的平均年龄:{0:0.0}", avg);

                //in
                int[] studentIds = new[] { 1, 2 };
                var students = studentContext.Students.Where(i => studentIds.Contains(i.Id))
                    .Select(i => new { i.Id, i.Name });
                foreach (var student in students)
                {
                    Console.WriteLine($"{student.Id}  {student.Name}");
                }

                //in
                var students2 = studentContext.Students
                    .Where(i => studentContext.ClassGrades.Select(cg => cg.Id).Contains(i.ClassId))
                    .Select(i => new { i.Id, i.Name });
                foreach (var student in students2)
                {
                    Console.WriteLine($"{student.Id}  {student.Name}");
                }
            }
            Console.ReadLine();
        }
    }
}
