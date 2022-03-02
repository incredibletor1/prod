using BLL.DTO.VM;
using BLL.Interfaces;
using DAL.Entities;
using DAL.HelpModels;
using DAL.Interfaces;
using DAL.Repositories;
using Jose;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class TokenService : ITokenService
    {
        private static IOptions<JwtInfo> _config;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IOptions<JwtInfo> config, TokenValidationParameters tokenValidationParameters, IUnitOfWork unitOfWork)
        {
            _config = config;
            _tokenValidationParameters = tokenValidationParameters;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponseVM> CreateToken(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim("client", userId.ToString())
            };

            var jwt = new JwtSecurityToken(
                issuer: _config.Value.issuer,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config.Value.lifeTime)),
                claims: claims,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.Value.keyString)), SecurityAlgorithms.HmacSha256)
            );
            
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refreshToken = await _unitOfWork.Users.GetRefreshTokenByUserId(userId);
            
            if (refreshToken is null)
            {
                refreshToken = await _unitOfWork.Users.CreateRefreshToken(userId, DateTime.Now.AddDays(Convert.ToDouble(_config.Value.refreshTokenLifeTimeInDays)));
            }

            if (refreshToken.ExpiryDate < DateTime.Now)
            {
                refreshToken = await _unitOfWork.Users.UpdateRefreshToken(refreshToken, DateTime.Now.AddDays(Convert.ToDouble(_config.Value.refreshTokenLifeTimeInDays)));
            }
            
            return new LoginResponseVM
            {
                Token = encodedToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<LoginResponseVM> RefreshTokenAsync(string token, string refreshToken)
        {   
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken is null)
                throw new Exception("Invalid token");

            var tokenExpiryDate = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var test = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(tokenExpiryDate);

            if (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(tokenExpiryDate) > DateTime.UtcNow)
                throw new Exception("token not expired");

            var storedRefreshToken = await _unitOfWork.Users.GetRefreshToken(refreshToken);

            if (storedRefreshToken is null)
                throw new Exception("Token don't exist");

            if (storedRefreshToken.ExpiryDate < DateTime.Now)
                throw new Exception("refresh token expired");

            var newToken = await CreateToken(Convert.ToInt32(validatedToken.Claims.First(c => c.Type == "client").Value));

            return newToken;
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecutiryAlgorithm(validatedToken))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecutiryAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        private JwtPayload DecodeToken(string jwtToken)
        {
            JwtPayload payload = null;
            try
            {
                if (jwtToken.StartsWith("Bearer "))
                {
                    jwtToken = jwtToken.Substring(7);
                }

                var fromBase64TokenBytes = Convert.FromBase64String(jwtToken);
                var decodedFromBase64Token = Encoding.UTF8.GetString(fromBase64TokenBytes);
                var fromBase64KeyBytes = Convert.FromBase64String(_config.Value.keyString);
                payload = new JwtPayload(JWT.Decode<JwtPayloadBase>(decodedFromBase64Token, fromBase64KeyBytes, JwsAlgorithm.HS256));

                if (DateTime.UtcNow < payload.NotBefore)
                {
                    throw new UnauthorizedAccessException("Token is not yet valid");
                }

                if (DateTime.UtcNow > payload.Expiration)
                {
                    throw new UnauthorizedAccessException("Token is not longer valid");
                }

                if (payload.UserId == 0)
                {
                    throw new UnauthorizedAccessException("Token does not defined for a valid user");
                }
            }
            catch (Exception ex)
            {
                var errorMsg = "Error decoding the JWT token";
                throw new UnauthorizedAccessException(errorMsg, ex);
            }
            return payload;
        }


        // MODELS RELATED TO TOKEN
        public class JwtPayloadBase
        {
            public int client { get; set; }
            public long exp { get; set; }
            public long nbf { get; set; }
            public string iss { get; set; }
        }

        public class JwtPayload
        {
            public JwtPayload(JwtPayloadBase originalPayload)
            {
                if (originalPayload != null)
                {
                    this.UserId = originalPayload.client;
                    this.ExpirationUnixTimeStamp = originalPayload.exp;
                    this.NotBeforeUnixTimeStamp = originalPayload.nbf;
                    this.Issuer = originalPayload.iss;
                }
            }

            [JsonProperty("client")]
            public int UserId { get; set; }

            [JsonProperty("exp")]
            public long ExpirationUnixTimeStamp
            {
                get => new DateTimeOffset(Expiration).ToUnixTimeSeconds();
                set => Expiration = DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
            }

            [JsonProperty("nbf")]
            public long NotBeforeUnixTimeStamp
            {
                get => new DateTimeOffset(NotBefore).ToUnixTimeSeconds();
                set => NotBefore = DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
            }

            [JsonProperty("iss")]
            public string Issuer { get; set; }


            [JsonIgnore]
            public DateTime NotBefore { get; set; }

            [JsonIgnore]
            public DateTime Expiration { get; set; }
        }
    }
}
