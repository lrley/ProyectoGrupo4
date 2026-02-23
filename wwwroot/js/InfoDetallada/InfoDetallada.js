// InfoDetallada.js - Módulo unificado para gestión de entidades (Persona, Casa, Edificio, Vehículo)
// Variables globales para cachear datos seleccionados
const entidades = {
    persona: { seleccionada: null, config: null },
    casa: { seleccionada: null, config: null },
    edificio: { seleccionada: null, config: null },
    vehiculo: { seleccionada: null, config: null }
};

$(document).ready(function () {
    // Configuración de cada entidad
    const configs = [
        {
            tipo: 'persona',
            nombre: 'Persona',
            selectId: 'personaSelect',
            btnVerId: 'btnVerPersona',
            btnResetId: 'btnResetPersona', // NUEVO: ID del botón reset
            badgeId: 'badgePersona',
            previewId: 'previewPersona',
            previewContentId: 'previewContent',
            modalId: 'modalPersona',
            datosId: 'datosPersona',
            loadingId: 'loadingPersona',
            urlBuscar: '/Admin/InfoDetalllada/BuscarPersona',
            urlDetalle: '/Admin/InfoDetalllada/GetPersonaById',
            placeholder: 'Escriba nombre, cédula o email...',
            mensajeNoResultados: 'No se encontraron personas',
            mensajeSeleccionar: 'Seleccione una persona primero',
            usaCache: true,
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text:
                                'Nombres: '+item.nombre + ' ' + item.apellido +
                                ' | CI: ' + (item.cedula ?? '-') +
                                ' | Email: ' + (item.email ?? '-') +
                                ' | Teléfono: ' + (item.telefono ?? '-')+
                                ' | Tipo de Propietario: ' + (item.tipopersona ?? '-')+
                                ' | Genero: ' + (item.sexoNombre ?? '-')

                        };
                    })
                };
            },
            camposPreview: [
                { label: 'Nombre', campo: 'nombre', icono: 'bi-person-fill', template: (d) => `${d.nombre || ''} ${d.apellido || ''}` },
                { label: 'Cédula', campo: 'cedula', icono: 'bi-card-text' },
                { label: 'Email', campo: 'email', icono: 'bi-envelope-fill' },
                { label: 'Teléfono', campo: 'telefono', icono: 'bi-envelope-fill' }

            ],
            camposModal: [
                { id: 'mpNombre', template: (d) => `${d.nombre || ''} ${d.apellido || ''}` },
                { id: 'mpCedula', campo: 'cedula' },
                { id: 'mpTelefono', campo: 'telefono' },
                { id: 'mpEmail', campo: 'email' },
                { id: 'mpRol', campo: 'rolNombre', default: 'N/A' },
                { id: 'mpTipo', campo: 'tipoPersonaNombre', default: 'N/A' }
            ],
            camposResumen: [
                { label: 'Nombre Completo', campo: 'nombre', template: (d) => `${d.nombre || ''} ${d.apellido || ''}` },
                { label: 'Cédula', campo: 'cedula' },
                { label: 'Teléfono', campo: 'telefono' },
                { label: 'Email', campo: 'email' },
                { label: 'Rol', campo: 'rolNombre' },
                { label: 'Tipo Persona', campo: 'tipoPersonaNombre' }
            ],
            iconoPreview: 'bi-person',
            iconoPreviewDetalle: 'bi-person-fill',
            iconoResumen: 'bi-person-badge'
        },
        {
            tipo: 'casa',
            nombre: 'Casa',
            selectId: 'casaSelect',
            btnVerId: 'btnVerCasa',
            btnResetId: 'btnResetCasa', // NUEVO: ID del botón reset
            badgeId: 'badgeCasa',
            previewId: 'previewCasa',
            previewContentId: 'previewCasaContent',
            modalId: 'modalCasa',
            datosId: 'datosCasa',
            loadingId: 'loadingCasa',
            urlBuscar: '/Admin/InfoDetalllada/BuscarCasa',
            urlDetalle: '/Admin/InfoDetalllada/GetCasaById',
            placeholder: 'Escriba nombre de familia, manzana, villa o etapa...',
            mensajeNoResultados: 'No se encontraron casas',
            mensajeSeleccionar: 'Seleccione una casa primero',
            usaCache: true,
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text: 'Vivienda o Familia: ' + item.nombreFamilia +
                                ' / Manzana: ' + item.manzana +
                                ' / Villa: ' + item.villa +
                                ' / Etapa: ' + item.etapa +
                                '/ Urbanizacion: ' + item.urbanizacion
                        };
                    })
                };
            },
            camposPreview: [
                { label: 'Familia', campo: 'NombreFamilia', icono: 'bi-house-door-fill' },
                { label: 'Manzana', campo: 'Manzana', icono: 'bi-grid-3x3-gap-fill' },
                { label: 'Villa', campo: 'Villa', icono: 'bi-door-open-fill' },
                { label: 'Etapa', campo: 'Etapa', icono: 'bi-diagram-3-fill' },
                { label: 'Urbanizacion', campo: 'Urbanizacion', icono: 'bi-diagram-3-fill' }
            ],
            camposModal: [
                { id: 'mcNombreFamilia', campo: 'NombreFamilia' },
                { id: 'mcManzana', campo: 'Manzana' },
                { id: 'mcVilla', campo: 'Villa' },
                { id: 'mcEtapa', campo: 'Etapa' },
                { id: 'Urbanizacion', campo: 'Urbanizacion' }
            ],
            camposResumen: [
                { label: 'Familia', campo: 'NombreFamilia' },
                { label: 'Manzana', campo: 'Manzana' },
                { label: 'Villa', campo: 'Villa' },
                { label: 'Etapa', campo: 'Etapa' },
                { label: 'Urbanizacion', campo: 'Urbanizacion' }
            ],
            iconoPreview: 'bi-house',
            iconoPreviewDetalle: 'bi-house-door-fill',
            iconoResumen: 'bi-house-door'
        },
        {
            tipo: 'edificio',
            nombre: 'Edificio',
            selectId: 'edificioSelect',
            btnVerId: 'btnVerEdificio',
            btnResetId: 'btnResetEdificio', // NUEVO: ID del botón reset
            badgeId: 'badgeEdificio',
            previewId: 'previewEdificio',
            previewContentId: 'previewEdificioContent',
            modalId: 'modalEdificio',
            datosId: 'datosEdificio',
            loadingId: 'loadingEdificio',
            urlBuscar: '/Admin/InfoDetalllada/BuscarEdificio',
            urlDetalle: '/Admin/InfoDetalllada/GetEdificioById',
            placeholder: 'Escriba nombre de edificio, familia, piso, departamento o etapa...',
            mensajeNoResultados: 'No se encontraron edificios',
            mensajeSeleccionar: 'Seleccione un edificio primero',
            usaCache: true,
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text: 
                                'Familia o Vivienda: ' + (item.NombreFamilia || '-') +
                                ' - Piso: ' + (item.Piso || '-') +
                                ' - Departamento: ' + (item.Departamento || '-') +
                                ' - Etapa: ' + (item.Etapa || '-')+
                                ' - Urbanizacion: ' + (item.Urbanizacion || '-')
                        };
                    })
                };
            },
            camposPreview: [
                { label: 'Familia o Vivienda', campo: 'NombreFamilia', icono: 'bi-people-fill' },
                { label: 'Piso', campo: 'Piso', icono: 'bi-layers' },
                { label: 'Departamento', campo: 'Departamento', icono: 'bi-door-open' },
                { label: 'Etapa', campo: 'Etapa', icono: 'bi-diagram-3' },
                { label: 'Urbanizacion', campo: 'Urbanizacion', icono: 'bi-diagram-3' }
            ],
            camposModal: [
                { id: 'NombreFamilia', campo: 'NombreFamilia' },
                { id: 'mcPiso', campo: 'Piso' },
                { id: 'mcDepartamento', campo: 'Departamento' },
                { id: 'Etapa', campo: 'Etapa' },
                { id: 'Urbanizacion', campo: 'Urbanizacion' }
            ],
            camposResumen: [
                { label: 'Edificio', campo: 'NombreEdificio' },
                { label: 'Familia', campo: 'NombreFamilia' },
                { label: 'Piso', campo: 'Piso' },
                { label: 'Departamento', campo: 'Departamento' },
                { label: 'Etapa', campo: 'Etapa' },
                { label: 'Urbanizacion', campo: 'Urbanizacion' }
            ],
            iconoPreview: 'bi-building',
            iconoPreviewDetalle: 'bi-building',
            iconoResumen: 'bi-building'
        },

        {
            tipo: 'vehiculo',
            nombre: 'Vehículo',
            selectId: 'vehiculoSelect',
            btnVerId: 'btnVerVehiculo',
            btnResetId: 'btnResetVehiculo', // NUEVO: ID del botón reset
            badgeId: 'badgeVehiculo',
            previewId: 'previewVehiculo',
            previewContentId: 'previewVehiculoContent',
            modalId: 'modalVehiculo',
            datosId: 'datosVehiculo',
            loadingId: 'loadingVehiculo',
            urlBuscar: '/Admin/InfoDetalllada/BuscarVehiculo',
            urlDetalle: '/Admin/InfoDetalllada/GetVehiculoById',
            placeholder: 'Escriba Año, marca, modelo o color...',
            mensajeNoResultados: 'No se encontraron vehículos',
            mensajeSeleccionar: 'Seleccione un vehículo primero',
            usaCache: false,

            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text: "Año: " + item.anio + " / " +
                                "Marca: " + item.marca + " / " +
                                "Modelo: " + item.modelo + " / (" +
                                "Color: " + item.color + ")" +
                                "Tipo de Vehiculo: " + item.tipo + ")"

                        };
                    })
                };
            },

            camposPreview: [
                { label: 'Año: ', campo: 'anio', icono: 'bi-car-front' },
                { label: 'Marca: ', campo: 'Marca', icono: 'bi-tag' },
                { label: 'Modelo: ', campo: 'Modelo', icono: 'bi-car-front-fill' },
                { label: 'Color: ', campo: 'Color', icono: 'bi-palette' },
                { label: 'Tipo de Vehiculo: ', campo: 'Tipo', icono: 'bi-palette' }
            ],
            camposModal: [
                { id: 'mvPlaca', campo: 'anio' },
                { id: 'mvMarca', campo: 'Marca' },
                { id: 'mvModelo', campo: 'Modelo' },
                { id: 'mvColor', campo: 'Color' },
                { id: 'mvTipo', campo: 'Tipo' }
            ],
            camposResumen: [
                { label: 'Año del Vehiculo', campo: 'anio' },
                { label: 'Marca', campo: 'Marca' },
                { label: 'Modelo', campo: 'Modelo' },
                { label: 'Color', campo: 'Color' },
                { label: 'Tipo de Vehiculo', campo: 'Tipo' }
            ],
            iconoPreview: 'bi-car-front',
            iconoPreviewDetalle: 'bi-car-front',
            iconoResumen: 'bi-car-front-fill'
        }
    ];

    // Inicializar cada entidad
    configs.forEach(config => {
        entidades[config.tipo].config = config;
        inicializarEntidad(config);
    });

    // Configurar botón de guardar existente para que abra modal de resumen
    configurarBotonGuardar();

    // Validación solo números en Tag y Tarjeta
    $('input[name="InfoDetallada.NumeroTag"], input[name="InfoDetallada.NumeroTarjeta"]').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    toggleUbicacion();
});



function configurarBotonGuardar() {
    // El botón de guardar ahora está manejado directamente en la vista (HTML)
    // No hacemos nada aquí para evitar conflictos
    console.log('configurarBotonGuardar: El botón se maneja desde el HTML');
}

/**
 * Obtiene los valores de los campos adicionales del formulario (Placa, Tag, Tarjeta)
 */
function obtenerCamposAdicionales() {
    const campos = {};

    // Intentar obtener por name (común en ASP.NET MVC)
    campos.placa = $('input[name="InfoDetallada.NumeroPlaca"]').val() ||
        $('input[name="NumeroPlaca"]').val() ||
        $('#NumeroPlaca').val() || '';

    campos.numeroTag = $('input[name="InfoDetallada.NumeroTag"]').val() ||
        $('input[name="NumeroTag"]').val() ||
        $('#NumeroTag').val() || '';

    campos.numeroTarjeta = $('input[name="InfoDetallada.NumeroTarjeta"]').val() ||
        $('input[name="NumeroTarjeta"]').val() ||
        $('#NumeroTarjeta').val() || '';

    // Si hay algún valor, retornar el objeto, si no, retornar null
    const tieneValores = campos.placa || campos.numeroTag || campos.numeroTarjeta;
    return tieneValores ? campos : null;
}


/**
 * Muestra el modal con el resumen de todas las entidades seleccionadas
 */
function mostrarResumenFinal() {
    // Este modal ya no se usa porque usamos el modalConfirmarGuardar del HTML
    // Pero mantenemos la función vacía por compatibilidad

    // Si quieres, puedes hacer que esta función abra el modal del HTML:
    var modalEl = document.getElementById('modalConfirmarGuardar');
    if (modalEl) {
        var modal = new bootstrap.Modal(modalEl);
        modal.show();
    } else {
        console.warn('modalConfirmarGuardar no encontrado');
    }
}


/**
 * Inicializa una entidad completa (Select2, eventos, handlers)
 * INCLUYE el botón de reset
 */
function inicializarEntidad(config) {
    const tipo = config.tipo;
    const $select = $(`#${config.selectId}`);

    // Inicializar Select2
    $select.select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: config.placeholder,
        allowClear: true,
        ajax: {
            url: config.urlBuscar,
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { criterio: params.term };
            },
            processResults: config.processResults
        },
        language: {
            noResults: function () { return config.mensajeNoResultados; },
            searching: function () { return "Buscando..."; }
        }
    });

    // Evento: selección
    $select.on('select2:select', function (e) {
        const data = e.params.data;
        if (data.id) {
            $(`#${config.btnVerId}`).prop('disabled', false);
            // Habilitar botón de reset cuando hay selección
            if (config.btnResetId) {
                $(`#${config.btnResetId}`).prop('disabled', false);
            }
            $(`#${config.badgeId}`)
                .removeClass('bg-white text-primary')
                .addClass('bg-success')
                .html('<i class="bi bi-check-circle"></i> ' + (tipo === 'persona' || tipo === 'casa' ? 'Seleccionada' : 'Seleccionado'));

            cargarPreview(data.id, config);
        }
    });

    // Evento: limpiar (desde el select2)
    $select.on('select2:clear', function () {
        limpiarEntidad(config);
    });

    // Evento: botón ver detalle
    $(`#${config.btnVerId}`).on('click', function () {
        abrirModal(config);
    });

    // NUEVO: Evento botón reset
    if (config.btnResetId) {
        $(`#${config.btnResetId}`).on('click', function () {
            limpiarEntidad(config);
        });
    }
}

/**
 * Limpia completamente una entidad (select, preview, badge, datos)
 */
function limpiarEntidad(config) {
    const tipo = config.tipo;
    const $select = $(`#${config.selectId}`);

    // 1. Limpiar Select2
    $select.val(null).trigger('change');

    // 2. Deshabilitar botones
    $(`#${config.btnVerId}`).prop('disabled', true);
    if (config.btnResetId) {
        $(`#${config.btnResetId}`).prop('disabled', true);
    }

    // 3. Resetear badge
    $(`#${config.badgeId}`)
        .removeClass('bg-success')
        .addClass('bg-white text-primary')
        .text('Pendiente');

    // 4. Ocultar preview
    $(`#${config.previewId}`).addClass('d-none');
    $(`#${config.previewContentId}`).html('');

    // 5. Limpiar datos cacheados
    entidades[tipo].seleccionada = null;

    // 6. Si hay modal abierto, cerrarlo (opcional)
    const modalEl = document.getElementById(config.modalId);
    if (modalEl) {
        const modalInstance = bootstrap.Modal.getInstance(modalEl);
        if (modalInstance) {
            modalInstance.hide();
        }
    }
}

/**
 * Carga el preview de una entidad
 */
function cargarPreview(id, config) {
    const tipo = config.tipo;

    $.ajax({
        url: `${config.urlDetalle}/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            entidades[tipo].seleccionada = data;
            mostrarPreview(data, config);
        },
        error: function (xhr, status, error) {
            console.error(`Error cargando preview ${tipo}:`, xhr.status, error);
            const textoSelect = $(`#${config.selectId} option:selected`).text();
            $(`#${config.previewId}`).removeClass('d-none');
            $(`#${config.previewContentId}`).html(`
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi ${config.iconoPreview}"></i></div>
                    <div>
                        <span class="preview-label">${tipo.charAt(0).toUpperCase() + tipo.slice(1)}:</span>
                        <span class="preview-value name">${textoSelect}</span>
                    </div>
                </div>
            `);
        }
    });
}

/**
 * Muestra el preview con datos completos
 */
function mostrarPreview(data, config) {
    let html = '';

    config.camposPreview.forEach(campo => {
        const valor = campo.template ? campo.template(data) : (data[campo.campo] || '-');
        html += `
            <div class="preview-item">
                <div class="preview-icon"><i class="bi ${campo.icono}"></i></div>
                <div>
                    <span class="preview-label">${campo.label}:</span>
                    <span class="preview-value ${campo.campo === 'nombre' || campo.campo === 'NombreFamilia' || campo.campo === 'NombreEdificio' || campo.campo === 'Placa' ? 'name' : ''}">${valor}</span>
                </div>
            </div>
        `;
    });

    $(`#${config.previewId}`).removeClass('d-none');
    $(`#${config.previewContentId}`).html(html);
}

/**
 * Abre el modal de detalle individual
 */
function abrirModal(config) {
    const tipo = config.tipo;
    const id = $(`#${config.selectId}`).val();

    if (!id) {
        alert(`⚠️ ${config.mensajeSeleccionar}`);
        return;
    }

    // Si usa cacheo y tenemos los datos, usarlos directamente
    if (config.usaCache && entidades[tipo].seleccionada && entidades[tipo].seleccionada.id == id) {
        resetModal(config);
        $(`#${config.datosId}`).addClass('d-none');
        $(`#${config.loadingId}`).removeClass('d-none');

        const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById(config.modalId));
        modal.show();

        mostrarDatosModal(entidades[tipo].seleccionada, config);
        return;
    }

    // Cargar por AJAX
    resetModal(config);
    $(`#${config.datosId}`).addClass('d-none');
    $(`#${config.loadingId}`).removeClass('d-none');

    const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById(config.modalId));
    modal.show();

    $.ajax({
        url: `${config.urlDetalle}/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            if (config.usaCache) {
                entidades[tipo].seleccionada = data;
            }
            mostrarDatosModal(data, config);
        },
        error: function (xhr, status, error) {
            console.error(`Error AJAX ${tipo}:`, xhr.status, xhr.responseText);
            $(`#${config.loadingId}`).addClass('d-none');
            $(`#${config.datosId}`).removeClass('d-none');

            if (config.camposModal.length > 0) {
                const primerCampo = config.camposModal[0];
                $(`#${primerCampo.id}`).text("Error al cargar datos").addClass('text-danger');
            }
        }
    });
}

/**
 * Resetea los campos del modal
 */
function resetModal(config) {
    config.camposModal.forEach(campo => {
        $(`#${campo.id}`).text("-").removeClass('text-danger');
    });
}

/**
 * Muestra los datos en el modal individual
 */
function mostrarDatosModal(data, config) {
    $(`#${config.loadingId}`).addClass('d-none');
    $(`#${config.datosId}`).removeClass('d-none').hide().fadeIn(300);

    config.camposModal.forEach(campo => {
        const valor = campo.template ? campo.template(data) : (data[campo.campo] || campo.default || '-');
        $(`#${campo.id}`).text(valor);
    });
}

// Funciones de compatibilidad hacia atrás
// Persona
function cargarDatosPreview(id) { cargarPreview(id, entidades.persona.config); }
function mostrarPreviewPersona(data) { mostrarPreview(data, entidades.persona.config); }
function verPersona() { abrirModal(entidades.persona.config); }
function resetModalPersona() { resetModal(entidades.persona.config); }
function mostrarDatosPersona(data) { mostrarDatosModal(data, entidades.persona.config); }

// Casa
function cargarDatosPreviewCasa(id) { cargarPreview(id, entidades.casa.config); }
function mostrarPreviewCasa(data) { mostrarPreview(data, entidades.casa.config); }
function verCasa() { abrirModal(entidades.casa.config); }
function resetModalCasa() { resetModal(entidades.casa.config); }
function mostrarDatosCasa(data) { mostrarDatosModal(data, entidades.casa.config); }

// Edificio
function cargarDatosPreviewEdificio(id) { cargarPreview(id, entidades.edificio.config); }
function mostrarPreviewEdificio(data) { mostrarPreview(data, entidades.edificio.config); }
function verEdificio() { abrirModal(entidades.edificio.config); }
function resetModalEdificio() { resetModal(entidades.edificio.config); }
function mostrarDatosEdificio(data) { mostrarDatosModal(data, entidades.edificio.config); }

// Vehiculo
function cargarDatosPreviewVehiculo(id) { cargarPreview(id, entidades.vehiculo.config); }
function mostrarPreviewVehiculo(data) { mostrarPreview(data, entidades.vehiculo.config); }
function verVehiculo() { abrirModal(entidades.vehiculo.config); }
function resetModalVehiculo() { resetModal(entidades.vehiculo.config); }
function mostrarDatosVehiculo(data) { mostrarDatosModal(data, entidades.vehiculo.config); }

// =============================
// TOGGLE ENTRE CASA Y EDIFICIO
// =============================
function toggleUbicacion() {

    const radioCasa = document.getElementById("radioCasa");
    const radioEdificio = document.getElementById("radioEdificio");

    const cardCasa = document.getElementById("cardCasa");
    const cardEdificio = document.getElementById("cardEdificio");

    if (!radioCasa || !radioEdificio) return;

    if (radioCasa.checked) {

        // Mostrar Casa
        cardCasa.classList.remove("d-none");
        cardEdificio.classList.add("d-none");

        // Limpiar selección edificio
        if ($('#edificioSelect').length) {
            $('#edificioSelect').val(null).trigger('change');
        }

    } else if (radioEdificio.checked) {

        // Mostrar Edificio
        cardEdificio.classList.remove("d-none");
        cardCasa.classList.add("d-none");

        // Limpiar selección casa
        if ($('#casaSelect').length) {
            $('#casaSelect').val(null).trigger('change');
        }
    }
}



/*******************fin******************** */