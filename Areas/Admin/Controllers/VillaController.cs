using DlaccessCore.AccesoDatos.Data.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class VillaController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        public IActionResult Index()
        {
            return View();
        }












        /*****************************************************************************************************************/


        #region LLamadas a la API

        public IActionResult GetAll()
        {
            //var listaManzana = _contenedorTrabajo.Manzana.GetAll();
            var listaVillas = _contenedorTrabajo.Villa
                .GetAll();
            // .Where(m => m.Estado == true);
            return Json(new { data = listaVillas });
        }



        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var objFromDb = _contenedorTrabajo.Villa.Get(id);



            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Villa" });
            }

            //_contenedorTrabajo.Manzana.Remove(objFromDb);
            objFromDb.Estado = false;
            _contenedorTrabajo.Villa.Update(objFromDb);
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "Villa Borrada Correctamente" });


        }



        #endregion


    }
}
