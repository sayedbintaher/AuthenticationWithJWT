using LoginPractice.DTOs;
using LoginPractice.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginPractice.Controllers;

[ApiController]
[Route("api/[controller]")]
public class User : ControllerBase
{
    private readonly IUserService _userService;
    public User(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDTO loginDto) 
    {
        var loginReponse = _userService.Login(loginDto);
        if(loginReponse.user == null || string.IsNullOrEmpty(loginReponse.Token))
        {
            return BadRequest(new { message = "Username or Password is incorrect" });
        }
        return Ok(loginReponse);
    }


    [HttpPost("register")]
    public IActionResult Register([FromBody] RegistrationRequestDTO userDto)
    {
        var result = _userService.Register(userDto);
        return Ok(result);
    }

}