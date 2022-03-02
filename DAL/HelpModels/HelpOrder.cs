using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.HelpModels
{
    public class HelpOrder
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public byte[] ProductImage { get; set; }
    }
}
