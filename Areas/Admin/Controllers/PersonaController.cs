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
            Console.WriteLine(">>> Entrando a Create Persona");
            Console.WriteLine($"ArchivoImagen: {(vm.ArchivoImagen != null ? vm.ArchivoImagen.FileName : "VACÍO")}");
            Console.WriteLine($"FotoBase64: {(string.IsNullOrEmpty(fotoBase64) ? "VACÍO" : "CONTIENE DATOS")}");

            // Validación: debe venir archivo o foto
            if (string.IsNullOrEmpty(fotoBase64) && vm.ArchivoImagen == null)
            {
                ModelState.AddModelError(nameof(vm.ArchivoImagen), "Debe subir un archivo o tomar una foto.");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine(">>> ModelState inválido");
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Campo: {kvp.Key} - Error: {error.ErrorMessage}");
                    }
                }

                TempData["Error"] = "Por favor corrija los errores del formulario.";

                // 🔁 Recargar listas porque llegan null en el POST
                vm.ListaRoles = _contenedorTrabajo.Rol.GetListaRoles();
                vm.ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo();
                vm.ListaTipoPersona = _contenedorTrabajo.TiposPersona.GetListaTipoPersona();

                return View(vm);
            }

            // Datos base de la Persona
            vm.Persona.CreatedAt = DateTime.Now;
            vm.Persona.UpdatedAt = DateTime.Now;
            vm.Persona.Estado = true;

            // Carpeta de fotos
            var carpetaFotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos");
            if (!Directory.Exists(carpetaFotos))
                Directory.CreateDirectory(carpetaFotos);

            string nombreArchivo = "";

            // 📸 Foto desde cámara
            if (!string.IsNullOrEmpty(fotoBase64))
            {
                nombreArchivo = Guid.NewGuid().ToString() + ".jpg";
                var ruta = Path.Combine(carpetaFotos, nombreArchivo);
                Console.WriteLine($">>> Guardando foto cámara en: {ruta}");

                var base64Data = fotoBase64.Split(',')[1];
                var bytes = Convert.FromBase64String(base64Data);
                await System.IO.File.WriteAllBytesAsync(ruta, bytes);

                Console.WriteLine(">>> Foto cámara guardada en disco");
                vm.Persona.Img = "/fotos/" + nombreArchivo;
            }
            // 🖼️ Archivo subido desde PC
            else if (vm.ArchivoImagen != null)
            {
                nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(vm.ArchivoImagen.FileName);
                var ruta = Path.Combine(carpetaFotos, nombreArchivo);
                Console.WriteLine($">>> Guardando archivo PC en: {ruta}");

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await vm.ArchivoImagen.CopyToAsync(stream);
                }

                Console.WriteLine(">>> Archivo PC guardado en disco");
                vm.Persona.Img = "/fotos/" + nombreArchivo;
            }
            else
            {
                vm.Persona.Img = "";
            }

            // Guardar Persona en BD
            _contenedorTrabajo.Persona.Add(vm.Persona);
            _contenedorTrabajo.Save();

            Console.WriteLine($">>> Persona guardada con ID: {vm.Persona.Id}, Img: {vm.Persona.Img}");

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
