using APIDEMO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
namespace APIDEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly TestDBContext _context;
        public RegistrationController(TestDBContext context)
        {
            _context = context;
        }
        //add user ---Registration----------
        [HttpPost("register")]
        public async Task<ActionResult<UserInfo>> AddUser(UserInfo user)
        {
            user.Password = BC.HashPassword(user.Password);
            _context.UserInfo.Add(user);
            await _context.SaveChangesAsync();
            // return CreatedAtAction("GetUsers", new { id = user.UserId }, user);
            return user;
        }

    }
}
