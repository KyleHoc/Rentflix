var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/movie/getall' },
        "columns": [
            { data: 'movieId', "width": "5%" },
            { data: 'title', "width": "25%" },
            { data: 'genre', "width": "15%" },
            { data: 'releaseYear', "width": "10%" },
            { data: 'director', "width": "20%" },
            {
                data: 'movieId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/movie/upsert?id=${data}" class="btn btn-warning mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                     <script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>
                     <a onClick=Delete('/admin/movie/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>`
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
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
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        };
    })
}