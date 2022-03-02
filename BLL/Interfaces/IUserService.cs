using BLL.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task AddUser(UserDTO userDTO);

        Task<UserDTO> LoginUser(UserDTO userDTO);
        Task Test(IFormFile file);
    }
}
