using LoginPractice.DTOs;
using LoginPractice.Model;

namespace LoginPractice.Services;

public interface IUserService
{
    bool isUniqueUser(string email);
    LoginResponseDTO Login(LoginRequestDTO loginRequestDto);
    SaveVM Register(RegistrationRequestDTO registrationRequestDto);
}