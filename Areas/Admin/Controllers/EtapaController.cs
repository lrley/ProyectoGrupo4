using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.ViviendaViewModels.Urbanizacion;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EtapaController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;


        public EtapaController(IContenedorTrabajo contenedorTrabajo)
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

            EtapaVM vm = new EtapaVM()
            {
                Etapa = new Etapa(),
                ListaUrbanizaciones = _contenedorTrabajo.Urbanizacion.GetListaUrbanizaciones(),
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EtapaVM vm)
        {
            // Normalizar a mayúsculas y quitar espacios
            vm.Etapa.NombreEtapa = vm.Etapa.NombreEtapa?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                vm.ListaUrbanizaciones = _contenedorTrabajo.Urbanizacion.GetListaUrbanizaciones();
                return View(vm);
            }

            // ✅ Validar duplicados
            var etapaDuplicada = _contenedorTrabajo.Etapa.GetFirstOrDefault(
                e => e.NombreEtapa == vm.Etapa.NombreEtapa
            );

            if (etapaDuplicada != null)
            {
                TempData["Error"] = "Ya existe una etapa con ese nombre.";
                vm.ListaUrbanizaciones = _contenedorTrabajo.Urbanizacion.GetListaUrbanizaciones();
                return View(vm);
            }

            // ✅ Normalizar también la urbanización asociada
            var urbanizacion = _contenedorTrabajo.Urbanizacion.Get(vm.Etapa.UrbanizacionId);
            if (urbanizacion != null)
            {
                urbanizacion.NombreUrbanizacion = urbanizacion.NombreUrbanizacion?.Trim().ToUpper();
                _contenedorTrabajo.Urbanizacion.Update(urbanizacion);
            }

            vm.Etapa.CreatedAt = DateTime.Now;
            vm.Etapa.UpdatedAt = DateTime.Now;
            vm.Etapa.Estado = true;

            _contenedorTrabajo.Etapa.Add(vm.Etapa);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Etapa creada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        /*******************************************************************************************************************************************************/



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var etapa = _contenedorTrabajo.Etapa.Get(id);

            if (etapa == null)
            {
                return NotFound();
            }

            EtapaVM vm = new EtapaVM()
            {
                Etapa = etapa,
                ListaUrbanizaciones = _contenedorTrabajo.Urbanizacion.GetListaUrbanizaciones(),
               
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EtapaVM vm)
        {
            // Normalizar a mayúsculas y quitar espacios
            vm.Etapa.NombreEtapa = vm.Etapa.NombreEtapa?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                vm.ListaUrbanizaciones = _contenedorTrabajo.Urbanizacion.GetListaUrbanizaciones();
                return View(vm);
            }

            var etapaFromDb = _contenedorTrabajo.Etapa.Get(id);
            if (etapaFromDb == null)
            {
                return NotFound();
            }

            // ✅ Validar duplicados (otra etapa con el mismo nombre pero distinto Id)
            var etapaDuplicada = _contenedorTrabajo.Etapa.GetFirstOrDefault(
                e => e.NombreEtapa == vm.Etapa.NombreEtapa && e.Id != id
            );

            if (etapaDuplicada != null)
            {
                TempData["Error"] = "Ya existe otra etapa con ese nombre.";
                vm.ListaUrbanizaciones = _contenedorTrabajo.Urbanizacion.GetListaUrbanizaciones();
                return View(vm);
            }

            // ✅ Actualizar campos
            etapaFromDb.NombreEtapa = vm.Etapa.NombreEtapa;
            etapaFromDb.UrbanizacionId = vm.Etapa.UrbanizacionId;
            etapaFromDb.Estado = vm.Etapa.Estado;
            etapaFromDb.UpdatedAt = DateTime.Now;

            // ⚠️ Normalizar también el nombre de la urbanización
            var urbanizacion = _contenedorTrabajo.Urbanizacion.Get(etapaFromDb.UrbanizacionId);
            if (urbanizacion != null)
            {
                urbanizacion.NombreUrbanizacion = urbanizacion.NombreUrbanizacion?.Trim().ToUpper();
                _contenedorTrabajo.Urbanizacion.Update(urbanizacion);
            }

            _contenedorTrabajo.Etapa.Update(etapaFromDb);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Etapa actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }






        /*****************************************************************************************************************/




        #region LLamadas a la API

        public IActionResult GetAll()
        {

            //var listaCasa = _contenedorTrabajo.Casa.GetAll(includeProperties: "Manzana,Villa");
            var listaEtapa = _contenedorTrabajo.Etapa.GetAll(includeProperties: "Urbanizacion");
            // .Where(m => m.Estado == true);
            return Json(new { data = listaEtapa });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Etapa.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Etapa" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Etapa.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Etapa Borrada Correctamente" });


        }



        #endregion


    }
}
