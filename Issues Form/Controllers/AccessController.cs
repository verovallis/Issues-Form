using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Issues_Form.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Issues_Form.Models
{
    public class AccessController : Controller 
    {
        public IActionResult Login()
        {
            //check if the user is already logged in
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity != null && claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Create", "Form");

            return View();
        }

        [HttpPost]
public async Task<IActionResult> Login(VMLogin modelLogin)
{
    if(modelLogin.Email == "admin@inchcape.co.id" && 
        modelLogin.Password == "Admin1234$")
    {
        // For admin login
        List<Claim> claims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        AuthenticationProperties properties = new AuthenticationProperties() {
            AllowRefresh = true,
            IsPersistent = modelLogin.KeepLoggedIn
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), properties);

        return RedirectToAction("Edit", "Form");
    }
    else if(modelLogin.Email == "user@inchcape.co.id" &&
        modelLogin.Password == "Inchcape1234$")
    {
        // For regular user login
        List<Claim> claims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
            new Claim("OtherProperties", "Example Role" )
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        AuthenticationProperties properties = new AuthenticationProperties() {
            AllowRefresh = true,
            IsPersistent = modelLogin.KeepLoggedIn
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), properties);

        return RedirectToAction("Create", "Form");
    }
    else
    {
        ViewData["ValidateMessage"] = "user not found";
        return View();
    }
}
    }
}
