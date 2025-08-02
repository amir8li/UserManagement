using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using UserManagement;
using UserManagement.Dtos;
using UserManagement.Entities;
using UserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.WriteIndented = false;    
}); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped <AppDbContext>();
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabase"));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IClassService, ClassService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("user/Create", async (IUserService userService, UserCreateParams dto) =>
{
    var result = await userService.Create(dto);
    return Results.Ok(result);
});

app.MapGet("user/Read", async (IUserService userService) =>
{
    var result = await userService.Read();
    return Results.Ok(result);
});

app.MapGet("user/Read/{id:guid}", async (IUserService userService, Guid id) =>
{
    var result = await userService.ReadById(id);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapPut("user/Update", async (IUserService userService, UserUpdateParams param) =>
{
    var result = await userService.Update(param);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("user/Delete", async (IUserService userService, Guid id) =>
{
    await userService.Delete(id);
    return Results.Ok();
});

app.MapPost("class/Create", async (IClassService classService, ClassCreateDto dto) =>
{
    var result = await classService.Create(dto);
    return Results.Ok(result);
    
});

app.MapGet("class/Read", async (IClassService classService) =>
{
    var result = await classService.Read();
    return Results.Ok(result);
});

app.MapGet("class/Read/{id:guid}", async (IClassService classService, Guid id) =>
{
    var result = await classService.ReadById(id);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapPut("class/Update", async (IClassService classService, ClassEntity param) =>
{
    var result = await classService.Update(param);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("class/Delete", async (IClassService classService, Guid id) =>
{
    await classService.Delete(id);
    return Results.Ok();
});


app.MapPost("school/Create", async (ISchoolService schoolService, SchoolEntity dto) =>
{
    var result = await schoolService.Create(dto);
    return Results.Ok(result);
    
});

app.MapGet("school/Read", async (ISchoolService schoolService) =>
{
    var result = await schoolService.Read();
    return Results.Ok(result);
});

app.MapGet("school/Read/{id:guid}", async (ISchoolService schoolService, Guid id) =>
{
    var result = await schoolService.ReadById(id);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapPut("school/Update", async (ISchoolService schoolService, SchoolEntity param) =>
{
    var result = await schoolService.Update(param);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("school/Delete", async (ISchoolService schoolService, Guid id) =>
{
    await schoolService.Delete(id);
    return Results.Ok();
});


app.Run();
