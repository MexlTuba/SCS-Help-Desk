using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class UserPreferences
    {
        public int PreferenceId { get; set; } // Primary Key
        public string UserId { get; set; }    // Foreign Key to User table
        public int DefaultCategoryId { get; set; } // FK to Categories table
        public int DefaultStatusId { get; set; }   // FK to Statuses table
        public int DefaultPriorityId { get; set; } // FK to Priorities table

        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual User User { get; set; }

        public virtual Category DefaultCategory { get; set; }
        public virtual Status DefaultStatus { get; set; }
        public virtual Priority DefaultPriority { get; set; }
    }
}
