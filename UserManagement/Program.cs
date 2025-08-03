using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.UseInlineDefinitionsForEnums();
    c.OrderActionsBy(s => s.RelativePath);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "JWT Authorization header.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    // c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme {
    //     Description = "Api Key is Required",
    //     Name = "X-API-Key",
    //     In = ParameterLocation.Header,
    //     Type = SecuritySchemeType.ApiKey,
    //     Scheme = "ApiKey"
    // });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() },
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" } }, Array.Empty<string>() },
    });
});

builder.Services.AddScoped <AppDbContext>();
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabase"));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters {
        RequireSignedTokens = true,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = false,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = "https://Amirali84.com,123456789987654321",
        ValidIssuer = "https://Amirali84.com,123456789987654321",
        IssuerSigningKey = new SymmetricSecurityKey("https://Amirali84.com,123456789987654321"u8.ToArray())
    };
});

builder.Services.AddAuthorization();

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

app.MapPost("auth/login", async (LoginParam param, IAuthService authService) =>
{
    var response = await authService.Login(param);
    return response == null ? Results.Unauthorized() : Results.Ok(response); 
});

app.MapPost("auth/register", async (IAuthService authService, UserCreateParams dto) =>
{
    var result = await authService.Register(dto);
    return Results.Ok(result);
});

app.MapGet("user/getProfile", async (IUserService userService) =>
{
    UserResponse? result = await userService.GetProfile();
    return result == null ? Results.Unauthorized() : Results.Ok(result);
}).RequireAuthorization();

app.Run();
