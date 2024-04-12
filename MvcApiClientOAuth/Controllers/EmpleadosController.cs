using Microsoft.AspNetCore.Mvc;
using MvcApiClientOAuth.Filters;
using MvcApiClientOAuth.Models;
using MvcApiClientOAuth.Services;
using System.Security.Claims;

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

        [AuthorizeEmpleados]
        public async Task<IActionResult> Details
            (int id)
        {
            Empleado emp = await
                this.service.FindEmpleadoAsync(id);
            return View(emp);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Perfil()
        {
            var data =
                HttpContext.User.FindFirst
                (x => x.Type == ClaimTypes.NameIdentifier).Value;
            int id = int.Parse(data);
            Empleado emp = await
                this.service.FindEmpleadoAsync(id);
            return View(emp);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> CompisCurro()
        {
            //necesito el id delp
            var data =
                HttpContext.User.FindFirst
                (x => x.Type == "IDDEPARTAMENTO").Value;
            int idDept = int.Parse(data);
            Empleado emp = await
                this.service.FindCompisAsync(id);
            return View(emp);
        }
    }
}
