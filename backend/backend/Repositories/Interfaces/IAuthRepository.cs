using backend.Dtos;

namespace backend.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> UserExistsAsync(string email);
        Task<bool> RegisterUserAsync(UserForRegistrationDto userForRegistration, byte[] passwordHash, byte[] passwordSalt);
        Task<UserForLoginConfirmationDto?> GetUserCredentialsAsync(string email);
        Task<int> GetUserIdByEmailAsync(string email);
    }
}