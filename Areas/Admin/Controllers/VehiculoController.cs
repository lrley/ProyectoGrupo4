using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.VehiculoViewModels;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VehiculoController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;


        public VehiculoController(IContenedorTrabajo contenedorTrabajo)
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

            VehiculoVM vm = new VehiculoVM()
            {

                Vehiculo = new Vehiculo(),
                ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas(),
                ListaModelos = _contenedorTrabajo.Modelo.GetListaModelos(),
                ListaTipoVehiculos = _contenedorTrabajo.TipoVehiculo.GetListaTipoVehiculos(),


            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VehiculoVM vm)
        {
            // Normalizar a mayúsculas y quitar espacios
            vm.Vehiculo.NombreVehiculo = vm.Vehiculo.NombreVehiculo?.Trim().ToUpper();
            vm.Vehiculo.Color = vm.Vehiculo.Color?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                vm.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
                vm.ListaModelos = _contenedorTrabajo.Modelo.GetListaModelos();
                vm.ListaTipoVehiculos = _contenedorTrabajo.TipoVehiculo.GetListaTipoVehiculos();
                return View(vm);
            }

            // ✅ Validar duplicados: mismo nombre, marca, modelo, color y tipo
            var vehiculoDuplicado = _contenedorTrabajo.Vehiculo.GetFirstOrDefault(v =>
                v.NombreVehiculo == vm.Vehiculo.NombreVehiculo &&
                v.IdMarca == vm.Vehiculo.IdMarca &&
                v.IdModelo == vm.Vehiculo.IdModelo &&
                v.Color == vm.Vehiculo.Color &&
                v.IdTipoVehiculo == vm.Vehiculo.IdTipoVehiculo
            );

            if (vehiculoDuplicado != null)
            {
                TempData["Error"] = "Ya existe un vehículo con esas características.";
                vm.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
                vm.ListaModelos = _contenedorTrabajo.Modelo.GetListaModelos();
                vm.ListaTipoVehiculos = _contenedorTrabajo.TipoVehiculo.GetListaTipoVehiculos();
                return View(vm);
            }

            // Datos adicionales
            vm.Vehiculo.CreatedAt = DateTime.Now;
            vm.Vehiculo.UpdatedAt = DateTime.Now;
            vm.Vehiculo.Estado = true;

            _contenedorTrabajo.Vehiculo.Add(vm.Vehiculo);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Vehículo creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        /*******************************************************************************************************************************************************/

        /*******************************************************************************************************************************************************/

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var vehiculo = _contenedorTrabajo.Vehiculo.Get(id);

            if (vehiculo == null)
            {
                return NotFound();
            }

            VehiculoVM vm = new VehiculoVM()
            {
                Vehiculo = vehiculo,
                ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas(),
                ListaModelos = _contenedorTrabajo.Modelo.GetListaModelos(),
                ListaTipoVehiculos = _contenedorTrabajo.TipoVehiculo.GetListaTipoVehiculos(),
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(VehiculoVM vm)
        {
            // Normalizar a mayúsculas y quitar espacios
            vm.Vehiculo.NombreVehiculo = vm.Vehiculo.NombreVehiculo?.Trim().ToUpper();
            vm.Vehiculo.Color = vm.Vehiculo.Color?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                vm.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
                vm.ListaModelos = _contenedorTrabajo.Modelo.GetListaModelos();
                vm.ListaTipoVehiculos = _contenedorTrabajo.TipoVehiculo.GetListaTipoVehiculos();
                return View(vm);
            }

            var vehiculoFromDb = _contenedorTrabajo.Vehiculo.Get(vm.Vehiculo.Id);
            if (vehiculoFromDb == null)
            {
                return NotFound();
            }

            // ✅ Validar duplicados: mismo nombre, color, marca, modelo y tipo
            // pero EXCLUYENDO el mismo Id que estamos editando
            var vehiculoDuplicado = _contenedorTrabajo.Vehiculo.GetFirstOrDefault(v =>
                v.Id != vm.Vehiculo.Id && // excluye el mismo registro
                v.NombreVehiculo == vm.Vehiculo.NombreVehiculo &&
                v.Color == vm.Vehiculo.Color &&
                v.IdMarca == vm.Vehiculo.IdMarca &&
                v.IdModelo == vm.Vehiculo.IdModelo &&
                v.IdTipoVehiculo == vm.Vehiculo.IdTipoVehiculo
            );

            if (vehiculoDuplicado != null)
            {
                TempData["Error"] = "Ya existe otro vehículo con esas características.";
                vm.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
                vm.ListaModelos = _contenedorTrabajo.Modelo.GetListaModelos();
                vm.ListaTipoVehiculos = _contenedorTrabajo.TipoVehiculo.GetListaTipoVehiculos();
                return View(vm);
            }

            // ✅ Actualizar campos
            vehiculoFromDb.NombreVehiculo = vm.Vehiculo.NombreVehiculo;
            vehiculoFromDb.Color = vm.Vehiculo.Color;
            vehiculoFromDb.IdMarca = vm.Vehiculo.IdMarca;
            vehiculoFromDb.IdModelo = vm.Vehiculo.IdModelo;
            vehiculoFromDb.IdTipoVehiculo = vm.Vehiculo.IdTipoVehiculo;
            vehiculoFromDb.Estado = vm.Vehiculo.Estado;
            vehiculoFromDb.UpdatedAt = DateTime.Now;

            _contenedorTrabajo.Vehiculo.Update(vehiculoFromDb);
            _contenedorTrabajo.Save();

            TempData["Success"] = "Vehículo actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }






        /*****************************************************************************************************************/





        /*****************************************************************************************************************/


        #region LLamadas a la API

        public IActionResult GetAll()
        {

            //var listaCasa = _contenedorTrabajo.Casa.GetAll(includeProperties: "Manzana,Villa");
            var listaVehiculo = _contenedorTrabajo.Vehiculo.GetAll(includeProperties: "Modelo,Marca,TipoVehiculo");
            // .Where(m => m.Estado == true);
            return Json(new { data = listaVehiculo });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Vehiculo.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Vehiculo" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Vehiculo.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Vehiculo Borrada Correctamente" });


        }



        #endregion



    }
}
