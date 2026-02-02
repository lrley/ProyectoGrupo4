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
            // 🔴 Validación SOLO de imagen
            if (string.IsNullOrEmpty(vm.FotoBase64) && vm.ArchivoImagen == null)
            {
                ModelState.AddModelError("ArchivoImagen", "Debe subir un archivo o tomar una foto.");
            }

            ModelState.Remove("ArchivoImagen");
            ModelState.Remove("Persona.Img");

            if (!ModelState.IsValid)
            {
                vm.ListaRoles = _contenedorTrabajo.Rol.GetListaRoles();
                vm.ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo();
                vm.ListaTipoPersona = _contenedorTrabajo.TiposPersona.GetListaTipoPersona();
                return View(vm);
            }

            // ======================================================
            // 1️⃣ GUARDAR PERSONA SIN IMAGEN (para obtener el ID)
            // ======================================================
            vm.Persona.CreatedAt = DateTime.Now;
            vm.Persona.UpdatedAt = DateTime.Now;
            vm.Persona.Estado = true;
            vm.Persona.Img = "";

            _contenedorTrabajo.Persona.Add(vm.Persona);
            _contenedorTrabajo.Save();

            int personaId = vm.Persona.Id;
            string apellidos = vm.Persona.Apellido?.Trim().Replace(" ", "_");
            string fechaCreacion = vm.Persona.CreatedAt.ToString("yyyyMMdd_HHmmss");

            // ======================================================
            // 2️⃣ CONFIGURAR RUTA Y NOMBRE FIJO
            // ======================================================
            var carpetaFotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos");
            if (!Directory.Exists(carpetaFotos))
                Directory.CreateDirectory(carpetaFotos);

            string nombreArchivo = $"{apellidos}_{fechaCreacion}_{personaId}.jpg";
            string rutaCompleta = Path.Combine(carpetaFotos, nombreArchivo);

            // ======================================================
            // 3️⃣ FOTO DESDE CÁMARA
            // ======================================================
            if (!string.IsNullOrEmpty(vm.FotoBase64))
            {
                var base64Data = vm.FotoBase64.Split(',')[1];
                byte[] bytes = Convert.FromBase64String(base64Data);

                await System.IO.File.WriteAllBytesAsync(rutaCompleta, bytes);
            }
            // ======================================================
            // 4️⃣ FOTO DESDE ARCHIVO PC
            // ======================================================
            else if (vm.ArchivoImagen != null)
            {
                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await vm.ArchivoImagen.CopyToAsync(stream);
            }

            // ======================================================
            // 5️⃣ ACTUALIZAR PERSONA CON RUTA DE IMAGEN
            // ======================================================
            vm.Persona.Img = "/fotos/" + nombreArchivo;
            _contenedorTrabajo.Persona.Update(vm.Persona);
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
