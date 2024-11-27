var dataTable;
$(document).ready(function () {
    loadDataTable();
})

// Match the key name with whats in the json returned from the API.
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            // Make sure the 'key' property is exacrtly like it is in the API JSON.
            { data: 'title', 'width': '15%' },
            { data: 'isbn', 'width': '15%' },
            { data: 'listPrice', 'width': '15%' },
            { data: 'author', 'width': '15%' },
            // This is category.name because it is nested in product.
            { data: 'category.name', 'width': '15%' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"
                                    <i class="bi bi-pencil-square"></i>
                                    Edit
                                </a>
                                <a onClick=deleteProduct('/admin/product/delete/${data}') class="btn btn-danger mx-2"
                                    <i class="bi bi-trash-fill"></i>
                                   Delete
                                </a>
                            </div>`
                }, 
                'width': '15%'
            }
        ]
    })
}


function deleteProduct(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}