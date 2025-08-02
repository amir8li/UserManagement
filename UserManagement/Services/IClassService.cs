using Microsoft.EntityFrameworkCore;
using UserManagement.Dtos;
using UserManagement.Entities;

namespace UserManagement.Services;

public interface IClassService
{
    Task<List<ClassEntity>> Read();
    Task<ClassEntity?> ReadById(Guid id);
    Task<ClassEntity> Create(ClassCreateDto dto);
    Task<ClassEntity?> Update(ClassEntity param);
    Task Delete(Guid id);
}

public class ClassService(AppDbContext dbContext) : IClassService
{
    public async Task<List<ClassEntity>> Read()
    {
        List<ClassEntity> result = await dbContext.Class
            .Include(clas => clas.School)
            .Include(clas => clas.Users)
            .ToListAsync();
        return result;
    }

    public async Task<ClassEntity?> ReadById(Guid id)
    {
        ClassEntity? response = await dbContext.Class.FindAsync(id);
        return response ?? null;
    }

    public async Task<ClassEntity> Create(ClassCreateDto dto)
    {
        List<UserEntity> userList = [];
        foreach (var userId in dto.Users)
        {
            UserEntity? userEntity = await dbContext.Users.FindAsync(userId);
            userList.Add(userEntity);
        }
        ClassEntity? e = new ClassEntity
        {
            Id = Guid.CreateVersion7(),
            Title = dto.Title,
            Subject = dto.Subject,
            SchoolId = dto.SchoolId,
            Users = userList
        };
        var entity = dbContext.Class.Add(e).Entity;
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<ClassEntity?> Update(ClassEntity param)
    {
        ClassEntity? response = await dbContext.Class.FindAsync(param.Id);
        if (response == null) return null;
        response.Title = param.Title;
        response.Subject  = param.Subject;
        if (param.SchoolId != null)
        {
            response.SchoolId = param.SchoolId.Value;
        }
        dbContext.Class.Update(response);
        await dbContext.SaveChangesAsync();
        return new ClassEntity
        {
            Id = response.Id,
            Title = response.Title,
            Subject = response.Subject,
            SchoolId = response.SchoolId
        };
    }

    public async Task Delete(Guid id)
    {
        ClassEntity? entity = await dbContext.Class.FindAsync(id);
        if (entity == null) return;
        dbContext.Class.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}