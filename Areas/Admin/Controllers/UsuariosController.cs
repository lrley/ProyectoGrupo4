using DlaccessCore.AccesoDatos.Data.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsuariosController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;


        public UsuariosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }



        [HttpGet]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(_contenedorTrabajo.Usuario.ObtenerTodos(usuarioActual.Value) );
           
        }



        //[HttpGet]
        //public IActionResult Bloquear(string id)
        //{
        //    if (id==null)
        //    {
        //        return NotFound();

        //    }
        //    _contenedorTrabajo.Usuario.BloquearUsuario(id);
        //    return RedirectToAction(nameof(Index));
        //}


        //[HttpGet]
        //public IActionResult Desbloquear(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();

        //    }


        //    _contenedorTrabajo.Usuario.DesbloquearUsuario(id);
        //    return RedirectToAction(nameof(Index));
        //}



        [HttpGet]
        public IActionResult Bloquear(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. Ejecutamos tu lógica de bloqueo de Identity (esto suele poner LockoutEnd en 100 años)
            _contenedorTrabajo.Usuario.BloquearUsuario(id);

            // 2. Buscamos al usuario para forzar el campo Permiso en false
            var usuario = _contenedorTrabajo.Usuario.ObtenerUsuario(id);
            if (usuario != null)
            {
                usuario.Permiso = false; // <-- Aquí forzamos el permiso
                                         // Si tu repositorio no tiene un método "Actualizar", asegúrate de que el Save guarde este cambio
                _contenedorTrabajo.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Desbloquear(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. Quitamos el bloqueo de fecha
            _contenedorTrabajo.Usuario.DesbloquearUsuario(id);

            // 2. Buscamos al usuario para forzar el campo Permiso en true
            var usuario = _contenedorTrabajo.Usuario.ObtenerUsuario(id);
            if (usuario != null)
            {
                usuario.Permiso = true; // <-- Devolvemos el permiso
                _contenedorTrabajo.Save();
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
