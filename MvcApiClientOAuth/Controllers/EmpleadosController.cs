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
            
            Empleado emp = await
                this.service.GetPerfilAsync();
            return View(emp);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> CompisCurro()
        {
            
            List<Empleado> emps = await
                this.service.GetCompisAsync();
            return View(emps);
        }

        public async Task<IActionResult> EmpleadosOficio()
        {
            List<string> oficios = await
                this.service.GetOficiosAsync();
            ViewData["OF"] = oficios;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio
            (int? incremento, List<string> oficio,string accion)
        {
            List<string> oficios = await
               this.service.GetOficiosAsync();
            ViewData["OF"] = oficios;

            if(accion.ToLower() == "update")
            {
                await this.service.SubirSalarioAsync(incremento.Value, oficio);
            }
            List<Empleado> empleados = await
                this.service.GetEmpleadosOficioAsync(oficio);

            return View(empleados);
        }
    }
}
