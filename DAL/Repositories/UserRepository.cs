using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository<User>
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddUser(User user)
        {
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Test(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();
                await _context.ProductImages.AddAsync(new ProductImage
                {
                    ByteImage = bytes
                });

                await _context.SaveChangesAsync();
                string s = Convert.ToBase64String(bytes);
            }
        }

        public async Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<UserRefreshToken> GetRefreshToken(string token)
        {
            return await _context.UserRefreshTokens.FirstOrDefaultAsync(a => a.Token == token);
        }

        public async Task<UserRefreshToken> CreateRefreshToken(int userId, DateTime expDate)
        {
            var result = await _context.UserRefreshTokens.AddAsync(new UserRefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                ExpiryDate = expDate
            });

            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<UserRefreshToken> UpdateRefreshToken(UserRefreshToken token, DateTime expDate)
        {
            token.Token = Guid.NewGuid().ToString();
            token.ExpiryDate = expDate;

            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<UserRefreshToken> GetRefreshTokenByUserId(int userId)
        {
            return await _context.UserRefreshTokens.FirstOrDefaultAsync(a => a.UserId == userId);
        }
    }
}
