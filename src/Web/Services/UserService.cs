using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using System.Security.Claims;

namespace RestoreMonarchy.PaymentGateway.Web.Services
{
    public class UserService
    {
        public async Task SignInAsync(HttpContext httpContext, MUser user, bool isPersistent)
        {
            ClaimsIdentity identity = new(GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);            

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() 
            { 
                AllowRefresh = true,
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddDays(3)
            });
        }

        public async Task SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
        }

        private IEnumerable<Claim> GetUserClaims(MUser user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
            return claims;
        }
    }
}
