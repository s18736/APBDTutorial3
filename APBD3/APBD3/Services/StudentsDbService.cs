using APBD3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace APBD3.Services
{
    public class StudentsDbService : IStudentsDbService
    {
        private static readonly string _databaseString = "Data Source=db-mssql;Initial Catalog=s18736;Integrated Security=True";
        //adding student

        public Enrollment AddStudent(StudentEnrollmentRequest request)
        {
            
            using var connection = new SqlConnection(_databaseString);
            connection.Open();
            var transaction = connection.BeginTransaction();

            var studiesId = getStudiesId(request.Studies, connection, transaction);
            if (studiesId == -1)
            {
                return null;
            }

            var enrollmentId = getEnrollmentId(studiesId, connection, transaction);

            if (isIndexNumberOccupied(connection, transaction, request.IndexNumber))
            {
                return null;
            }

            insertStudent(connection, transaction, request, enrollmentId);
            var enrollment = new Enrollment();
            enrollment.Semester = 1;
            enrollment.Studies = request.Studies;
            return enrollment;
        }

        private void insertStudent(SqlConnection connection, SqlTransaction transaction, StudentEnrollmentRequest request, int idEnrollment)
        {
            using (var command = new SqlCommand())
            {
                bindCommand(connection, command, transaction);
                command.CommandText =
                    "INSERT INTO Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) " +
                    "VALUES ( @IndexNumber , @FirstName , @LastName , @BirthDate , @IdEnrollment )";
                command.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                command.Parameters.AddWithValue("FirstName", request.FirstName);
                command.Parameters.AddWithValue("LastName", request.LastName);
                command.Parameters.AddWithValue("BirthDate", request.BirthDate);
                command.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        
        private int getEnrollmentId(int studiesId, SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand())
            {
                bindCommand(connection, command, transaction);
                command.CommandText =
                "SELECT IdEnrollment " +
                "FROM Enrollment " +
                "WHERE Enrollment.IdStudy = @Id " +
                "AND Enrollment.Semester = 1 " +
                "ORDER BY Enrollment.StartDate DESC;";
                command.Parameters.AddWithValue("Id", studiesId);
                var reader = command.ExecuteReader();
                int id;
                if (!reader.Read())
                {
                    reader.Close();
                    id = createEnrollment(studiesId, connection, transaction);
                }
                else
                {
                    id = int.Parse(reader["IdEnrollment"].ToString());
                    reader.Close();
                }
                return id;
            }
            
        }

        private bool isIndexNumberOccupied(SqlConnection connection, SqlTransaction transaction, string index)
        {
            using (var command = new SqlCommand())
            {
                bindCommand(connection, command, transaction);
                command.CommandText = "SELECT COUNT(IndexNumber) FROM Student WHERE IndexNumber = @Index ;" ;
                command.Parameters.AddWithValue("Index", index);
                var reader = command.ExecuteReader();
                reader.Read();
                var id = int.Parse(reader[0].ToString());
                reader.Close();
                return id != 0;
            }
        }

        private int getMaxId(SqlConnection connection, SqlTransaction transaction, string table)
        {
            using (var command = new SqlCommand())
            {
                bindCommand(connection, command, transaction);
                command.CommandText = "SELECT MAX(IdEnrollment) FROM " + table + ";";
                command.Parameters.AddWithValue("TableName", table);
                var reader = command.ExecuteReader();
                reader.Read();
                var str = reader[0].ToString();
                if (str.Equals(""))
                {
                    reader.Close();
                    return 0;
                }
                var id = int.Parse(str);

                reader.Close();
                return id;
            }
        }

        //also returns an id of created enrollmnent
        private int createEnrollment(int studiesId, SqlConnection connection, SqlTransaction transaction)
        {
            var id = getMaxId(connection, transaction, "Enrollment") + 1;
            using (var command = new SqlCommand())
            {
                bindCommand(connection, command, transaction);
                command.CommandText = "INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) " +
                    "VALUES ( @Id , 1 , @IdStudy , SYSDATETIME())";
                command.Parameters.AddWithValue("Id", id);
                command.Parameters.AddWithValue("IdStudy", studiesId);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            return id;
        }

        private int getStudiesId(string name, SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand())
            {
                bindCommand(connection, command, transaction);
                command.CommandText = "SELECT IdStudy FROM STUDIES WHERE Studies.Name = @Name";
                command.Parameters.AddWithValue("Name", name);


                var reader = command.ExecuteReader();

                //if nothing has been found
                if (!reader.Read())
                {
                    return -1;
                }
               
                var id = int.Parse(reader["IdStudy"].ToString());
                reader.Close();
                return id;
            }
            
        }

        //for code encapsulation
        private void bindCommand(SqlConnection connection, SqlCommand command, SqlTransaction transaction)
        {
            command.Connection = connection;
            command.Transaction = transaction;
        }


        //PROMOTING STUDENTS


        public Enrollment PromoteStudents(PromotionRequest request)
        {
            using (var connection = new SqlConnection(_databaseString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var idEnrollment = getEnrollmentIdBySemester(request, connection, transaction);
                if (idEnrollment == -1)
                {
                    return null;
                }
                promote(connection, transaction, request.Studies, request.Semester);
                Enrollment enrollment = new Enrollment();
                enrollment.Studies = request.Studies;
                enrollment.Semester = request.Semester + 1;
                return enrollment;
            }
                
        }

        private int getEnrollmentIdBySemester(PromotionRequest request, SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand())
            {
                bindCommand(connection, command, transaction);
                command.CommandText = "SELECT IdEnrollment FROM Enrollment " +
                                      "JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy " +
                                      "WHERE Enrollment.Semester = @Semester " +
                                      "AND Studies.Name = @StudiesName ;";
                command.Parameters.AddWithValue("Semester", request.Semester);
                command.Parameters.AddWithValue("StudiesName", request.Studies);
                var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    reader.Close();
                    return -1;
                }
                var id = int.Parse(reader[0].ToString());
                reader.Close();
                return id;
            }
                
            
        }

        private void promote(SqlConnection connection, SqlTransaction transaction, string studies, int semester)
        {
            using (var command = new SqlCommand("promoteStudents", connection)
            { CommandType = CommandType.StoredProcedure })
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("@Studies", studies);
                command.Parameters.AddWithValue("@Semester", semester);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public bool ExistsStudent(string index)
        {
            using (var connection = new SqlConnection(_databaseString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                bindCommand(connection, command, transaction);
                command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = @Index";
                command.Parameters.AddWithValue("Index", index);
                var reader = command.ExecuteReader();
                var exists = reader.Read();
                reader.Close();
                return exists;
            }
        }
    }
}
