using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserListViewModel
    {
        public List<User> Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Role { get; set; }
    }
}