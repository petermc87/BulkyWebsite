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
                                <a href="" class="btn btn-danger mx-2"
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

