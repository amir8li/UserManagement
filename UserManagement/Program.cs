using Microsoft.EntityFrameworkCore;
using UserManagement;
using UserManagement.Dtos;
using UserManagement.Entities;
using UserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped <AppDbContext>();
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabase"));
});

builder.Services.AddScoped<IUserService, UserService>();

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

app.Run();
