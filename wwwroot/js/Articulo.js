var dataTable;

$(document).ready(function () {
    cargarDatatable();
});


function cargarDatatable() {
    dataTable = $("#tblArticulo").DataTable({
        "ajax": {
            "url": "/Admin/Articulo/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "Id", "width": "8%" },
            { "data": "Nombre", "width": "20%" },
            { "data": "Categoria.Nombre", "width": "20%" },
            { "data": "Descripcion", "width": "30%" },
            {
                data: "urlImagen",
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
                "data": "FechaCreacion",
                "width": "15%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC")
                }
            },

            {
                "data": "CreatedAt",
                "width": "15%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC")
                }
            },
            {
                "data": "UpdatedAt",
                "width": "15%",
                "render": function (data) {
                    if (!data) return "";
                    return new Date(data).toLocaleString("es-EC")
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
                "data": "Id",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/Articulo/Edit/${data}" class="btn btn-warning text-white" style="cursor:pointer; width:60px;">
                               <i class="bi bi-pencil-square"></i> 
                            </a>
                            &nbsp;
                            <a onclick=Delete("/Admin/Articulo/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:60px;">
                                <i class="bi bi-trash"></i> 
                            </a>
                        </div>`;
                }, "width": "20%"
            }
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
        "width": "100%"
    });
}

function Delete(url) {
    swal({
        title: "Esta seguro de borrar?",
        text: "Este contenido no se puede recuperar!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, borrar!",
        closeOnconfirm: true
    }, function () {
        $.ajax({
            type: 'DELETE',
            url: url,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                }
                else {
                    toastr.error(data.message);
                }
            }
        });
    });
}