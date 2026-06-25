using System.Security.Claims;
using FunicularSwitch;
using Lernzeit.Application.Contracts;
using Lernzeit.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace LernzeitBackend.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration, IUserRepository userRepository) : ControllerBase
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
    public async Task<IActionResult> GetCurrentUser()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToOption();
            return await googleId.Match(async g =>
                {
                    await userRepository.CreateUserIfNotExists(new GoogleUserId(g), User.Identity.Name ?? string.Empty);

                    return Ok(new
                    {
                        IsAuthenticated = true,
                        Name = User.Identity.Name,
                        Email = User.FindFirstValue(ClaimTypes.Email),
                        Avatar = User.FindFirstValue("picture")
                    });
                },
                () => Ok(new { IsAuthenticated = false })
            );
        }

        return Ok(new { IsAuthenticated = false });
    }
}
