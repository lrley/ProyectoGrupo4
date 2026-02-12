using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.VehiculoViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ModeloController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ModeloController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        public IActionResult Index()
        {
            return View();
        }

        /*******************************************************************************************************************************************************/

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Modelo modelo)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                modelo.NombreModelo = modelo.NombreModelo?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.Modelo
                    .GetAll()
                    .Any(m => m.NombreModelo == modelo.NombreModelo);

                if (existe)
                {
                    ModelState.AddModelError("NombreModelo", "Ya existe un Modelo con ese nombre.");
                    return View(modelo);
                }

                _contenedorTrabajo.Modelo.Add(modelo);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Marca creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(modelo);
        }
        /************************************************FIN CREATE*****************************************************************/


        /***************************************************EDIT**************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var objFromDb = _contenedorTrabajo.Modelo.Get(id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Modelo modelo)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                modelo.NombreModelo = modelo.NombreModelo?.Trim().ToUpper();

                // Validar duplicados en otro Id
                var existe = _contenedorTrabajo.Modelo
                    .GetAll()
                    .Any(m => m.NombreModelo == modelo.NombreModelo && m.Id != modelo.Id);

                if (existe)
                {
                    ModelState.AddModelError("NombreMarca", "Ya existe otra Marca con ese nombre.");
                    return View(modelo);
                }

                _contenedorTrabajo.Modelo.Update(modelo);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Modelo editado correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(modelo);
        }


        /*******************************************************************************************************************************************************/

        #region LLamadas a la API

        public IActionResult GetAll()
        {

            //var listaCasa = _contenedorTrabajo.MArca.GetAll(includeProperties: "listaMarca");
            var listaModelo = _contenedorTrabajo.Modelo.GetAll();
            // .Where(m => m.Estado == true);
            return Json(new { data = listaModelo });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Modelo.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Modelo" });
            }

            //_contenedorTrabajo.Categoria.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Modelo.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Modelo Borrada Correctamente" });


        }



        #endregion






    }
}
