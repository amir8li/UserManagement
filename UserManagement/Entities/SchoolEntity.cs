﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Entities;

[Table("Schools")]
public class SchoolEntity
{  
    [Key] 
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public IEnumerable<ClassEntity>? Classes { get; set; }
}