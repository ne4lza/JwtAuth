using Application.Contracts;
using Application.DTOs;
using Application.Response;
using Domain.Entities;
using Infrastructre.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructre.Repo
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration configuration;

        public UserRepo(AppDbContext appDbContext,IConfiguration configuration)
        {
            this.appDbContext = appDbContext;
            this.configuration = configuration;
        }
        public async Task<ApplicationUser> FindUserByEmail(string email)=> await appDbContext.Users.FirstOrDefaultAsync(x => x.Email == email!);

        public async Task<LoginResponse> LoginUserAsync(LoginUserDTO loginUserDTO)
        {
            var getUser = await FindUserByEmail(loginUserDTO.Email);
            if (getUser == null)
                return new LoginResponse(false, "Kullanıcı bulunamadı");

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginUserDTO.Password, getUser.Password);
            if (checkPassword)
                return new LoginResponse(true, "Giriş Başarılı", GenerateToken(getUser));
            else
                return new LoginResponse(false, "Giriş bilgileri hatalı");
        }

        public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            var getUser = await FindUserByEmail(registerUserDTO.Email);
            if (getUser != null)
                return new RegistrationResponse(false, "Bu kullanıcı zaten var");

            appDbContext.Users.Add(new ApplicationUser()
            {
                Name = registerUserDTO.Name,
                Email = registerUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDTO.Password)
            });
            await appDbContext.SaveChangesAsync();
            return new RegistrationResponse(true, "Kayıt başarılı");
        }
        private string GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Name!),
                new Claim(ClaimTypes.Email,user.Email!),
            };
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
