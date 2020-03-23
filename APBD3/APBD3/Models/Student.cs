using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD3.Models
{
    public class Student
    {

        public Student(int id, string firstName, string lastName, string indexNumber)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.IndexNumber = indexNumber;
        }

        public Student()
        {

        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
    }
}
