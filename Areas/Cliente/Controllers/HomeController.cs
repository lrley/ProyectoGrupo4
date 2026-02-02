using System.Diagnostics;
using DLACCESS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DLACCESS.Areas.Cliente.Controllers
{
    public class HomeController : Controller
    {
        [Area("Cliente")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
