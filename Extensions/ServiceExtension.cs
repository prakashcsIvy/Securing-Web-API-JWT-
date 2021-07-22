using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using my_new_app.Helpers;
using my_new_app.Models;

namespace my_new_app.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // configure jwt authentication
            var appSettingsSection = configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var key1 = Encoding.Default.GetBytes("ProEMLh5e_qnzdNU");
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = appSettings.Issuer,
                    ValidAudience = appSettings.Issuer,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero,
                    TokenDecryptionKey = new SymmetricSecurityKey(key1)
                };
            });
            return services;
        }

        public static IServiceCollection RegisterAuthentication1(this IServiceCollection services, IConfiguration configuration)
        {
            // configure jwt authentication
            var appSettingsSection = configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                //var publicKey = File.ReadAllText(@"D:\My Works\Practice Examples\my-new-app\assets\public_key.pem").ToByteArray();
                var publicKey = public_key.ToByteArray();
                using RSA rsa = RSA.Create();
                rsa.ImportRSAPublicKey(publicKey, out _);

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = appSettings.Issuer,
                    ValidAudience = appSettings.Issuer,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    CryptoProviderFactory = new CryptoProviderFactory() { CacheSignatureProviders = false },
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero,
                };
            });
            return services;
        }
        private static string public_key = "AAAAB3NzaC1yc2EAAAABJQAAAQEAqYWmNY07vB3vFAQXka5sLdXODZREYLF7CQdX2vmU8W5ii5Xcyfhaj03jBnUKMxCVGIsMCMyQCqF6Og0Ig122aFxWLEf4HgOdKUZw51sxoJMR5mxDm1jSa5qtyutmmqx2LqfmdPGWn38eVUVHR18SBFwz/uqWe9cFrg5Z52SFMB9HDL0oEnAlGlAmc71IwtkAzbXM13eDVteObPz98XrNC5RGW12Uol7D41BuJoGQPix09dHbAMSW5wgG8VJiNyiWAhC9IXUo+vM3XUYtu+oXS5iD6UC39yfK4tQmgtXsc9Lu1TvqHoEaw/s5s1p95F2GXuEhAsVt9RMkutCCCFnyeQ==";
    }
}