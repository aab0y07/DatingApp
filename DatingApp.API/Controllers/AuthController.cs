using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using AutoMapper;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
           _mapper = mapper;
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        // username and password come as one whole string 
        public async Task<IActionResult> Register(UserForRegisterDTOs userForRegisterDTOs)
        {
            // validate request 
            //if (!ModelState.IsValid)
                //return BadRequest(ModelState);

           userForRegisterDTOs.Username  = userForRegisterDTOs.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDTOs.Username))
                return BadRequest("Username alredy exists");

            var userToCreate = _mapper.Map<User>(userForRegisterDTOs);

            var createdUser = await _repo.Register(userToCreate, userForRegisterDTOs.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);
            
            return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id}, userToReturn); 
        } 

        [HttpPost("login")]

        public async Task<IActionResult> Login(UserForLoginDto userForLoginDtos)
        {
            
            // here we are checking username and password 
            var userFromRepo = await _repo.Login(userForLoginDtos.Username.ToLower(), userForLoginDtos.Password);

            if(userFromRepo == null)
                return Unauthorized();

            // build Token
            var claims = new[]
            {
                //our token has 2 claims 
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));   
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject  = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();
           // create a jwt token on the base of token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);
            return Ok(new {
                // write the token into the respond to send back to client 
                token = tokenHandler.WriteToken(token),
                user
            });    
        }    

        
    }
}