using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public User()
        {
        }

        public string UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public String Role{ get; set; }
        public string Email { get; set; }

        public virtual UserPreferences UserPreferences { get; set; }
    }
}
