using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using United.Challenge.Api.Models;


namespace United.Challenge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUser request)
        {
            return Created("api/users/123", new { message = "User registered successfully" });
        }
    }
}
