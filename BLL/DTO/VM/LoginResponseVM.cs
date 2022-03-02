using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.VM
{
    public class LoginResponseVM
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
