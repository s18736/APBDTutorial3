using APBD3.Entities;
using APBD3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APBD3.Services
{
    public interface IStudentsDbService
    {
        List<StudentResponse> GetStudents();

        bool AddStudent(Entities.Student student);

        bool RemoveStudent(string id);

        Student UpdateStudent(Student student);
        Models.Enrollment EnrollStudent(StudentEnrollmentRequest request);
        Models.Enrollment PromoteStudents(PromotionRequest request);
        bool ExistsStudent(string index);

        bool canLogIn(LoginModel model);
        //int GetStudiesId(string name, SqlCommand command);
        //int GetEnrollmentId(int studiesId);
    }
}
