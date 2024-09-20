using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cognita.API.Service.Contracts;
using Cognita_Shared.Dtos.Course;
using Microsoft.AspNetCore.Mvc;

namespace Cognita_API.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CourseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            var courses = await _serviceManager.CourseService.GetCoursesAsync();
            return Ok(courses);
        }
    }
}
