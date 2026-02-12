using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CasaController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;


        public CasaController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /*******************************************************************************************************************************************************/

        [HttpGet]
        public IActionResult Create()
        {

            CasaVM vm = new CasaVM()
            {

                Casa = new Casa(),
                ListaManzana = _contenedorTrabajo.Manzana.GetListaManzanas(),
                ListaVilla = _contenedorTrabajo.Villa.GetListaVilla(),
                ListaEtapa = _contenedorTrabajo.Etapa.GetListaEtapas(),


            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CasaVM vm)
        {
            vm.Casa.NombreFamilia = vm.Casa.NombreFamilia?.Trim();
                              
            if (!ModelState.IsValid)
            {
                vm.ListaManzana = _contenedorTrabajo.Manzana.GetListaManzanas();
                vm.ListaVilla = _contenedorTrabajo.Villa.GetListaVilla();
                vm.ListaEtapa = _contenedorTrabajo.Etapa.GetListaEtapas();

                return View(vm);
            }

         
            vm.Casa.CreatedAt = DateTime.Now;
            vm.Casa.UpdatedAt = DateTime.Now;
            vm.Casa.Estado = true;
         

            _contenedorTrabajo.Casa.Add(vm.Casa);
            _contenedorTrabajo.Save();


            TempData["Success"] = "Persona creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        /*******************************************************************************************************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var casa = _contenedorTrabajo.Casa.Get(id);

            if (casa == null)
            {
                return NotFound();
            }

            CasaVM vm = new CasaVM()
            {
                Casa = casa,
                ListaManzana = _contenedorTrabajo.Manzana.GetListaManzanas(),
                ListaVilla = _contenedorTrabajo.Villa.GetListaVilla(),
                ListaEtapa = _contenedorTrabajo.Etapa.GetListaEtapas(),
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CasaVM vm)
        {
            vm.Casa.NombreFamilia = vm.Casa.NombreFamilia?.Trim();

            if (!ModelState.IsValid)
            {
                vm.ListaManzana = _contenedorTrabajo.Manzana.GetListaManzanas();
                vm.ListaVilla = _contenedorTrabajo.Villa.GetListaVilla();
                vm.ListaEtapa = _contenedorTrabajo.Etapa.GetListaEtapas();
                return View(vm);
            }

            var casaFromDb = _contenedorTrabajo.Casa.Get(vm.Casa.Id);
            if (casaFromDb == null)
            {
                return NotFound();
            }

            // Actualizar campos
            casaFromDb.NombreFamilia = vm.Casa.NombreFamilia;
            casaFromDb.ManzanaId = vm.Casa.ManzanaId;
            casaFromDb.VillaId = vm.Casa.VillaId;
            casaFromDb.EtapaId = vm.Casa.EtapaId;
            casaFromDb.UpdatedAt = DateTime.Now;
            casaFromDb.Estado = vm.Casa.Estado;

            _contenedorTrabajo.Casa.Update(casaFromDb);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Casa actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }







        /*****************************************************************************************************************/


        #region LLamadas a la API

        public IActionResult GetAll()
        {
            
            //var listaCasa = _contenedorTrabajo.Casa.GetAll(includeProperties: "Manzana,Villa");
            var listaCasa = _contenedorTrabajo.Casa.GetAll(includeProperties: "Manzana,Villa,Etapa");
            // .Where(m => m.Estado == true);
            return Json(new { data = listaCasa });
        }








        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Casa.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Casa" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Casa.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Casa Borrada Correctamente" });


        }



        #endregion


    }
}
