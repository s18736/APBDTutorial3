using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD3.Models;

namespace APBD3.DAL
{
    public class RealDbService : IDbService
    {
        public IEnumerable<Student> GetStudents()
        {
            List<Student> students = new List<Student>();
            using (var sqlClient = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18736;Integrated Security=True"))
            using (var sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = sqlClient;
                sqlCommand.CommandText = "SELECT S.FirstName, S.LastName, S.BirthDate, E.Semester, ST.Name " +
                                         "FROM STUDENT S " +
                                         "JOIN ENROLLMENT E ON E.IdEnrollment = S.IdEnrollment " +
                                         "JOIN STUDIES ST ON ST.IdStudy = E.IdStudy;";
                sqlClient.Open();

                var reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    var student = new Student();
                    student.FirstName = reader["FirstName"].ToString();
                    student.LastName = reader["LastName"].ToString();
                    student.BirthDate = DateTime.Parse(reader["BirthDate"].ToString());
                    student.Semester = int.Parse(reader["Semester"].ToString());
                    student.Studies = reader["Name"].ToString();
                    students.Add(student);
                }
                Console.Write("aaa");
            }
            return students;
        }

        public Student GetStudent(string id)
        {
            var student = new Student();
            using (var sqlClient = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18736;Integrated Security=True"))
            using (var sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = sqlClient;
                sqlCommand.CommandText = "SELECT S.FirstName, S.LastName, S.BirthDate, E.Semester, ST.Name " +
                                         "FROM STUDENT S " +
                                         "JOIN ENROLLMENT E ON E.IdEnrollment = S.IdEnrollment " +
                                         "JOIN STUDIES ST ON ST.IdStudy = E.IdStudy " + 
                                         "WHERE S.IndexNumber = @Id";
                sqlCommand.Parameters.AddWithValue("id", id);
                sqlClient.Open();

                var reader = sqlCommand.ExecuteReader();

                if (reader.Read() == false)
                {
                    return null;
                }
              
                
                student.FirstName = reader["FirstName"].ToString();
                student.LastName = reader["LastName"].ToString();
                student.BirthDate = DateTime.Parse(reader["BirthDate"].ToString());
                student.Semester = int.Parse(reader["Semester"].ToString());
                student.Studies = reader["Name"].ToString();
            
                
                
            }
            return student;
        }
    }
}
