using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD3.DAL;
using APBD3.Entities;
using APBD3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APBD3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;
        

       public StudentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;
        }

        

        [HttpGet]
        public IActionResult getStudents()
        {
            var students = _dbService.GetStudents();
            return Ok(students);
        }

 

        [HttpPost]
        public IActionResult addStudent(Student student)
        {
            bool result = _dbService.AddStudent(student);
            if (result == true)
            {
                return Ok("Added!");
            }
            return BadRequest("Something went wrong!");
        }

        [HttpDelete("{id}")]
        public IActionResult deleteStudent(string id)
        {
            var result = _dbService.RemoveStudent(id);
            if (result == true)
            {
                return Ok("Deleted!");
            }
            return BadRequest("Something went wrong! Probably you chose wrong id!");
        }

        [HttpPut()]
        public IActionResult updateStudent(Student student)
        {
            var updated = _dbService.UpdateStudent(student);
            if (updated == null)
            {
                return BadRequest("Something went wrong! Probably you chose wrong id!");
            }
            return Ok(updated);
        }
    }
}