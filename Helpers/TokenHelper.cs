using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using my_new_app.Models;

namespace my_new_app.Helpers
{
    public static class TokenHelper
    {
        public static string GenerateJwtToken(UserModel user, string secretKey, string issuer)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var securityKey1 = new SymmetricSecurityKey(Encoding.Default.GetBytes("ProEMLh5e_qnzdNU"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "ADMIN")
                }),
                Issuer = issuer,
                Audience = issuer,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                ,EncryptingCredentials = new EncryptingCredentials(securityKey1, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
           // var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static RefreshToken GenerateRefreshToken(string ipAddress, int id)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddMinutes(2),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress,
                    UserId = id
                };
            }
        }
        public static JwtSecurityToken ReadToken(string token)
        {
            if (string.IsNullOrEmpty(token)) { return null; }
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
    }
}