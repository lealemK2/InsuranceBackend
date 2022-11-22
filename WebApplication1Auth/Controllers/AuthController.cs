using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Insurance.Data;
using Insurance.Dto;
using Insurance.Model;

namespace Insurance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthController(IConfiguration configuration, IUserRepository users)
        {
            _configuration = configuration;
            _userRepository = users;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAsync()
        {
            List<User> result= await _userRepository.Get();
            if (result == null)
            {
                return BadRequest("Unfortunately there are no registered users");
            }
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            return Ok(await _userRepository.Register(request));
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserDto request)
        {
            var user = await _userRepository.Login(request);
            if (user == null)
            {
                return BadRequest("User doesn't exist");
            }

            if (!request.CorrectPassword)
            {
                return BadRequest("Password doesn't match");
            }
            return Ok( user);
        }              
    }
}
