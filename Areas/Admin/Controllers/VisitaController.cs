using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using DlaccessCore.Models.Models.RelacionesModels;
using DlaccessCore.Models.ViewModels;
using DlaccessCore.Utilidades.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Linq; // <--- Para asegurar compatibilidad
using System.Security.Claims; // <--- Necesario para Claims

namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VisitaController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly ValidadorInfoDetallada _validador;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public VisitaController(IContenedorTrabajo contenedorTrabajo, ValidadorInfoDetallada validador, IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _validador = validador;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new VisitaVM
            {
                Visitante = new Visitante(),
                ListaPersonas = _contenedorTrabajo.Persona.GetListaPersonas() ?? new List<SelectListItem>(),
                ListaCasas = _contenedorTrabajo.Casa.GetListaCasas() ?? new List<SelectListItem>(),
                ListaEdificios = _contenedorTrabajo.Edificio.GetListaEdificios() ?? new List<SelectListItem>(),
                ListaVehiculos = _contenedorTrabajo.Vehiculo.GetListaVehiculos() ?? new List<SelectListItem>()
            };
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitaVM model)
        {
            // 1. IDENTIFICAR OPERADOR LOGUEADO
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Challenge();

            // ==========================================================
            // NIVEL 1: SEGURIDAD DEL OPERADOR (Solo campo Permiso)
            // ==========================================================
            var usuarioSistema = _contenedorTrabajo.Usuario.ObtenerUsuario(claim.Value);
            if (usuarioSistema != null && usuarioSistema.Permiso == false)
            {
                TempData["Error"] = "Tu usuario operativo está BLOQUEADO. Por favor, hable con el administrador del sistema.";
                CargarListas(model);
                return View(model);
            }

            // 2. EXTRAER IDS PARA BÚSQUEDA DE DESTINO
            int pId = model.Visitante.IdPersona;
            int? cId = (model.Visitante.CasaId > 0) ? model.Visitante.CasaId : null;
            int? eId = (model.Visitante.IdEdificio > 0) ? model.Visitante.IdEdificio : null;

            // 3. BUSCAR LA RELACIÓN DE DESTINO (INFODETALLADA)
            var infoDb = _contenedorTrabajo.InfoDetallada.GetFirstOrDefault(u =>
                (pId > 0 && u.IdPersona == pId) || (cId != null && u.CasaId == cId) || (eId != null && u.IdEdificio == eId)
            );

            if (infoDb == null)
            {
                TempData["Error"] = "No se pudo encontrar el destino seleccionado. Hable con el administrador.";
                CargarListas(model);
                return View(model);
            }

            // ==========================================================
            // NIVEL 2: SEGURIDAD DEL DESTINO (Solo campo Permiso)
            // ==========================================================
            if (infoDb.Permiso == false)
            {
                TempData["Error"] = "El destino (Casa/Edificio) está BLOQUEADO para recibir visitas. Hable con el administrador.";
                CargarListas(model);
                return View(model);
            }

            // ==========================================================
            // NIVEL 3: SEGURIDAD DE LA PERSONA A VISITAR (Solo campo Permiso)
            // ==========================================================
            var personaARecibir = _contenedorTrabajo.Persona.GetFirstOrDefault(p => p.Id == infoDb.IdPersona);
            if (personaARecibir != null && personaARecibir.Permiso == false)
            {
                TempData["Error"] = "La persona a la que desea visitar está BLOQUEADA. Hable con el administrador.";
                CargarListas(model);
                return View(model);
            }

            // ==========================================================
            // TODO CORRECTO -> PROCEDER A GUARDAR
            // ==========================================================
            try
            {
                model.Visitante.IdPersona = infoDb.IdPersona;
                model.Visitante.CasaId = infoDb.CasaId;
                model.Visitante.IdEdificio = infoDb.IdEdificio;
                model.Visitante.AspNetUsersId = claim.Value;
                model.Visitante.CreatedAt = DateTime.Now;
                model.Visitante.Estado = true;
                model.Visitante.Img = "";

                ModelState.Clear();
                _contenedorTrabajo.Visitante.Add(model.Visitante);
                _contenedorTrabajo.Save();

                // 4. PROCESAR IMAGEN CON EL ID GENERADO
                if (!string.IsNullOrEmpty(model.FotoBase64) || model.ArchivoImagen != null)
                {
                    string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string carpeta = "imagenVisita";
                    string nombreArchivo = $"{model.Visitante.Id}.jpg";
                    string rutaFisica = Path.Combine(webRootPath, carpeta, nombreArchivo);

                    if (!Directory.Exists(Path.Combine(webRootPath, carpeta)))
                        Directory.CreateDirectory(Path.Combine(webRootPath, carpeta));

                    if (!string.IsNullOrEmpty(model.FotoBase64))
                    {
                        var base64Data = model.FotoBase64.Contains(",") ? model.FotoBase64.Split(',')[1] : model.FotoBase64;
                        await System.IO.File.WriteAllBytesAsync(rutaFisica, Convert.FromBase64String(base64Data));
                    }
                    else if (model.ArchivoImagen != null)
                    {
                        using (var stream = new FileStream(rutaFisica, FileMode.Create))
                        {
                            await model.ArchivoImagen.CopyToAsync(stream);
                        }
                    }

                    model.Visitante.Img = $"/{carpeta}/{nombreArchivo}";
                    _contenedorTrabajo.Visitante.Update(model.Visitante);
                    _contenedorTrabajo.Save();
                }

                TempData["Success"] = "Visita registrada con éxito.";
                return RedirectToAction("Index", "Visita", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la visita: " + ex.Message;
                CargarListas(model);
                return View(model);
            }
        }

    

        // Método auxiliar indispensable para que los combos no fallen al retornar la vista
        private void CargarListas(VisitaVM model)
        {
            model.ListaPersonas = _contenedorTrabajo.Persona.GetListaPersonas();
            model.ListaCasas = _contenedorTrabajo.Casa.GetListaCasas();
            model.ListaEdificios = _contenedorTrabajo.Edificio.GetListaEdificios();
        }


       
    }
}