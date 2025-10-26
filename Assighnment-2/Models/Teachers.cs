using System.ComponentModel.DataAnnotations;

namespace web_assignment_2.Models;

/// <summary>
/// Represents an instructor who teaches one or more courses
/// </summary>
public class Teachers
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    public DateTime HireDate { get; set; }

    // Navigation property: One teacher can teach many courses
    public ICollection<Courses> Courses { get; set; } = new List<Courses>();
}
