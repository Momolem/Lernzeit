using Lernzeit.RaumzeitAPI;
using Microsoft.AspNetCore.Mvc;

namespace LernzeitBackend
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly RaumzeitService raumzeitService;
        private readonly RaumzeitCredentials credentials;

        public UserController(IConfiguration configuration, RaumzeitService raumzeitService, RaumzeitCredentials credentials)
        {
            this.configuration = configuration;
            this.raumzeitService = raumzeitService;
            this.credentials = credentials;
        }

        [HttpGet]
        public IActionResult GetUser()
        {
            var name = configuration.GetRequiredSection("UserConfig").GetValue<string>("Name");
            
            return this.Ok(new { name = name });    
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendar()
        {
            var calendar = await this.raumzeitService.GetPersonalCalendar(credentials);
            return this.Ok(calendar);
        }
    }
}
