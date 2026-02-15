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
                            text: item.nombre + ' ' + item.apellido + ' (' + item.cedula + ')'
                        };
                    })
                };
            },
            camposPreview: [
                { label: 'Nombre', campo: 'nombre', icono: 'bi-person-fill', template: (d) => `${d.nombre || ''} ${d.apellido || ''}` },
                { label: 'Cédula', campo: 'cedula', icono: 'bi-card-text' },
                { label: 'Email', campo: 'email', icono: 'bi-envelope-fill' }
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
                            text: item.nombreFamilia + ' - Mz: ' + item.manzana + ' - Villa: ' + item.villa + ' - Etapa: ' + item.etapa
                        };
                    })
                };
            },
            camposPreview: [
                { label: 'Familia', campo: 'NombreFamilia', icono: 'bi-house-door-fill' },
                { label: 'Manzana', campo: 'Manzana', icono: 'bi-grid-3x3-gap-fill' },
                { label: 'Villa', campo: 'Villa', icono: 'bi-door-open-fill' },
                { label: 'Etapa', campo: 'Etapa', icono: 'bi-diagram-3-fill' }
            ],
            camposModal: [
                { id: 'mcNombreFamilia', campo: 'NombreFamilia' },
                { id: 'mcManzana', campo: 'Manzana' },
                { id: 'mcVilla', campo: 'Villa' },
                { id: 'mcEtapa', campo: 'Etapa' }
            ],
            camposResumen: [
                { label: 'Familia', campo: 'NombreFamilia' },
                { label: 'Manzana', campo: 'Manzana' },
                { label: 'Villa', campo: 'Villa' },
                { label: 'Etapa', campo: 'Etapa' }
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
                            text: (item.NombreEdificio || '-') +
                                ' - Familia: ' + (item.NombreFamilia || '-') +
                                ' - Piso: ' + (item.Piso || '-') +
                                ' - Departamento: ' + (item.Departamento || '-') +
                                ' - Etapa: ' + (item.Etapa || '-')
                        };
                    })
                };
            },
            camposPreview: [
                { label: 'Edificio', campo: 'NombreEdificio', icono: 'bi-building' },
                { label: 'Familia', campo: 'NombreFamilia', icono: 'bi-people-fill' },
                { label: 'Piso', campo: 'Piso', icono: 'bi-layers' },
                { label: 'Departamento', campo: 'Departamento', icono: 'bi-door-open' },
                { label: 'Etapa', campo: 'Etapa', icono: 'bi-diagram-3' }
            ],
            camposModal: [
                { id: 'mcNombreEdificio', campo: 'NombreEdificio' },
                { id: 'NombreFamilia', campo: 'NombreFamilia' },
                { id: 'mcPiso', campo: 'Piso' },
                { id: 'mcDepartamento', campo: 'Departamento' },
                { id: 'Etapa', campo: 'Etapa' }
            ],
            camposResumen: [
                { label: 'Edificio', campo: 'NombreEdificio' },
                { label: 'Familia', campo: 'NombreFamilia' },
                { label: 'Piso', campo: 'Piso' },
                { label: 'Departamento', campo: 'Departamento' },
                { label: 'Etapa', campo: 'Etapa' }
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
                            text: (item.Placa || '-') +
                                ' - Marca: ' + (item.Marca || '-') +
                                ' - Modelo: ' + (item.Modelo || '-') +
                                ' - Color: ' + (item.Color || '-')
                        };
                    })
                };
            },
            camposPreview: [
                { label: 'Año del Vehiculo', campo: 'Placa', icono: 'bi-car-front' },
                { label: 'Marca', campo: 'Marca', icono: 'bi-tag' },
                { label: 'Modelo', campo: 'Modelo', icono: 'bi-car-front-fill' },
                { label: 'Color', campo: 'Color', icono: 'bi-palette' }
            ],
            camposModal: [
                { id: 'mvPlaca', campo: 'Placa' },
                { id: 'mvMarca', campo: 'Marca' },
                { id: 'mvModelo', campo: 'Modelo' },
                { id: 'mvColor', campo: 'Color' }
            ],
            camposResumen: [
                { label: 'Año del Vehiculo', campo: 'Placa' },
                { label: 'Marca', campo: 'Marca' },
                { label: 'Modelo', campo: 'Modelo' },
                { label: 'Color', campo: 'Color' }
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
});


/**
 * Configura el botón de "Agregar Datos" para que abra el modal de resumen
 */
//function configurarBotonGuardar() {
//    var $btnGuardar = $('#btnAgregarDatos');

//    // Si no existe, buscar el botón de submit original y reemplazarlo
//    if ($btnGuardar.length === 0) {
//        var $btnOriginal = $('form button[type="submit"]').first();
//        if ($btnOriginal.length > 0) {
//            $btnOriginal.attr('type', 'button');
//            $btnOriginal.attr('id', 'btnAgregarDatos');
//            $btnOriginal.html('<i class="bi bi-clipboard-check me-2"></i>Agregar Datos');
//            $btnOriginal.removeClass('btn-primary').addClass('btn-info');
//            $btnGuardar = $btnOriginal;
//        }
//    }

//    if ($btnGuardar.length > 0) {
//        $btnGuardar.off('click').on('click', function (e) {
//            e.preventDefault();
//            mostrarResumenFinal();
//        });
//    }
//}

function configurarBotonGuardar() {
    // El botón de guardar ahora está manejado directamente en la vista (HTML)
    // No hacemos nada aquí para evitar conflictos
    console.log('configurarBotonGuardar: El botón se maneja desde el HTML');
}

/**
 * 
 * 
 * Configura el botón de "Guardar Información" existente para que abra el modal de resumen
 * en lugar de hacer submit directo
 */
/*function configurarBotonGuardar() {
    // Buscar el botón de guardar por texto (puede variar según tu HTML)
    // Opción 1: Por texto exacto
    let $btnGuardar = $('button:contains("Guardar Información"), button:contains("Guardar"), input[value="Guardar Información"], input[value="Guardar"]');

    // Opción 2: Por clase común de botones de submit
    if ($btnGuardar.length === 0) {
        $btnGuardar = $('form button[type="submit"], form input[type="submit"]').last();
    }

    if ($btnGuardar.length > 0) {
        // Guardar referencia al botón original para usarlo después
        $btnGuardar.data('original-type', 'submit');

        // Cambiar texto y apariencia
        $btnGuardar.html('<i class="bi bi-clipboard-check me-2"></i>Agregar Datos');
        $btnGuardar.removeClass('btn-primary').addClass('btn-info');

        // Cambiar comportamiento: abrir modal en lugar de submit
        $btnGuardar.on('click', function (e) {
            e.preventDefault(); // Evitar submit directo
            mostrarResumenFinal();
        });
    } else {
        console.warn('No se encontró el botón de Guardar. Asegúrate de tener un botón con texto "Guardar Información" o "Guardar".');
    }
}*/


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
//function mostrarResumenFinal() {
//    const seleccionados = [];

//    Object.keys(entidades).forEach(tipo => {
//        if (entidades[tipo].seleccionada) {
//            seleccionados.push({
//                tipo: tipo,
//                config: entidades[tipo].config,
//                datos: entidades[tipo].seleccionada
//            });
//        }
//    });

//    const camposAdicionales = obtenerCamposAdicionales();

//    if (seleccionados.length === 0 && !camposAdicionales) {
//        alert('⚠️ No ha seleccionado ninguna entidad ni ingresado datos adicionales.');
//        return;
//    }

//    let contenidoHTML = '';

//    if (seleccionados.length > 0) {
//        seleccionados.forEach((item) => {
//            const config = item.config;
//            const datos = item.datos;

//            contenidoHTML += `
//                <div class="card mb-3 border-primary shadow-sm">
//                    <div class="card-header bg-primary text-white d-flex align-items-center">
//                        <i class="bi ${config.iconoResumen} me-2 fs-5"></i>
//                        <h5 class="mb-0">${config.nombre}</h5>
//                        <span class="badge bg-light text-primary ms-auto">Seleccionado</span>
//                    </div>
//                    <div class="card-body">
//                        <div class="row">
//                            ${config.camposResumen.map(campo => {
//                const valor = campo.template ? campo.template(datos) : (datos[campo.campo] || '-');
//                return `<div class="col-md-6 mb-2">
//                            <small class="text-muted d-block">${campo.label}</small>
//                            <span class="fw-bold text-dark">${valor}</span>
//                        </div>`;
//            }).join('')}
//                        </div>
//                    </div>
//                </div>
//            `;
//        });
//    }

//    if (camposAdicionales) {
//        contenidoHTML += `
//            <div class="card mb-3 border-warning shadow-sm">
//                <div class="card-header bg-warning text-dark d-flex align-items-center">
//                    <i class="bi bi-input-cursor-text me-2 fs-5"></i>
//                    <h5 class="mb-0">Información Adicional</h5>
//                </div>
//                <div class="card-body">
//                    <div class="row">
//                        ${camposAdicionales.placa ? `
//                            <div class="col-md-4 mb-2">
//                                <small class="text-muted d-block">Placa</small>
//                                <span class="fw-bold text-dark">${camposAdicionales.placa}</span>
//                            </div>` : ''}
//                        ${camposAdicionales.numeroTag ? `
//                            <div class="col-md-4 mb-2">
//                                <small class="text-muted d-block">Número de Tag</small>
//                                <span class="fw-bold text-dark">${camposAdicionales.numeroTag}</span>
//                            </div>` : ''}
//                        ${camposAdicionales.numeroTarjeta ? `
//                            <div class="col-md-4 mb-2">
//                                <small class="text-muted d-block">Número de Tarjeta</small>
//                                <span class="fw-bold text-dark">${camposAdicionales.numeroTarjeta}</span>
//                            </div>` : ''}
//                    </div>
//                </div>
//            </div>
//        `;
//    }

//    // Crear modal si no existe
//    if ($('#modalResumenFinal').length === 0) {
//        const modalHTML = `
//            <div class="modal fade" id="modalResumenFinal" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
//                <div class="modal-dialog modal-lg modal-dialog-scrollable">
//                    <div class="modal-content">
//                        <div class="modal-header bg-info text-white">
//                            <h5 class="modal-title">
//                                <i class="bi bi-clipboard-check me-2"></i>
//                                ¿Está seguro de guardar?
//                            </h5>
//                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
//                        </div>
//                        <div class="modal-body bg-light" id="contenidoResumenFinal"></div>
//                        <div class="modal-footer bg-white border-top">
//                            <button type="button" class="btn btn-secondary btn-lg" data-bs-dismiss="modal">
//                                <i class="bi bi-arrow-left-circle me-1"></i>
//                                No, revisar datos
//                            </button>
//                            <button type="button" class="btn btn-success btn-lg" id="btnConfirmarGuardarFinal">
//                                <i class="bi bi-check-circle me-1"></i>
//                                Sí, guardar información
//                            </button>
//                        </div>
//                    </div>
//                </div>
//            </div>
//        `;
//        $('body').append(modalHTML);

//        // Evento para confirmar guardado - IMPORTANTE: type="button" no submit
//        $(document).off('click', '#btnConfirmarGuardarFinal').on('click', '#btnConfirmarGuardarFinal', function () {
//            $('#modalResumenFinal').modal('hide');

//            setTimeout(function () {
//                var form = document.getElementById('formCreate');
//                if (form) {
//                    form.submit();
//                } else {
//                    $('form').first().submit();
//                }
//            }, 300);
//        });
//    }

//    $('#contenidoResumenFinal').html(contenidoHTML);
//    $('#modalResumenFinal').modal('show');
//}


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