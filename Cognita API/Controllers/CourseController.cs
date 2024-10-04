using Cognita.API.Service.Contracts;
using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Dtos.Document;
using Cognita_Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Cognita_API.Controllers
{
    [Authorize]
    [Route("api/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CourseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/Courses
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get all applicable courses",
            Description = "Get all applicable courses",
            OperationId = "GetAllCourses"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Courses found", Type = typeof(CourseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No courses found")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            // HttpContext is given to the API when the call is made.
            if (HttpContext.User.Identity is ClaimsIdentity identity) {

                var user = await ControllerUtil.GetUserWithRole(identity);

                if (user is not null) {

                    var courses = (user.Role == UserRole.Teacher) ?
                        await _serviceManager.CourseService.GetCoursesAsync() :
                        await _serviceManager.AuthService.GetCoursesForUserAsync(user.Id);

                    if (courses == null)
                    {
                        return NotFound("No courses were found.");
                    }

                    return Ok(courses);
                }
            }

            return NotFound("The user was not found.");
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
        public async Task<ActionResult<CourseWithDetailsDto>> GetCourse(int id)
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        // POST: api/Courses/id/documents
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost("{id}/documents")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Adds a document to the course",
            Description = "Add a new document",
            OperationId = "AddDocument"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Document added successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
        public async Task<ActionResult> AddDocumentToCourse(int id, IFormFile file)
        {
            // HttpContext is given to the API when the call is made.
            if (HttpContext.User.Identity is ClaimsIdentity identity) {
                var user = await ControllerUtil.GetUserWithRole(identity);

                var documentDto = await ControllerUtil.AddDocument(file);

                if (documentDto is null) return BadRequest("There was no file or the file was in the wrong format (pdf).");

                var response = _serviceManager.AuthService.AddDocumentToUser(id, documentDto);
            }

            return NotFound("The user was not found.");
        }
    }
}
