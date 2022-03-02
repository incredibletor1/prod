using DAL.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUserRepository<T> where T : class
    {
        Task AddUser(User user);
        Task<User> GetUserById(int id);
        Task Test(IFormFile file);
        Task<User> GetUserByEmailAndPassword(string email, string password);

        //
        Task<UserRefreshToken> GetRefreshToken(string token);
        Task<UserRefreshToken> GetRefreshTokenByUserId(int userId);
        Task<UserRefreshToken> CreateRefreshToken(int userId, DateTime expDate);
        Task<UserRefreshToken> UpdateRefreshToken(UserRefreshToken token, DateTime expDate);
    }
}
