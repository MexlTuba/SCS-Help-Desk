using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string userid, string password, ref User user);
        void AddUser(UserViewModel model);

        public User GetUserByName(string name);

        List<User> GetAllUsers();
        void DeleteUser(string userId);
        User GetUserById(string userId);
        void UpdateUser(User user);
        void ResetPassword(string userId, string newPassword);
    }
}
