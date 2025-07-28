using UserManagement.Dtos;
using UserManagement.Entities;

namespace UserManagement.Services;

public interface IUserService
{ 
    Task<UserResponse> Create(UserCreateParams user);
}

public class UserService(AppDbContext dbContext) : IUserService
{
    public async Task<UserResponse> Create(UserCreateParams dto)
    {
        UserEntity user = new()
        {
            Id = Guid.CreateVersion7(),
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Birthdate = dto.Birthdate,
            IsMarried = dto.IsMarried
        };
        var entity = dbContext.Users.Add(user).Entity;
        await dbContext.SaveChangesAsync();

        int? age = null;
        if (entity.Birthdate != null)
        {
            age = DateTime.UtcNow.Year - entity.Birthdate.Value.Year;
        }
        
        return  new UserResponse
        {
            FullName = entity.FullName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            Birthdate = entity.Birthdate,
            IsMarried = entity.IsMarried,
            Age = age
        };
    }
}