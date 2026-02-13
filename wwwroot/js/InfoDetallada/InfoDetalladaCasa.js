// Variable global para guardar los datos de la casa seleccionada
let casaSeleccionada = null;

$(document).ready(function () {

    // Inicializar Select2 con búsqueda AJAX
    $('#casaSelect').select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: 'Escriba nombre de familia, manzana, villa o etapa...',
        allowClear: true,
        ajax: {
            url: '/Admin/InfoDetalllada/BuscarCasa',
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
                            text: item.nombreFamilia + ' - Mz: ' + item.manzana + ' - Villa: ' + item.villa + ' - Etapa: ' + item.etapa
                        };
                    })
                };
            }
        },
        language: {
            noResults: function () { return "No se encontraron casas"; },
            searching: function () { return "Buscando..."; }
        }
    });

    // Evento cuando cambia la selección
    $('#casaSelect').on('select2:select', function (e) {
        var data = e.params.data;
        if (data.id) {
            // Habilitar botón y actualizar badge
            $('#btnVerCasa').prop('disabled', false);
            $('#badgeCasa').removeClass('bg-white text-primary').addClass('bg-success')
                .html('<i class="bi bi-check-circle"></i> Seleccionada');

            // Cargar datos completos para el preview
            cargarDatosPreviewCasa(data.id);
        }
    });

    $('#casaSelect').on('select2:clear', function () {
        $('#btnVerCasa').prop('disabled', true);
        $('#badgeCasa').removeClass('bg-success').addClass('bg-white text-primary').text('Pendiente');
        $('#previewCasa').addClass('d-none');
        casaSeleccionada = null;
    });

    // Botón ver casa
    $('#btnVerCasa').on('click', function () {
        verCasa();
    });
});

// Función para cargar datos completos del preview
function cargarDatosPreviewCasa(id) {
    $.ajax({
        url: `/Admin/InfoDetalllada/GetCasaById/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            casaSeleccionada = data;
            mostrarPreviewCasa(data);
        },
        error: function (xhr, status, error) {
            console.error("Error cargando preview:", xhr.status, error);
            var textoSelect = $('#casaSelect option:selected').text();
            $('#previewCasa').removeClass('d-none');
            $('#previewCasaContent').html(`
                <div class="preview-item">
                    <div class="preview-icon"><i class="bi bi-house"></i></div>
                    <div>
                        <span class="preview-label">Casa:</span>
                        <span class="preview-value name">${textoSelect}</span>
                    </div>
                </div>
            `);
        }
    });
}

// Mostrar preview con datos completos
function mostrarPreviewCasa(data) {
    $('#previewCasa').removeClass('d-none');
    $('#previewCasaContent').html(`
        <div class="preview-item">
            <div class="preview-icon"><i class="bi bi-house-door-fill"></i></div>
            <div>
                <span class="preview-label">Familia:</span>
                <span class="preview-value name">${data.NombreFamilia || '-'}</span>
            </div>
        </div>
        <div class="preview-item">
            <div class="preview-icon"><i class="bi bi-grid-3x3-gap-fill"></i></div>
            <div>
                <span class="preview-label">Manzana:</span>
                <span class="preview-value">${data.Manzana || '-'}</span>
            </div>
        </div>
        <div class="preview-item">
            <div class="preview-icon"><i class="bi bi-door-open-fill"></i></div>
            <div>
                <span class="preview-label">Villa:</span>
                <span class="preview-value">${data.Villa || '-'}</span>
            </div>
        </div>
        <div class="preview-item">
            <div class="preview-icon"><i class="bi bi-diagram-3-fill"></i></div>
            <div>
                <span class="preview-label">Etapa:</span>
                <span class="preview-value">${data.Etapa || '-'}</span>
            </div>
        </div>
    `);
}

// Ver detalle de casa en modal
function verCasa() {
    let id = $("#casaSelect").val();

    if (!id) {
        alert("⚠️ Seleccione una casa primero");
        return;
    }

    // Si ya tenemos los datos, usarlos directamente
    if (casaSeleccionada && casaSeleccionada.id == id) {
        resetModalCasa();
        $('#datosCasa').addClass('d-none');
        $('#loadingCasa').removeClass('d-none');

        var modalEl = document.getElementById('modalCasa');
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();

        mostrarDatosCasa(casaSeleccionada);
        return;
    }

    // Si no, cargar por AJAX
    resetModalCasa();
    $('#datosCasa').addClass('d-none');
    $('#loadingCasa').removeClass('d-none');

    var modalEl = document.getElementById('modalCasa');
    var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    modal.show();

    $.ajax({
        url: `/Admin/InfoDetalllada/GetCasaById/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            casaSeleccionada = data;
            mostrarDatosCasa(data);
        },
        error: function (xhr, status, error) {
            console.error("Error AJAX:", xhr.status, xhr.responseText);
            $('#loadingCasa').addClass('d-none');
            $('#datosCasa').removeClass('d-none');
            $("#mcNombreFamilia").text("Error al cargar datos").addClass('text-danger');
        }
    });
}

function resetModalCasa() {
    $("#mcNombreFamilia").text("-");
    $("#mcManzana").text("-");
    $("#mcVilla").text("-");
    $("#mcEtapa").text("-");
}

function mostrarDatosCasa(data) {
    $('#loadingCasa').addClass('d-none');
    $('#datosCasa').removeClass('d-none').hide().fadeIn(300);

    $("#mcNombreFamilia").text(data.NombreFamilia || '-');
    $("#mcManzana").text(data.Manzana || '-');
    $("#mcVilla").text(data.Villa || '-');
    $("#mcEtapa").text(data.Etapa || '-');
}