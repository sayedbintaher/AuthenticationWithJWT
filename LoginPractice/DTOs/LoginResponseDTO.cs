using LoginPractice.Model;

namespace LoginPractice.DTOs;

public class LoginResponseDTO
{
    public User user { get; set; }
    public string Token { get; set; }
}