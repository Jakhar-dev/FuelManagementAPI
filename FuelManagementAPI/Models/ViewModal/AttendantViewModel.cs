using System.ComponentModel.DataAnnotations;

public class AttendantViewModel
{
    [Required]
    public string AttendantName { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    public string Address { get; set; }

    [Required]
    public DateTime JoiningDate { get; set; }

    public DateTime? TerminationDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Photo { get; set; } // Base64 or URL

    public string? Description { get; set; }
}
