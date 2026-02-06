using DlaccessCore.AccesoDatos.Data.IRepository;

using DlaccessCore.Models.Models.ViviendaViewModels.Edificio;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DepartamentoController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;


        public DepartamentoController(IContenedorTrabajo contenedorTrabajo)
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
        public IActionResult Create(Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                departamento.NombreDepart = departamento.NombreDepart?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.Departamento
                    .GetAll()
                    .Any(m => m.NombreDepart == departamento.NombreDepart);

                if (existe)
                {
                    ModelState.AddModelError("NombreMz", "Ya existe un Departamento con ese nombre.");
                    return View(departamento);
                }

                _contenedorTrabajo.Departamento.Add(departamento);
                _contenedorTrabajo.Save();

                TempData["success"] = "Departamento creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(departamento);
        }
        /************************************************FIN CREATE*****************************************************************/


        /***************************************************EDIT**************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var objFromDb = _contenedorTrabajo.Departamento.Get(id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                // Normalizar a mayúsculas
                departamento.NombreDepart = departamento.NombreDepart?.Trim().ToUpper();

                // Validar duplicados en otro Id
                var existe = _contenedorTrabajo.Departamento
                    .GetAll()
                    .Any(m => m.NombreDepart == departamento.NombreDepart && m.Id != departamento.Id);

                if (existe)
                {
                    ModelState.AddModelError("NombreDepart", "Ya existe otro Departamento con ese nombre.");
                    return View(departamento);
                }

                _contenedorTrabajo.Departamento.Update(departamento);
                _contenedorTrabajo.Save();

                TempData["success"] = "´Departamento editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(departamento);
        }












        /*****************************************************************************************************************/






        /*****************************************************************************************************************/


        #region LLamadas a la API

        public IActionResult GetAll()
        {
            //var listaManzana = _contenedorTrabajo.Departamento.GetAll();
            var listaDepartamento = _contenedorTrabajo.Departamento
                .GetAll();
            // .Where(m => m.Estado == true);
            return Json(new { data = listaDepartamento });
        }




        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Departamento.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Departamento" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Departamento.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Departamento Borrado Correctamente" });


        }



        #endregion


    }
}
