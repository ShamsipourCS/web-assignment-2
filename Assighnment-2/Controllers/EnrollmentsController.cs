using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_assignment_2.Data;
using web_assignment_2.Models;

namespace web_assignment_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly SchoolContext _context;

        public EnrollmentsController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ThenInclude(c => c.Teacher)
                .ToListAsync();

            return Ok(enrollments);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ThenInclude(c => c.Teacher)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
                return NotFound(new { message = $"Enrollment with ID {id} not found." });

            return Ok(enrollment);
        }

        [HttpGet("student/{studentId:int}")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollmentsByStudent(int studentId)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
            if (!studentExists)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Teacher)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();

            return Ok(enrollments);
        }

        [HttpGet("course/{courseId:int}")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollmentsByCourse(int courseId)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
                return NotFound(new { message = $"Course with ID {courseId} not found." });

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            return Ok(enrollments);
        }

        [HttpPost]
        public async Task<ActionResult<Enrollment>> PostEnrollment(Enrollment enrollment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentExists = await _context.Students.AnyAsync(s => s.Id == enrollment.StudentId);
            if (!studentExists)
                return BadRequest(new { message = $"Student with ID {enrollment.StudentId} does not exist." });

            var courseExists = await _context.Courses.AnyAsync(c => c.Id == enrollment.CourseId);
            if (!courseExists)
                return BadRequest(new { message = $"Course with ID {enrollment.CourseId} does not exist." });

            var existingEnrollment = await _context.Enrollments
                .AnyAsync(e => e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId);
            if (existingEnrollment)
                return Conflict(new { message = "This student is already enrolled in this course." });

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollment);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutEnrollment(int id, Enrollment enrollment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != enrollment.Id)
                return BadRequest(new { message = "ID mismatch." });

            var existingEnrollment = await _context.Enrollments.FindAsync(id);
            if (existingEnrollment == null)
                return NotFound(new { message = $"Enrollment with ID {id} not found." });

            var studentExists = await _context.Students.AnyAsync(s => s.Id == enrollment.StudentId);
            if (!studentExists)
                return BadRequest(new { message = $"Student with ID {enrollment.StudentId} does not exist." });

            var courseExists = await _context.Courses.AnyAsync(c => c.Id == enrollment.CourseId);
            if (!courseExists)
                return BadRequest(new { message = $"Course with ID {enrollment.CourseId} does not exist." });

            if (existingEnrollment.StudentId != enrollment.StudentId || existingEnrollment.CourseId != enrollment.CourseId)
            {
                var duplicateEnrollment = await _context.Enrollments
                    .AnyAsync(e => e.Id != id && e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId);
                if (duplicateEnrollment)
                    return Conflict(new { message = "This student is already enrolled in this course." });
            }

            existingEnrollment.StudentId = enrollment.StudentId;
            existingEnrollment.CourseId = enrollment.CourseId;
            existingEnrollment.EnrollDate = enrollment.EnrollDate;
            existingEnrollment.Grade = enrollment.Grade;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentExists(id))
                    return NotFound(new { message = $"Enrollment with ID {id} not found." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound(new { message = $"Enrollment with ID {id} not found." });

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}
