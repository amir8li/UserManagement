using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Dtos;
using UserManagement.Entities;

namespace UserManagement.Services;

public interface IAuthService
{
    Task<LoginResponse?> Login(LoginParam param);
    Task<UserResponse> Register(UserCreateParams user);
}

public class AuthService(AppDbContext dbContext, IUserService userService) : IAuthService
{
    public async Task<LoginResponse?> Login(LoginParam param)
    {
        UserEntity? user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == param.Email && u.Password == param.Password);
        if(user == null) return null;
        
        string token = CreateToken(user);
        return new LoginResponse
        {
            Token = token
        };
    }
    
    public async Task<UserResponse> Register(UserCreateParams dto)
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
            Classes = classList,
            Password = "123456789"
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
    
    private string CreateToken(UserEntity user) {
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                "https://Amirali84.com,123456789987654321",
                "https://Amirali84.com,123456789987654321",
                [
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.Add(TimeSpan.FromSeconds(60)).ToString(CultureInfo.InvariantCulture))
                ],
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("https://Amirali84.com,123456789987654321")),
                    SecurityAlgorithms.HmacSha256
                )
            )
        );
    }
}