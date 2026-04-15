using Microsoft.AspNetCore.Mvc;

namespace Lernzeit_Backend
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public UserController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetUser()
        {
            var name = configuration.GetRequiredSection("UserConfig").GetValue<string>("Name");
            
            return this.Ok(new { name = name });    
        }
    }
}
