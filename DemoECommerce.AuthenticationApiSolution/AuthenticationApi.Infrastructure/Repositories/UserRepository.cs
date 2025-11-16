using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories;

public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
{
    private async Task<AppUser> GetAppUserByEmail(string email)
    {
        var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
        return user is null ? null! : user;
    }
    public async Task<GetUserDTO> GetUser(int userID)
    {
        var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Id == userID);
        return user is null ? null! : new GetUserDTO(
            Id: user.Id,
            Name: user.Name!,
            TelephoneNumber: user.TelephoneNumber!,
            Address: user.Address!,
            Email: user.Email!,
            Role: user.Role!
        );
    }

    public async Task<Response> Login(LoginDTO loginDTO)
    {
        var user = await GetAppUserByEmail(loginDTO.Email);
        if (user is null)
        {
            return new Response
            {
                Flag = false,
                Message = "User with this email does not exist."
            };
        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password!);
        if (!isPasswordValid)
        {
            return new Response
            {
                Flag = false,
                Message = "Invalid password."
            };
        }
        string token = GenerateToken(user);
        return new Response
        {
            Flag = true,
            Message = token
        };
    }

    private string GenerateToken(AppUser user)
    {
        var key = Encoding.UTF8.GetBytes(config["Authentication:SecretKey"]!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name!),
            new(ClaimTypes.Email, user.Email!)
        };
        if(!string.IsNullOrEmpty(user.Role) || !Equals("string",user.Role))
            claims.Add(new Claim(ClaimTypes.Role, user.Role!));

        var token = new JwtSecurityToken(
            issuer: config["Authentication:Issuer"],
            audience: config["Authentication:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Response> Register(AppUserDTO appUserDTO)
    {
        var user = await GetAppUserByEmail(appUserDTO.Email);
        if (user is not null)
        {
            return new Response
            {
                Flag = false,
                Message = "User with this email already exists."
            };
        }

        var newUser = new AppUser
        {
            Name = appUserDTO.Name,
            TelephoneNumber = appUserDTO.TelephoneNumber,
            Address = appUserDTO.Address,
            Email = appUserDTO.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
            Role = appUserDTO.Role,
            DateRegistered = DateTime.UtcNow
        };
        
        var result = context.AppUsers.Add(newUser);
        await context.SaveChangesAsync();

        return result.Entity.Id > 0
            ? new Response
            {
                Flag = true,
                Message = "User registered successfully."
            }
            : new Response
            {
                Flag = false,
                Message = "Failed to register user."
            };

    }
}
