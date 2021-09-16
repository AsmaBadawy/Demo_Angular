using APIDEMO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace APIDEMO.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersInfoController : ControllerBase
    {
        private readonly TestDBContext _context;
        public UsersInfoController(TestDBContext context)
        {
            _context = context;
        }
        //get all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUsers()
        {

            return await _context.UserInfo.ToListAsync();
        }
        //get user by id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfo>> GetUser(int id)
        {
            var user = _context.UserInfo.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return await user;
        }

        // this way or another way in another controller
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserInfo>> AddUser(UserInfo user)
        {
            user.Password = BC.HashPassword(user.Password);
            _context.UserInfo.Add(user);
            await _context.SaveChangesAsync();
            // return CreatedAtAction("GetUsers", new { id = user.UserId }, user);
            return user;
        }
        //edit user
        [HttpPut("{id}")]

        public async Task<ActionResult<UserInfo>> EditUser(int id, UserInfo user)
        {
            if (id != user.UserId)
            {
                return BadRequest("id not equal userid");
            }
            if (!UsersExist(id))
            {
                return BadRequest("user not exist");
            }

            try
            {
               _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message} __ {ex.InnerException.Message} __ {ex.InnerException.InnerException.Message}");
            }
            return user;
        }

        private bool UsersExist(int id)
        {
            return _context.UserInfo.Any(user => user.UserId == id);
        }

       
        // delete 
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserInfo>> DeleteUser(int id)
        {
            var user = await _context.UserInfo.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.UserInfo.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

    }
}
