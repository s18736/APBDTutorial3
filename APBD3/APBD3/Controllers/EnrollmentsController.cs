using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD3.Models;
using APBD3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService _service;

        public EnrollmentsController(IStudentsDbService service)
        {
            _service = service;
        }

        [Route("promotions")]
        [Authorize(Roles = "employee")]
        [HttpPost]
        public ActionResult promoteStudents([FromBody] PromotionRequest request)
        {
            var enrollment = _service.PromoteStudents(request);
            if (enrollment == null)
            {
                return NotFound("Enrollment not found!");
            }
            return Created("localhost", enrollment);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public ActionResult enrollStudent([FromBody] StudentEnrollmentRequest request)
        {
            if (!isRequestValid(request))
            {
                return BadRequest("Request is wrong!");
            }
            var enrollment = _service.AddStudent(request);
            if (enrollment == null)
            {
                return BadRequest("Wrong input data!");
            }
            return Created("localhost", enrollment);
        }

        private bool isRequestValid(StudentEnrollmentRequest request)
        {
            if (request.IndexNumber == null)
            {
                return false;
            }
            if (request.FirstName == null)
            {
                return false;
            }
            if (request.LastName == null)
            {
                return false;
            }
            if (request.BirthDate == null)
            {
                return false;
            }
            if (request.Studies == null)
            {
                return false;
            }
            return true;
        }
    }
}