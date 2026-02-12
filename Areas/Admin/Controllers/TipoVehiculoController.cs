using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.VehiculoViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TipoVehiculoController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public TipoVehiculoController(IContenedorTrabajo contenedorTrabajo)
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
        public IActionResult Create(TipoVehiculo tipoVehiculo)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                tipoVehiculo.NombreTipoVehiculo = tipoVehiculo.NombreTipoVehiculo?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.TipoVehiculo
                    .GetAll()
                    .Any(m => m.NombreTipoVehiculo == tipoVehiculo.NombreTipoVehiculo);

                if (existe)
                {
                    ModelState.AddModelError("NombreTipoVehiculo", "Ya existe un Tipo de Vehiculo con ese nombre.");
                    return View(tipoVehiculo);
                }

                _contenedorTrabajo.TipoVehiculo.Add(tipoVehiculo);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Tipo Vehiculo creado correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(tipoVehiculo);
        }
        /************************************************FIN CREATE*****************************************************************/


        /***************************************************EDIT**************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var objFromDb = _contenedorTrabajo.TipoVehiculo.Get(id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TipoVehiculo tipoVehiculo)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                tipoVehiculo.NombreTipoVehiculo = tipoVehiculo.NombreTipoVehiculo?.Trim().ToUpper();

                // Validar duplicados en otro Id
                var existe = _contenedorTrabajo.TipoVehiculo
                    .GetAll()
                    .Any(m => m.NombreTipoVehiculo == tipoVehiculo.NombreTipoVehiculo && m.Id != tipoVehiculo.Id);

                if (existe)
                {
                    ModelState.AddModelError("NombreTipoVehiculo", "Ya existe otro Tipo Vehiculo con ese nombre.");
                    return View(tipoVehiculo);
                }

                _contenedorTrabajo.TipoVehiculo.Update(tipoVehiculo);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Tipo Vehiculo editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(tipoVehiculo);
        }


        /*******************************************************************************************************************************************************/

        #region LLamadas a la API

        public IActionResult GetAll()
        {

            //var listaCasa = _contenedorTrabajo.MArca.GetAll(includeProperties: "listaMarca");
            var listaTipoVehiculo = _contenedorTrabajo.TipoVehiculo.GetAll();
            // .Where(m => m.Estado == true);
            return Json(new { data = listaTipoVehiculo });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.TipoVehiculo.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Tipo Vehiculo" });
            }

            //_contenedorTrabajo.Categoria.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.TipoVehiculo.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Tipo Vehiculo Borrada Correctamente" });


        }



        #endregion








    }
}
