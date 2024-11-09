using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Category
    {
        public int CategoryId { get; set; }      // Primary key
        public string CategoryType { get; set; } // Description of the category
    }
}
