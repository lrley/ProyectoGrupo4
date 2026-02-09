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



        [HttpGet]
        public IActionResult Bloquear(string id)
        {
            if (id==null)
            {
                return NotFound();

            }
            _contenedorTrabajo.Usuario.BloquearUsuario(id);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Desbloquear(string id)
        {
            if (id == null)
            {
                return NotFound();

            }


            _contenedorTrabajo.Usuario.DesbloquearUsuario(id);
            return RedirectToAction(nameof(Index));
        }






    }
}
