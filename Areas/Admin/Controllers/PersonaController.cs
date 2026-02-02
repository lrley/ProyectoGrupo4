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
        public async Task<IActionResult> Create(PersonaVM vm)
        {
            Console.WriteLine(">>> Entrando a Create Persona");

            vm.Persona ??= new Persona();

            // ❌ quitar validaciones que no aplican
            ModelState.Remove("ArchivoImagen");
            ModelState.Remove("Persona.Img");

            // ✅ VALIDACIÓN REAL
            if (string.IsNullOrEmpty(vm.FotoBase64) && vm.ArchivoImagen == null)
            {
                ModelState.AddModelError("ArchivoImagen", "Debe subir un archivo o tomar una foto.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor corrija los errores del formulario.";

                vm.ListaRoles = _contenedorTrabajo.Rol.GetListaRoles();
                vm.ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo();
                vm.ListaTipoPersona = _contenedorTrabajo.TiposPersona.GetListaTipoPersona();

                return View(vm);
            }

            vm.Persona.CreatedAt = DateTime.Now;
            vm.Persona.UpdatedAt = DateTime.Now;
            vm.Persona.Estado = true;

            var carpetaFotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos");
            Directory.CreateDirectory(carpetaFotos);

            string nombreArchivo;

            // 📸 cámara
            if (!string.IsNullOrEmpty(vm.FotoBase64))
            {
                nombreArchivo = Guid.NewGuid() + ".jpg";
                var ruta = Path.Combine(carpetaFotos, nombreArchivo);

                var base64Data = vm.FotoBase64.Split(',')[1];
                var bytes = Convert.FromBase64String(base64Data);
                await System.IO.File.WriteAllBytesAsync(ruta, bytes);

                vm.Persona.Img = "/fotos/" + nombreArchivo;
            }
            // 🖼️ archivo
            else
            {
                nombreArchivo = Guid.NewGuid() + Path.GetExtension(vm.ArchivoImagen.FileName);
                var ruta = Path.Combine(carpetaFotos, nombreArchivo);

                using var stream = new FileStream(ruta, FileMode.Create);
                await vm.ArchivoImagen.CopyToAsync(stream);

                vm.Persona.Img = "/fotos/" + nombreArchivo;
            }

            _contenedorTrabajo.Persona.Add(vm.Persona);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Persona creada correctamente.";
            return RedirectToAction(nameof(Index));
        }






        #region LLamadas a la API

        public IActionResult GetAll()
        {
            var listaPersona = _contenedorTrabajo.Persona.GetAll(includeProperties: "Rol,Sexo,TipoPersona");
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
