using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using CommonLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration configuration) : IUser
    {
        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null) return null;
            return new GetUserDTO(
                user.Id,
                user.Name,
                user.TelephoneNumber,
                user.Address,
                user.Email,
                user.Role
                );
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);
            if (getUser is null)
                return new Response(false, "Invalid credentials");

            //compare the hashed password stored in the database (getUser.Password) with the plain-text password provided
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            //bool verifyPassword = loginDTO.Password == getUser.Password;
            if (!verifyPassword) 
                return new Response(false, "Invalid credentials");

            string token = GenerateToken(getUser);
            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:Key").Value);
            var sercurityKy = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(sercurityKy, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email)
            };
            if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
            {
                claims.Add(new(ClaimTypes.Role, user.Role));
            }

            var token = new JwtSecurityToken(
                issuer: configuration["Authentication:Issuer"],
                audience: configuration["Authentication:Audience"],
                claims:claims,
                expires: null,
                signingCredentials:credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var getUser = await GetUserByEmail(appUserDTO.Email);
            if (getUser is not null)
                return new Response(false, $"This email already registred");

            //hash password in database
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password);
            
            var result = context.Users.Add( new AppUser()
                    {
                        Name = appUserDTO.Name,
                        Email = appUserDTO.Email,
                        Password = hashedPassword, // Store hashed password
                TelephoneNumber = appUserDTO.TelephoneNumber,
                        Address = appUserDTO.Address,
                        Role = appUserDTO.Role
                    }
                );
            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? 
                new Response(true, "User registered successfully"): 
                new Response(false, "Invalid data provided");
        }
    }
}
