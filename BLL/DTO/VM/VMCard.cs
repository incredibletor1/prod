using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.VM
{
    public class VMCard
    {
        public string CardNumber { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string Cvc { get; set; }

        public int Value { get; set; }
    }
}
