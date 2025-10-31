using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_assignment_2.Models;

/// <summary>
/// Represents a course created and taught by a teacher
/// </summary>
public class Courses
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Course title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    // Foreign key to Teacher
    [Required]
    public int TeacherId { get; set; }

    // Navigation property: Many courses belong to one teacher
    [ForeignKey(nameof(TeacherId))]
    public Teachers Teacher { get; set; } = null!;

    // Navigation property: One course can have many enrollments
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
