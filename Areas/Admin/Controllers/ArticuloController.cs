using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using DlaccessCore.Models.Models.ProductoModel;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticuloController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;


        public ArticuloController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        /*****************************************************************************************************************/

        /*******************************************************************************************************************************************************/
        [HttpGet]
        public IActionResult Create()
        {
            ArticuloVM vm = new ArticuloVM()
            {
                Articulo = new Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias(),
            
            };
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticuloVM vm)
        {

            // ================= NORMALIZAR DATOS =================
            vm.Articulo.Nombre = vm.Articulo.Nombre?.Trim();
            vm.Articulo.Descripcion = vm.Articulo.Descripcion?.Trim().ToLower(); 

            // ================= VALIDACIONES DE DUPLICADOS =================
            bool existeArticulo = _contenedorTrabajo.Articulo
                .GetFirstOrDefault(p => p.Nombre == vm.Articulo.Nombre) != null;

            if (existeArticulo)
            {
                ModelState.AddModelError("Articulo.Nombre", "El articulo ya está registrado.");
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
                vm.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                return View(vm);
            }

            // ======================================================
            // 1️⃣ GUARDAR PERSONA SIN IMAGEN (para obtener el ID)
            // ======================================================
            vm.Articulo.CreatedAt = DateTime.Now;
            vm.Articulo.UpdatedAt = DateTime.Now;
            vm.Articulo.Estado = true;
            vm.Articulo.urlImagen = "";

            _contenedorTrabajo.Articulo.Add(vm.Articulo);
            _contenedorTrabajo.Save();

            int articuloId = vm.Articulo.Id;
            string articulo = vm.Articulo.Nombre?.Trim().Replace(" ", "_");
            string fechaCreacion = vm.Articulo.CreatedAt.ToString("yyyyMMdd_HHmmss");

            // ======================================================
            // 2️⃣ CONFIGURAR RUTA Y NOMBRE FIJO
            // ======================================================
            var carpetaFotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotosArticulo");
            if (!Directory.Exists(carpetaFotos))
                Directory.CreateDirectory(carpetaFotos);

            string nombreArchivo = $"{articulo}_{fechaCreacion}_{articuloId}.jpg";
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
            vm.Articulo.urlImagen = "/fotosArticulo/" + nombreArchivo;
            _contenedorTrabajo.Articulo.Update(vm.Articulo);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Articulo creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        /*******************************************************************************************************************************************************/


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var articulo = _contenedorTrabajo.Articulo.GetFirstOrDefault(p => p.Id == id);

            if (articulo == null)
                return NotFound();

            ArticuloVM vm = new ArticuloVM
            {
                Articulo = articulo,
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias(),

            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticuloVM vm)
        {
            // ================= NORMALIZAR =================
            vm.Articulo.Nombre = vm.Articulo.Nombre?.Trim().ToUpper();
            vm.Articulo.Descripcion = vm.Articulo.Descripcion?.Trim().ToUpper();



            // ================= VALIDAR DUPLICADOS =================
            Console.WriteLine("ID VM: " + vm.Articulo.Id);
            Console.WriteLine("ID VM (ArticuloId): " + vm.Articulo.Id);
            // Validar cédula
            var ArticuloDuplicado = _contenedorTrabajo.Articulo.GetFirstOrDefault(p =>
                p.Nombre == vm.Articulo.Nombre && p.Id != vm.Articulo.Id);

            if (ArticuloDuplicado != null)
            {
                TempData["Error"] = "Ya existe otro Articulo con ese Nombre.";
                vm.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                return View(vm);
            }


            // ================= VALIDACIÓN IMAGEN =================
            ModelState.Remove("ArchivoImagen");
            ModelState.Remove("Articulo.urlImagen");

            if (!ModelState.IsValid)
            {
                vm.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                return View(vm);
            }

            // ================= OBTENER PERSONA BD =================
            var ArticuloDB = _contenedorTrabajo.Articulo.GetFirstOrDefault(p => p.Id == vm.Articulo.Id);
            if (ArticuloDB == null)
                return NotFound();

            // ================= ACTUALIZAR CAMPOS =================
            ArticuloDB.Nombre = vm.Articulo.Nombre;
            ArticuloDB.Descripcion = vm.Articulo.Descripcion;
            ArticuloDB.CategoriaId = vm.Articulo.CategoriaId;
            ArticuloDB.FechaCreacion = vm.Articulo.FechaCreacion;
            ArticuloDB.UpdatedAt = DateTime.Now;
            ArticuloDB.Estado = vm.Articulo.Estado;
          

            // ================= MANEJO DE IMAGEN =================
            if (!string.IsNullOrEmpty(vm.FotoBase64) || vm.ArchivoImagen != null)
            {
                var carpetaFotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotosArticulo");
                if (!Directory.Exists(carpetaFotos))
                    Directory.CreateDirectory(carpetaFotos);

                // 🔴 Eliminar la imagen anterior si existe
                if (!string.IsNullOrEmpty(ArticuloDB.urlImagen))
                {
                    string rutaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ArticuloDB.urlImagen.TrimStart('/'));
                    if (System.IO.File.Exists(rutaAnterior))
                    {
                        System.IO.File.Delete(rutaAnterior);
                    }
                }



                string nombreArticulo = ArticuloDB.Nombre.Replace(" ", "_");
                string fechaEdicion = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string nombreArchivo = $"{nombreArticulo}_{fechaEdicion}_{ArticuloDB.Id}.jpg";
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

                ArticuloDB.urlImagen = "/fotosArticulo/" + nombreArchivo;
            }

            // ================= GUARDAR =================
            _contenedorTrabajo.Articulo.Update(ArticuloDB);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Articulo actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }








        /*****************************************************************************************************************/


        #region LLamadas a la API

        public IActionResult GetAll()
        {

            //var listaCasa = _contenedorTrabajo.Casa.GetAll(includeProperties: "Manzana,Villa");
            var listaArticulo = _contenedorTrabajo.Articulo.GetAll(includeProperties: "Categoria");
            // .Where(m => m.Estado == true);
            return Json(new { data = listaArticulo });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Articulo.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Articulo" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Articulo.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Articulo Borrada Correctamente" });


        }









        #endregion


    }
}
