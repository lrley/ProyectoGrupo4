using DlaccessCore.AccesoDatos.Data.IRepository;

using DlaccessCore.Models.Models.ViviendaViewModels.Edificio;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class PisoController : Controller
    {


        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PisoController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


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
        public IActionResult Create(Piso piso)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                piso.NombrePiso = piso.NombrePiso?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.Piso
                    .GetAll()
                    .Any(m => m.NombrePiso == piso.NombrePiso);

                if (existe)
                {
                    ModelState.AddModelError("NombrePiso", "El nombre ingresado ya está registrado en otro Piso activo.");
                    return View(piso);
                }

                _contenedorTrabajo.Piso.Add(piso);
                _contenedorTrabajo.Save();

                TempData["success"] = "Piso creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(piso);
        }
        /************************************************FIN CREATE*****************************************************************/


        /***************************************************EDIT**************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var objFromDb = _contenedorTrabajo.Piso.Get(id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Piso piso)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                piso.NombrePiso = piso.NombrePiso?.Trim().ToUpper();

                // Validar duplicados en otro Id
                var existe = _contenedorTrabajo.Piso
                    .GetAll()
                    .Any(m => m.NombrePiso == piso.NombrePiso && m.Id != piso.Id);

                if (existe)
                {
                    ModelState.AddModelError("NombrePiso", "El nombre ingresado ya está registrado en otra Piso activo.");
                    return View(piso);
                }

                _contenedorTrabajo.Piso.Update(piso);
                _contenedorTrabajo.Save();

                TempData["success"] = "Villa editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(piso);
        }

        /*****************************************************************************************************************/


        




        /*****************************************************************************************************************/

        #region LLamadas a la API

        public IActionResult GetAll()
        {
            //var listaManzana = _contenedorTrabajo.Manzana.GetAll();
            var listaPisos = _contenedorTrabajo.Piso
                .GetAll();
            // .Where(m => m.Estado == true);
            return Json(new { data = listaPisos });
        }



        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Piso.Get(id);



            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Piso" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Piso.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Piso Borrada Correctamente" });


        }



        #endregion



    }
}
