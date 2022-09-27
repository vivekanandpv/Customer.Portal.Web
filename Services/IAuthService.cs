using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Customer.Portal.Web.Context;
using Customer.Portal.Web.Exceptions;
using Customer.Portal.Web.Models;
using Customer.Portal.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Customer.Portal.Web.Services {
    public interface IAuthService {
        Task Register(UserCreateViewModel user);
        Task<JwtViewModel> Login(LoginViewModel loginViewModel);
    }

    public class AuthService : IAuthService {
        private readonly CustomerPortalContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(CustomerPortalContext context, ILoggerFactory loggerFactory) {
            _context = context;
            _logger = loggerFactory.CreateLogger<AuthService>();
        }


        public async Task Register(UserCreateViewModel user) {
            //  ensure the username is not taken
            if (await UserExistsAsync(user.Username)) {
                throw new UserRegistrationFailedException();
            }

            //  ensure the roles exist
            if (!await RolesExistAsync(user.Roles)) {
                throw new UserRegistrationFailedException();
            }

            //  hash the password and salt
            CreatePasswordHashAndSalt(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
            
            //  Create the user
            var userEntity = new User {
                Username = user.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                AvatarUrl = user.AvatarUrl,
                FullName = user.FullName
            };

            //  attach the roles
            foreach (var role in user.Roles) {
                userEntity.UserRoles.Add(new UserRole {
                    User = userEntity,
                    Role = await GetRoleAsync(role)
                });
            }
            
            //  add the user to the context
            await _context.AddAsync(userEntity);

            //  save the user
            await _context.SaveChangesAsync();
        }

        public async Task<JwtViewModel> Login(LoginViewModel loginViewModel) {
            throw new System.NotImplementedException();
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