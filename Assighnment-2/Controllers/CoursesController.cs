using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_assignment_2.Data;
using web_assignment_2.Models;

namespace web_assignment_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Courses>>> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Courses>> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound(new { message = $"Course with ID {id} not found." });

            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<Courses>> PostCourse(Courses course)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teacherExists = await _context.Teachers.AnyAsync(t => t.Id == course.TeacherId);
            if (!teacherExists)
                return BadRequest(new { message = $"Teacher with ID {course.TeacherId} does not exist." });

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutCourse(int id, Courses course)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != course.Id)
                return BadRequest(new { message = "ID mismatch." });

            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null)
                return NotFound(new { message = $"Course with ID {id} not found." });

            var teacherExists = await _context.Teachers.AnyAsync(t => t.Id == course.TeacherId);
            if (!teacherExists)
                return BadRequest(new { message = $"Teacher with ID {course.TeacherId} does not exist." });

            existingCourse.Title = course.Title;
            existingCourse.Description = course.Description;
            existingCourse.StartDate = course.StartDate;
            existingCourse.TeacherId = course.TeacherId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                    return NotFound(new { message = $"Course with ID {id} not found." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound(new { message = $"Course with ID {id} not found." });

            if (course.Enrollments.Any())
                return BadRequest(new { message = "Cannot delete a course that still has enrollments. Please remove enrollments first." });

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(c => c.Id == id);
        }
    }
}