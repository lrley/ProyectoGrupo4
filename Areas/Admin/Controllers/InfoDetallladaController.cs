using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using DlaccessCore.Models.Models.RelacionesModels;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DLACCESS.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class InfoDetallladaController : Controller
    {


        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public InfoDetallladaController(IWebHostEnvironment hostingEnvironment, IContenedorTrabajo contenedorTrabajo)
        {
            _hostingEnvironment = hostingEnvironment;
            _contenedorTrabajo = contenedorTrabajo;
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}



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
        // ==================== CREATE POST ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InfoDetalladaVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // ========== VALIDACIONES DE UNICIDAD (solo si tienen valor) ==========

                    // Validar Placa solo si no está vacía
                    if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca))
                    {
                        var placaExiste = _contenedorTrabajo.InfoDetallada
                            .GetFirstOrDefault(i => i.NumeroPlaca == vm.InfoDetallada.NumeroPlaca);

                        if (placaExiste != null)
                        {
                            ModelState.AddModelError("InfoDetallada.NumeroPlaca", "La placa ya está registrada");
                            CargarListas(vm);
                            return View(vm);
                        }
                    }

                    // Validar Tag solo si no está vacío
                    if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag))
                    {
                        var tagExiste = _contenedorTrabajo.InfoDetallada
                            .GetFirstOrDefault(i => i.NumeroTag == vm.InfoDetallada.NumeroTag);

                        if (tagExiste != null)
                        {
                            ModelState.AddModelError("InfoDetallada.NumeroTag", "El número de Tag ya está registrado");
                            CargarListas(vm);
                            return View(vm);
                        }
                    }

                    // Validar Tarjeta solo si no está vacía
                    if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTarjeta))
                    {
                        var tarjetaExiste = _contenedorTrabajo.InfoDetallada
                            .GetFirstOrDefault(i => i.NumeroTarjeta == vm.InfoDetallada.NumeroTarjeta);

                        if (tarjetaExiste != null)
                        {
                            ModelState.AddModelError("InfoDetallada.NumeroTarjeta", "El número de Tarjeta ya está registrado");
                            CargarListas(vm);
                            return View(vm);
                        }
                    }

                    // ========== CREAR ENTIDAD ==========
                    var infoDetallada = new InfoDetallada
                    {
                        IdPersona = vm.InfoDetallada.IdPersona,
                        // Usar null si no tiene valor o es 0, para evitar problemas con FK
                        CasaId = vm.InfoDetallada.CasaId > 0 ? vm.InfoDetallada.CasaId : null,
                        IdEdificio = vm.InfoDetallada.IdEdificio > 0 ? vm.InfoDetallada.IdEdificio : null,
                        IdVehiculo = vm.InfoDetallada.IdVehiculo > 0 ? vm.InfoDetallada.IdVehiculo : null,
                        NumeroPlaca = string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca) ? null : vm.InfoDetallada.NumeroPlaca.ToUpper(),
                        NumeroTag = string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag) ? null : vm.InfoDetallada.NumeroTag,
                        NumeroTarjeta = string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTarjeta) ? null : vm.InfoDetallada.NumeroTarjeta,
                        ImgVehiculo = vm.InfoDetallada.ImgVehiculo,
                        Permiso = vm.InfoDetallada.Permiso,
                        FechaCreacion = DateTime.Now,
                        Estado = true,
                        CreatedAt = DateTime.Now
                    };

                    // ========== GUARDAR ==========
                    _contenedorTrabajo.InfoDetallada.Add(infoDetallada);
                    _contenedorTrabajo.Save();

                    // ========== MENSAJE DE ÉXITO ==========
                    TempData["Success"] = "Información detallada guardada correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }

            // Si hay errores, recargar listas
            CargarListas(vm);
            return View(vm);
        }

        // Método auxiliar para cargar listas - USANDO TU ESTRUCTURA
        private void CargarListas(InfoDetalladaVM vm)
        {
            vm.ListaPersonas = _contenedorTrabajo.Persona.GetListaPersonas();
            vm.ListaCasas = _contenedorTrabajo.Casa.GetListaCasas();
            vm.ListaEdificios = _contenedorTrabajo.Edificio.GetListaEdificios();
            vm.ListaVehiculos = _contenedorTrabajo.Vehiculo.GetListaVehiculos();
        }

        // ==================== INDEX (para redirección) ====================
        public IActionResult Index()
        {
            var lista = _contenedorTrabajo.InfoDetallada.GetAll(
                includeProperties: "Persona,Casa,Edificio,Vehiculo"
            );
            return View(lista);
        }




        /*****************************************************************************************************************/


        #region LLamadas a la API
        // ==================== API PARA MODAL PERSONA (AJAX) ====================
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
        // ==================== API PARA MODAL CASA (AJAX) ====================
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
        // ==================== API PARA MODAL EDIFICIO (AJAX) ====================
        [HttpGet]
        public IActionResult GetEdificioById(int id)
        {
            var edificio = _contenedorTrabajo.Edificio.GetFirstOrDefault(
                filter: e => e.Id == id,
                includeProperties: "Piso,Departamento,Etapa"
            );

            if (edificio == null)
                return NotFound(new { message = "Edificio no encontrado" });

            return Json(new
            {
                id = edificio.Id,
                NombreEdificio = edificio.NombreEdificio,
                NombreFamilia = edificio.NombreFamilia,
                Piso = edificio.Piso?.NombrePiso ?? "N/A",
                Departamento = edificio.Departamento?.NombreDepart ?? "N/A",
                Etapa = edificio.Etapa?.NombreEtapa ?? "N/A"
            });
        }
        #endregion


        #region LLamadas a la API
        [HttpGet]
        public IActionResult BuscarEdificio(string criterio)
        {
            var palabras = criterio?.ToLower()
                                   .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                   ?? Array.Empty<string>();

            var edificios = _contenedorTrabajo.Edificio.GetAll(
                includeProperties: "Piso,Departamento,Etapa"
            );

            var filtrados = edificios.Where(e =>
                palabras.All(p =>
                    (e.NombreEdificio != null && e.NombreEdificio.ToLower().Contains(p)) ||
                    (e.NombreFamilia != null && e.NombreFamilia.ToLower().Contains(p)) ||
                    (e.Piso != null && e.Piso.NombrePiso.ToLower().Contains(p)) ||
                    (e.Departamento != null && e.Departamento.NombreDepart.ToLower().Contains(p)) ||
                    (e.Etapa != null && e.Etapa.NombreEtapa.ToLower().Contains(p))
                )
            );

            var resultado = filtrados
                .Take(20)
                .Select(e => new {
                    id = e.Id,
                    NombreEdificio = e.NombreEdificio,
                    NombreFamilia = e.NombreFamilia,
                    Piso = e.Piso?.NombrePiso,
                    Departamento = e.Departamento?.NombreDepart,
                    Etapa = e.Etapa?.NombreEtapa
                })
                .ToList();

            return Json(resultado);
        }
        #endregion


        /****************************************************************************************************************************************************/

        /****************************************************************************************************************************************************/

        #region LLamadas a la API
        // ==================== API PARA MODAL VEHÍCULO (AJAX) ====================
        [HttpGet]
        public IActionResult GetVehiculoById(int id)
        {
            var vehiculo = _contenedorTrabajo.Vehiculo.GetFirstOrDefault(
                filter: v => v.Id == id,
                includeProperties: "Marca,Modelo,TipoVehiculo"
            );

            if (vehiculo == null)
                return NotFound(new { message = "Vehículo no encontrado" });

            return Json(new
            {
                id = vehiculo.Id,
                Placa = vehiculo.NombreVehiculo, // este es tu campo de placa/nombre
                Marca = vehiculo.Marca?.NombreMarca ?? "N/A",
                Modelo = vehiculo.Modelo?.NombreModelo ?? "N/A",
                Color = vehiculo.Color ?? "N/A",
                Tipo = vehiculo.TipoVehiculo?.NombreTipoVehiculo ?? "N/A"
            });
        }
        #endregion

        #region LLamadas a la API
        [HttpGet]
        public IActionResult BuscarVehiculo(string criterio)
        {
            var palabras = criterio?.ToLower()
                                   .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                   ?? Array.Empty<string>();

            var vehiculos = _contenedorTrabajo.Vehiculo.GetAll(
                includeProperties: "Marca,Modelo,TipoVehiculo"
            );

            var filtrados = vehiculos.Where(v =>
                palabras.All(p =>
                    (v.NombreVehiculo != null && v.NombreVehiculo.ToLower().Contains(p)) ||
                    (v.Color != null && v.Color.ToLower().Contains(p)) ||
                    (v.Marca != null && v.Marca.NombreMarca.ToLower().Contains(p)) ||
                    (v.Modelo != null && v.Modelo.NombreModelo.ToLower().Contains(p)) ||
                    (v.TipoVehiculo != null && v.TipoVehiculo.NombreTipoVehiculo.ToLower().Contains(p))
                )
            );

            var resultado = filtrados
                .Take(20)
                .Select(v => new {
                    id = v.Id,
                    Placa = v.NombreVehiculo,
                    Marca = v.Marca?.NombreMarca,
                    Modelo = v.Modelo?.NombreModelo,
                    Color = v.Color,
                    Tipo = v.TipoVehiculo?.NombreTipoVehiculo
                })
                .ToList();

            return Json(resultado);
        }
        #endregion

        /****************************************************************************************************************************************************/



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


