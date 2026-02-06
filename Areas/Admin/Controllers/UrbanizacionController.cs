using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.ViviendaViewModels.Urbanizacion;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UrbanizacionController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;


        public UrbanizacionController(IContenedorTrabajo contenedorTrabajo)
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
        public IActionResult Create(Urbanizacion urbanizacion)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                urbanizacion.NombreUrbanizacion = urbanizacion.NombreUrbanizacion?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.Urbanizacion
                    .GetAll()
                    .Any(m => m.NombreUrbanizacion == urbanizacion.NombreUrbanizacion);

                if (existe)
                {
                    ModelState.AddModelError("NombreUrbanizacion", "Ya existe una Urbanizacion con ese nombre.");
                    return View(urbanizacion);
                }

                _contenedorTrabajo.Urbanizacion.Add(urbanizacion);
                _contenedorTrabajo.Save();

                TempData["success"] = "Urbanizacion creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(urbanizacion);
        }
        /************************************************FIN CREATE*****************************************************************/


        /***************************************************EDIT**************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var objFromDb = _contenedorTrabajo.Urbanizacion.Get(id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Urbanizacion urbanizacion)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                urbanizacion.NombreUrbanizacion = urbanizacion.NombreUrbanizacion?.Trim().ToUpper();

                // Validar duplicados en otro Id
                var existe = _contenedorTrabajo.Urbanizacion
                    .GetAll()
                    .Any(m => m.NombreUrbanizacion == urbanizacion.NombreUrbanizacion && m.Id != urbanizacion.Id);

                if (existe)
                {
                    ModelState.AddModelError("NombreUrbanizacion", "Ya existe otra Urbanizacion con ese nombre.");
                    return View(urbanizacion);
                }

                _contenedorTrabajo.Urbanizacion.Update(urbanizacion);
                _contenedorTrabajo.Save();

                TempData["success"] = "Urbanizacion editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(urbanizacion);
        }



        /*****************************************************************************************************************/


        #region LLamadas a la API

        public IActionResult GetAll()
        {
            //var listaManzana = _contenedorTrabajo.Manzana.GetAll();
            var listaUrbanizacion = _contenedorTrabajo.Urbanizacion
                .GetAll();
            // .Where(m => m.Estado == true);
            return Json(new { data = listaUrbanizacion });
        }



        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Urbanizacion.Get(id);



            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Urbanizacion" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Urbanizacion.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Urbanizacion Borrado Correctamente" });


        }



        #endregion








    }
}
