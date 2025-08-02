using Microsoft.EntityFrameworkCore;
using UserManagement.Dtos;
using UserManagement.Entities;

namespace UserManagement.Services;

public interface IUserService
{ 
    Task<UserResponse> Create(UserCreateParams user);
    Task<IEnumerable<UserResponse>> Read();
    Task<UserResponse?> ReadById(Guid id);
    Task<UserResponse?> Update(UserUpdateParams param);
    Task Delete(Guid id);   
}

public class UserService(AppDbContext dbContext) : IUserService
{
    public async Task<UserResponse> Create(UserCreateParams dto)
    {
        List<ClassEntity> classList = [];
        foreach (var clssId in dto.Classes)
        {
            ClassEntity? classEntity = await dbContext.Class.FindAsync(clssId);
            classList.Add(classEntity);
        }
        UserEntity user = new()
        {
            Id = Guid.CreateVersion7(),
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Birthdate = dto.Birthdate,
            IsMarried = dto.IsMarried,
            Classes = classList
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
            Id = entity.Id,
            FullName = entity.FullName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            Birthdate = entity.Birthdate,
            IsMarried = entity.IsMarried,
            Age = age
        };
    }

    public async Task<IEnumerable<UserResponse>> Read()
    {
        var list = await dbContext.Users
            .AsNoTracking().Include(userEntity => userEntity.Classes)
            .ToListAsync();

        var response = list.Select(user =>
        {
            int? age = null;
            if (user.Birthdate != null)
            {
                age = DateTime.UtcNow.Year - user.Birthdate.Value.Year;
            }

            return new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Birthdate = user.Birthdate,
                IsMarried = user.IsMarried,
                Age = age,
                Classes = user.Classes.ToList()
            };
        });

        return response;
    }

    public async Task<UserResponse?> ReadById(Guid id)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        if (user == null)
        {
            return null;
        }
        int? age = null;
        if (user.Birthdate != null)
        {
            age = DateTime.UtcNow.Year - user.Birthdate.Value.Year;
        }
        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Birthdate = user.Birthdate,
            IsMarried = user.IsMarried,
            Age = age
        };
    }

    public async Task<UserResponse?> Update(UserUpdateParams param)
    {
        var user = await dbContext.Users.FindAsync(param.Id);
        if (user == null) return null;
        if(param.IsMarried != null) user.IsMarried = param.IsMarried.Value;
        if(param.PhoneNumber != null) user.PhoneNumber = param.PhoneNumber;
        if(param.Birthdate != null) user.Birthdate = param.Birthdate;
        if(param.FullName != null) user.FullName = param.FullName;
        if(param.Email != null) user.Email = param.Email;
        
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
        
        int? age = null;
        if (user.Birthdate != null)
        {
            age = DateTime.UtcNow.Year - user.Birthdate.Value.Year;
        }
        
        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Birthdate = user.Birthdate,
            IsMarried = user.IsMarried,
            Age = age
        };
    }

    public async Task Delete(Guid id)
    {
        UserEntity? user = await dbContext.Users.FindAsync(id);
        if (user == null) return;
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        // await dbContext.Users.Where(user => user.Id == id).ExecuteDeleteAsync(); 
    }
}