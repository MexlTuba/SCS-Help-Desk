using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Status
    {
        public int StatusId { get; set; }        // Primary key
        public string StatusType { get; set; }   // Description of the status
    }
}
