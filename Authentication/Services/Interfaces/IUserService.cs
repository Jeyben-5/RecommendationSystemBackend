using Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authentication.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUser(User user);
        Task<User> Login(User user);
        Task<bool> ChangePassword(User user);
        Task<bool> UpdateUser(User user);
        ICollection<User> ListUsers();
        User FindById(int id);
        Task<IdentityUser> FindIdentityUserByName(string username);
    }
}
