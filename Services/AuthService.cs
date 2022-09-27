using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Customer.Portal.Web.Configurations;
using Customer.Portal.Web.Context;
using Customer.Portal.Web.Exceptions;
using Customer.Portal.Web.Models;
using Customer.Portal.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Customer.Portal.Web.Services {
    public class AuthService : IAuthService {
        private readonly CustomerPortalContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly AppSettings _settings;

        public AuthService(CustomerPortalContext context, ILoggerFactory loggerFactory, IOptions<AppSettings> options) {
            _context = context;
            _logger = loggerFactory.CreateLogger<AuthService>();
            _settings = options.Value;
        }


        public async Task Register(UserCreateViewModel user) {
            if (await UserExistsAsync(user.Username)) {
                throw new UserRegistrationFailedException();
            }

            if (!await RolesExistAsync(user.Roles)) {
                throw new UserRegistrationFailedException();
            }

            CreatePasswordHashAndSalt(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var userEntity = new User {
                Username = user.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                AvatarUrl = user.AvatarUrl,
                FullName = user.FullName
            };

            foreach (var role in user.Roles) {
                userEntity.UserRoles.Add(new UserRole {
                    User = userEntity,
                    Role = await GetRoleAsync(role)
                });
            }

            await _context.AddAsync(userEntity);

            await _context.SaveChangesAsync();
        }

        public async Task<JwtViewModel> Login(LoginViewModel loginViewModel) {
            var userDb = await GetUserAsync(loginViewModel.Username);
            
            if (!VerifyPasswordHash(loginViewModel.Password, userDb.PasswordHash, userDb.PasswordSalt)) {
                throw new LoginFailedException();
            }
            
            return GetToken(userDb);
        }

        private JwtViewModel GetToken(User userDb) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, userDb.Username),
                new Claim("FullName", userDb.FullName),
                new Claim("AvatarUrl", userDb.AvatarUrl)
            };

            foreach (var role in GetRoles(userDb)) {
                claims.Add(new Claim("Roles", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSecret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtViewModel { Jwt = tokenHandler.WriteToken(securityToken) };
        }

        private IEnumerable<string> GetRoles(User user) {
            return user.UserRoles.Select(ur => ur.Role.Name).ToList();
        }

        private async Task<User> GetUserAsync(string username) {
            var userDb = await _context
                .Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (userDb == null) {
                throw new LoginFailedException();
            }

            return userDb;
        }

        private async Task<bool> UserExistsAsync(string username) {
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        private async Task<bool> RolesExistAsync(IEnumerable<string> roles) {
            foreach (var role in roles) {
                var result = await _context.Roles.AnyAsync(r => r.Name.ToLower() == role.ToLower());
                if (!result) {
                    return false;
                }
            }

            return true;
        }

        private async Task<Role> GetRoleAsync(string role) {
            var roleDb = await _context
                .Roles
                .FirstOrDefaultAsync(u => u.Name.ToLower() == role.ToLower());

            if (roleDb == null) {
                throw new UserRegistrationFailedException();
            }

            return roleDb;
        }

        private void CreatePasswordHashAndSalt(string rawPassword, out byte[] passwordHash, out byte[] passwordSalt) {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawPassword));
        }

        private bool VerifyPasswordHash(string rawPassword, byte[] passwordHash, byte[] passwordSalt) {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawPassword));

            for (int i = 0; i < computedHash.Length; i++) {
                if (computedHash[i] != passwordHash[i]) {
                    return false;
                }
            }

            return true;
        }
    }
}