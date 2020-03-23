using APBD3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        static MockDbService() {
            _students = new List<Student>
            {
                new Student(1, "Andrzej", "Karakan", "4636"),
                new Student(2, "Maciej", "Luchon", "342"),
                new Student(3, "Arnold", "Kawowski", "9978")
            };
        }
           

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}
