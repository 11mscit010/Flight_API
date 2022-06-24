using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserService.Entity;
using UserService.Model;
using UserService.Service;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //public static UserEntity userEntity = new UserEntity();

        public IConfiguration Configuration { get; }
        private readonly IUserRepository userRepository;
        private readonly AppDbContext context;
        public UserController(IConfiguration configuration, AppDbContext _context)
        {
            Configuration = configuration;
            context = _context;
            userRepository = new UserRepository(context);
        }

        [HttpPost("Register")]
        public ActionResult<UserModel> Register(UserModel input)
        {
            try
            {
                var user = new UserEntity();
                user.UserName = input.UserName;
                CreatePasswordHash(input.Password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;
                user.Role = (int)UserRoleEnum.User;
                user.Name = input.Name;
                user.Gender = input.Gender;
                user.DOB = input.DOB;
                //Register user 
                var result = userRepository.RegisterUser(user);
                if (result == 0)
                    return Ok("0");

                userRepository.SaveChanges();
                return Ok("1");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public ActionResult<string> Login(UserModel input)
        {
            var user = userRepository.GetUserByUsername(input.UserName);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }

            if (!VerifyPasswordHash(input.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong Password");
            }

            var token = GenerateToken(user);

            var refToken = GenerateRefreshToken();
            SetRefreshToken(refToken);

            return Ok(token);
        }

        //[HttpPost("Refresh-Token")]
        //public ActionResult<string> RefreshToken()
        //{
        //    var refToken = Request.Cookies["refreshToken"];

        //    if (!user.RefreshToken.Equals(refToken))
        //    {
        //        return Unauthorized("Invalid Refresh Token");
        //    }
        //    else if (user.TokenExpires <= DateTime.Now)
        //    {
        //        return Unauthorized("Token Expires");
        //    }

        //    var logModel = new UserModel() { UserName = user.UserName, Password = string.Empty };
        //    var token = GenerateToken(logModel);

        //    var newRefToken = GenerateRefreshToken();
        //    SetRefreshToken(newRefToken);

        //    return Ok(token);
        //}

        #region Private Methods
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private RefreshToken GenerateRefreshToken()
        {
            var token = BitConverter.GetBytes(new Random().Next());
            return new RefreshToken()
            {
                Token = Convert.ToBase64String(token),
                Created = DateTime.Now,
                Expires = DateTime.Now.AddDays(1)
            };
        }

        private void SetRefreshToken(RefreshToken refToken)
        {
            //    var cookieOption = new CookieOptions
            //    {
            //        HttpOnly = true,
            //        Expires = refToken.Expires
            //    };

            //    Response.Cookies.Append("refreshToken", refToken.Token, cookieOption);

            //    user.RefreshToken = refToken.Token;
            //    user.TokenCreated = refToken.Created;
            //    user.TokenExpires = refToken.Expires;
        }

        private string GenerateToken(UserEntity input)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, input.UserName),
                new Claim(ClaimTypes.Role, input.Role == 1 ? "Admin" : "User"),
                new Claim("UserRole", input.Role == 1 ? "Admin" : "User"),
                new Claim("UserId", input.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds, issuer: Configuration["Jwt:Issuer"], audience: Configuration["Jwt:Audience"]);

            var secureToken = new JwtSecurityTokenHandler().WriteToken(token);

            return secureToken;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var cHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return cHash.SequenceEqual(passwordHash);
            }
        }
        #endregion

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Test")]
        public string test()
        {
            return "test";
        }
    }
}
