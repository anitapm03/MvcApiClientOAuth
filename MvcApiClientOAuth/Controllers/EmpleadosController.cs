using Microsoft.AspNetCore.Mvc;
using MvcApiClientOAuth.Filters;
using MvcApiClientOAuth.Models;
using MvcApiClientOAuth.Services;

namespace MvcApiClientOAuth.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;
        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }
        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await
                this.service.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> Details
            (int id)
        {
            string token = HttpContext.Session.GetString("TOKEN");
            if(token == null)
            {
                ViewData["MSG"] = "Validate";
                return View();
            }
            else
            {
                Empleado emp = await
                this.service.FindEmpleadoAsync(id, token);
                return View(emp);
            }
        }
    }
}
