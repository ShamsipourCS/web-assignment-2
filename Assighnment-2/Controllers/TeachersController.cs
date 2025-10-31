using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using web_assignment_2.Data;
using web_assignment_2.Models;

namespace web_assignment_2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController:ControllerBase
    {
        private readonly SchoolContext _context;

        public TeachersController(SchoolContext context)
        {
            _context = context;
        }

 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teachers>>> GetAll()
        {
            var teachers = await _context.Teachers
                .Include(t => t.Courses) 
                .ToListAsync();

            return Ok(teachers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Teachers>> GetTeacherById(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
                return NotFound(new { message = $"Teacher with ID {id} not found." });

            return Ok(teacher);
        }
        [HttpPost]
        public async Task<ActionResult<Teachers>> CreateTeacher(Teachers teacher)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (teacher == null)
                return BadRequest(new { message = "Invalid teacher data." });

            var existing = await _context.Teachers.AnyAsync(t => t.Email == teacher.Email);
            if (existing)
                return Conflict(new { message = "A teacher with this email already exists." });

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacherById), new { id = teacher.Id }, teacher);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTeacher(int id, Teachers updatedTeacher)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updatedTeacher.Id)
                return BadRequest(new { message = "ID mismatch." });

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound(new { message = $"Teacher with ID {id} not found." });

            teacher.FullName = updatedTeacher.FullName;
            teacher.Email = updatedTeacher.Email;
            teacher.HireDate = updatedTeacher.HireDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("IX_Teachers_Email") == true)
                    return Conflict(new { message = "Email must be unique." });

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
                return NotFound(new { message = $"Teacher with ID {id} not found." });

            if (teacher.Courses.Any())
                return BadRequest(new { message = "Cannot delete a teacher who still has courses assigned." });

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
