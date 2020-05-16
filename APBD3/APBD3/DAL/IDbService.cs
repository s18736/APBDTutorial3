using APBD3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD3.DAL
{
    public interface IDbService
    {
        IEnumerable<StudentResponse> GetStudents();
        StudentResponse GetStudent(string id);
    }
}
