using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
                ,
                EncryptingCredentials = new EncryptingCredentials(securityKey1, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256)
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

        public static string GenerateJwtToken(UserModel user, string issuer)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //var privateKey = private_key.ToByteArray();
            var privateKey = File.ReadAllText(@"D:\My Works\Practice Examples\my-new-app\assets\putty_key_pem.pem").ToByteArray();
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            { CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false } };

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
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static bool ValidateRSAToken(string token, string issuer)
        {
            var publicKey = public_key.ToByteArray();

            using RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out _);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                CryptoProviderFactory = new CryptoProviderFactory() { CacheSignatureProviders = false }
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, validationParameters, out var validatedSecurityToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static string private_key = "AAABABCGIV7tnD3/Fi5ki45oVAP4jGKP/PCaLILQ+jQA156+qNd3h4F3xHAYovEuz3Gltzb+1B0pQnjvrOIwZjYfApNjpm2hG9P2FWR2JPzRxLFL19hb2Es6r8sYLwiyUyh2OqrOXE/s3px7uluoshPagnO6zKqV/oI+vL+SD0u3Y5PuHVMAnECVj7kY0pSJkWLJ4svjZcp8V0tCWRb7kz27BwfyKyYf4lrzOR7e9AKBZexNeSbWAS6aHe+AjapnG8+Tv8UOHkQt73l6B4VuWB5fXcVw5QIGy4hT12eHFRfjE55DHTt30yENbnKM6P6udR3kk5HE30qd4uRzc2yravFJHVUAAACBAOVh1pFSLWK10AImfAA2ukR6yi3/J+K0j4Vk09vP5AlmP/IjACx3TIDO/0IJF7KDSaN6KvX7qdASgUQqTddNb5h6LhHqeIFZ7dG810X919ctsNJN+n64OJLuOd0iApIvvIyZphyo9VXHewZk8GtInIcuefLKFCTAjUCGmMxOMcKFAAAAgQDjcawCwzfC2F/bqr2wz7Fzp0RimDMflmhc+ZE+NxXkJ9RuuY0q5uPUoRKlz7S7NdHD62LIyYK2fsH0ywPO6nkwuMSYuX5m95ZrV956fJeiQo9HssFX7KC2hPRbKbMN08vwI/tVavmnSVDu3AQfLwweXsb7ycCvIF4JHqZHJ4dMBwAAAIBCXnzRUvoG+tlXRkzBex3HRBfuHIr61YA0nJRehAJM8eYhlE/TJzvA5AdlQ8xHxIBPliCk/DyuvOJoCw6pVKoBBuBsoRR7KG5+zZN2UxA/82Qe4PkKhgfq+oL7QAjZ+AtdZey49PpBddAWfazjEg8fMcxFjGrijlUX8A4Km8JGvw==";
        private static string public_key = "AAAAB3NzaC1yc2EAAAABJQAAAQEAy8ubksfcUfS8PC1j3FwMMPluFO/aQhgk+BFjLApjUITNDBeHPMUhZoUu8+v+eaZ+UPGNZ6eJKOFUOv+XRikfxHevSBhXNjBdLFvILhskiqdmFxdrn9PNc3+Za0IBnbIo5UZyhGlkNKClFSCUSjGePf/cN+SYRlsX5V68pdXMIHqH0zTF3Jm8zmIC+S+yHiWlSgMirgM26yotJIPcqvyZmr8fssE/FT1rQiT+ol2sG4GS7zJ4A+6icHS21Z6n4rGxBqDKl4LHK7qZ17FUm7JTSLB3xGT6NjiqGrA8clDnThmP8GdIq5SklZXZhgbZkwEW6GewgXAXjSXZNtaPwD7Now==";
    }

    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) =>
               Convert.FromBase64String(value);
    }
}

