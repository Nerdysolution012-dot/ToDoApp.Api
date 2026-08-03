using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Api.Common.Exceptions;
using ToDoApp.Api.Data;
using ToDoApp.Api.DTOs.Auth;
using ToDoApp.Api.Models;
using ToDoApp.Api.Services.Interfaces;

namespace ToDoApp.Api.Services.Implementations;

public class AuthService(
    AppDbContext db,
    IPasswordHasher<User> passwordHasher,
    IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var email = NormalizeEmail(dto.Email);

        if (await db.Users.AnyAsync(x => x.Email == email))
        {
            throw new ConflictException("Email is already registered");
        }

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = RoleNames.Member,
            CreatedAt = DateTime.Now
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Protects against race conditions if two registrations arrive together.
            throw new ConflictException("Email is already registered");
        }

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var email = NormalizeEmail(dto.Email);
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        var expiryMinutes = configuration.GetValue<int>("Jwt:ExpiryMinutes");
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is missing.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
