using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using my_new_app.Models;
using my_new_app.Services;

namespace my_new_app.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<HomeController> _logger;
        private readonly IUserService userService;
        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            this.userService = userService;
        }
        [Authorize]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            //User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //User.FindFirst().Subject.HasClaim()
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserLoginRequest model)
        {
            Console.WriteLine("Authenticate");
            var response = userService.Authenticate(model, ipAddress());
            if (response == null) { return BadRequest(new { message = "Username or password is incorrect" }); }

            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            Console.WriteLine("RefreshToken");
            var refreshToken = Request.Cookies["refreshToken"];
            var response = userService.RefreshToken(refreshToken, ipAddress());

            if (response == null) { return Unauthorized(new { message = "Invalid token" }); }

            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("revoke-token")]
        public IActionResult RevokeToken()
        {
            Console.WriteLine("RefreshToken");
            var refreshToken = Request.Cookies["refreshToken"];
            var response = userService.RevokeToken(refreshToken, ipAddress());
            if (!response) { return Unauthorized(new { message = "Invalid token" }); }

            Response.Cookies.Delete("refreshToken");
            return Ok(response);
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For")) { return Request.Headers["X-Forwarded-For"]; }
            else { return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(); }
        }

    }
}