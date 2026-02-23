var dataTable;

$(document).ready(function () {
    cargarDatatable();
});

function cargarDatatable() {
    dataTable = $("#tblInfoDetallada").DataTable({
        "ajax": {
            "url": "/Admin/InfoDetalllada/GetAll", // ✅ CORREGIDO
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            {
                "data": "persona",
                "width": "20%",
                "render": function (data) {
                    if (!data) return '<span class="text-muted">-</span>';
                    return `<strong>${data.nombre || ''} ${data.apellido || ''}</strong><br>
                            <small class="text-muted">${data.cedula || ''}</small>`;
                }
            },
            {
                "data": "casa",
                "width": "15%",
                "render": function (data, type, row) {
                    if (data) {
                        return `<span class="badge bg-success"><i class="bi bi-house-door"></i> Casa</span><br>
                                <small>${data.nombreFamilia || ''}<br>Mz: ${data.manzana || ''} - Villa: ${data.villa || ''}</small>`;
                    }
                    if (row.edificio) {
                        return `<span class="badge bg-info"><i class="bi bi-building"></i> Edificio</span><br>
                                <small>${row.edificio.nombreEdificio || ''}<br>Piso: ${row.edificio.piso || ''} - Dpto: ${row.edificio.departamento || ''}</small>`;
                    }
                    return '<span class="text-muted">-</span>';
                }
            },
            {
                "data": "vehiculo",
                "width": "15%",
                "render": function (data) {
                    if (!data) return '<span class="text-muted">-</span>';
                    return `<i class="bi bi-car-front"></i> ${data.placa || '-'}<br>
                            <small>${data.marca || ''} ${data.modelo || ''}</small>`;
                }
            },
            {
                "data": "numeroPlaca",
                "width": "8%",
                "render": function (data) {
                    return data ? `<span class="badge bg-primary">${data.toUpperCase()}</span>` : '<span class="text-muted">-</span>';
                }
            },
            {
                "data": "numeroTag",
                "width": "8%",
                "render": function (data) {
                    return data ? `<span class="badge bg-warning text-dark">${data}</span>` : '<span class="text-muted">-</span>';
                }
            },
            {
                "data": "numeroTarjeta",
                "width": "8%",
                "render": function (data) {
                    return data ? `<span class="badge bg-secondary">${data}</span>` : '<span class="text-muted">-</span>';
                }
            },
            {
                "data": "persona.tipopropietario",
                "width": "8%",
                "render": function (data) {
                    return data ? `<span class="badge bg-secondary">${data}</span>` : '<span class="text-muted">-</span>';
                }
            },
            {
                "data": "foto",
                "width": "8%",
                "render": function (data) {
                    if (!data || data === "Sin-Foto")
                        return '<span class="text-muted">Sin foto</span>';

                    return `
                        <img src="${data}" alt="Foto" width="60" class="img-thumbnail" 
                             style="cursor:pointer" onclick="mostrarImagen('${data}')" />
                    `;
                }
            },
            {
                "data": "fechaCreacion",
                "width": "12%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC");
                }
            },
            {
                "data": "permiso", // ✅ Permiso 
                "render": function (data) {
                    return data
                        ? '<span class="badge bg-success"><i class="bi bi-check-circle"></i> Activo</span>'
                        : '<span class="badge bg-danger"><i class="bi bi-x-circle"></i> Inactivo</span>';
                },
                "width": "8%"
            },
            {
                "data": "estado", // ✅  ESTADO
                "render": function (data) {
                    return data
                        ? '<span class="badge bg-success"><i class="bi bi-check-circle"></i> Activo</span>'
                        : '<span class="badge bg-danger"><i class="bi bi-x-circle"></i> Inactivo</span>';
                },
                "width": "8%"
            },

            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/InfoDetalllada/Edit/${data}" class="btn btn-warning text-white btn-sm" title="Editar">
                               <i class="bi bi-pencil-square"></i> 
                            </a>
                            <a onclick="Delete('/Admin/InfoDetalllada/Delete/${data}')" class="btn btn-danger text-white btn-sm" title="Eliminar">
                                <i class="bi bi-trash"></i> 
                            </a>
                          
                        </div>`;
                },
                "width": "15%"
            }
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 a 0 de 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "width": "100%",
        "order": [[0, "desc"]]
    });
}

function Delete(url) {
    swal({
        title: "¿Está seguro de eliminar?",
        text: "Este registro no se puede recuperar!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Sí, eliminar!",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true
    }, function () {
        $.ajax({
            type: 'DELETE',
            url: url,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                } else {
                    toastr.error(data.message);
                }
            },
            error: function () {
                toastr.error("Error al eliminar el registro");
            }
        });
    });
}

function VerDetalle(id) {
    $.ajax({
        url: '/Admin/InfoDetalllada/GetDetalle/' + id, // ✅ CORREGIDO
        type: 'GET',
        success: function (data) {
            mostrarModalDetalle(data);
        },
        error: function () {
            toastr.error("Error al cargar el detalle");
        }
    });
}


function mostrarImagen(ruta) {
    $("#imagenExpandida").attr("src", ruta);
    var modal = new bootstrap.Modal(document.getElementById('modalImagen'));
    modal.show();
}
