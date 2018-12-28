
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApplication.API.Data;
using DatingApplication.API.Dtos;
using DatingApplication.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApplication.API.Controllers

{
      [Route("api/[controller]")]
      [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly iAuthRepository _repo;
        public readonly IConfiguration _config;
    
      public AuthController(iAuthRepository repo,IConfiguration config)
      {
          _config=config;
          _repo=repo;
      }

      [HttpPost("register")]
      
        public async Task<IActionResult> Register(UserToRegisterDto userForRegisterDto)
        {
            //validate request
            userForRegisterDto.Username=userForRegisterDto.Username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username Already Exist");
            
            var userToCreate=new User
            {
                Username=userForRegisterDto.Username
            };
            var createdUser= await _repo.Register(userToCreate,userForRegisterDto.password);

            return StatusCode(201);
            
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo= 
                await _repo.Login(userForLoginDto.Username.ToLower(),userForLoginDto.Password);
            if(userFromRepo==null)
                return Unauthorized();

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username.ToString()),
            };
            var key =new SymmetricSecurityKey(System.Text.Encoding.UTF8.
                GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds =new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject =new ClaimsIdentity(claims),
                Expires =DateTime.Now.AddDays(1),
                SigningCredentials=creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token=tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{
                token=tokenHandler.WriteToken(token)
            });
        }
    }
}