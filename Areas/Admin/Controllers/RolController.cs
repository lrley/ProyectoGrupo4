using DlaccessCore.AccesoDatos.Data.Repository.IRepository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;

        public RolController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        /**********************************************************CREAR***************************************************************************************/

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rol rol)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Convertir a mayúsculas antes de validar/guardar
                    rol.NombreRol = rol.NombreRol.ToUpper();

                    // Validar si ya existe un rol con el mismo nombre
                    var existeRol = _contenedorTrabajo.Rol.GetFirstOrDefault(r => r.NombreRol == rol.NombreRol);
                    if (existeRol != null)
                    {
                        TempData["Error"] = "El rol ya existe, no se puede grabar.";
                        return View(rol); // Devuelve la vista con el mensaje
                    }


                    // Inicializar valores de auditoría
                    rol.CreatedAt = DateTime.Now;
                    rol.UpdatedAt = DateTime.Now;
                    rol.Estado = true; // por defecto activo

                    // Guardar en base de datos
                    _contenedorTrabajo.Rol.Add(rol);
                    _contenedorTrabajo.Save();

                    TempData["Success"] = "Rol creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Registrar error (puedes usar logger)
                    ModelState.AddModelError("", $"Error al crear el rol: {ex.Message}");
                }
            }

            // Si hay errores, devolver la vista con el modelo para que se mantenga lo escrito
            return View(rol);
        }

        /*************************************************************EDITAR************************************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Rol rol = new Rol();

            rol = _contenedorTrabajo.Rol.Get(id);

            if(rol==null)
            {
                return NotFound();
            }

            return View(rol);
            
        }


        /*        [HttpPost]
                [ValidateAntiForgeryToken]
                public IActionResult Edit(Rol rol)
                {
                    if (ModelState.IsValid)
                    {

                        // Convertir a mayúsculas antes de validar/guardar
                        rol.NombreRol = rol.NombreRol.ToUpper();
                        _contenedorTrabajo.Rol.Update(rol);
                        _contenedorTrabajo.Save();
                        TempData["Success"] = "Rol actualizado correctamente.";
                        return RedirectToAction(nameof(Index));

                    }


                    return View(rol);
                }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Rol rol)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, se devuelve la vista con los errores
                return View(rol);
            }

            try
            {
                // Convertir a mayúsculas antes de guardar
                rol.NombreRol = rol.NombreRol.ToUpper();

                // Verificar si el rol existe en la base de datos
                var rolExistente = _contenedorTrabajo.Rol.Get(rol.IdRol);
                if (rolExistente == null)
                {
                    TempData["Error"] = "El rol no existe.";
                    return RedirectToAction(nameof(Index));
                }

                // Validar si el nuevo nombre ya está en uso por otro rol
                var duplicado = _contenedorTrabajo.Rol.GetFirstOrDefault(r =>
                    r.NombreRol == rol.NombreRol && r.IdRol != rol.IdRol);

                if (duplicado != null)
                {
                    TempData["Error"] = "Ya existe otro rol con ese nombre.";
                    return View(rol);
                }

                // Actualizar campos
                rolExistente.NombreRol = rol.NombreRol;
                rolExistente.Estado = rol.Estado;
                rolExistente.UpdatedAt = DateTime.Now;

                _contenedorTrabajo.Rol.Update(rolExistente);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Rol actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el rol: {ex.Message}");
                return View(rol);
            }
        }


        /**********************************************************************REGION A LA API***************************************************************************/

        
        
        
        
        
        
        #region LLamadas a la API

        public IActionResult GetAll()
        {
            var listaRoles = _contenedorTrabajo.Rol.GetAll();
            return Json(new { data = listaRoles });
        }

        [HttpDelete]
        public IActionResult Delete(int id) {

            var objFromDb = _contenedorTrabajo.Rol.Get(id);
            if (objFromDb == null){
                return Json(new { success = false, message="Error Borrando Rol" });
            }

            _contenedorTrabajo.Rol.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Rol Borrado Correctamente" });


        }

        

        #endregion








    }
}
