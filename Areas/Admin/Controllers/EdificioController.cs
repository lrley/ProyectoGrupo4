using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using DlaccessCore.Models.Models.ViviendaViewModels.Edificio;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class EdificioController : Controller
    {


        private readonly IContenedorTrabajo _contenedorTrabajo;


        public EdificioController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {

            EdificioVM vm = new EdificioVM()
            {

                Edificio = new Edificio(),
                ListaDepartamentos = _contenedorTrabajo.Departamento.GetListaDepartamentos(),
                ListaPisos = _contenedorTrabajo.Piso.GetListaPisos(),
                ListaEtapa = _contenedorTrabajo.Etapa.GetListaEtapas(),

            };
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EdificioVM vm)
        {
            vm.Edificio.NombreFamilia = vm.Edificio.NombreFamilia?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                vm.ListaDepartamentos = _contenedorTrabajo.Departamento.GetListaDepartamentos();
                vm.ListaPisos = _contenedorTrabajo.Piso.GetListaPisos();
                vm.ListaEtapa = _contenedorTrabajo.Etapa.GetListaEtapas();

                return View(vm);
            }

            // ======================================================
            // 1️⃣ GUARDAR PERSONA SIN IMAGEN (para obtener el ID)
            // ======================================================
            vm.Edificio.CreatedAt = DateTime.Now;
            vm.Edificio.UpdatedAt = DateTime.Now;
            vm.Edificio.Estado = true;


            _contenedorTrabajo.Edificio.Add(vm.Edificio);
            _contenedorTrabajo.Save();


            TempData["Success"] = "Persona creada correctamente.";
            return RedirectToAction(nameof(Index));
        }



        /*******************************************************************************************************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var edificio = _contenedorTrabajo.Edificio.Get(id);

            if (edificio == null)
            {
                return NotFound();
            }

            EdificioVM vm = new EdificioVM()
            {
                Edificio = edificio,
                ListaDepartamentos = _contenedorTrabajo.Departamento.GetListaDepartamentos(),
                ListaPisos = _contenedorTrabajo.Piso.GetListaPisos(),
                ListaEtapa = _contenedorTrabajo.Etapa.GetListaEtapas(),
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EdificioVM vm)
        {
            vm.Edificio.NombreFamilia = vm.Edificio.NombreFamilia?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                vm.ListaDepartamentos = _contenedorTrabajo.Departamento.GetListaDepartamentos();
                vm.ListaPisos = _contenedorTrabajo.Piso.GetListaPisos();
                vm.ListaEtapa = _contenedorTrabajo.Etapa.GetListaEtapas();
                return View(vm);
            }

            var EdificioFromDb = _contenedorTrabajo.Edificio.Get(vm.Edificio.Id);
            if (EdificioFromDb == null)
            {
                return NotFound();
            }

            // Actualizar campos
            EdificioFromDb.NombreFamilia = vm.Edificio.NombreFamilia;
            EdificioFromDb.NombreEdificio = vm.Edificio.NombreEdificio;
            
            EdificioFromDb.DepartamentoId = vm.Edificio.DepartamentoId;
            EdificioFromDb.PisoId = vm.Edificio.PisoId;
            EdificioFromDb.EtapaId = vm.Edificio.EtapaId;

            EdificioFromDb.UpdatedAt = DateTime.Now;
            EdificioFromDb.Estado = vm.Edificio.Estado;

            _contenedorTrabajo.Edificio.Update(EdificioFromDb);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Casa actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }







        /*****************************************************************************************************************/




        #region LLamadas a la API

        public IActionResult GetAll()
        {

            //var listaCasa = _contenedorTrabajo.Casa.GetAll(includeProperties: "Manzana,Villa");
            var listaedificio = _contenedorTrabajo.Edificio.GetAll(includeProperties: "Departamento,Piso,Etapa");
            // .Where(m => m.Estado == true);
            return Json(new { data = listaedificio });
        }








        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Edificio.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Edificio" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Edificio.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Edificio Borrada Correctamente" });


        }



        #endregion







    }
}
