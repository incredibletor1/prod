using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.HelpModels
{
    public class JwtInfo
    {
        public string issuer { get; set; }
        public string lifeTime { get; set; }
        public string keyString { get; set; }
        public string refreshTokenLifeTimeInDays { get; set; }
    }
}
