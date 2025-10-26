using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_assignment_2.Models;

/// <summary>
/// Represents a course created and taught by a teacher
/// </summary>
public class Courses
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

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
