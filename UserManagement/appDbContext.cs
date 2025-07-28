using Microsoft.EntityFrameworkCore;
using UserManagement.Entities;

namespace UserManagement;

public class AppDbContext ( DbContextOptions options): DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; } = null!;
}