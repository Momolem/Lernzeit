using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace LernzeitBackend;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? returnUrl)
    {
        var redirectUrl = string.IsNullOrEmpty(returnUrl)
            ? configuration["FrontendUrl"] ?? "http://localhost:3000"
            : returnUrl;
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out" });
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return Ok(new
            {
                IsAuthenticated = true,
                Name = User.Identity.Name,
                Email = User.FindFirstValue(ClaimTypes.Email),
                Avatar = User.FindFirstValue("picture") // Google often provides this
            });
        }

        return Ok(new { IsAuthenticated = false });
    }
}
