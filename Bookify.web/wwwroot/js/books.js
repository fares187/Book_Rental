$(document).ready(function () {
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this);
        datatable.search(input.val()).draw();
    });
    datatable = $('#books').DataTable({
        serverSide: true,
        processing: true,
        stateSave: true,
        lengthMenu: [3, 5, 10, 20],
        ajax: {
            url: '/Books/GetBooks',
            type: 'POST'

        }, order: [[
            1, 'asc'
        ]],
        columnDefs: [{
            targets: [0],
            visible: false,
            searchable: false
        }],
        columns: [
            { "data": "id", "name": "Id" },

            {
                "name": "Title",
                "className": "d-flex align-items-center",
                "render": function (data, type, row) {
                    return `<div class="symbol symbol-50px overflow-hidden me-3">
                                        <a href="/Books/Details/${row.id}">
                                            <div class="symbol-label h-70px">
                                                <img src="${(row.imageThumbnailUrl === null ? '/Images/Books/nobooks-ic.png' : row.imageThumbnailUrl)}" alt="cover" class="w-100">
                                            </div>
                                        </a>
                                    </div>
                                    <div class="d-flex flex-column">
                                        <a href="/Books/details/${row.id}" class="text-primary fw-bolder mb-1">${row.title}</a>
                                                <span>${row.authorName}</span>
                                    </div>`;
                }
            },
            { "data": "publisher", "name": "Publisher" },
            {
                "name": "PublishingDaTe",
                "render": function (data, type, row) {
                    return moment(row.publishingDaTe).format('LLL')
                }
            },
            { "data": "hall", "name": "Hall" },
            { "data": "categories", "name": "categories", "orderable": false },
            {
                "name": "IsAvailableForRental", "render": function (data, type, row) {
                    return `<span class="badge text-${row.isDeleted === true ? "warning" : "success"} bg-light-${row.isAvailableForRental === true ? "success" : "warning"}">${row.isAvailableForRental === true ? "Available" : "Booked"}</span>`
                }
            },
            {
                "name": "IsDeleted", "render": function (data, type, row) {
                    return `<span class="js-status badge text-${row.isDeleted === true ? "danger" : "success"} bg-light-${row.isDeleted === true ? "danger" : "success"}">${row.isDeleted === true ? "Disabled" : "Active"}</span>`
                }
            },
            {
                "orderable": false
                , "className": " text-end "
                , "render": function (data, type, row) {
                    return `<div class="dropdown">
                                      <button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false">
                                        action
                                      </button>
                                      <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1">
                                        <li><a class="dropdown-item" href="/Books/Edit/${row.id}">Edit</a></li>
                                                <li><a href="javascript:;" class="dropdown-item js-toggle-status"  data-url="/Books/Toggle/${row.id}">toggle status</a></li>
                                        <li><a class="dropdown-item"  href="/Books/details/${row.id}">details</a></li>
                                      </ul>
                                    </div>`
                }
            }
        ]

    });
});
