using System.ComponentModel.DataAnnotations;

namespace UserManagement.Dtos;

public class UserCreateParams
{    
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public required string FullName { get; set; }
    
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MaxLength(12)]
    [MinLength(10)]
    public required string PhoneNumber { get; set; }
    
    public DateTime? Birthdate { get; set; }

    public bool IsMarried { get; set; } = false;
}

public class UserResponse
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public DateTime? Birthdate { get; set; }
    public bool IsMarried { get; set; } = false;  
    public int? Age { get; set; }
}