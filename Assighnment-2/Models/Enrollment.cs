using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_assignment_2.Models;

/// <summary>
/// Join entity representing a student enrolled in a course (many-to-many relationship)
/// </summary>
public class Enrollment
{
    public int Id { get; set; }

    public DateTime EnrollDate { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal Grade { get; set; }

    // Foreign key to Student
    [Required]
    public int StudentId { get; set; }

    // Navigation property: Many enrollments belong to one student
    [ForeignKey(nameof(StudentId))]
    public Students Student { get; set; } = null!;

    // Foreign key to Course
    [Required]
    public int CourseId { get; set; }

    // Navigation property: Many enrollments belong to one course
    [ForeignKey(nameof(CourseId))]
    public Courses Course { get; set; } = null!;
}
