using EntityFrameworkCore.RawSQLExtensions.Extensions;
using LoginPractice.DataContext;
using LoginPractice.DTOs;
using LoginPractice.Model;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginPractice.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;
    private string secretKey;

    public UserService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:Key");
    }

    public bool isUniqueUser(string email)
    {
        var user = _db.Database.SqlQuery<User>("GetUserByEmail_sp '" + email + "'").FirstOrDefault();
        if (user == null)
        {
            return true;
        }
        return false;
    }

    public LoginResponseDTO Login(LoginRequestDTO loginRequestDto)
    {
        var user = _db.Database.SqlQuery<User>("Aunthentication_sp '" + loginRequestDto.Email + "','" + loginRequestDto.Password + "'").FirstOrDefault();
        if (user == null)
        {
            return new LoginResponseDTO
            {
                Token = "",
                user = null
            };
        }

        //Generate JWT Token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name,user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new ( new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDTO loginResponseDto = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
            user = user
        };
        return loginResponseDto;
    }   

    public SaveVM Register(RegistrationRequestDTO registrationRequestDto)
    {
        if (isUniqueUser(registrationRequestDto.Email))
        {
            User user = new User()
            {
                Email = registrationRequestDto.Email,
                Password = registrationRequestDto.Password,
                Name = registrationRequestDto.Name,
                Role = registrationRequestDto.Role
            };
            var result = _db.Database.SqlQuery<SaveVM>("AddUpdateUser_sp '" + registrationRequestDto.Email + "','" +
                                                        registrationRequestDto.Name + "','" +
                                                        registrationRequestDto.Password + "','" +
                                                        registrationRequestDto.Role + "'").FirstOrDefault();
            user.Password = "";
            return result;
        }
        else
        {
            SaveVM res = new SaveVM()
            {
                ID = -1,
                Message = "User Exist Already",
                IsSuccess = false
            };
            return res;
        }
    }
}