using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Entities;

[Table("Users")]
public class UserEntity
{
    [Key]
    public Guid Id { get; set; }
    
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