using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using MuseuDeBugs.Api.DTOs.Auth;
using MuseuDeBugs.Api.Security;
using MuseuDeBugs.Api.Services;

namespace MuseuDeBugs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly LoginAttemptLimiter _loginAttemptLimiter;

        public AuthController(AuthService authService, LoginAttemptLimiter loginAttemptLimiter)
        {
            _authService = authService;
            _loginAttemptLimiter = loginAttemptLimiter;
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (_loginAttemptLimiter.IsBlocked())
            {
                return StatusCode(
                    StatusCodes.Status429TooManyRequests,
                    "Muitas tentativas de login. Espere alguns minutos e tente de novo.");
            }

            var credenciaisValidas = _authService.ValidarCredenciais(request);

            if (!credenciaisValidas)
            {
                _loginAttemptLimiter.RegisterFailure();
                return Unauthorized("Usuario ou senha invalidos.");
            }

            _loginAttemptLimiter.RegisterSuccess();

            var principal = _authService.CriarPrincipalAdmin();

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            return Ok(new MeResponse
            {
                Username = request.Username,
                IsAuthenticated = true,
                Roles = ["Admin"]
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return NoContent();
        }

        [HttpGet("me")]
        public ActionResult<MeResponse> Me()
        {
            var username = User.Identity?.Name ?? string.Empty;
            var roles = User.Claims
                .Where(claim => claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToArray();

            return Ok(new MeResponse
            {
                Username = username,
                IsAuthenticated = User.Identity?.IsAuthenticated == true,
                Roles = roles
            });
        }
    }
}
