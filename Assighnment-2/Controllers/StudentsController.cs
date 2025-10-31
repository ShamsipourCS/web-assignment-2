using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_assignment_2.Data;
using web_assignment_2.Models;

namespace web_assignment_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Students>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Students>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound(new { message = $"Student with ID {id} not found." });

            return Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<Students>> PostStudent(Students student)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutStudent(int id, Students student)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != student.Id)
                return BadRequest(new { message = "ID mismatch." });

            var existingStudent = await _context.Students.FindAsync(id);
            if (existingStudent == null)
                return NotFound(new { message = $"Student with ID {id} not found." });

            existingStudent.FullName = student.FullName;
            existingStudent.BirthDate = student.BirthDate;
            existingStudent.IsActive = student.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                    return NotFound(new { message = $"Student with ID {id} not found." });
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound(new { message = $"Student with ID {id} not found." });

            if (student.Enrollments.Any())
                return BadRequest(new { message = "Cannot delete a student who still has enrollments. Please remove enrollments first." });

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
