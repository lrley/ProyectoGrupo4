using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.ProductoModel;
using DlaccessCore.Models.Models.VehiculoViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarcaController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public MarcaController(IContenedorTrabajo contenedorTrabajo)
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
        public IActionResult Create(Marca marca)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                marca.NombreMarca = marca.NombreMarca?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.Marca
                    .GetAll()
                    .Any(m => m.NombreMarca == marca.NombreMarca);

                if (existe)
                {
                    ModelState.AddModelError("NombreMarca", "Ya existe una Marca con ese nombre.");
                    return View(marca);
                }

                _contenedorTrabajo.Marca.Add(marca);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Marca creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(marca);
        }
        /************************************************FIN CREATE*****************************************************************/


        /***************************************************EDIT**************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var objFromDb = _contenedorTrabajo.Marca.Get(id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Marca marca)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                marca.NombreMarca = marca.NombreMarca?.Trim().ToUpper();

                // Validar duplicados en otro Id
                var existe = _contenedorTrabajo.Marca
                    .GetAll()
                    .Any(m => m.NombreMarca == marca.NombreMarca && m.Id != marca.Id);

                if (existe)
                {
                    ModelState.AddModelError("NombreMarca", "Ya existe otra Marca con ese nombre.");
                    return View(marca);
                }

                _contenedorTrabajo.Marca.Update(marca);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Marca editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(marca);
        }


        /*******************************************************************************************************************************************************/

        #region LLamadas a la API

        public IActionResult GetAll()
        {

            //var listaCasa = _contenedorTrabajo.MArca.GetAll(includeProperties: "listaMarca");
            var listaMarca = _contenedorTrabajo.Marca.GetAll();
            // .Where(m => m.Estado == true);
            return Json(new { data = listaMarca });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Marca.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Marca" });
            }

            //_contenedorTrabajo.Categoria.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Marca.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Marca Borrada Correctamente" });


        }



        #endregion







    }
}
