var dataTable;

$(document).ready(function () {
    cargarDatatable();
});

function cargarDatatable() {
    dataTable = $("#tblPersona").DataTable({
        "ajax": {
            "url": "/Admin/Persona/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "Id", "width": "10%" },
            { "data": "Nombre", "width": "25%" },
            { "data": "Apellido", "width": "25%" },
            { "data": "Cedula", "width": "25%" },
            { "data": "Telefono", "width": "25%" },
            { "data": "Email", "width": "25%" },
            { "data": "Rol.NombreRol", "width": "25%" },
            { "data": "Sexo.NombreSexo", "width": "25%" },
            { "data": "TipoPersona.NombreTipoPersona", "width": "25%" },
            {
                "data": "Img",
                "width": "25%",
                "render": function (imagen) {
                    if (!imagen) return "";
                    return `<img src="${imagen}" alt="Foto" width="120" class="img-thumbnail"
                        style="cursor:pointer" onclick="mostrarImagen('${imagen}')"/>`;
                }
            },
            {
                "data": "FechaCreacion",
                "width": "20%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC");
                }
            },
            {
                "data": "Nacimiento",
                "width": "20%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC");
                }
            },
            {
                "data": "FechaInicio",
                "width": "20%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC");
                }
            },
            {
                "data": "FechaFin",
                "width": "20%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC");
                }
            },
            { "data": "Direccion", "width": "25%" },
            {
                "data": "CreatedAt",
                "width": "20%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC");
                }
            },
            {
                "data": "UpdatedAt",
                "width": "20%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC");
                }
            },
            {
                "data": "Estado",
                "render": function (data) {
                    return data
                        ? '<span class="badge bg-success"><i class="bi bi-hand-thumbs-up-fill"></i> Activo</span>'
                        : '<span class="badge bg-danger"><i class="bi bi-eye-slash-fill"></i> Inactivo</span>';
                },
                "width": "10%"
            },
            {
                "data": "Permiso",
                "render": function (data) {
                    return data
                        ? '<span class="badge bg-success"><i class="bi bi-hand-thumbs-up-fill"></i> Activo</span>'
                        : '<span class="badge bg-danger"><i class="bi bi-eye-slash-fill"></i> Inactivo</span>';
                },
                "width": "10%"
            },
            {
                "data": "Google",
                "render": function (data) {
                    return data
                        ? '<span class="badge bg-success"><i class="bi bi-hand-thumbs-up-fill"></i> Activo</span>'
                        : '<span class="badge bg-danger"><i class="bi bi-eye-slash-fill"></i> Inactivo</span>';
                },
                "width": "10%"
            },
            {
                "data": "Id",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/Persona/Edit/${data}" class="btn btn-warning text-white" style="cursor:pointer; width:60px;">
                               <i class="bi bi-pencil-square"></i> 
                            </a>
                            &nbsp;
                            <a onclick=Delete("/Admin/Persona/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:60px;">
                                <i class="bi bi-trash"></i> 
                            </a>
                        </div>`;
                },
                "width": "15%"
            } // <- última columna, sin coma
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 to 0 of 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "scrollX": true,
        "width": "100%"
    });
}

function Delete(url) {
    swal({
        title: "¿Está seguro de borrar?",
        text: "Este contenido no se puede recuperar!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Sí, borrar!",
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
            }
        });
    });
}