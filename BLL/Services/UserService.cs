using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddUser(UserDTO userDTO)
        {
            User user = AutoMapperService<UserDTO, User>.Mapper(userDTO);
            await _unitOfWork.Users.AddUser(user);
        }

        public async Task<UserDTO> LoginUser(UserDTO userDTO)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAndPassword(userDTO.Email, userDTO.Password);
            return AutoMapperService<User, UserDTO>.Mapper(user);
        }

        public async Task Test(IFormFile file)
        {
            await _unitOfWork.Users.Test(file);
        }
    }
}

