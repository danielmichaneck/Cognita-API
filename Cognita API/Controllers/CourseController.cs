using Cognita.API.Service.Contracts;
using Cognita_Shared.Dtos.Course;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

        // GET: api/Courses/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get all courses",
            Description = "Get all available courses",
            OperationId = "GetAllCourses"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Courses found", Type = typeof(CourseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No courses found")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            var courses = await _serviceManager.CourseService.GetCoursesAsync();

            if (courses == null)
            {
                return NotFound();
            }

            return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get a course by id",
            Description = "Get a course by id",
            OperationId = "GetCourseById"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "The course was found", Type = typeof(CourseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The course was not found")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await _serviceManager.CourseService.GetSingleCourseAsync(id);

            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Edit a course by id",
            Description = "Edit a course by id",
            OperationId = "EditCourseById"
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Course edited successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The course was not found")]
        public async Task<IActionResult> PutCourse(int id, CourseForUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (await _serviceManager.CourseService.EditCourseAsync(id, dto))
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Create a new course",
            Description = "Create a new course",
            OperationId = "CreateMovie"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Course created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
        public async Task<ActionResult<CourseDto>> PostCourse(CourseForCreationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            CourseDto courseDTO = await _serviceManager.CourseService.CreateCourseAsync(dto);
            return CreatedAtAction(nameof(PostCourse), new { id = courseDTO.CourseId }, courseDTO);
        }
    }
}
