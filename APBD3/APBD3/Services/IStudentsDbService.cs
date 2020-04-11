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
        Enrollment AddStudent(StudentEnrollmentRequest request);
        Enrollment PromoteStudents(PromotionRequest request);
        bool ExistsStudent(string index);
        //int GetStudiesId(string name, SqlCommand command);
        //int GetEnrollmentId(int studiesId);
    }
}
