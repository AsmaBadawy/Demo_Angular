using APIDEMO.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;


namespace APIDEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("allowcors")]

    public class TokenController : ControllerBase
    {
        public IConfiguration _configration;
        private readonly TestDBContext _context;
        public TokenController(IConfiguration configration, TestDBContext context)
        {
            _configration = configration;
            _context = context;
        }
        [HttpPost]
        public  async Task<IActionResult> Post(UserInfo _userInfo)
            
        {
            
            if (_userInfo!=null&& _userInfo.UserName!=null&& _userInfo.Password!=null)
            {
                // var User = await GetUser(_userInfo.UserName, _userInfo.Password);

                //var User = _context.UserInfo.SingleOrDefault(x => x.UserName == _userInfo.UserName);
                //or 
                var User = await CheckUser(_userInfo.UserName);
                if (User!=null && BC.Verify(_userInfo.Password, User.Password))
                {
                    var claims = new[]
                    {
                    new Claim (JwtRegisteredClaimNames.Sub,_configration["Jwt:Subject"]),
                    new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim  (JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                    new Claim  ("id",User.UserId.ToString()),
                    new Claim  ("FirstName",User.FirstName),
                    new Claim  ("LastName",User.LastName),
                    new Claim  ("UserName",User.UserName)

                    };

                    var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configration["Jwt:Key"]));
                    var SignIn = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(_configration["Jwt:Issuer"], _configration["Jwt:Audience"],claims,expires:DateTime.UtcNow.AddDays(1),signingCredentials:SignIn);
                    var Token = new JwtSecurityTokenHandler().WriteToken(token);
                    //return token
                    return Ok(new { Token,User.UserId } );
                }
                else
                {
                    return BadRequest("invalid username or password");
                }
            }
            else
            {
                return BadRequest();

            }
        }

        //    private async Task<UserInfo> GetUser(string userName, string password)
        //   {
        //      return await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
        //}

          private async Task<UserInfo> CheckUser(string userName)
          {
              return await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == userName);
       }
    }
}
