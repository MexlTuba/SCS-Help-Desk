using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserPreferencesViewModel
    {
        public string UserId { get; set; } // Foreign Key
        public int DefaultCategoryId { get; set; }
        public int DefaultStatusId { get; set; }
        public int DefaultPriorityId { get; set; }
    }
}
