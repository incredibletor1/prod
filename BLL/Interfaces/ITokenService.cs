using BLL.DTO.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITokenService
    {
        Task<LoginResponseVM> CreateToken(int userId);
        Task<LoginResponseVM> RefreshTokenAsync(string token, string refreshToken);
    }
}
