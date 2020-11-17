using Authentication.DataAccess.Interfaces;
using Authentication.Entities;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IPasswordHasher<IdentityUser> passwordHasher;
        private readonly IConfiguration configuration;
        private readonly IdentityDbContext context;

        public UserRepository(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IPasswordHasher<IdentityUser> passwordHasher,
            IConfiguration configuration,
            IdentityDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
            this.configuration = configuration;
            this.context = context;
        }

        public IQueryable<User> List
        {
            get
            {
                return context.Set<User>();
            }
        }

        public async Task<bool> ChangePassword(User user)
        {
            var identityUser = await userManager.FindByNameAsync(user.Username);

            if (identityUser != null)
            {
                if (passwordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, user.OldPassword) != PasswordVerificationResult.Failed)
                {
                    var result = await userManager.ChangePasswordAsync(identityUser, user.OldPassword, user.Password);

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException("The password must contain at least one capital letter.");
                    }
                    else
                    {
                        return result.Succeeded;
                    }
                }
                else
                {
                    throw new ApplicationException("The old password is incorrect.");
                }
            }
            else
            {
                throw new ApplicationException("User not found.");
            }
        }

        public async Task<IdentityUser> FindIdentityUserByEmail(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityUser> FindIdentityUserByName(string username)
        {
            return await userManager.FindByNameAsync(username);
        }

        public User FindById(int id)
        {
            return context.Set<User>().Find(id);
        }

        public async Task<IList<string>> GetRoles(IdentityUser user)
        {
            var roles = userManager.GetRolesAsync(user);

            return await roles;
        }

        public User FindByUsername(string userName)
        {
            return context.Set<User>().SingleOrDefault(u => u.Username == userName);
        }

        public async Task<User> Login(User user)
        {
            var result = await signInManager.PasswordSignInAsync(user.Username, user.Password, false, false);

            if (result.Succeeded)
            {
                var appUser = userManager.Users.SingleOrDefault(r => r.UserName == user.Username);
                var userToClaim = FindByUsername(user.Username);
                user.Token = await GenerateJwtToken(userToClaim, appUser);

                return user;
            }

            throw new ApplicationException("Username or password is incorrect.");
        }

        public async Task<bool> RegisterUser(User user)
        {
            var newUser = new IdentityUser
            {
                UserName = user.Username,
                Email = user.Email
            };

            var result = await userManager.CreateAsync(newUser, user.Password);

            if (result.Succeeded)
            {
                context.Set<User>().Add(user);
                context.SaveChanges();

                if (user.Roles != null)
                {
                    foreach (var role in user.Roles)
                    {
                        await userManager.AddToRoleAsync(newUser, role.Name);
                    }
                }
            }

            return result.Succeeded;
        }

        public async Task<bool> UpdateUser(User user)
        {
            var identityUser = userManager.FindByNameAsync(user.Username).Result;

            if (user.Password != null)
            {
                identityUser.PasswordHash = userManager.PasswordHasher.HashPassword(identityUser, user.Password);
            }
            identityUser.UserName = user.Username;
            identityUser.Email = user.Email;
            var result = userManager.UpdateAsync(identityUser).Result;

            if (result.Succeeded)
            {
                var userToUpdate = context.Set<User>().Find(user.Id);

                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Username = user.Username;
                userToUpdate.Email = user.Email;
                userToUpdate.GivenName = user.FirstName + ' ' + user.LastName;
                userToUpdate.IsEnabled = user.IsEnabled;
                context.SaveChanges();
            }

            return result.Succeeded;
        }

        private async Task<string> GenerateJwtToken(User user, IdentityUser identityUser)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                new Claim("user_id", user.Id.ToString())
            };

            var roles = GetRoles(identityUser);

            foreach (var role in roles.Result)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (roles.Result.Count == 1 && roles.Result.First() == "Moderator")
            {
                var client = context.Set<Moderator>().SingleOrDefault(c => c.Email == user.Email);
                claims.Add(new Claim("moderator_id", client.Id.ToString()));
            }

            if (roles.Result.Count == 1 && roles.Result.First() == "Student")
            {
                var employee = context.Set<Student>().SingleOrDefault(e => e.Email == user.Email);
                claims.Add(new Claim("student_id", employee.Id.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(configuration["JwtExpireMinutes"]));

            var token = new JwtSecurityToken(
                configuration["JwtIssuer"],
                configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}