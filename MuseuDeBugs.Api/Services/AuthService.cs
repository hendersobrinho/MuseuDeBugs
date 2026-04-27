using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MuseuDeBugs.Api.DTOs.Auth;
using MuseuDeBugs.Api.Options;

namespace MuseuDeBugs.Api.Services
{
    public class AuthService
    {
        private const string AdminRole = "Admin";
        private readonly AdminOptions _adminOptions;
        private readonly PasswordHasher<string> _passwordHasher = new();

        public AuthService(IOptions<AdminOptions> adminOptions)
        {
            _adminOptions = adminOptions.Value;
        }

        public bool ValidarCredenciais(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(_adminOptions.Username) ||
                string.IsNullOrWhiteSpace(_adminOptions.PasswordHash))
            {
                return false;
            }

            if (request.Username != _adminOptions.Username)
            {
                return false;
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                _adminOptions.Username,
                _adminOptions.PasswordHash,
                request.Password);

            return resultado == PasswordVerificationResult.Success ||
                   resultado == PasswordVerificationResult.SuccessRehashNeeded;
        }

        public ClaimsPrincipal CriarPrincipalAdmin()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, _adminOptions.Username),
                new(ClaimTypes.Role, AdminRole)
            };

            var identity = new ClaimsIdentity(claims, "AdminCookie");

            return new ClaimsPrincipal(identity);
        }
    }
}