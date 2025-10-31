using System.ComponentModel.DataAnnotations;

namespace web_assignment_2.Models;

/// <summary>
/// Represents an instructor who teaches one or more courses
/// </summary>
public class Teachers
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hire date is required")]
    public DateTime HireDate { get; set; }

    // Navigation property: One teacher can teach many courses
    public ICollection<Courses> Courses { get; set; } = new List<Courses>();
}
