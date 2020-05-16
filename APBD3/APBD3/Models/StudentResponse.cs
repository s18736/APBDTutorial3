using System;


namespace APBD3.Models
{
    public class StudentResponse
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Semester { get; set; }
        public DateTime BirthDate { get; set; }
        public string Studies { get; set; }
    }
}
