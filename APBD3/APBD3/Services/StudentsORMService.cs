using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using APBD3.Entities;
using APBD3.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD3.Services
{
    public class StudentsORMService : IStudentsDbService
    {

        private readonly StudentsContext _studentsContext;
        public StudentsORMService(StudentsContext studentsContext)
        {
            _studentsContext = studentsContext;
        }

        //CRUD

        public List<StudentResponse> GetStudents()
        {
            var students = _studentsContext.Student
                .Include(e => e.IdEnrollmentNavigation)
                .ThenInclude(e => e.IdStudyNavigation)
                .Select(e => new StudentResponse()
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    BirthDate = e.BirthDate,
                    Studies = e.IdEnrollmentNavigation.IdStudyNavigation.Name,
                    Semester = e.IdEnrollmentNavigation.Semester,
                    IndexNumber = e.IndexNumber
                })
                .ToList();
            return students;
        }

        public bool AddStudent(Entities.Student student)
        {
            try
            {
                _studentsContext.Student.Add(student);
                _studentsContext.SaveChanges();
            }
            catch (Exception exc)
            {
                return false;
            }

            return true;
        }

        public bool RemoveStudent(string id)
        {
            try
            {
                var student = _studentsContext.Student.Where(s => s.IndexNumber == id)
                .First();
                _studentsContext.Remove(student);
                _studentsContext.SaveChanges();
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public Student UpdateStudent(Student student)
        {
            try
            {
                var selectedStudent = _studentsContext.Student.Where(s => s.IndexNumber == student.IndexNumber)
                .First();
                selectedStudent.FirstName = student.FirstName;
                selectedStudent.LastName = student.LastName;
                selectedStudent.IdEnrollment = student.IdEnrollment;
                selectedStudent.BirthDate = student.BirthDate;
                selectedStudent.Password = student.Password;
                _studentsContext.SaveChanges();
                return selectedStudent;
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        //ENROLLING STUDENTS


        public Models.Enrollment EnrollStudent(StudentEnrollmentRequest request)
        {

            var studiesId = getStudiesId(request.Studies);
            if (studiesId == -1)
            {
                Console.WriteLine("No student found!");
                return null;
            }

            var enrollmentId = getEnrollmentId(studiesId);

            if (isIndexNumberOccupied(request.IndexNumber))
            {
                Console.WriteLine("Index Number Occupied!");
                return null;
            }

            var result = insertStudent(request, enrollmentId);
            if (result == false)
            {
                Console.WriteLine("Error parsing the date!");
                return null;
            }
            var enrollment = new Models.Enrollment();
            enrollment.Semester = 1;
            enrollment.Studies = request.Studies;
            return enrollment;
        }

        private bool insertStudent(StudentEnrollmentRequest request, int idEnrollment)
        {
            DateTime date;
            try
            {
                date = DateTime.Parse(request.BirthDate);
            } catch (Exception exc)
            {
                return false;
            }

            var student = new Student()
            {
                IndexNumber = request.IndexNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = date,
                IdEnrollment = idEnrollment
            };
            _studentsContext.Add(student);
            _studentsContext.SaveChanges();
            return true;
        }


        private bool isIndexNumberOccupied(string index)
        {
            return _studentsContext.Student.Where(s => s.IndexNumber == index).Any();
        }

        private int getEnrollmentId(int studiesId)
        {
            try
            {
                var id = _studentsContext.Enrollment
                .Where(e => e.IdStudy == studiesId && e.Semester == 1)
                .OrderByDescending(e => e.StartDate)
                .Select(e => e.IdEnrollment)
                .First();
                return id;
            }
            catch (Exception exc)
            {
                return -1;
            }
        }

        private int getStudiesId(string name)
        {
            try
            {
                var id = _studentsContext.Studies.Where(s => s.Name == name)
                .Select(s => s.IdStudy)
                .First();
                return id;
            } catch (Exception exc)
            {
                return -1;
            }
        }

        //PROMOTING STUDENTS
        public Models.Enrollment PromoteStudents(PromotionRequest request)
        {
            var idEnrollment = getEnrollmentIdByStudiesAndSemester(request.Studies, request.Semester);
            if (idEnrollment == -1)
            {
                return null;
            }
            var newSemester = request.Semester + 1;

            var idEnrollmentNew = getEnrollmentIdByStudiesAndSemester(request.Studies, newSemester);
            if (idEnrollmentNew == -1)
            {
                idEnrollmentNew = _studentsContext.Enrollment.Max(e => e.IdEnrollment) + 1;
                var idStudies = _studentsContext.Studies.Where(s => s.Name == request.Studies).Select(s => s.IdStudy).First();
                var enrollment = new Entities.Enrollment()
                {
                    IdEnrollment = idEnrollmentNew,
                    Semester = newSemester,
                    IdStudy = idStudies,
                    StartDate = new DateTime()
                };
                _studentsContext.Add(enrollment);
            }

            var toUpdate = _studentsContext.Student.Where(s => s.IdEnrollment == idEnrollment).ToList();
            toUpdate.ForEach(s => s.IdEnrollment = idEnrollmentNew);
            _studentsContext.SaveChanges();

            return new Models.Enrollment()
            {
                Studies = request.Studies,
                Semester = request.Semester + 1
            };
        }

        public int getEnrollmentIdByStudiesAndSemester(string studies, int semester)
        {
            try
            {
                return _studentsContext.Enrollment.Include(e => e.IdStudyNavigation)
                    .Where(e => e.Semester == semester && e.IdStudyNavigation.Name == studies)
                    .Select(e => e.IdEnrollment)
                    .First();
            } catch (Exception exc)
            {
                return -1;
            }
            
        }


        //Trash from other tasks
        public bool canLogIn(LoginModel model)
        {
            return false;
        }

        public bool ExistsStudent(string index)
        {
            return false;
        }
    }
}
