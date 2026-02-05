using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.AccesoDatos.Data.Repository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SexoController : Controller
    {


        private readonly IContenedorTrabajo _contenedorTrabajo;


        public SexoController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }



        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /***********************************************************************************************************/
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sexo sexo)
        {
            if (!ModelState.IsValid)
            {
                return View(sexo);
            }

            try
            {
                // Convertir a mayúsculas antes de guardar
                sexo.NombreSexo = sexo.NombreSexo?.Trim().ToUpper();

                // Validar duplicados
                var existe = _contenedorTrabajo.Sexo
                    .GetAll()
                    .Any(s => s.NombreSexo == sexo.NombreSexo);

                if (existe)
                {
                    TempData["Error"] = "Ya existe un registro con ese nombre.";
                    return View(sexo);
                }

                // Valores por defecto
                sexo.CreatedAt = DateTime.Now;
                sexo.UpdatedAt = DateTime.Now;
                sexo.Estado = true;

                _contenedorTrabajo.Sexo.Add(sexo);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Sexo creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Si por alguna razón se escapa el error de SQL, lo traducimos
                if (ex.InnerException?.Message.Contains("IX_Sexos_NombreSexo") == true)
                {
                    TempData["Error"] = "El género ya existe en la base de datos.";
                }
                else
                {
                    TempData["Error"] = "Ocurrió un error inesperado al crear el registro.";
                }

                return View(sexo);


            }
        }

        /**********************************************************************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var sexo = _contenedorTrabajo.Sexo.Get(id);
            if (sexo == null)
            {
                return NotFound();
            }
            return View(sexo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Sexo sexo)
        {
            if (!ModelState.IsValid)
            {
                return View(sexo);
            }

            try
            {
                // Convertir a mayúsculas
                sexo.NombreSexo = sexo.NombreSexo?.Trim().ToUpper();

                // Validar duplicados (excluyendo el mismo registro que se está editando)
                var existe = _contenedorTrabajo.Sexo
                    .GetAll()
                    .Any(s => s.NombreSexo == sexo.NombreSexo && s.IdSexo != sexo.IdSexo);

                if (existe)
                {
                    TempData["Error"] = "El género ya existe en la base de datos.";
                    return View(sexo);
                }

                // Actualizar campos
                var sexoDb = _contenedorTrabajo.Sexo.Get(sexo.IdSexo);
                if (sexoDb == null)
                {
                    TempData["Error"] = "No se encontró el registro a actualizar.";
                    return View(sexo);
                }

                sexoDb.NombreSexo = sexo.NombreSexo;
                sexoDb.Estado = sexo.Estado;
                sexoDb.UpdatedAt = DateTime.Now;

                _contenedorTrabajo.Sexo.Update(sexoDb);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Sexo actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error al actualizar el registro: {ex.Message}";
                return View(sexo);
            }
        }




        #region LLamadas a la API

        public IActionResult GetAll()
        {
            var listaSexo = _contenedorTrabajo.Sexo.GetAll();
            return Json(new { data = listaSexo });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Sexo.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Genero" });
            }


            var personasConSexo = _contenedorTrabajo.Persona.GetAll()
            .Any(p => p.IdSexo == id);

            if (personasConSexo)
            {
                return Json(new { success = false, message = "No se puede eliminar el Sexo porque está asignado a una persona." });
            }



            // _contenedorTrabajo.Sexo.Remove(objFromDb);
            //_contenedorTrabajo.Save();
            objFromDb.Estado = false;
            _contenedorTrabajo.Sexo.Update(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Genero Borrado Correctamente" });


        }



        #endregion


    }
}
