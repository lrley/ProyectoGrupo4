using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManzanaController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
       
        
        public ManzanaController(IContenedorTrabajo contenedorTrabajo) {
            _contenedorTrabajo = contenedorTrabajo;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }



        /******************************************POST***********************************************************************/

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Manzana manzana)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                manzana.NombreMz = manzana.NombreMz?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.Manzana
                    .GetAll()
                    .Any(m => m.NombreMz == manzana.NombreMz);

                if (existe)
                {
                    ModelState.AddModelError("NombreMz", "Ya existe una manzana con ese nombre.");
                    return View(manzana);
                }

                _contenedorTrabajo.Manzana.Add(manzana);
                _contenedorTrabajo.Save();

                TempData["success"] = "Manzana creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(manzana);
        }
        /************************************************FIN CREATE*****************************************************************/


        /***************************************************EDIT**************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var objFromDb = _contenedorTrabajo.Manzana.Get(id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Manzana manzana)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                manzana.NombreMz = manzana.NombreMz?.Trim().ToUpper();

                // Validar duplicados en otro Id
                var existe = _contenedorTrabajo.Manzana
                    .GetAll()
                    .Any(m => m.NombreMz == manzana.NombreMz && m.Id != manzana.Id);

                if (existe)
                {
                    ModelState.AddModelError("NombreMz", "Ya existe otra manzana con ese nombre.");
                    return View(manzana);
                }

                _contenedorTrabajo.Manzana.Update(manzana);
                _contenedorTrabajo.Save();

                TempData["success"] = "Manzana editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(manzana);
        }












        /*****************************************************************************************************************/


        #region LLamadas a la API

        public IActionResult GetAll()
        {
            //var listaManzana = _contenedorTrabajo.Manzana.GetAll();
            var listaManzana = _contenedorTrabajo.Manzana
                .GetAll();
               // .Where(m => m.Estado == true);
            return Json(new { data = listaManzana });
        }



        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Manzana.Get(id);
          


            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Manzana" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Manzana.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Manzana Borrado Correctamente" });


        }



        #endregion


    }
}
