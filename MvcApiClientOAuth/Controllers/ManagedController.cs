using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcApiClientOAuth.Models;
using MvcApiClientOAuth.Services;
using System.Security.Claims;

namespace MvcApiClientOAuth.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceEmpleados service;
        public ManagedController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> 
            Login(LoginModel model)
        {
            string token = await this.service
                .GetTokenAsync(model.UserName, model.Password);
            if(token == null)
            {
                ViewData["MSG"] = "Usuario/Password incorrectos";
                return View();
            }
            else
            {
                ViewData["MSG"] = "Ya tienes tu token";
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, ClaimTypes.Role);
                //almacenamos el nombre de usuario
                identity.AddClaim(new Claim
                    (ClaimTypes.Name, model.UserName));
                //almacenamos el id del usuario
                identity.AddClaim(new Claim
                    (ClaimTypes.NameIdentifier, model.Password));

                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);

                //damos de alta al user indicando que esta 
                //validado por 30 min
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });

                return RedirectToAction("Index", "Home");
            }
            
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
