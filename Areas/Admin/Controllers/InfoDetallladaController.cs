using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using DlaccessCore.Models.Models.RelacionesModels;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace DLACCESS.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class InfoDetallladaController : Controller
    {


        private readonly IContenedorTrabajo _contenedorTrabajo;

        public InfoDetallladaController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        public IActionResult Index()
        {
            return View();
        }



        /*******************************************************************************************************************************************************/

        // ==================== CREATE GET ====================
        [HttpGet]
        public IActionResult Create()
        {
            InfoDetalladaVM vm = new InfoDetalladaVM()
            {
                InfoDetallada = new InfoDetallada(),
                ListaPersonas = _contenedorTrabajo.Persona.GetListaPersonas(),
                ListaCasas = _contenedorTrabajo.Casa.GetListaCasas(),
                ListaEdificios = _contenedorTrabajo.Edificio.GetListaEdificios(),
                ListaVehiculos = _contenedorTrabajo.Vehiculo.GetListaVehiculos(),
            };

            return View(vm);
        }

        // ==================== CREATE POST ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InfoDetalladaVM vm)
        {
            // Remover validación de campos que no usamos en este formulario
            ModelState.Remove("FotoBase64");
            ModelState.Remove("ArchivoImagen");

            if (ModelState.IsValid)
            {
                try
                {
                    // ========== VALIDACIONES ==========

                    // 1. Validar que la persona existe
                    var persona = _contenedorTrabajo.Persona.Get(vm.InfoDetallada.IdPersona);

                    if (persona == null)
                    {
                        ModelState.AddModelError("InfoDetallada.IdPersona",
                            "La persona seleccionada no existe");
                        RecargarListas(vm);
                        return View(vm);
                    }

                    // 2. Validar unicidad de Placa (si se ingresó)
                    if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca))
                    {
                        var placaExiste = _contenedorTrabajo.InfoDetallada.GetFirstOrDefault(
                            filter: i => i.NumeroPlaca == vm.InfoDetallada.NumeroPlaca
                        );

                        if (placaExiste != null)
                        {
                            ModelState.AddModelError("InfoDetallada.NumeroPlaca",
                                "Esta placa ya está registrada");
                            RecargarListas(vm);
                            return View(vm);
                        }
                    }

                    // 3. Validar unicidad de Tag (si se ingresó)
                    if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag))
                    {
                        var tagExiste = _contenedorTrabajo.InfoDetallada.GetFirstOrDefault(
                            filter: i => i.NumeroTag == vm.InfoDetallada.NumeroTag
                        );

                        if (tagExiste != null)
                        {
                            ModelState.AddModelError("InfoDetallada.NumeroTag",
                                "Este tag ya está registrado");
                            RecargarListas(vm);
                            return View(vm);
                        }
                    }

                    // 4. Validar unicidad de Tarjeta (si se ingresó)
                    if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTarjeta))
                    {
                        var tarjetaExiste = _contenedorTrabajo.InfoDetallada.GetFirstOrDefault(
                            filter: i => i.NumeroTarjeta == vm.InfoDetallada.NumeroTarjeta
                        );

                        if (tarjetaExiste != null)
                        {
                            ModelState.AddModelError("InfoDetallada.NumeroTarjeta",
                                "Esta tarjeta ya está registrada");
                            RecargarListas(vm);
                            return View(vm);
                        }
                    }

                    // ========== GUARDAR ==========

                    // Setear valores por defecto
                    vm.InfoDetallada.FechaCreacion = DateTime.Now;
                    vm.InfoDetallada.Permiso = true;

                    _contenedorTrabajo.InfoDetallada.Add(vm.InfoDetallada);
                    _contenedorTrabajo.Save();

                    TempData["Success"] = "✅ Información detallada creada correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }

            // Si hay errores, recargar listas
            RecargarListas(vm);
            return View(vm);
        }

        /*****************************************************************************************************************/


        #region LLamadas a la API
        // ==================== API PARA MODAL (AJAX) ====================
        [HttpGet]
        public IActionResult GetPersonaById(int id)
        {
            var persona = _contenedorTrabajo.Persona.GetFirstOrDefault(
                filter: p => p.Id == id,
                includeProperties: "Rol,TipoPersona,Sexo"
            );

            if (persona == null)
                return NotFound(new { message = "Persona no encontrada" });

            return Json(new
            {
                id = persona.Id,
                nombre = persona.Nombre,
                apellido = persona.Apellido,
                cedula = persona.Cedula,
                telefono = persona.Telefono,
                email = persona.Email,
                direccion = persona.Direccion,
                nacimiento = persona.Nacimiento.ToString("dd/MM/yyyy"),
                fechaInicio = persona.FechaInicio.ToString("dd/MM/yyyy"),
                fechaFin = persona.FechaFin.ToString("dd/MM/yyyy"),
                rolNombre = persona.Rol?.NombreRol ?? "N/A",
                tipoPersonaNombre = persona.TipoPersona?.NombreTipoPersona ?? "N/A",
                sexoNombre = persona.Sexo?.NombreSexo ?? "N/A"
            });
        }


        #region LLamadas a la API
        [HttpGet]
        public IActionResult BuscarPersona(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return Json(new List<object>()); // si no hay criterio, devolver vacío

            // Normalizar criterio a minúsculas
            criterio = criterio.ToLower();

            IEnumerable<Persona> personas;

            // Si el criterio es numérico, buscar por cédula exacta
            if (criterio.All(char.IsDigit))
            {
                personas = _contenedorTrabajo.Persona.GetAll(
                    filter: p => p.Cedula == criterio
                );
            }
            else
            {
                // Búsqueda por nombre, apellido o email
                personas = _contenedorTrabajo.Persona.GetAll(
                    filter: p => p.Nombre.ToLower().Contains(criterio)
                              || p.Apellido.ToLower().Contains(criterio)
                              || p.Email.ToLower().Contains(criterio)
                );
            }

            // Limitar resultados para que la consulta sea rápida
            var resultado = personas
                .Take(20) // máximo 20 resultados
                .Select(p => new {
                    id = p.Id,
                    nombre = p.Nombre,
                    apellido = p.Apellido,
                    cedula = p.Cedula,
                    email = p.Email
                }).ToList();

            return Json(resultado);
        }



        #endregion




        /*****************************************************************************************************************/


        #region LLamadas a la API
        // ==================== API PARA MODAL (AJAX) ====================
        [HttpGet]
        public IActionResult GetCasaById(int id)
        {
            var casa = _contenedorTrabajo.Casa.GetFirstOrDefault(
                filter: p => p.Id == id,
                includeProperties: "Manzana,Villa,Etapa"
            );

            if (casa == null)
                return NotFound(new { message = "Casa no encontrada" });

            return Json(new
            {
                id = casa.Id,
                NombreFamilia = casa.NombreFamilia,
                Manzana = casa.Manzana?.NombreMz ?? "N/A",
                Villa = casa.Villa?.NombreVilla ?? "N/A",
                Etapa = casa.Etapa?.NombreEtapa ?? "N/A",

                
                
                
            });
        }

        #endregion

        #region LLamadas a la API
        [HttpGet]
        public IActionResult BuscarCasa(string criterio)
        {
            var palabras = criterio?.ToLower()
                                   .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                   ?? Array.Empty<string>();

            var casas = _contenedorTrabajo.Casa.GetAll(
                includeProperties: "Manzana,Villa,Etapa"
            );

            var filtradas = casas.Where(c =>
                palabras.All(p =>
                    (c.NombreFamilia != null && c.NombreFamilia.ToLower().Contains(p)) ||
                    (c.Manzana != null && c.Manzana.NombreMz.ToLower().Contains(p)) ||
                    (c.Villa != null && c.Villa.NombreVilla.ToLower().Contains(p)) ||
                    (c.Etapa != null && c.Etapa.NombreEtapa.ToLower().Contains(p))
                )
            );

            var resultado = filtradas
                .Take(20)
                .Select(c => new {
                    id = c.Id,
                    nombreFamilia = c.NombreFamilia,
                    manzana = c.Manzana?.NombreMz,
                    villa = c.Villa?.NombreVilla,
                    etapa = c.Etapa?.NombreEtapa
                })
                .ToList();

            return Json(resultado);
        }



        #endregion



        /****************************************************************************************************************************************************/

        #region LLamadas a la API

        public IActionResult GetAll()
        {
            var lista = _contenedorTrabajo.InfoDetallada.GetAll(
                includeProperties: "Persona,Casa,Edificio,Vehiculo"
            );
            return Json(new { data = lista });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.InfoDetallada.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error: InfoDetallada no encontrada" });
            }

            // Soft delete - desactivar permiso
            objFromDb.Permiso = false;
            // Como tu Repository no tiene Update, usamos el DbContext directamente
            _contenedorTrabajo.Save();

            return Json(new { success = true, message = "InfoDetallada eliminada correctamente" });
        }

        #endregion

        // ==================== MÉTODO AUXILIAR ====================
        private void RecargarListas(InfoDetalladaVM vm)
        {
            vm.ListaPersonas = _contenedorTrabajo.Persona.GetListaPersonas();
            vm.ListaCasas = _contenedorTrabajo.Casa.GetListaCasas();
            vm.ListaEdificios = _contenedorTrabajo.Edificio.GetListaEdificios();
            vm.ListaVehiculos = _contenedorTrabajo.Vehiculo.GetListaVehiculos();
        }





        #endregion


        /*******************************************************************************************************************************************************/






    }
}


