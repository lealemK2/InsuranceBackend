using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Insurance.Model;
using Insurance.Dto;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Xml.Linq;

namespace Insurance.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRepository(AuthDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<User>>? Get()//?I dont get this
        {
            Task<List<User>> myTask = Task.Run(() => _context.Users.ToList());
            List<User> result = await myTask;
            return result;
        }

        private static void CreatedPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        public async Task<User> Login(UserDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                userDto.CorrectPassword = false;
            }

            return user;
        }

        public async Task<User> Register(UserDto userDto)
        {
            CreatedPasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
