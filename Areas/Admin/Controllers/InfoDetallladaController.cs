using DlaccessCore.AccesoDatos.Data.IRepository;
using DlaccessCore.Models.Models.DatosPersonalesModels;
using DlaccessCore.Models.Models.RelacionesModels;
using DlaccessCore.Models.Models.ViviendaViewModels.Casa;
using DlaccessCore.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DLACCESS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InfoDetallladaController : Controller
    {


        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public InfoDetallladaController(IWebHostEnvironment webHostEnvironment, IContenedorTrabajo contenedorTrabajo)
        {
            _webHostEnvironment = webHostEnvironment;
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
            // =====================================================
            // CARGAR LISTAS PRIMERO (para mantener dropdowns funcionales)
            // =====================================================
            CargarListas(vm);

            List<string> errores = new List<string>();

            // =====================================================
            // 🔹 VALIDACIONES PERSONALIZADAS
            // =====================================================

            // Persona obligatoria
            if (vm.InfoDetallada.IdPersona == 0)
            {
                ModelState.AddModelError("InfoDetallada.IdPersona",
                    "Debe seleccionar una persona.");
                errores.Add("Debe seleccionar una persona.");
            }

            // Casa o Edificio obligatorio
            if (vm.InfoDetallada.CasaId == null && vm.InfoDetallada.IdEdificio == null)
            {
                ModelState.AddModelError("InfoDetallada.CasaId",
                    "Debe seleccionar Casa o Edificio.");
                errores.Add("Debe seleccionar una Casa o un Edificio.");
            }

            // =====================================================
            // 🔹 VALIDACIÓN GRUPO VEHÍCULO (TODO O NADA)
            // =====================================================

            bool hayVehiculo = vm.InfoDetallada.IdVehiculo != null;
            bool hayPlaca = !string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca);
            bool hayTag = !string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag);

            bool algunoLleno = hayVehiculo || hayPlaca || hayTag;
            bool todosLlenos = hayVehiculo && hayPlaca && hayTag;

            if (algunoLleno && !todosLlenos)
            {
                if (!hayVehiculo)
                {
                    ModelState.AddModelError("InfoDetallada.IdVehiculo",
                        "Debe seleccionar un vehículo.");
                    errores.Add("Debe seleccionar un vehículo.");
                }

                if (!hayPlaca)
                {
                    ModelState.AddModelError("InfoDetallada.NumeroPlaca",
                        "Debe ingresar la placa.");
                    errores.Add("Debe ingresar la placa.");
                }

                if (!hayTag)
                {
                    ModelState.AddModelError("InfoDetallada.NumeroTag",
                        "Debe ingresar el número de Tag.");
                    errores.Add("Debe ingresar el número de Tag.");
                }
            }

            // =====================================================
            // 🔹 VALIDACIONES CUANDO HAY VEHÍCULO COMPLETO
            // =====================================================
            if (todosLlenos)
            {
                // Placa única (case-insensitive) - CON PROTECCIÓN NULL
                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca))
                {
                    string placaNormalizada = vm.InfoDetallada.NumeroPlaca.ToUpper().Trim();
                    bool placaExiste = _contenedorTrabajo.InfoDetallada
                        .GetAll()
                        .Any(x => x.NumeroPlaca != null &&
                                  x.NumeroPlaca.ToUpper() == placaNormalizada &&
                                  x.Id != vm.InfoDetallada.Id);

                    if (placaExiste)
                    {
                        ModelState.AddModelError("InfoDetallada.NumeroPlaca",
                            "Esta placa ya está registrada.");
                        errores.Add("La placa ingresada ya existe en el sistema.");
                    }
                }

                // Tag único (case-insensitive) - CON PROTECCIÓN NULL
                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag))
                {
                    string tagNormalizado = vm.InfoDetallada.NumeroTag.ToUpper().Trim();
                    bool tagExiste = _contenedorTrabajo.InfoDetallada
                        .GetAll()
                        .Any(x => x.NumeroTag != null &&
                                  x.NumeroTag.ToUpper() == tagNormalizado &&
                                  x.Id != vm.InfoDetallada.Id);

                    if (tagExiste)
                    {
                        ModelState.AddModelError("InfoDetallada.NumeroTag",
                            "Este número de Tag ya está registrado.");
                        errores.Add("El número de Tag ya está registrado.");
                    }
                }

                // Tarjeta única (si la escriben, case-insensitive) - CON PROTECCIÓN NULL
                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTarjeta))
                {
                    string tarjetaNormalizada = vm.InfoDetallada.NumeroTarjeta.ToUpper().Trim();
                    bool tarjetaExiste = _contenedorTrabajo.InfoDetallada
                        .GetAll()
                        .Any(x => x.NumeroTarjeta != null &&
                                  x.NumeroTarjeta.ToUpper() == tarjetaNormalizada &&
                                  x.Id != vm.InfoDetallada.Id);

                    if (tarjetaExiste)
                    {
                        ModelState.AddModelError("InfoDetallada.NumeroTarjeta",
                            "Este número de tarjeta ya está registrado.");
                        errores.Add("El número de tarjeta ya está registrado.");
                    }
                }
            }

            // =====================================================
            // 🔹 VALIDAR DUPLICIDAD PERSONA + CASA
            // (solo si NO hay vehículo)
            // =====================================================
            if (!todosLlenos && vm.InfoDetallada.CasaId != null)
            {
                bool existePersonaCasa = _contenedorTrabajo.InfoDetallada
                    .GetAll()
                    .Any(x =>
                        x.IdPersona == vm.InfoDetallada.IdPersona &&
                        x.CasaId == vm.InfoDetallada.CasaId &&
                        x.IdVehiculo == null &&
                        x.Id != vm.InfoDetallada.Id);

                if (existePersonaCasa)
                {
                    ModelState.AddModelError("InfoDetallada.CasaId",
                        "Esta persona ya está registrada en esa misma casa.");
                    errores.Add("Esta persona ya está asignada a esa casa.");
                }
            }

            // =====================================================
            // 🔹 VALIDAR DUPLICIDAD PERSONA + EDIFICIO
            // (solo si NO hay vehículo)
            // =====================================================
            if (!todosLlenos && vm.InfoDetallada.IdEdificio != null)
            {
                bool existePersonaEdificio = _contenedorTrabajo.InfoDetallada
                    .GetAll()
                    .Any(x =>
                        x.IdPersona == vm.InfoDetallada.IdPersona &&
                        x.IdEdificio == vm.InfoDetallada.IdEdificio &&
                        x.IdVehiculo == null &&
                        x.Id != vm.InfoDetallada.Id);

                if (existePersonaEdificio)
                {
                    ModelState.AddModelError("InfoDetallada.IdEdificio",
                        "Esta persona ya está registrada en ese mismo edificio.");
                    errores.Add("Esta persona ya está asignada a ese edificio.");
                }
            }

            // =====================================================
            // 🔹 SI HAY ERRORES → MOSTRAR ALERTA GENERAL
            // =====================================================
            if (errores.Any())
            {
                TempData["Error"] = string.Join("<br>", errores);
            }

            if (!ModelState.IsValid)
            {
                // Las listas ya están cargadas arriba, solo regresamos la vista
                return View(vm);
            }

            // =====================================================
            // 🔹 GUARDAR SI TODO ESTÁ VÁLIDO
            // =====================================================
            try
            {
                string rutaImagen = null;

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string carpeta = Path.Combine(wwwRootPath, "imagenVehiculo");

                // Crear carpeta si no existe
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                // ==========================
                // 1️⃣ SI VIENE ARCHIVO
                // ==========================
                if (vm.ArchivoImagen != null && vm.ArchivoImagen.Length > 0)
                {
                    // Validar extensión permitida
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                    string extension = Path.GetExtension(vm.ArchivoImagen.FileName).ToLower();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("", "Solo se permiten imágenes JPG, JPEG o PNG.");
                        TempData["Error"] = "Solo se permiten imágenes JPG, JPEG o PNG.";
                        return View(vm);
                    }

                    // Validar tamaño máximo (5MB)
                    if (vm.ArchivoImagen.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "La imagen no debe superar los 5MB.");
                        TempData["Error"] = "La imagen no debe superar los 5MB.";
                        return View(vm);
                    }

                    string nombreArchivo = Guid.NewGuid().ToString() + extension;
                    string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                    using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        vm.ArchivoImagen.CopyTo(fileStream);
                    }

                    rutaImagen = "/imagenVehiculo/" + nombreArchivo;
                }
                // ==========================
                // 2️⃣ SI VIENE FOTO BASE64 (CÁMARA)
                // ==========================
                else if (!string.IsNullOrEmpty(vm.FotoBase64))
                {
                    string base64Data;

                    // Validar formato Base64 de forma segura
                    if (vm.FotoBase64.Contains(","))
                    {
                        base64Data = vm.FotoBase64.Split(',')[1];
                    }
                    else
                    {
                        base64Data = vm.FotoBase64;
                    }

                    // Validar que sea Base64 válido
                    try
                    {
                        byte[] bytes = Convert.FromBase64String(base64Data);

                        // Validar tamaño máximo (5MB)
                        if (bytes.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "La imagen de la cámara no debe superar los 5MB.");
                            TempData["Error"] = "La imagen de la cámara no debe superar los 5MB.";
                            return View(vm);
                        }

                        string nombreArchivo = Guid.NewGuid().ToString() + ".jpg";
                        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                        System.IO.File.WriteAllBytes(rutaCompleta, bytes);
                        rutaImagen = "/imagenVehiculo/" + nombreArchivo;
                    }
                    catch (FormatException)
                    {
                        ModelState.AddModelError("", "La imagen de la cámara no tiene un formato válido.");
                        TempData["Error"] = "La imagen de la cámara no tiene un formato válido.";
                        return View(vm);
                    }
                }

                // ==========================
                // ASIGNAR IMAGEN
                // ==========================
                vm.InfoDetallada.ImgVehiculo = rutaImagen;

                // Normalizar datos de texto (mayúsculas y sin espacios extras)
                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca))
                    vm.InfoDetallada.NumeroPlaca = vm.InfoDetallada.NumeroPlaca.ToUpper().Trim();

                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag))
                    vm.InfoDetallada.NumeroTag = vm.InfoDetallada.NumeroTag.ToUpper().Trim();

                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTarjeta))
                    vm.InfoDetallada.NumeroTarjeta = vm.InfoDetallada.NumeroTarjeta.ToUpper().Trim();

                // Fechas y estado
                vm.InfoDetallada.FechaCreacion = DateTime.Now;
                vm.InfoDetallada.CreatedAt = DateTime.Now;
                vm.InfoDetallada.Estado = true;

                // Guardar en BD
                _contenedorTrabajo.InfoDetallada.Add(vm.InfoDetallada);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Información guardada correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                return View(vm);
            }
        }

        private void CargarListas(InfoDetalladaVM vm)
        {
            vm.ListaPersonas = _contenedorTrabajo.Persona.GetListaPersonas();
            vm.ListaCasas = _contenedorTrabajo.Casa.GetListaCasas();
            vm.ListaEdificios = _contenedorTrabajo.Edificio.GetListaEdificios();
            vm.ListaVehiculos = _contenedorTrabajo.Vehiculo.GetListaVehiculos();
        }



        /*********************************************EDIT********************************************************************/
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infoDetallada = _contenedorTrabajo.InfoDetallada.Get(id.Value);
            if (infoDetallada == null)
            {
                return NotFound();
            }

            // ✅ DEBUG: Verificar qué valor tiene en la BD
            System.Diagnostics.Debug.WriteLine($"=== DEBUG EDIT GET ===");
            System.Diagnostics.Debug.WriteLine($"ID: {infoDetallada.Id}");
            System.Diagnostics.Debug.WriteLine($"Estado en BD: {infoDetallada.Estado}");
            System.Diagnostics.Debug.WriteLine($"Estado tipo: {infoDetallada.Estado.GetType()}");

            InfoDetalladaVM vm = new InfoDetalladaVM
            {
                InfoDetallada = infoDetallada
            };

            // Cargar nombres para mostrar en los selects
            CargarNombresParaEdit(vm, infoDetallada);
            CargarListas(vm);

            return View(vm);
        }
        private void CargarNombresParaEdit(InfoDetalladaVM vm, InfoDetallada info)
        {
            // Cargar nombres para que Select2 muestre la opción seleccionada
            if (info.IdPersona > 0)
            {
                var persona = _contenedorTrabajo.Persona.Get(info.IdPersona);
                if (persona != null)
                {
                    vm.NombrePersonaSeleccionada = $"{persona.Nombre} {persona.Apellido} - {persona.Cedula}";
                }
            }

            if (info.CasaId != null && info.CasaId > 0)
            {
                var casa = _contenedorTrabajo.Casa.Get(info.CasaId.Value);
                if (casa != null)
                {
                    vm.NombreCasaSeleccionada = $"{casa.NombreFamilia} (Mz: {casa.Manzana}, Villa: {casa.Villa})";
                }
            }

            if (info.IdEdificio != null && info.IdEdificio > 0)
            {
                var edificio = _contenedorTrabajo.Edificio.Get(info.IdEdificio.Value);
                if (edificio != null)
                {
                    vm.NombreEdificioSeleccionado = $"{edificio.NombreEdificio} (Piso: {edificio.Piso}, Dpto: {edificio.Departamento})";
                }
            }

            if (info.IdVehiculo != null && info.IdVehiculo > 0)
            {
                var vehiculo = _contenedorTrabajo.Vehiculo.Get(info.IdVehiculo.Value);
                if (vehiculo != null)
                {
                    // Usar NombreVehiculo (año) + NombreMarca + NombreModelo + Color
                    vm.NombreVehiculoSeleccionado = $"{vehiculo.NombreVehiculo} {vehiculo.Marca?.NombreMarca} {vehiculo.Modelo?.NombreModelo} ({vehiculo.Color})";
                }
            }
        }
        private void CargarNombresSeleccionados(InfoDetalladaVM vm)
        {
            if (vm.InfoDetallada.IdPersona > 0)
            {
                var persona = _contenedorTrabajo.Persona.Get(vm.InfoDetallada.IdPersona);
                if (persona != null)
                {
                    vm.NombrePersonaSeleccionada = $"{persona.Nombre} {persona.Apellido} - {persona.Cedula}";
                }
            }

            if (vm.InfoDetallada.CasaId != null && vm.InfoDetallada.CasaId > 0)
            {
                var casa = _contenedorTrabajo.Casa.Get(vm.InfoDetallada.CasaId.Value);
                if (casa != null)
                {
                    vm.NombreCasaSeleccionada = $"{casa.NombreFamilia} (Mz: {casa.Manzana}, Villa: {casa.Villa})";
                }
            }

            if (vm.InfoDetallada.IdEdificio != null && vm.InfoDetallada.IdEdificio > 0)
            {
                var edificio = _contenedorTrabajo.Edificio.Get(vm.InfoDetallada.IdEdificio.Value);
                if (edificio != null)
                {
                    vm.NombreEdificioSeleccionado = $"{edificio.NombreEdificio} (Piso: {edificio.Piso}, Dpto: {edificio.Departamento})";
                }
            }

            if (vm.InfoDetallada.IdVehiculo != null && vm.InfoDetallada.IdVehiculo > 0)
            {
                var vehiculo = _contenedorTrabajo.Vehiculo.Get(vm.InfoDetallada.IdVehiculo.Value);
                if (vehiculo != null)
                {
                    vm.NombreVehiculoSeleccionado = $"{vehiculo.NombreVehiculo} {vehiculo.Marca?.NombreMarca} {vehiculo.Modelo?.NombreModelo} ({vehiculo.Color})";
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(InfoDetalladaVM vm)
        {
            // ✅ Cargar listas al inicio para que no se pierdan los dropdowns si hay error
            CargarListas(vm);

            if (vm.InfoDetallada.Id == 0)
            {
                return NotFound();
            }

            List<string> errores = new List<string>();

            // =====================================================
            // 🔹 VALIDACIONES PERSONALIZADAS
            // =====================================================

            // Persona obligatoria
            if (vm.InfoDetallada.IdPersona == 0)
            {
                ModelState.AddModelError("InfoDetallada.IdPersona",
                    "Debe seleccionar una persona.");
                errores.Add("Debe seleccionar una persona.");
            }

            // Casa o Edificio obligatorio
            if (vm.InfoDetallada.CasaId == null && vm.InfoDetallada.IdEdificio == null)
            {
                ModelState.AddModelError("InfoDetallada.CasaId",
                    "Debe seleccionar Casa o Edificio.");
                errores.Add("Debe seleccionar una Casa o un Edificio.");
            }

            // =====================================================
            // 🔹 VALIDACIÓN GRUPO VEHÍCULO (TODO O NADA)
            // =====================================================

            bool hayVehiculo = vm.InfoDetallada.IdVehiculo != null;
            bool hayPlaca = !string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca);
            bool hayTag = !string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag);

            bool algunoLleno = hayVehiculo || hayPlaca || hayTag;
            bool todosLlenos = hayVehiculo && hayPlaca && hayTag;

            if (algunoLleno && !todosLlenos)
            {
                if (!hayVehiculo)
                {
                    ModelState.AddModelError("InfoDetallada.IdVehiculo",
                        "Debe seleccionar un vehículo.");
                    errores.Add("Debe seleccionar un vehículo.");
                }

                if (!hayPlaca)
                {
                    ModelState.AddModelError("InfoDetallada.NumeroPlaca",
                        "Debe ingresar la placa.");
                    errores.Add("Debe ingresar la placa.");
                }

                if (!hayTag)
                {
                    ModelState.AddModelError("InfoDetallada.NumeroTag",
                        "Debe ingresar el número de Tag.");
                    errores.Add("Debe ingresar el número de Tag.");
                }
            }

            // =====================================================
            // 🔹 VALIDACIONES CUANDO HAY VEHÍCULO COMPLETO
            // =====================================================
            if (todosLlenos)
            {
                // Placa única (case-insensitive) - CON PROTECCIÓN NULL
                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca))
                {
                    string placaNormalizada = vm.InfoDetallada.NumeroPlaca.ToUpper().Trim();
                    bool placaExiste = _contenedorTrabajo.InfoDetallada
                        .GetAll()
                        .Any(x => x.NumeroPlaca != null &&
                                  x.NumeroPlaca.ToUpper() == placaNormalizada &&
                                  x.Id != vm.InfoDetallada.Id); // ✅ Excluir el actual

                    if (placaExiste)
                    {
                        ModelState.AddModelError("InfoDetallada.NumeroPlaca",
                            "Esta placa ya está registrada.");
                        errores.Add("La placa ingresada ya existe en el sistema.");
                    }
                }

                // Tag único (case-insensitive) - CON PROTECCIÓN NULL
                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag))
                {
                    string tagNormalizado = vm.InfoDetallada.NumeroTag.ToUpper().Trim();
                    bool tagExiste = _contenedorTrabajo.InfoDetallada
                        .GetAll()
                        .Any(x => x.NumeroTag != null &&
                                  x.NumeroTag.ToUpper() == tagNormalizado &&
                                  x.Id != vm.InfoDetallada.Id); // ✅ Excluir el actual

                    if (tagExiste)
                    {
                        ModelState.AddModelError("InfoDetallada.NumeroTag",
                            "Este número de Tag ya está registrado.");
                        errores.Add("El número de Tag ya está registrado.");
                    }
                }

                // Tarjeta única (si la escriben, case-insensitive) - CON PROTECCIÓN NULL
                if (!string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTarjeta))
                {
                    string tarjetaNormalizada = vm.InfoDetallada.NumeroTarjeta.ToUpper().Trim();
                    bool tarjetaExiste = _contenedorTrabajo.InfoDetallada
                        .GetAll()
                        .Any(x => x.NumeroTarjeta != null &&
                                  x.NumeroTarjeta.ToUpper() == tarjetaNormalizada &&
                                  x.Id != vm.InfoDetallada.Id); // ✅ Excluir el actual

                    if (tarjetaExiste)
                    {
                        ModelState.AddModelError("InfoDetallada.NumeroTarjeta",
                            "Este número de tarjeta ya está registrado.");
                        errores.Add("El número de tarjeta ya está registrado.");
                    }
                }
            }

            // =====================================================
            // 🔹 VALIDAR DUPLICIDAD PERSONA + CASA
            // =====================================================
            if (!todosLlenos && vm.InfoDetallada.CasaId != null)
            {
                bool existePersonaCasa = _contenedorTrabajo.InfoDetallada
                    .GetAll()
                    .Any(x =>
                        x.IdPersona == vm.InfoDetallada.IdPersona &&
                        x.CasaId == vm.InfoDetallada.CasaId &&
                        x.IdVehiculo == null &&
                        x.Id != vm.InfoDetallada.Id); // ✅ Excluir el actual

                if (existePersonaCasa)
                {
                    ModelState.AddModelError("InfoDetallada.CasaId",
                        "Esta persona ya está registrada en esa misma casa.");
                    errores.Add("Esta persona ya está asignada a esa casa.");
                }
            }

            // =====================================================
            // 🔹 VALIDAR DUPLICIDAD PERSONA + EDIFICIO
            // =====================================================
            if (!todosLlenos && vm.InfoDetallada.IdEdificio != null)
            {
                bool existePersonaEdificio = _contenedorTrabajo.InfoDetallada
                    .GetAll()
                    .Any(x =>
                        x.IdPersona == vm.InfoDetallada.IdPersona &&
                        x.IdEdificio == vm.InfoDetallada.IdEdificio &&
                        x.IdVehiculo == null &&
                        x.Id != vm.InfoDetallada.Id); // ✅ Excluir el actual

                if (existePersonaEdificio)
                {
                    ModelState.AddModelError("InfoDetallada.IdEdificio",
                        "Esta persona ya está registrada en ese mismo edificio.");
                    errores.Add("Esta persona ya está asignada a ese edificio.");
                }
            }

            // =====================================================
            // 🔹 SI HAY ERRORES → MOSTRAR ALERTA GENERAL
            // =====================================================
            if (errores.Any())
            {
                TempData["Error"] = string.Join("<br>", errores);
            }

            if (!ModelState.IsValid)
            {
                // ✅ Recargar nombres para que los selects muestren el texto correcto
                CargarNombresSeleccionados(vm);
                return View(vm);
            }

            // =====================================================
            // 🔹 ACTUALIZAR REGISTRO
            // =====================================================
            try
            {
                // Obtener el registro existente de la BD
                var infoExistente = _contenedorTrabajo.InfoDetallada.Get(vm.InfoDetallada.Id);
                if (infoExistente == null)
                {
                    return NotFound();
                }

                string rutaImagen = infoExistente.ImgVehiculo; // Mantener imagen actual por defecto

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string carpeta = Path.Combine(wwwRootPath, "imagenVehiculo");

                // Crear carpeta si no existe
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                // ==========================
                // 1️⃣ SI VIENE NUEVA IMAGEN ARCHIVO
                // ==========================
                if (vm.ArchivoImagen != null && vm.ArchivoImagen.Length > 0)
                {
                    // Validar extensión permitida
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                    string extension = Path.GetExtension(vm.ArchivoImagen.FileName).ToLower();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("", "Solo se permiten imágenes JPG, JPEG o PNG.");
                        TempData["Error"] = "Solo se permiten imágenes JPG, JPEG o PNG.";
                        CargarNombresSeleccionados(vm);
                        return View(vm);
                    }

                    // Validar tamaño máximo (5MB)
                    if (vm.ArchivoImagen.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "La imagen no debe superar los 5MB.");
                        TempData["Error"] = "La imagen no debe superar los 5MB.";
                        CargarNombresSeleccionados(vm);
                        return View(vm);
                    }

                    // Eliminar imagen anterior si existe
                    if (!string.IsNullOrEmpty(infoExistente.ImgVehiculo))
                    {
                        string rutaAnterior = Path.Combine(wwwRootPath, infoExistente.ImgVehiculo.TrimStart('/'));
                        if (System.IO.File.Exists(rutaAnterior))
                        {
                            System.IO.File.Delete(rutaAnterior);
                        }
                    }

                    string nombreArchivo = Guid.NewGuid().ToString() + extension;
                    string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                    using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        vm.ArchivoImagen.CopyTo(fileStream);
                    }

                    rutaImagen = "/imagenVehiculo/" + nombreArchivo;
                }
                // ==========================
                // 2️⃣ SI VIENE NUEVA FOTO BASE64 (CÁMARA)
                // ==========================
                else if (!string.IsNullOrEmpty(vm.FotoBase64))
                {
                    string base64Data;

                    if (vm.FotoBase64.Contains(","))
                    {
                        base64Data = vm.FotoBase64.Split(',')[1];
                    }
                    else
                    {
                        base64Data = vm.FotoBase64;
                    }

                    try
                    {
                        byte[] bytes = Convert.FromBase64String(base64Data);

                        if (bytes.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "La imagen de la cámara no debe superar los 5MB.");
                            TempData["Error"] = "La imagen de la cámara no debe superar los 5MB.";
                            CargarNombresSeleccionados(vm);
                            return View(vm);
                        }

                        // Eliminar imagen anterior si existe
                        if (!string.IsNullOrEmpty(infoExistente.ImgVehiculo))
                        {
                            string rutaAnterior = Path.Combine(wwwRootPath, infoExistente.ImgVehiculo.TrimStart('/'));
                            if (System.IO.File.Exists(rutaAnterior))
                            {
                                System.IO.File.Delete(rutaAnterior);
                            }
                        }

                        string nombreArchivo = Guid.NewGuid().ToString() + ".jpg";
                        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                        System.IO.File.WriteAllBytes(rutaCompleta, bytes);
                        rutaImagen = "/imagenVehiculo/" + nombreArchivo;
                    }
                    catch (FormatException)
                    {
                        ModelState.AddModelError("", "La imagen de la cámara no tiene un formato válido.");
                        TempData["Error"] = "La imagen de la cámara no tiene un formato válido.";
                        CargarNombresSeleccionados(vm);
                        return View(vm);
                    }
                }

                // ==========================
                // ACTUALIZAR PROPIEDADES DEL REGISTRO EXISTENTE
                // ==========================
                infoExistente.IdPersona = vm.InfoDetallada.IdPersona;
                infoExistente.CasaId = vm.InfoDetallada.CasaId;
                infoExistente.IdEdificio = vm.InfoDetallada.IdEdificio;
                infoExistente.IdVehiculo = vm.InfoDetallada.IdVehiculo;
                infoExistente.Estado = vm.InfoDetallada.Estado;
                infoExistente.Permiso = vm.InfoDetallada.Permiso;

                // Normalizar textos
                infoExistente.NumeroPlaca = !string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroPlaca)
                    ? vm.InfoDetallada.NumeroPlaca.ToUpper().Trim()
                    : null;
                infoExistente.NumeroTag = !string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTag)
                    ? vm.InfoDetallada.NumeroTag.ToUpper().Trim()
                    : null;
                infoExistente.NumeroTarjeta = !string.IsNullOrWhiteSpace(vm.InfoDetallada.NumeroTarjeta)
                    ? vm.InfoDetallada.NumeroTarjeta.ToUpper().Trim()
                    : null;

                infoExistente.ImgVehiculo = rutaImagen;

                // Fechas de actualización
                infoExistente.UpdatedAt = DateTime.Now;
                // No modificar CreatedAt ni FechaCreacion (preservar originales)

                // Actualizar en BD
                _contenedorTrabajo.InfoDetallada.Update(infoExistente);
                _contenedorTrabajo.Save();

                TempData["Success"] = "Información actualizada correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                CargarNombresSeleccionados(vm);
                return View(vm);
            }
        }

        /************************************************END EDIT*******************************************************************/

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
                return Json(new List<object>());

            criterio = criterio.Trim();

            // Convertir a minúscula para búsqueda case-insensitive
            var criterioLower = criterio.ToLower();

            var personas = _contenedorTrabajo.Persona.GetAll(
                includeProperties: "Rol,Sexo,TipoPersona"
            );

            // Filtrar en memoria incluyendo TipoPersona
            var resultado = personas
                .Where(p =>
                    (p.Nombre != null && p.Nombre.ToLower().Contains(criterioLower)) ||
                    (p.Apellido != null && p.Apellido.ToLower().Contains(criterioLower)) ||
                    (p.Email != null && p.Email.ToLower().Contains(criterioLower)) ||
                    (p.Telefono != null && p.Telefono.ToLower().Contains(criterioLower)) ||
                    (p.Cedula != null && p.Cedula.ToLower().Contains(criterioLower)) ||
                    // ✅ AGREGADO: Búsqueda por TipoPersona
                    (p.TipoPersona != null && p.TipoPersona.NombreTipoPersona != null &&
                     p.TipoPersona.NombreTipoPersona.ToLower().Contains(criterioLower))
                )
                .OrderBy(p => p.Nombre)
                .Take(20)
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    apellido = p.Apellido,
                    cedula = p.Cedula,
                    email = p.Email,
                    telefono = p.Telefono,
                    rolNombre = p.Rol?.NombreRol ?? "",
                    sexoNombre = p.Sexo?.NombreSexo ?? "",
                    tipopersona = p.TipoPersona?.NombreTipoPersona ?? "",
                })
                .ToList();

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
                includeProperties: "Manzana,Villa,Etapa,Etapa.Urbanizacion"
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
                Urbanizacion = casa.Etapa?.Urbanizacion?.NombreUrbanizacion ?? "N/A"





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
                includeProperties: "Manzana,Villa,Etapa,Etapa.Urbanizacion"
            );

            var filtradas = casas.Where(c =>
                palabras.All(p =>
                    (c.NombreFamilia != null && c.NombreFamilia.ToLower().Contains(p)) ||
                    (c.Manzana != null && c.Manzana.NombreMz.ToLower().Contains(p)) ||
                    (c.Villa != null && c.Villa.NombreVilla.ToLower().Contains(p)) ||
                    (c.Etapa != null && c.Etapa.NombreEtapa.ToLower().Contains(p)) ||
                    (c.Etapa != null && c.Etapa.Urbanizacion != null && c.Etapa.Urbanizacion.NombreUrbanizacion.ToLower().Contains(p))



                )
            );

            var resultado = filtradas
                .Take(20)
                .Select(c => new {
                    id = c.Id,
                    nombreFamilia = c.NombreFamilia,
                    manzana = c.Manzana?.NombreMz,
                    villa = c.Villa?.NombreVilla,
                    etapa = c.Etapa?.NombreEtapa,
                    urbanizacion = c.Etapa?.Urbanizacion?.NombreUrbanizacion
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
                includeProperties: "Piso,Departamento,Etapa,Etapa.Urbanizacion"
            );

            if (edificio == null)
                return NotFound(new { message = "Edificio no encontrado" });

            return Json(new
            {
                id = edificio.Id,
                NombreFamilia = edificio.NombreFamilia,
                Piso = edificio.Piso?.NombrePiso ?? "N/A",
                Departamento = edificio.Departamento?.NombreDepart ?? "N/A",
                Etapa = edificio.Etapa?.NombreEtapa ?? "N/A",
                Urbanizacion = edificio.Etapa?.Urbanizacion?.NombreUrbanizacion ?? "N/A"
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
                includeProperties: "Piso,Departamento,Etapa,Etapa.Urbanizacion"
            );

            var filtrados = edificios.Where(e =>
                palabras.All(p =>
                    (e.NombreFamilia != null && e.NombreFamilia.ToLower().Contains(p)) ||
                    (e.Piso != null && e.Piso.NombrePiso.ToLower().Contains(p)) ||
                    (e.Departamento != null && e.Departamento.NombreDepart.ToLower().Contains(p)) ||
                    (e.Etapa != null && e.Etapa.NombreEtapa.ToLower().Contains(p)) ||
                    (e.Etapa != null && e.Etapa.Urbanizacion != null && e.Etapa.Urbanizacion.NombreUrbanizacion.ToLower().Contains(p))
                )
            );

            var resultado = filtrados
                .Take(20)
                .Select(e => new {
                    id = e.Id,
                    NombreFamilia = e.NombreFamilia ?? "",
                    Piso = e.Piso?.NombrePiso ?? "",
                    Departamento = e.Departamento?.NombreDepart ?? "",
                    Etapa = e.Etapa?.NombreEtapa ?? "",
                    Urbanizacion = e.Etapa?.Urbanizacion?.NombreUrbanizacion ?? ""
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
                anio = vehiculo.NombreVehiculo,
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
            if (string.IsNullOrWhiteSpace(criterio))
                return Json(new List<object>());

            criterio = criterio.Trim().ToLower();

            var vehiculos = _contenedorTrabajo.Vehiculo.GetAll(
                includeProperties: "Marca,Modelo,TipoVehiculo"
            );

            var filtrados = vehiculos.Where(v =>
                (v.NombreVehiculo != null && v.NombreVehiculo.ToLower().Contains(criterio)) ||
                (v.Color != null && v.Color.ToLower().Contains(criterio)) ||
                (v.Marca != null && v.Marca.NombreMarca.ToLower().Contains(criterio)) ||
                (v.Modelo != null && v.Modelo.NombreModelo.ToLower().Contains(criterio)) ||
                (v.TipoVehiculo != null && v.TipoVehiculo.NombreTipoVehiculo.ToLower().Contains(criterio))
            );

            var resultado = filtrados
                .OrderBy(v => v.NombreVehiculo)
                .Take(20)
                .Select(v => new
                {
                    id = v.Id,
                    anio = v.NombreVehiculo,   // 👈 dejamos como año
                    marca = v.Marca?.NombreMarca,
                    modelo = v.Modelo?.NombreModelo,
                    color = v.Color,
                    tipo = v.TipoVehiculo?.NombreTipoVehiculo
                })
                .ToList();

            return Json(resultado);
        }


        #endregion

        /******************************************************************FIN DE TODO CREATE CON APIS MODALES**********************************************************************************/



        /****************************************************************************************************************************************************/
        #region LLamadas a la API





        /****************************************************************************************************************************************************/
        #region MÉTODOS PARA DATATABLE (INDEX)

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // CORREGIDO: Incluir todas las relaciones necesarias con sus sub-relaciones
                var lista = _contenedorTrabajo.InfoDetallada.GetAll(
                    includeProperties: "Persona,Persona.TipoPersona,Casa,Casa.Manzana,Casa.Villa,Casa.Etapa,Edificio,Edificio.Piso,Edificio.Departamento,Edificio.Etapa,Vehiculo,Vehiculo.Marca,Vehiculo.Modelo"
                );

                var resultado = lista.Select(i => new
                {
                    id = i.Id,
                    persona = new
                    {
                        nombre = i.Persona?.Nombre ?? "N/A",
                        apellido = i.Persona?.Apellido ?? "",
                        cedula = i.Persona?.Cedula ?? "N/A",
                        tipopropietario = i.Persona?.TipoPersona?.NombreTipoPersona ?? "N/A"
                    },
                    // CORREGIDO: Usar las propiedades de navegación incluidas
                    casa = i.Casa == null ? null : new
                    {
                        nombreFamilia = i.Casa.NombreFamilia,
                        manzana = i.Casa.Manzana?.NombreMz,        // CORREGIDO
                        villa = i.Casa.Villa?.NombreVilla,          // CORREGIDO
                        etapa = i.Casa.Etapa?.NombreEtapa,         // CORREGIDO
                        urbanizacion = i.Casa.Etapa?.Urbanizacion?.NombreUrbanizacion
                    },
                    edificio = i.Edificio == null ? null : new
                    {
                        nombreEdificio = i.Edificio.NombreEdificio,
                        nombreFamilia = i.Edificio.NombreFamilia,   // CORREGIDO: agregado
                        piso = i.Edificio.Piso?.NombrePiso,         // CORREGIDO
                        departamento = i.Edificio.Departamento?.NombreDepart,  // CORREGIDO
                        urbanizacion = i.Edificio.Etapa?.Urbanizacion?.NombreUrbanizacion
                    },
                    vehiculo = i.Vehiculo == null ? null : new
                    {
                        fabricacion = i.Vehiculo.NombreVehiculo,
                        placa = i.Vehiculo.NombreVehiculo,          // CORREGIDO: este es tu campo
                        marca = i.Vehiculo.Marca?.NombreMarca,      // CORREGIDO
                        modelo = i.Vehiculo.Modelo?.NombreModelo    // CORREGIDO
                    },
                    numeroPlaca = i.NumeroPlaca ?? "-",
                    numeroTag = i.NumeroTag ?? "-",
                    numeroTarjeta = i.NumeroTarjeta ?? "-",
                    foto = string.IsNullOrEmpty(i.ImgVehiculo) || i.ImgVehiculo == "Sin Foto"
                        ? "Sin-Foto"
                        : i.ImgVehiculo,
                    fechaCreacion = i.FechaCreacion,
                    permiso = i.Permiso,
                    estado = i.Estado
                }).ToList();

                return Json(new { data = resultado });
            }
            catch (Exception ex)
            {
                // Log del error para depuración
                Console.WriteLine($"ERROR en GetAll: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new { data = new object[] { }, error = ex.Message });
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var info = _contenedorTrabajo.InfoDetallada.Get(id);

                if (info == null)
                    return Json(new { success = false, message = "Registro no encontrado" });

                // Soft delete usando Permiso
                info.Permiso = false;
                info.UpdatedAt = DateTime.Now;

                _contenedorTrabajo.InfoDetallada.Update(info);
                _contenedorTrabajo.Save();

                return Json(new { success = true, message = "Registro eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetDetalle(int id)
        {
            try
            {
                // CORREGIDO: Mismos includeProperties que GetAll
                var info = _contenedorTrabajo.InfoDetallada.GetFirstOrDefault(
                    filter: i => i.Id == id,
                    includeProperties: "Persona,Casa,Casa.Manzana,Casa.Villa,Casa.Etapa,Edificio,Edificio.Piso,Edificio.Departamento,Edificio.Etapa,Vehiculo,Vehiculo.Marca,Vehiculo.Modelo"
                );

                if (info == null)
                    return NotFound();

                var resultado = new
                {
                    id = info.Id,
                    persona = new
                    {
                        nombre = info.Persona?.Nombre ?? "N/A",
                        apellido = info.Persona?.Apellido ?? "",
                        cedula = info.Persona?.Cedula ?? "N/A",
                        email = info.Persona?.Email ?? "-",
                        telefono = info.Persona?.Telefono ?? "-"
                    },
                    casa = info.Casa == null ? null : new
                    {
                        nombreFamilia = info.Casa.NombreFamilia,
                        manzana = info.Casa.Manzana?.NombreMz,
                        villa = info.Casa.Villa?.NombreVilla,
                        etapa = info.Casa.Etapa?.NombreEtapa,
                        urbanizacion = info.Casa.Etapa?.Urbanizacion?.NombreUrbanizacion
                    },
                    edificio = info.Edificio == null ? null : new
                    {
                        nombreEdificio = info.Edificio.NombreEdificio,
                        nombreFamilia = info.Edificio.NombreFamilia,
                        piso = info.Edificio.Piso?.NombrePiso,
                        departamento = info.Edificio.Departamento?.NombreDepart,
                        urbanizacion = info.Edificio.Etapa?.Urbanizacion?.NombreUrbanizacion
                    },
                    vehiculo = info.Vehiculo == null ? null : new
                    {
                        placa = info.Vehiculo.NombreVehiculo,
                        marca = info.Vehiculo.Marca?.NombreMarca,
                        modelo = info.Vehiculo.Modelo?.NombreModelo,
                        color = info.Vehiculo.Color
                    },
                    numeroPlaca = info.NumeroPlaca,
                    numeroTag = info.NumeroTag,
                    numeroTarjeta = info.NumeroTarjeta,
                    foto = string.IsNullOrEmpty(info.ImgVehiculo) || info.ImgVehiculo == "Sin Foto"
                        ? "Sin-Foto"
                        : info.ImgVehiculo,
                    fechaCreacion = info.FechaCreacion,
                    permiso = info.Permiso
                };

                return Json(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en GetDetalle: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion




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

