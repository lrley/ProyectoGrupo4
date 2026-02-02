using DlaccessCore.AccesoDatos.Data.Repository.IRepository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PersonaController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PersonaController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }



        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

/*******************************************************************************************************************************************************/
        [HttpGet]
        public IActionResult Create()
        {
            PersonaVM vm = new PersonaVM()
            {
                Persona = new Persona(),
                ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo(),
                ListaTipoPersona= _contenedorTrabajo.TiposPersona.GetListaTipoPersona(),
                ListaRoles= _contenedorTrabajo.Rol.GetListaRoles(),
            };
            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonaVM vm, string fotoBase64)
        {
            if (string.IsNullOrEmpty(fotoBase64) && vm.ArchivoImagen == null)
            {
                ModelState.AddModelError(nameof(vm.ArchivoImagen), "Debe subir un archivo o tomar una foto.");
            }

            if (!ModelState.IsValid)
            {
                // Depuración: imprimir errores
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"{kvp.Key}: {error.ErrorMessage}");
                    }
                }

                vm.ListaRoles = _contenedorTrabajo.Rol.GetListaRoles();
                vm.ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo();
                vm.ListaTipoPersona = _contenedorTrabajo.TiposPersona.GetListaTipoPersona();
                TempData["Error"] = "Por favor corrija los errores del formulario.";
                return View(vm);
            }

            vm.Persona.CreatedAt = DateTime.Now;
            vm.Persona.UpdatedAt = DateTime.Now;
            vm.Persona.Estado = true;
            vm.Persona.Img = ""; // evitar NULL

            _contenedorTrabajo.Persona.Add(vm.Persona);
            _contenedorTrabajo.Save();

            var carpetaFotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos");
            if (!Directory.Exists(carpetaFotos))
                Directory.CreateDirectory(carpetaFotos);

            var nombreArchivo = $"{vm.Persona.Id}.jpg";
            var ruta = Path.Combine(carpetaFotos, nombreArchivo);

            if (!string.IsNullOrEmpty(fotoBase64))
            {
                var base64Data = fotoBase64.Split(',')[1];
                var bytes = Convert.FromBase64String(base64Data);
                await System.IO.File.WriteAllBytesAsync(ruta, bytes);
                vm.Persona.Img = "/fotos/" + nombreArchivo;
            }
            else if (vm.ArchivoImagen != null)
            {
                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await vm.ArchivoImagen.CopyToAsync(stream);
                }
                vm.Persona.Img = "/fotos/" + nombreArchivo;
            }

            _contenedorTrabajo.Persona.Update(vm.Persona);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Persona creada correctamente.";
            return RedirectToAction(nameof(Index));
        }



        #region LLamadas a la API

        public IActionResult GetAll()
        {
            var listaPersona = _contenedorTrabajo.Persona.GetAll();
            return Json(new { data = listaPersona });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Persona.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Rol" });
            }

            _contenedorTrabajo.Persona.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Rol Borrado Correctamente" });


        }



        #endregion


    }
}
