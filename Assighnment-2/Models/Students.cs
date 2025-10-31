using System.ComponentModel.DataAnnotations;

namespace web_assignment_2.Models;

/// <summary>
/// Represents a learner enrolled in one or more courses
/// </summary>
public class Students
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public DateTime BirthDate { get; set; }

    public bool IsActive { get; set; }

    // Navigation property: One student can have many enrollments
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
