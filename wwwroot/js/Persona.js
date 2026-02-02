var dataTable;

$(document).ready(function () {
    cargarDatatable();
});

function cargarDatatable() {
    dataTable = $("#tblPersona").DataTable({

        ajax: {
            url: "/Admin/Persona/GetAll",
            type: "GET",
            datatype: "json"
        },

        autoWidth: false,
        responsive: false,
        scrollX: false,

        initComplete: function () {
            this.api().columns.adjust();
        },

        columns: [
            { data: "Id", width: "4%" },
            { data: "Nombre", width: "8%" },
            { data: "Apellido", width: "8%" },
            { data: "Cedula", width: "7%" },
            { data: "Telefono", width: "7%" },
            { data: "Email", width: "12%" },
            { data: "Rol.NombreRol", width: "6%" },
            { data: "Sexo.NombreSexo", width: "5%" },
            { data: "TipoPersona.NombreTipoPersona", width: "7%" },
            {
                data: "Img",
                width: "6%",
                render: function (imagen) {
                    if (!imagen) return "";
                    return `
                        <img src="${imagen}"
                             alt="Foto"
                             width="70"
                             class="img-thumbnail"
                             style="cursor:pointer"
                             onclick="mostrarImagen('${imagen}')" />
                    `;
                }
            },
            {
                data: "FechaCreacion",
                width: "8%",
                render: function (data) {
                    return data
                        ? new Date(data).toLocaleString("es-EC")
                        : "";
                }
            },
            {
                data: "Nacimiento",
                width: "7%",
                render: function (data) {
                    return data
                        ? new Date(data).toLocaleDateString("es-EC")
                        : "";
                }
            },
            {
                data: "FechaInicio",
                width: "7%",
                render: function (data) {
                    return data
                        ? new Date(data).toLocaleDateString("es-EC")
                        : "";
                }
            },
            {
                data: "FechaFin",
                width: "7%",
                render: function (data) {
                    return data
                        ? new Date(data).toLocaleDateString("es-EC")
                        : "";
                }
            },
            { data: "Direccion", width: "12%" },
            {
                data: "CreatedAt",
                width: "8%",
                render: function (data) {
                    return data
                        ? new Date(data).toLocaleString("es-EC")
                        : "";
                }
            },
            {
                data: "UpdatedAt",
                width: "8%",
                render: function (data) {
                    return data
                        ? new Date(data).toLocaleString("es-EC")
                        : "";
                }
            },
            {
                data: "Estado",
                width: "5%",
                render: function (data) {
                    return data
                        ? '<span class="badge bg-success"><i class="bi bi-hand-thumbs-up-fill"></i> Activo</span>'
                        : '<span class="badge bg-danger"><i class="bi bi-eye-slash-fill"></i> Inactivo</span>';
                }
            },
            {
                data: "Permiso",
                width: "5%",
                render: function (data) {
                    return data
                        ? '<span class="badge bg-success"><i class="bi bi-hand-thumbs-up-fill"></i> Activo</span>'
                        : '<span class="badge bg-danger"><i class="bi bi-eye-slash-fill"></i> Inactivo</span>';
                }
            },
            {
                data: "Google",
                width: "5%",
                render: function (data) {
                    return data
                        ? '<span class="badge bg-success"><i class="bi bi-hand-thumbs-up-fill"></i> Activo</span>'
                        : '<span class="badge bg-danger"><i class="bi bi-eye-slash-fill"></i> Inactivo</span>';
                }
            },
            {
                data: "Id",
                width: "8%",
                render: function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Persona/Edit/${data}"
                               class="btn btn-warning btn-sm text-white"
                               title="Editar">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick="Delete('/Admin/Persona/Delete/${data}')"
                               class="btn btn-danger btn-sm ms-1"
                               title="Eliminar">
                                <i class="bi bi-trash"></i>
                            </a>
                        </div>
                    `;
                }
            }
        ],

        language: {
            emptyTable: "No hay registros",
            info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
            infoEmpty: "Mostrando 0 a 0 de 0 registros",
            infoFiltered: "(filtrado de _MAX_ registros totales)",
            lengthMenu: "Mostrar _MENU_ registros",
            loadingRecords: "Cargando...",
            processing: "Procesando...",
            search: "Buscar:",
            zeroRecords: "No se encontraron resultados",
            paginate: {
                first: "Primero",
                last: "Último",
                next: "Siguiente",
                previous: "Anterior"
            }
        }
    });
}

/* =========================
   ELIMINAR REGISTRO
========================= */
function Delete(url) {
    swal({
        title: "¿Está seguro de borrar?",
        text: "Este registro no se puede recuperar",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        confirmButtonText: "Sí, borrar",
        cancelButtonText: "Cancelar"
    }, function () {

        $.ajax({
            type: "DELETE",
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
