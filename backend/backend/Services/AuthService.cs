using backend.Dtos;
using backend.Helpers;
using backend.Repositories;
using backend.Services.Interfaces;
using System.Security.Cryptography;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly AuthHelper _authHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, AuthHelper authHelper, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _authHelper = authHelper;
            _logger = logger;
        }

        public async Task<bool> RegisterAsync(UserForRegistrationDto userForRegistration)
        {
            var existingUser = await _authRepository.UserExistsAsync(userForRegistration.Email);
            if (existingUser) return false;

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);
            return await _authRepository.RegisterUserAsync(userForRegistration, passwordHash, passwordSalt);
        }

        public async Task<string?> LoginAsync(UserForLoginDto userForLogin)
        {
            var credentials = await _authRepository.GetUserCredentialsAsync(userForLogin.Email);
            if (credentials == null)
            {
                _logger.LogWarning("Failed login attempt: User email {Email} not found.", userForLogin.Email);
                return null;
            }

            byte[] computedHash = _authHelper.GetPasswordHash(userForLogin.Password, credentials.PasswordSalt);

            if (!CryptographicOperations.FixedTimeEquals(computedHash, credentials.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt: Incorrect password signature for account {Email}.", userForLogin.Email);
                return null;
            }

            int userId = await _authRepository.GetUserIdByEmailAsync(userForLogin.Email);
            return _authHelper.CreateToken(userId);
        }

    }
}
