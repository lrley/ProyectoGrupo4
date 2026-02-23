// ============================================================
// InfoDetalladaEdit.js - Script específico para la vista Edit
// Usa los endpoints existentes: GetPersonaById, GetCasaById, etc.
// ============================================================

$(document).ready(function () {

    // Inicializar Select2
    inicializarSelect2();

    // Inicializar estado de ubicación (Casa/Edificio)
    toggleUbicacion();

  
    // ============================================================
    // 🔥 PRECARGAR SELECT2 EN MODO EDIT (CORREGIDO)
    // ============================================================

    // ================= PERSONA =================
    var personaId = $('#personaSelect').val();
    if (personaId && personaId > 0) {
        $.get('/Admin/InfoDetalllada/GetPersonaById/' + personaId, function (data) {

            var option = new Option(
                'Nombres: ' + data.nombre + ' ' + data.apellido +
                ' / Cédula: ' + data.cedula +
                ' / Email: ' + (data.email || '-') +
                ' / Teléfono: ' + (data.telefono || '-')+
                ' / Tipo de Propietario: ' + (data.tipoPersonaNombre || '-'),
                data.id,
                true,
                true
            );

            $('#personaSelect')
                .append(option)
                .trigger('change');

            cargarPreviewPersona(data.id);
        });
    }

    // ================= CASA =================
    var casaId = $('#casaSelect').val();
    if (casaId && casaId > 0 && !$('#cardCasa').hasClass('d-none')) {

        $.get('/Admin/InfoDetalllada/GetCasaById/' + casaId, function (data) {

            var option = new Option(
                'Familia o Vivienda: ' + data.NombreFamilia + ' (Manzana: ' + data.Manzana + ', Villa: ' + data.Villa + ')' + ' / Etapa: ' + data.Etapa +' / Urbanizacion: ' + data.Urbanizacion,
                data.id,
                true,
                true
            );

            $('#casaSelect')
                .append(option)
                .trigger('change');

            cargarPreviewCasa(data.id);
        });
    }

    // ================= EDIFICIO =================
    var edificioId = $('#edificioSelect').val();
    if (edificioId && edificioId > 0 && !$('#cardEdificio').hasClass('d-none')) {

        $.get('/Admin/InfoDetalllada/GetEdificioById/' + edificioId, function (data) {

            var option = new Option(
                'Familia o Vivienda: ' + data.NombreEdificio + ' (Piso: ' + data.Piso + ' / Departamento: ' + data.Departamento + ')' + ' / Urbanizacion: ' + data.Urbanizacion,
                data.id,
                true,
                true
            );

            $('#edificioSelect')
                .append(option)
                .trigger('change');

            cargarPreviewEdificio(data.id);
        });
    }

    // ================= VEHICULO =================
    var vehiculoId = $('#vehiculoSelect').val();
    if (vehiculoId && vehiculoId > 0) {

        $.get('/Admin/InfoDetalllada/GetVehiculoById/' + vehiculoId, function (data) {

            var option = new Option(
                'Año: ' + data.anio +
                '/ Marca: ' + data.Marca +
                ' / Modelo:  ' + data.Modelo +
                ' / ( Color: ' + data.Color + ' )' +
                ' / Tipo de Vehiculo:  ' + data.Tipo,
                data.id,
                true,
                true
            );

            $('#vehiculoSelect')
                .append(option)
                .trigger('change');

            cargarPreviewVehiculo(data.id);
        });
    }



    // Actualizar badges según selección
    actualizarBadges();

    // ============================================================
    // EVENTOS DE BOTONES (Ver detalle en modal)
    // ============================================================

    $('#btnVerPersona').click(function () {
        var id = $('#personaSelect').val();
        if (id) verPersonaEnModal(id);
    });

    $('#btnVerCasa').click(function () {
        var id = $('#casaSelect').val();
        if (id) verCasaEnModal(id);
    });

    $('#btnVerEdificio').click(function () {
        var id = $('#edificioSelect').val();
        if (id) verEdificioEnModal(id);
    });

    $('#btnVerVehiculo').click(function () {
        var id = $('#vehiculoSelect').val();
        if (id) verVehiculoEnModal(id);
    });

    // ============================================================
    // EVENTOS DE RESET
    // ============================================================

    $('#btnResetPersona').click(function () {
        $('#personaSelect').val(null).trigger('change');
        $('#previewPersona').addClass('d-none');
        $('#previewContent').empty();
        habilitarBotonesPersona(false);
        $('#badgePersona').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
    });

    $('#btnResetCasa').click(function () {
        $('#casaSelect').val(null).trigger('change');
        $('#previewCasa').addClass('d-none');
        $('#previewCasaContent').empty();
        habilitarBotonesCasa(false);
        $('#badgeCasa').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
    });

    $('#btnResetEdificio').click(function () {
        $('#edificioSelect').val(null).trigger('change');
        $('#previewEdificio').addClass('d-none');
        $('#previewEdificioContent').empty();
        habilitarBotonesEdificio(false);
        $('#badgeEdificio').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
    });

    $('#btnResetVehiculo').click(function () {
        $('#vehiculoSelect').val(null).trigger('change');
        $('#previewVehiculo').addClass('d-none');
        $('#previewVehiculoContent').empty();
        habilitarBotonesVehiculo(false);
        $('#badgeVehiculo').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
    });

    // ============================================================
    // BOTÓN GUARDAR (modal confirmación)
    // ============================================================

    $('#btnGuardarInfo').click(function (e) {
        e.preventDefault();
        actualizarResumenModal();
        var modal = new bootstrap.Modal(document.getElementById('modalConfirmarGuardar'));
        modal.show();
    });

    $('#btnSiGuardar').click(function (e) {
        e.preventDefault();
        var modalEl = document.getElementById('modalConfirmarGuardar');
        var modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
        setTimeout(function () {
            $('#formCreate').submit();
        }, 300);
    });
});

// ============================================================
// INICIALIZAR SELECT2
// ============================================================
function inicializarSelect2() {
    $('.select2-persona').select2({
        placeholder: "-- Escriba para buscar persona --",
        allowClear: true,
        theme: "bootstrap-5",
        width: '100%',
        ajax: {
            url: '/Admin/InfoDetalllada/BuscarPersona',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { criterio: params.term };
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text:
                                ' Nombres: '+ item.nombre + ' ' + item.apellido +
                                ' / CI: ' + (item.cedula ?? '-') +
                                ' / Email: ' + (item.email ?? '-') +
                                ' / Telefono: ' + (item.telefono ?? '-') +
                                ' / Tipo de Propietario: ' + (item.tipopersona ?? '-')


                        };
                    })
                };
            },
            //processResults: function (data) {
            //    return {
            //        results: data.map(function (item) {
            //            return {
            //                id: item.id,
            //                text: item.nombre + ' ' + item.apellido + ' - ' + item.cedula
            //            };
            //        })
            //    };
            //},
            cache: true
        }
    }).on('select2:select', function (e) {
        var id = e.params.data.id;
        cargarPreviewPersona(id);
    }).on('select2:clear', function () {
        $('#previewPersona').addClass('d-none');
        $('#previewContent').empty();
        habilitarBotonesPersona(false);
        $('#badgePersona').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
    });

    $('.select2-casa').select2({
        placeholder: "-- Escriba para buscar casa --",
        allowClear: true,
        theme: "bootstrap-5",
        width: '100%',
        ajax: {
            url: '/Admin/InfoDetalllada/BuscarCasa',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { criterio: params.term };
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text: 'Vivienda o Familia: ' +
                                item.nombreFamilia +
                                ' / Manzana: ' + item.manzana +
                                ' / Villa: ' + item.villa +
                                ' / Etapa: ' + item.etapa +
                                '/ Urbanizacion: ' + item.urbanizacion
                        };
                    })
                };
            },
            cache: true
        }
    }).on('select2:select', function (e) {
        var id = e.params.data.id;
        cargarPreviewCasa(id);
    }).on('select2:clear', function () {
        $('#previewCasa').addClass('d-none');
        $('#previewCasaContent').empty();
        habilitarBotonesCasa(false);
        $('#badgeCasa').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
    });

    // ✅ CORREGIDO: Edificio - usar MAYÚSCULAS como devuelve el Controller
    $('.select2-edificio').select2({
        placeholder: "-- Escriba para buscar edificio --",
        allowClear: true,
        theme: "bootstrap-5",
        width: '100%',
        ajax: {
            url: '/Admin/InfoDetalllada/BuscarEdificio',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { criterio: params.term };
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text:
                                'Familia: ' + (item.NombreFamilia || '-') +
                                ' - Piso: ' + (item.Piso || '-') +
                                ' - Departamento: ' + (item.Departamento || '-') +
                                ' - Etapa: ' + (item.Etapa || '-') +
                                ' - Urbanizacion: ' + item.Urbanizacion
                        };
                    })
                };
            },
            
            cache: true
        }
    }).on('select2:select', function (e) {
        var id = e.params.data.id;
        cargarPreviewEdificio(id);
    }).on('select2:clear', function () {
        $('#previewEdificio').addClass('d-none');
        $('#previewEdificioContent').empty();
        habilitarBotonesEdificio(false);
        $('#badgeEdificio').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
    });

    // ✅ CORREGIDO: Vehículo - usar MAYÚSCULAS como devuelve el Controller
    $('.select2-vehiculo').select2({
        placeholder: "-- Escriba para buscar vehículo --",
        allowClear: true,
        theme: "bootstrap-5",
        width: '100%',
        ajax: {
            url: '/Admin/InfoDetalllada/BuscarVehiculo',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { criterio: params.term };
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text: "Año: " + item.anio + " / " +
                                "Marca: " + item.marca + " / " +
                                "Modelo: " + item.modelo + " / (" +
                                "Color: " + item.color + ")" +
                                "Tipo de Vehiculo: " + item.tipo
                        };
                    })
                };
            },
           
            cache: true
        }
    }).on('select2:select', function (e) {
        var id = e.params.data.id;
        cargarPreviewVehiculo(id);
    }).on('select2:clear', function () {
        $('#previewVehiculo').addClass('d-none');
        $('#previewVehiculoContent').empty();
        habilitarBotonesVehiculo(false);
        $('#badgeVehiculo').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
    });
}

// ============================================================
// CARGAR PREVIEW PERSONA (para el preview debajo del select)
// ============================================================
function cargarPreviewPersona(id) {
    $.ajax({
        url: '/Admin/InfoDetalllada/GetPersonaById/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            mostrarPreviewPersona(data);
            habilitarBotonesPersona(true);
            $('#badgePersona').text('Seleccionado').removeClass('bg-white text-primary').addClass('bg-success');
        },
        error: function (xhr, status, error) {
            console.error('Error cargando persona:', error);
        }
    });
}

function mostrarPreviewPersona(data) {
    var html = `
        <div class="row g-3">
            <div class="col-md-4">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-person"></i></div>
                    <div>
                        <div class="preview-label">Nombre</div>
                        <div class="preview-value name">${data.nombre || ''} ${data.apellido || ''}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-card-text"></i></div>
                    <div>
                        <div class="preview-label">Cédula</div>
                        <div class="preview-value cedula">${data.cedula || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-telephone"></i></div>
                    <div>
                        <div class="preview-label">Teléfono</div>
                        <div class="preview-value">${data.telefono || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-envelope"></i></div>
                    <div>
                        <div class="preview-label">Email</div>
                        <div class="preview-value email">${data.email || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-shield"></i></div>
                    <div>
                        <div class="preview-label">Rol</div>
                        <div class="preview-value">
                            <span class="badge bg-info text-dark">${data.rolNombre || 'Sin rol'}</span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-person-badge"></i></div>
                    <div>
                        <div class="preview-label">Tipo</div>
                        <div class="preview-value">
                            <span class="badge bg-secondary">${data.tipoPersonaNombre || 'Sin tipo'}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('#previewContent').html(html);
    $('#previewPersona').removeClass('d-none');
}

// ============================================================
// CARGAR PREVIEW CASA
// ============================================================
function cargarPreviewCasa(id) {
    $.ajax({
        url: '/Admin/InfoDetalllada/GetCasaById/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            mostrarPreviewCasa(data);
            habilitarBotonesCasa(true);
            $('#badgeCasa').text('Seleccionado').removeClass('bg-white text-primary').addClass('bg-success');
        },
        error: function (xhr, status, error) {
            console.error('Error cargando casa:', error);
        }
    });
}

function mostrarPreviewCasa(data) {
    // ✅ Casa usa MAYÚSCULAS en el Controller (NombreFamilia, Manzana, Villa, Etapa)
    var html = `
        <div class="row g-3">
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-house"></i></div>
                    <div>
                        <div class="preview-label">Familia</div>
                        <div class="preview-value text-success">${data.NombreFamilia || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-grid"></i></div>
                    <div>
                        <div class="preview-label">Manzana</div>
                        <div class="preview-value">${data.Manzana || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-door-open"></i></div>
                    <div>
                        <div class="preview-label">Villa</div>
                        <div class="preview-value">${data.Villa || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-layers"></i></div>
                    <div>
                        <div class="preview-label">Etapa</div>
                        <div class="preview-value">${data.Etapa || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-layers"></i></div>
                    <div>
                        <div class="preview-label">Urbaniacion</div>
                        <div class="preview-value">${data.Urbanizacion || '-'}</div>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('#previewCasaContent').html(html);
    $('#previewCasa').removeClass('d-none');
}

// ============================================================
// CARGAR PREVIEW EDIFICIO
// ============================================================
function cargarPreviewEdificio(id) {
    $.ajax({
        url: '/Admin/InfoDetalllada/GetEdificioById/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            mostrarPreviewEdificio(data);
            habilitarBotonesEdificio(true);
            $('#badgeEdificio').text('Seleccionado').removeClass('bg-white text-primary').addClass('bg-success');
        },
        error: function (xhr, status, error) {
            console.error('Error cargando edificio:', error);
        }
    });
}

function mostrarPreviewEdificio(data) {
   
    var html = `
        <div class="row g-3">
            
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-people"></i></div>
                    <div>
                        <div class="preview-label">Familia</div>
                        <div class="preview-value">${data.NombreFamilia || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-arrow-up-square"></i></div>
                    <div>
                        <div class="preview-label">Piso</div>
                        <div class="preview-value">${data.Piso || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-door-closed"></i></div>
                    <div>
                        <div class="preview-label">Departamento</div>
                        <div class="preview-value">${data.Departamento || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-layers"></i></div>
                    <div>
                        <div class="preview-label">Etapa</div>
                        <div class="preview-value">${data.Etapa || '-'}</div>
                    </div>
                </div>
            </div>
             <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-layers"></i></div>
                    <div>
                        <div class="preview-label">Urbaniacion</div>
                        <div class="preview-value">${data.Urbanizacion || '-'}</div>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('#previewEdificioContent').html(html);
    $('#previewEdificio').removeClass('d-none');
}

// ============================================================
// CARGAR PREVIEW VEHÍCULO
// ============================================================
function cargarPreviewVehiculo(id) {
    $.ajax({
        url: '/Admin/InfoDetalllada/GetVehiculoById/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            mostrarPreviewVehiculo(data);
            habilitarBotonesVehiculo(true);
            $('#badgeVehiculo').text('Seleccionado').removeClass('bg-white text-primary').addClass('bg-success');
        },
        error: function (xhr, status, error) {
            console.error('Error cargando vehículo:', error);
        }
    });
}

function mostrarPreviewVehiculo(data) {
   
    var html = `
        <div class="row g-3">
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-calendar"></i></div>
                    <div>
                        <div class="preview-label">Año</div>
                        <div class="preview-value text-info">${data.anio || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-car-front"></i></div>
                    <div>
                        <div class="preview-label">Marca</div>
                        <div class="preview-value">${data.Marca || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-car-front-fill"></i></div>
                    <div>
                        <div class="preview-label">Modelo</div>
                        <div class="preview-value">${data.Modelo || '-'}</div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-palette"></i></div>
                    <div>
                        <div class="preview-label">Color</div>
                        <div class="preview-value">${data.Color || '-'}</div>
                    </div>
                </div>
            </div>
               <div class="col-md-6">
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-palette"></i></div>
                    <div>
                        <div class="preview-label">Tipo de Vehiculo</div>
                        <div class="preview-value">${data.Tipo || '-'}</div>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('#previewVehiculoContent').html(html);
    $('#previewVehiculo').removeClass('d-none');
}

// ============================================================
// VER EN MODAL (para el botón "Ver detalle")
// ============================================================
function verPersonaEnModal(id) {
    $('#loadingPersona').removeClass('d-none');
    $('#datosPersona').addClass('d-none');
    var modal = new bootstrap.Modal(document.getElementById('modalPersona'));
    modal.show();

    $.ajax({
        url: '/Admin/InfoDetalllada/GetPersonaById/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#mpNombre').text(data.nombre + ' - ' + data.apellido);
            $('#mpCedula').text(data.cedula);
            $('#mpTelefono').text(data.telefono);
            $('#mpEmail').text(data.email);
            $('#mpRol').text(data.rolNombre);
            $('#mpTipo').text(data.tipoPersonaNombre);

            $('#loadingPersona').addClass('d-none');
            $('#datosPersona').removeClass('d-none');
        },
        error: function () {
            $('#loadingPersona').addClass('d-none');
            $('#datosPersona').removeClass('d-none');
            toastr.error('Error al cargar datos de persona');
        }
    });
}

function verCasaEnModal(id) {
    $('#loadingCasa').removeClass('d-none');
    $('#datosCasa').addClass('d-none');
    var modal = new bootstrap.Modal(document.getElementById('modalCasa'));
    modal.show();

    $.ajax({
        url: '/Admin/InfoDetalllada/GetCasaById/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#mcNombreFamilia').text(data.NombreFamilia);
            $('#mcManzana').text(data.Manzana);
            $('#mcVilla').text(data.Villa);
            $('#mcEtapa').text(data.Etapa);
            $('#Urbanizacion').text(data.Urbanizacion);
            

            $('#loadingCasa').addClass('d-none');
            $('#datosCasa').removeClass('d-none');
        },
        error: function () {
            $('#loadingCasa').addClass('d-none');
            $('#datosCasa').removeClass('d-none');
            toastr.error('Error al cargar datos de casa');
        }
    });
}

function verEdificioEnModal(id) {
    $('#loadingEdificio').removeClass('d-none');
    $('#datosEdificio').addClass('d-none');
    var modal = new bootstrap.Modal(document.getElementById('modalEdificio'));
    modal.show();

    $.ajax({
        url: '/Admin/InfoDetalllada/GetEdificioById/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
         
            
            $('#meNombreFamilia').text(data.NombreFamilia);
            $('#mePiso').text(data.Piso);
            $('#meDepartamento').text(data.Departamento);
            $('#meEtapa').text(data.Etapa);
            $('#meUrbanizacion').text(data.Urbanizacion);

            $('#loadingEdificio').addClass('d-none');
            $('#datosEdificio').removeClass('d-none');
        },
        error: function () {
            $('#loadingEdificio').addClass('d-none');
            $('#datosEdificio').removeClass('d-none');
            toastr.error('Error al cargar datos de edificio');
        }
    });
}

function verVehiculoEnModal(id) {
    $('#loadingVehiculo').removeClass('d-none');
    $('#datosVehiculo').addClass('d-none');
    var modal = new bootstrap.Modal(document.getElementById('modalVehiculo'));
    modal.show();

    $.ajax({
        url: '/Admin/InfoDetalllada/GetVehiculoById/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // ✅ CORREGIDO: MAYÚSCULAS (Placa, Marca, Modelo, Color)
            // Nota: El modal HTML tiene id="mvAnio" pero mostramos Placa
            $('#mvAnio').text(data.anio);
            $('#mvMarca').text(data.Marca);
            $('#mvModelo').text(data.Modelo);
            $('#mvColor').text(data.Color);
            $('#mvTipo').text(data.Tipo);

            $('#loadingVehiculo').addClass('d-none');
            $('#datosVehiculo').removeClass('d-none');
        },
        error: function () {
            $('#loadingVehiculo').addClass('d-none');
            $('#datosVehiculo').removeClass('d-none');
            toastr.error('Error al cargar datos de vehículo');
        }
    });
}

// ============================================================
// HABILITAR/DESHABILITAR BOTONES
// ============================================================
function habilitarBotonesPersona(habilitar) {
    $('#btnVerPersona').prop('disabled', !habilitar);
    $('#btnResetPersona').prop('disabled', !habilitar);
}

function habilitarBotonesCasa(habilitar) {
    $('#btnVerCasa').prop('disabled', !habilitar);
    $('#btnResetCasa').prop('disabled', !habilitar);
}

function habilitarBotonesEdificio(habilitar) {
    $('#btnVerEdificio').prop('disabled', !habilitar);
    $('#btnResetEdificio').prop('disabled', !habilitar);
}

function habilitarBotonesVehiculo(habilitar) {
    $('#btnVerVehiculo').prop('disabled', !habilitar);
    $('#btnResetVehiculo').prop('disabled', !habilitar);
}

// ============================================================
// ACTUALIZAR BADGES
// ============================================================
function actualizarBadges() {
    if ($('#personaSelect').val() > 0) {
        $('#badgePersona').text('Seleccionado').removeClass('bg-white text-primary').addClass('bg-success');
    }

    if ($('#casaSelect').val() > 0 && !$('#cardCasa').hasClass('d-none')) {
        $('#badgeCasa').text('Seleccionado').removeClass('bg-white text-primary').addClass('bg-success');
    }

    if ($('#edificioSelect').val() > 0 && !$('#cardEdificio').hasClass('d-none')) {
        $('#badgeEdificio').text('Seleccionado').removeClass('bg-white text-primary').addClass('bg-success');
    }

    if ($('#vehiculoSelect').val() > 0) {
        $('#badgeVehiculo').text('Seleccionado').removeClass('bg-white text-primary').addClass('bg-success');
    }
}

// ============================================================
// TOGGLE UBICACIÓN (Casa/Edificio)
// ============================================================
function toggleUbicacion() {
    var esCasa = $('#radioCasa').is(':checked');

    if (esCasa) {
        $('#cardCasa').removeClass('d-none');
        $('#cardEdificio').addClass('d-none');

        // Limpiar edificio si se cambia a casa
        $('#edificioSelect').val(null).trigger('change');
        $('#previewEdificio').addClass('d-none');
        $('#previewEdificioContent').empty();
        $('#badgeEdificio').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
        habilitarBotonesEdificio(false);

    } else {
        $('#cardCasa').addClass('d-none');
        $('#cardEdificio').removeClass('d-none');

        // Limpiar casa si se cambia a edificio
        $('#casaSelect').val(null).trigger('change');
        $('#previewCasa').addClass('d-none');
        $('#previewCasaContent').empty();
        $('#badgeCasa').text('Pendiente').removeClass('bg-success').addClass('bg-white text-primary');
        habilitarBotonesCasa(false);
    }
}

// ============================================================
// ACTUALIZAR RESUMEN MODAL
// ============================================================
function actualizarResumenModal() {
    try {
        // Persona
        var personaSel = document.getElementById('personaSelect');
        var personaTxt = 'No seleccionada';
        if (personaSel && personaSel.value && personaSel.selectedIndex >= 0) {
            personaTxt = personaSel.options[personaSel.selectedIndex].text;
        }
        document.getElementById('resumenPersona').innerHTML = '👤 Persona: <strong>' + personaTxt + '</strong>';

        // Ubicación (Casa o Edificio)
        var radioCasa = document.getElementById('radioCasa');
        var esCasa = radioCasa ? radioCasa.checked : true;
        var ubicacionTxt = 'No seleccionada';

        if (esCasa) {
            var casaSel = document.getElementById('casaSelect');
            if (casaSel && casaSel.value && casaSel.selectedIndex >= 0) {
                ubicacionTxt = '🏠 Casa: ' + casaSel.options[casaSel.selectedIndex].text;
            } else {
                ubicacionTxt = '🏠 Casa: No seleccionada';
            }
        } else {
            var edificioSel = document.getElementById('edificioSelect');
            if (edificioSel && edificioSel.value && edificioSel.selectedIndex >= 0) {
                ubicacionTxt = '🏢 Edificio: ' + edificioSel.options[edificioSel.selectedIndex].text;
            } else {
                ubicacionTxt = '🏢 Edificio: No seleccionado';
            }
        }
        document.getElementById('resumenUbicacion').innerHTML = '<strong>' + ubicacionTxt + '</strong>';

        // Vehículo
        var vehiculoSel = document.getElementById('vehiculoSelect');
        var vehiculoTxt = 'No seleccionado';
        if (vehiculoSel && vehiculoSel.value && vehiculoSel.selectedIndex >= 0) {
            vehiculoTxt = vehiculoSel.options[vehiculoSel.selectedIndex].text;
        }
        document.getElementById('resumenVehiculo').innerHTML = '🚗 Vehículo: <strong>' + vehiculoTxt + '</strong>';

        // Placa
        var placaInp = document.querySelector('input[name="InfoDetallada.NumeroPlaca"]');
        var placaVal = (placaInp && placaInp.value) ? placaInp.value.toUpperCase() : '-';
        document.getElementById('resumenPlaca').innerHTML = '🔢 Placa: <strong>' + placaVal + '</strong>';

        // Tag
        var tagInp = document.querySelector('input[name="InfoDetallada.NumeroTag"]');
        var tagVal = (tagInp && tagInp.value) ? tagInp.value : '-';
        document.getElementById('resumenTag').innerHTML = '🏷️ Tag: <strong>' + tagVal + '</strong>';

        // Tarjeta
        var tarjetaInp = document.querySelector('input[name="InfoDetallada.NumeroTarjeta"]');
        var tarjetaVal = (tarjetaInp && tarjetaInp.value) ? tarjetaInp.value : '-';
        document.getElementById('resumenTarjeta').innerHTML = '💳 Tarjeta: <strong>' + tarjetaVal + '</strong>';

    } catch (err) {
        console.log('Error actualizando resumen: ' + err.message);
    }
}

