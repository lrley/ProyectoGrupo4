// Variable global para guardar los datos de la persona seleccionada
let personaSeleccionada = null;

$(document).ready(function () {

    //// Inicializar Select2 con búsqueda
    //$('#personaSelect').select2({
    //    theme: 'bootstrap-5',
    //    width: '100%',
    //    placeholder: 'Escriba nombre, cédula o email...',
    //    allowClear: true,
    //    language: {
    //        noResults: function () { return "No se encontraron personas"; },
    //        searching: function () { return "Buscando..."; }
    //    }
    //});

    $('#personaSelect').select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: 'Escriba nombre, cédula o email...',
        allowClear: true,
        ajax: {
            url: '/Admin/InfoDetalllada/BuscarPersona',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { criterio: params.term }; // lo que escribe el usuario
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text: item.nombre + ' ' + item.apellido + ' (' + item.cedula + ')'
                        };
                    })
                };
            }
        },
        language: {
            noResults: function () { return "No se encontraron personas"; },
            searching: function () { return "Buscando..."; }
        }
    });


    // Evento cuando cambia la selección
    $('#personaSelect').on('select2:select', function (e) {
        var data = e.params.data;
        if (data.id) {
            // Habilitar botón y actualizar badge
            $('#btnVerPersona').prop('disabled', false);
            $('#badgePersona').removeClass('bg-white text-primary').addClass('bg-success')
                .html('<i class="bi bi-check-circle"></i> Seleccionada');

            // Cargar datos completos para el preview
            cargarDatosPreview(data.id);
        }
    });

    $('#personaSelect').on('select2:clear', function () {
        $('#btnVerPersona').prop('disabled', true);
        $('#badgePersona').removeClass('bg-success').addClass('bg-white text-primary').text('Pendiente');
        $('#previewPersona').addClass('d-none');
        personaSeleccionada = null;
    });

    // Botón ver persona
    $('#btnVerPersona').on('click', function () {
        verPersona();
    });
});

// Función para cargar datos completos del preview
function cargarDatosPreview(id) {
    $.ajax({
        url: `/Admin/InfoDetalllada/GetPersonaById/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            personaSeleccionada = data;
            mostrarPreview(data);
        },
        error: function (xhr, status, error) {
            console.error("Error cargando preview:", xhr.status, error);
            // Si falla, mostrar al menos el texto del select
            var textoSelect = $('#personaSelect option:selected').text();
            $('#previewPersona').removeClass('d-none');
            $('#previewContent').html(`
                        <div class="preview-item">
                            <div class="preview-icon"><i class="bi bi-person"></i></div>
                            <div>
                                <span class="preview-label">Persona:</span>
                                <span class="preview-value name">${textoSelect}</span>
                            </div>
                        </div>
                    `);
        }
    });
}

// Mostrar preview con datos completos
function mostrarPreview(data) {
    $('#previewPersona').removeClass('d-none');
    $('#previewContent').html(`
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-person-fill"></i></div>
                    <div>
                        <span class="preview-label">Nombre:</span>
                        <span class="preview-value name">${data.nombre || ''} ${data.apellido || ''}</span>
                    </div>
                </div>
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-card-text"></i></div>
                    <div>
                        <span class="preview-label">Cédula:</span>
                        <span class="preview-value cedula">${data.cedula || '-'}</span>
                    </div>
                </div>
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-envelope-fill"></i></div>
                    <div>
                        <span class="preview-label">Email:</span>
                        <span class="preview-value email">${data.email || '-'}</span>
                    </div>
                </div>
            `);
}

function verPersona() {
    let id = $("#personaSelect").val();

    if (!id) {
        alert("⚠️ Seleccione una persona primero");
        return;
    }

    // Si ya tenemos los datos, usarlos directamente
    if (personaSeleccionada && personaSeleccionada.id == id) {
        resetModalPersona();
        $('#datosPersona').addClass('d-none');
        $('#loadingPersona').removeClass('d-none');

        var modalEl = document.getElementById('modalPersona');
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();

        mostrarDatosPersona(personaSeleccionada);
        return;
    }

    // Si no, cargar por AJAX
    resetModalPersona();
    $('#datosPersona').addClass('d-none');
    $('#loadingPersona').removeClass('d-none');

    var modalEl = document.getElementById('modalPersona');
    var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    modal.show();

    $.ajax({
        url: `/Admin/InfoDetalllada/GetPersonaById/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            personaSeleccionada = data;
            mostrarDatosPersona(data);
        },
        error: function (xhr, status, error) {
            console.error("Error AJAX:", xhr.status, xhr.responseText);
            $('#loadingPersona').addClass('d-none');
            $('#datosPersona').removeClass('d-none');
            $("#mpNombre").text("Error al cargar datos").addClass('text-danger');
            $("#mpCedula").text(`Error ${xhr.status}: ${error}`);
        }
    });
}

function resetModalPersona() {
    $("#mpNombre").text("-").removeClass('text-danger');
    $("#mpCedula").text("-");
    $("#mpTelefono").text("-");
    $("#mpEmail").text("-");
    $("#mpRol").text("-");
    $("#mpTipo").text("-");
}

function mostrarDatosPersona(data) {
    $('#loadingPersona').addClass('d-none');
    $('#datosPersona').removeClass('d-none').hide().fadeIn(300);

    $("#mpNombre").text((data.nombre || '') + ' ' + (data.apellido || ''));
    $("#mpCedula").text(data.cedula || '-');
    $("#mpTelefono").text(data.telefono || '-');
    $("#mpEmail").text(data.email || '-');
    $("#mpRol").text(data.rolNombre || 'N/A');
    $("#mpTipo").text(data.tipoPersonaNombre || 'N/A');
}

// Validación solo números en Tag y Tarjeta
$('input[name="InfoDetallada.NumeroTag"], input[name="InfoDetallada.NumeroTarjeta"]').on('input', function () {
    this.value = this.value.replace(/[^0-9]/g, '');
});