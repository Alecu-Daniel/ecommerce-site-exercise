using backend.Dtos;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(UserForRegistrationDto userForRegistration);
        Task<string?> LoginAsync(UserForLoginDto userForLogin);
    }
}
