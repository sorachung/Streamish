using System;
using Microsoft.AspNetCore.Mvc;
using Streamish.Repositories;
using Streamish.Models;

namespace Streamish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _userProfilerepository;

        public UserProfileController(IUserProfileRepository userProfileRepository)
        {
            _userProfilerepository = userProfileRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userProfilerepository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _userProfilerepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post(UserProfile user)
        {
            _userProfilerepository.Add(user);
            return CreatedAtAction("Get", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, UserProfile user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _userProfilerepository.Update(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userProfilerepository.Delete(id);
            return NoContent();
        }
    }
}
