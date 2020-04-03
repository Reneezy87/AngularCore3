using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{  
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _Repo;

        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _Repo = repo;
        }

       [HttpPost("register")]
       //public HttpResponseMessage Register(UserForRegisterDto userDto)
       public async Task<IActionResult> Register(UserForRegisterDto userDto)
       {
          // if(!ModelState.IsValid)
            //    return BadRequest(ModelState);


           userDto.Username = userDto.Username.ToLower();

           if (await _Repo.UserExists(userDto.Username))
           return BadRequest("User name already exists");


           var userToCreate = new User
           {
               Username = userDto.Username
           };

           var createUser = await _Repo.Register(userToCreate, userDto.Password);

           return StatusCode(201);

       }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserForLoginDto userLogin)
        {
            var userFromRepo = await _Repo.Login(userLogin.UserName.ToLower(), userLogin.Password);

            if(userFromRepo == null)
                return Unauthorized();

            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });


        }

    }
}