using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Issues_Form.Models;
using Microsoft.AspNetCore.Authorization;

namespace Issues_Form.Controllers // Change the namespace from Models to Controllers
{
    public class AccessController : Controller // Change the class namespace accordingly
    {
        // Only users with the "Admin" role can access the "Index" page
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        // All authenticated users can access the "Create" page
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Login()
        {
            // Check if the user is already logged in
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity != null && claimUser.Identity.IsAuthenticated)
                if (claimUser.IsInRole("Admin"))
                    return RedirectToAction("Edit", "Form");
                else if (claimUser.IsInRole("User"))
                    return RedirectToAction("Create", "Form");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {
            if (modelLogin.Email == "admin@inchcape.co.id" &&
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
                AuthenticationProperties properties = new AuthenticationProperties()
                {
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
            new Claim(ClaimTypes.Role, "User" )
        };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
                AuthenticationProperties properties = new AuthenticationProperties()
                {
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
    [Authorize]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    public IActionResult RedirectToAccessDenied()
    {
        return RedirectToAction("AccessDenied", "Form");
    }

    }
}