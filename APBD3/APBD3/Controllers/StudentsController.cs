using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD3.DAL;
using APBD3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult getStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult getStudent(int id)
        {
            if (id == 0)
            {
                return Ok("kebab");
            }
            else
            {
                return NotFound("nie ma kebaba");
            }
        }

        [HttpPost]
        public IActionResult addStudent(Student student)
        {
            student.IndexNumber = $"{new Random().Next(1, 2000)}";
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public IActionResult deleteStudent(int id)
        {
            return Ok($"Deleting {id}");
        }

        [HttpPut("{id}")]
        public IActionResult updateStudent(int id)
        {
            return Ok($"Updating {id}");
        }
    }
}