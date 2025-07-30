﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authentication;

namespace UserManagement.Entities;

[Table("Classes")]
public class ClassEntity
{  
    [Key] 
    public required Guid Id { get; set; }

    public required string Title { get; set; }
    
    public required string Subject { get; set; }

    public Guid? SchoolId { get; set; }
    
    public SchoolEntity? School { get; set; }

    public IEnumerable<UserEntity> Users { get; set; }
}