using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.AccesoDatos.Data.Repository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TipoPersonaController : Controller
    {


        private readonly IContenedorTrabajo _contenedorTrabajo;


        public TipoPersonaController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

/*****************************************************************************************************************************************/

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoPersona tipoPersona)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoPersona);
            }

            try
            {
                // Convertir a mayúsculas
                tipoPersona.NombreTipoPersona = tipoPersona.NombreTipoPersona?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.TipoPersona
                    .GetAll()
                    .Any(tp => tp.NombreTipoPersona == tipoPersona.NombreTipoPersona);

                if (existe)
                {
                    TempData["Error"] = "El tipo de persona ya existe en la base de datos.";
                    return View(tipoPersona);
                }

                // Valores por defecto
                tipoPersona.CreatedAt = DateTime.Now;
                tipoPersona.UpdatedAt = DateTime.Now;
                tipoPersona.Estado = true;

                _contenedorTrabajo.TipoPersona.Add(tipoPersona);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Tipo de persona creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error al crear el registro: {ex.Message}";
                return View(tipoPersona);
            }
        }


        /***********************************************************************************************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var tipoPersona = _contenedorTrabajo.TipoPersona.Get(id);
            if (tipoPersona == null)
            {
                return NotFound();
            }
            return View(tipoPersona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TipoPersona tipoPersona)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoPersona);
            }

            try
            {
                // Convertir a mayúsculas
                tipoPersona.NombreTipoPersona = tipoPersona.NombreTipoPersona?.Trim().ToUpper();

                // Validar duplicados (excluyendo el mismo registro que se está editando)
                var existe = _contenedorTrabajo.TipoPersona
                    .GetAll()
                    .Any(tp => tp.NombreTipoPersona == tipoPersona.NombreTipoPersona
                               && tp.IdTipoPersona != tipoPersona.IdTipoPersona);

                if (existe)
                {
                    TempData["Error"] = "El tipo de persona ya existe en la base de datos.";
                    return View(tipoPersona);
                }

                // Obtener registro desde la BD
                var tipoPersonaDb = _contenedorTrabajo.TipoPersona.Get(tipoPersona.IdTipoPersona);
                if (tipoPersonaDb == null)
                {
                    TempData["Error"] = "No se encontró el registro a actualizar.";
                    return View(tipoPersona);
                }

                // Actualizar campos
                tipoPersonaDb.NombreTipoPersona = tipoPersona.NombreTipoPersona;
                tipoPersonaDb.Estado = tipoPersona.Estado;
                tipoPersonaDb.UpdatedAt = DateTime.Now;

                _contenedorTrabajo.TipoPersona.Update(tipoPersonaDb);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Tipo de persona actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error al actualizar el registro: {ex.Message}";
                return View(tipoPersona);
            }
        }










        #region LLamadas a la API

        public IActionResult GetAll()
        {
            var listaTipoPersona = _contenedorTrabajo.TipoPersona.GetAll();
            return Json(new { data = listaTipoPersona });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.TipoPersona.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Tipo Persona" });
            }

            var personasConTipo = _contenedorTrabajo.Persona.GetAll()
            .Any(p => p.IdTipoPersona == id);

            if (personasConTipo)
            {
                return Json(new { success = false, message = "No se puede eliminar el Tipo de Persona porque está asignado a una persona." });
            }


            // _contenedorTrabajo.TipoPersona.Remove(objFromDb);
            // _contenedorTrabajo.Save();
            objFromDb.Estado = false;
            _contenedorTrabajo.TipoPersona.Update(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Tipo Persona Borrado Correctamente" });
        }



        #endregion


    }
}
