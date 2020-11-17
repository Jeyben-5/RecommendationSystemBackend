using Authentication.DataAccess.Interfaces;
using Authentication.Entities;
using Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;
        private readonly IPasswordService passwordService;
        private readonly IHostingEnvironment env;
        private readonly IConfiguration configuration;

        public UserService(
            IUserRepository userRepository,
            IEmailService emailService,
            IPasswordService passwordService,
            IHostingEnvironment env,
            IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.emailService = emailService;
            this.passwordService = passwordService;
            this.env = env;
            this.configuration = configuration;
        }

        public Task<bool> ChangePassword(User user)
        {
            return userRepository.ChangePassword(user);
        }

        public User FindById(int id)
        {
            var user = userRepository.FindById(id);

            return user;
        }

        public Task<IdentityUser> FindIdentityUserByName(string username)
        {
            return userRepository.FindIdentityUserByName(username);
        }

        public ICollection<User> ListUsers()
        {
            var users = userRepository.List;

            foreach (var user in users)
            {
                var identityUser = userRepository.FindIdentityUserByEmail(user.Email).Result;
                var roles = userRepository.GetRoles(identityUser).Result;
                var roleList = roles.Select(r => new Role { Name = r }).ToList();
                user.Roles = roleList;
            }

            return users.ToList();
        }

        public Task<User> Login(User user)
        {
            var registeredUser = userRepository.FindByUsername(user.Username);

            if (registeredUser != null)
            {
                if (registeredUser.IsEnabled)
                {
                    return userRepository.Login(user);
                }
                else
                {
                    throw new ApplicationException("User account disabled.");
                }
            }
            else
            {
                throw new ApplicationException("Username or password is incorrect.");
            }
        }

        public async Task<bool> RegisterUser(User user)
        {
            var identityUser = userRepository.FindIdentityUserByEmail(user.Email);

            if (identityUser.Result == null)
            {
                user.Username = user.FirstName[0].ToString().ToLower() + user.LastName.ToLower();
                user.GivenName = user.FirstName + ' ' + user.LastName;
                user.Password = passwordService.GeneratePassword(8);
                user.IsEnabled = true;

                var cont = 1;
                var usernameVerified = user.Username;
                while (FindIdentityUserByName(usernameVerified).Result != null)
                {
                    usernameVerified = user.Username + cont;
                    cont++;
                }
                user.Username = usernameVerified;

                var userCreated = userRepository.RegisterUser(user);

                if (userCreated.Result)
                {
                    var webRoot = env.WebRootPath;
                    var pathToFile = env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplate"
                            + Path.DirectorySeparatorChar.ToString()
                            + selectTemplate(user.Roles.First().Name);
                    var builder = new BodyBuilder();

                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }

                    string htmlBody = builder.HtmlBody
                        .Replace("{", "{{")
                        .Replace("}", "}}")
                        .Replace("#username#", user.Username)
                        .Replace("#password#", user.Password);

                    var email = new Email
                    {
                        Subject = "Welcome!",
                        Message = string.Format(htmlBody),
                        ToAddresses = user.Email,
                        IsBodyHtml = true
                    };

                    emailService.SendEmail(email);

                    return userCreated.Result;
                }
                else
                {
                    throw new ApplicationException("Cannot register the user, the username is already in use");
                }
            }
            else
            {
                throw new ApplicationException("Cannot register the user, the email is already in use");
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            var identityUser = userRepository.FindIdentityUserByEmail(user.Email);
            var registeredUser = userRepository.FindByUsername(user.Username);

            if (identityUser.Result == null || user.Email == registeredUser.Email)
            {
                if (user.Email != registeredUser.Email)
                {
                    user.Password = passwordService.GeneratePassword(8);
                }
                var result = await userRepository.UpdateUser(user);

                if (result && user.Password != null)
                {
                    var email = new Email
                    {
                        Subject = "Welcome!",
                        Message = string.Format("<p>Your username is: {0}</p><p>Your password is: {1}</p>", user.Username, user.Password),
                        ToAddresses = user.Email,
                        IsBodyHtml = true
                    };

                    emailService.SendEmail(email);
                }

                return result;
            }
            else
            {
                throw new ApplicationException("Unable to update the user, the email is already in use");
            }
        }

        string selectTemplate(string role)
        {
            var template = "";

            if (role == "Admin")
            {
                template = configuration["AdminEmailTemplate"];
            }
            if (role == "Moderator")
            {
                template = configuration["ModeratorEmailTemplate"];
            }
            if (role == "Student")
            {
                template = configuration["StudentEmailTemplate"];
            }

            return template;
        }
    }
}
