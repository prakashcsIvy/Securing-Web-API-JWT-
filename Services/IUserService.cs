using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using my_new_app.Helpers;
using my_new_app.Helpers.Cache;
using my_new_app.Models;

namespace my_new_app.Services
{
    public interface IUserService
    {
        UserLoginResponse Authenticate(UserLoginRequest model, string ipAddress);
        UserLoginResponse RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
    }
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly ICacheHelper cacheHelper;
        public UserService(IOptions<AppSettings> appSettings, ICacheHelper cacheHelper)
        {
            _appSettings = appSettings.Value;
            this.cacheHelper = cacheHelper;
        }
        public UserLoginResponse Authenticate(UserLoginRequest model, string ipAddress)
        {
            if (model.Username != "admin" || model.Password != "admin") { return null; }
            return GenerateUserTokens(ipAddress);
        }
        public UserLoginResponse RefreshToken(string token, string ipAddress)
        {
            var tokenModel = GetRefreshToken(token);
            if (tokenModel == null) { Console.WriteLine("Token Got Null"); return null; }
            if (!tokenModel.IsActive) { Console.WriteLine("Token Got Expired"); return null; }

            this.cacheHelper.RemoveItem(token);
            return GenerateUserTokens(ipAddress);
        }
        private UserLoginResponse GenerateUserTokens(string ipAddress)
        {
            var user = GetUser();
            var jwtToken = TokenHelper.GenerateJwtToken(user, _appSettings.Secret, _appSettings.Issuer);
            //var jwtToken = TokenHelper.GenerateJwtToken(user, _appSettings.Issuer);
            var refreshToken = TokenHelper.GenerateRefreshToken(ipAddress, user.Id);
            cacheHelper.SetItem(refreshToken.Token, refreshToken);

            return new UserLoginResponse(user, jwtToken, refreshToken.Token);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var tokenModel = GetRefreshToken(token);
            if (tokenModel == null) { return false; }
            if (!tokenModel.IsActive) { return false; }

            this.cacheHelper.RemoveItem(token);
            return true;
        }

        private RefreshToken GetRefreshToken(string token)
        {
            var tokenModel = this.cacheHelper.GetItem(token);
            return (tokenModel == null ? null : (RefreshToken)tokenModel);
        }
        private UserModel GetUser()
        {
            return new UserModel() { Id = 2, Username = "admin", FirstName = "Admin User" };
        }
    }
}