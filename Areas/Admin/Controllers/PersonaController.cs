using DlaccessCore.Models.Models.DatosPersonalesModels;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using DlaccessCore.AccesoDatos.Data.IRepository;


namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PersonaController : Controller
    {




        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PersonaController(IWebHostEnvironment hostingEnvironment, IContenedorTrabajo contenedorTrabajo)
        {
            _hostingEnvironment = hostingEnvironment;
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
                ListaTipoPersona= _contenedorTrabajo.TipoPersona.GetListaTipoPersona(),
                ListaRoles= _contenedorTrabajo.Rol.GetListaRoles(),
            };
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonaVM vm)
        {

            // ================= NORMALIZAR DATOS =================
            vm.Persona.Cedula = vm.Persona.Cedula?.Trim();
            vm.Persona.Email = vm.Persona.Email?.Trim().ToLower(); // 👈 email siempre minúscula

            vm.Persona.Nombre = vm.Persona.Nombre?.Trim().ToUpper();
            vm.Persona.Apellido = vm.Persona.Apellido?.Trim().ToUpper();
            vm.Persona.Direccion = vm.Persona.Direccion?.Trim().ToUpper();

            // ================= VALIDACIONES DE DUPLICADOS =================
            bool existeCedula = _contenedorTrabajo.Persona
                .GetFirstOrDefault(p => p.Cedula == vm.Persona.Cedula) != null;

            if (existeCedula)
            {
                ModelState.AddModelError("Persona.Cedula", "La cédula ya está registrada.");
            }

            bool existeEmail = _contenedorTrabajo.Persona
                .GetFirstOrDefault(p => p.Email == vm.Persona.Email) != null;

            if (existeEmail)
            {
                ModelState.AddModelError("Persona.Email", "El email ya está registrado.");
            }

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
                vm.ListaTipoPersona = _contenedorTrabajo.TipoPersona.GetListaTipoPersona();
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

/*******************************************************************************************************************************************************/



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var persona = _contenedorTrabajo.Persona.GetFirstOrDefault(p => p.Id == id);

            if (persona == null)
                return NotFound();

            PersonaVM vm = new PersonaVM
            {
                Persona = persona,
                ListaRoles = _contenedorTrabajo.Rol.GetListaRoles(),
                ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo(),
                ListaTipoPersona = _contenedorTrabajo.TipoPersona.GetListaTipoPersona()
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonaVM vm)
        {
            // ================= NORMALIZAR =================
            vm.Persona.Cedula = vm.Persona.Cedula?.Trim();
            vm.Persona.Email = vm.Persona.Email?.Trim().ToLower();

            vm.Persona.Nombre = vm.Persona.Nombre?.Trim().ToUpper();
            vm.Persona.Apellido = vm.Persona.Apellido?.Trim().ToUpper();
            vm.Persona.Direccion = vm.Persona.Direccion?.Trim().ToUpper();

            // ================= VALIDAR DUPLICADOS =================
            Console.WriteLine("ID VM: " + vm.Persona.Id);
            Console.WriteLine("ID VM (IdPersona): " + vm.Persona.Id);
            // Validar cédula
            var cedulaDuplicada = _contenedorTrabajo.Persona.GetFirstOrDefault(p =>
                p.Cedula == vm.Persona.Cedula && p.Id != vm.Persona.Id);

            if (cedulaDuplicada != null)
            {
                TempData["Error"] = "Ya existe otro usuario con esa cédula.";
                vm.ListaRoles = _contenedorTrabajo.Rol.GetListaRoles();
                vm.ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo();
                vm.ListaTipoPersona = _contenedorTrabajo.TipoPersona.GetListaTipoPersona();
                return View(vm);
            }

            // Validar email
            var emailDuplicado = _contenedorTrabajo.Persona.GetFirstOrDefault(p =>
                p.Email == vm.Persona.Email && p.Id != vm.Persona.Id);

            if (emailDuplicado != null)
            {
                TempData["Error"] = "Ya existe otro usuario con ese email.";
                vm.ListaRoles = _contenedorTrabajo.Rol.GetListaRoles();
                vm.ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo();
                vm.ListaTipoPersona = _contenedorTrabajo.TipoPersona.GetListaTipoPersona();
                return View(vm);
            }




            // ================= VALIDACIÓN IMAGEN =================
            ModelState.Remove("ArchivoImagen");
            ModelState.Remove("Persona.Img");

            // Quitar validación obligatoria de Clave
            ModelState.Remove("Persona.Clave");

            if (!ModelState.IsValid)
            {
                vm.ListaRoles = _contenedorTrabajo.Rol.GetListaRoles();
                vm.ListaSexo = _contenedorTrabajo.Sexo.GetListaSexo();
                vm.ListaTipoPersona = _contenedorTrabajo.TipoPersona.GetListaTipoPersona();
                return View(vm);
            }

            // ================= OBTENER PERSONA BD =================
            var personaDB = _contenedorTrabajo.Persona.GetFirstOrDefault(p => p.Id == vm.Persona.Id);
            if (personaDB == null)
                return NotFound();

            // 🔐 CLAVE
            if (!string.IsNullOrWhiteSpace(vm.Persona.Clave))
            {
                // Si escribió una nueva → actualizar
                personaDB.Clave = vm.Persona.Clave;
            }

            // ================= ACTUALIZAR CAMPOS =================
            personaDB.Nombre = vm.Persona.Nombre;
            personaDB.Apellido = vm.Persona.Apellido;
            personaDB.Cedula = vm.Persona.Cedula;
            personaDB.Telefono = vm.Persona.Telefono;
            personaDB.Email = vm.Persona.Email;
//            personaDB.Clave = vm.Persona.Clave;
            personaDB.IdRol = vm.Persona.IdRol;
            personaDB.IdSexo = vm.Persona.IdSexo;
            personaDB.IdTipoPersona = vm.Persona.IdTipoPersona;
            personaDB.Direccion = vm.Persona.Direccion;
            personaDB.Nacimiento = vm.Persona.Nacimiento;
            personaDB.FechaInicio = vm.Persona.FechaInicio;
            personaDB.FechaFin = vm.Persona.FechaFin;
            personaDB.UpdatedAt = DateTime.Now;

            
            personaDB.Estado = vm.Persona.Estado;
            //personaDB.Google = vm.Persona.Google;
            personaDB.Permiso = vm.Persona.Permiso;


            // ================= MANEJO DE IMAGEN =================
            if (!string.IsNullOrEmpty(vm.FotoBase64) || vm.ArchivoImagen != null)
            {
                var carpetaFotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos");
                if (!Directory.Exists(carpetaFotos))
                    Directory.CreateDirectory(carpetaFotos);

                string apellidos = personaDB.Apellido.Replace(" ", "_");
                string nombreArchivo = $"{apellidos}_{personaDB.CreatedAt:yyyyMMdd_HHmmss}_{personaDB.Id}.jpg";
                string rutaCompleta = Path.Combine(carpetaFotos, nombreArchivo);

                // 📸 Desde cámara
                if (!string.IsNullOrEmpty(vm.FotoBase64))
                {
                    var base64Data = vm.FotoBase64.Split(',')[1];
                    byte[] bytes = Convert.FromBase64String(base64Data);
                    await System.IO.File.WriteAllBytesAsync(rutaCompleta, bytes);
                }
                // 🖼️ Desde archivo
                else if (vm.ArchivoImagen != null)
                {
                    using var stream = new FileStream(rutaCompleta, FileMode.Create);
                    await vm.ArchivoImagen.CopyToAsync(stream);
                }

                personaDB.Img = "/fotos/" + nombreArchivo;
            }

            // ================= GUARDAR =================
            _contenedorTrabajo.Persona.Update(personaDB);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Persona actualizada correctamente.";
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
            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;

            // Quitar el "/" inicial de la ruta guardada en BD
            var rutaRelativa = objFromDb.Img.TrimStart('/');


            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, rutaRelativa);

                if (System.IO.File.Exists(rutaImagen))
                {
                    System.IO.File.Delete(rutaImagen);

                }



            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Persona" });
            }

            // _contenedorTrabajo.Persona.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Persona.Update(objFromDb);
            _contenedorTrabajo.Save();
            
            return Json(new { success = true, message = "Persona Borrado Correctamente" });


        }



        #endregion


    }
}
