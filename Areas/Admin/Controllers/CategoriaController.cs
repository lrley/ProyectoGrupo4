using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.AccesoDatos.Data.IRepository.ProductoIRepository;
using DlaccessCore.Models.Models.ProductoModel;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using DlaccessCore.Models.Models.ViviendaViewModels.Edificio;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriaController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;

        public CategoriaController(IContenedorTrabajo contenedorTrabajo)
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
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                categoria.Nombre = categoria.Nombre?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.Categoria
                    .GetAll()
                    .Any(m => m.Nombre == categoria.Nombre);

                if (existe)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un Departamento con ese nombre.");
                    return View(categoria);
                }

                _contenedorTrabajo.Categoria.Add(categoria);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Categoria creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }
        /************************************************FIN CREATE*****************************************************************/


        /***************************************************EDIT**************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var objFromDb = _contenedorTrabajo.Categoria.Get(id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                categoria.Nombre = categoria.Nombre?.Trim().ToUpper();

                // Validar duplicados en otro Id
                var existe = _contenedorTrabajo.Categoria
                    .GetAll()
                    .Any(m => m.Nombre == categoria.Nombre && m.Id != categoria.Id);

                if (existe)
                {
                    ModelState.AddModelError("NombreDepart", "Ya existe otro Departamento con ese nombre.");
                    return View(categoria);
                }

                _contenedorTrabajo.Categoria.Update(categoria);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Categoría editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }







        /*******************************************************************************************************************************************************/

        #region LLamadas a la API

        public IActionResult GetAll()
        {

            //var listaCasa = _contenedorTrabajo.Casa.GetAll(includeProperties: "Manzana,Villa");
            var listaCategoria = _contenedorTrabajo.Categoria.GetAll();
            // .Where(m => m.Estado == true);
            return Json(new { data = listaCategoria });
        }








        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Categoria.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Categoria" });
            }

            //_contenedorTrabajo.Categoria.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Categoria.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Categoria Borrada Correctamente" });


        }



        #endregion


    }
}
