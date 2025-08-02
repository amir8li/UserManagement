using Microsoft.EntityFrameworkCore;
using UserManagement.Entities;

namespace UserManagement.Services;

public interface ISchoolService
{
    Task<List<SchoolEntity>> Read();
    Task<SchoolEntity?> ReadById(Guid id);
    Task<SchoolEntity> Create(SchoolEntity param);
    Task<SchoolEntity?> Update(SchoolEntity param);
    Task Delete(Guid id);
}

public class SchoolService(AppDbContext dbContext) : ISchoolService
{
    public async Task<List<SchoolEntity>> Read()
    {
        List<SchoolEntity> schools = await dbContext.School.Include(school => school.Classes).ToListAsync();
        return schools;
    }

    public async Task<SchoolEntity?> ReadById(Guid id)
    {
        var school = await dbContext.School.FindAsync(id);
        return school;
    }

    public async Task<SchoolEntity> Create(SchoolEntity param)
    {
        SchoolEntity school = new SchoolEntity
        {
            Id = Guid.CreateVersion7(),
            Title = param.Title
        };
        SchoolEntity entity = dbContext.School.Add(school).Entity;
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<SchoolEntity?> Update(SchoolEntity param)
    {
        SchoolEntity? school = await dbContext.School.FindAsync(param.Id);
        if (school == null) return null;
        school.Title = param.Title;
        dbContext.School.Update(school);
        await dbContext.SaveChangesAsync();
        return new SchoolEntity
        {
            Id = school.Id,
            Title = param.Title
        };
    }

    public async Task Delete(Guid id)
    {
        SchoolEntity? school = await dbContext.School.FindAsync(id);
        if (school == null) return;
        dbContext.School.Remove(school);
        await dbContext.SaveChangesAsync();
    }
}