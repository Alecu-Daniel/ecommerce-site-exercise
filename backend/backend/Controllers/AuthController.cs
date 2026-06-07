using backend.DataContext;
using backend.Dtos;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Pipelines;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using backend.Services.Interfaces;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password != userForRegistration.PasswordConfirm)
            {
                return BadRequest("Passwords do not match.");
            }

            try
            {
                var success = await _authService.RegisterAsync(userForRegistration);
                if (!success) return BadRequest("An account with that email already exists");

                _logger.LogInformation("Successfully registered a new user account profile for {Email}.", userForRegistration.Email);
                return Ok(new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception thrown during execution route for user {Email}", userForRegistration.Email);
                return StatusCode(500, "An internal error occurred while processing your registration.");
            }
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            try
            {
                string? token = await _authService.LoginAsync(userForLogin);
                if (token == null)
                {
                    return Unauthorized("Incorrect Email or Password");
                }

                _logger.LogInformation("Account {Email} successfully passed identity validation checks.", userForLogin.Email);

                return Ok(new Dictionary<string, string>
                {
                    { "token", token }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during user login for email {Email}", userForLogin.Email);
                return StatusCode(500, "An internal error occurred while processing your login.");
            }

        }


    }
}
