using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Priority
    {
        public int PriorityId { get; set; }       // Primary key
        public string PriorityType { get; set; }  // Description of the priority level
    }
}
