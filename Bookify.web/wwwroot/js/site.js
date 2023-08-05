var updateRow;
var table;
var datatable;
var expotedColumns = [];


function showsuccess(massage = 'saved successfully') {
    swal.fire({
        icon: "success",
        title: "success",
        text: massage,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });

}
function showfailure(massage = 'some thing went wrong') {
    swal.fire({
        icon: "error",
        title: "Oops...",
        text: massage,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });

}
function DisableSubmitButton() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on')
}
function OnModelBegin() {
    DisableSubmitButton();
   // btn.toggleClass('btn-primary btn-secondary')


}
function onModelSuccess(row) {
    showsuccess()
    $('#myModal').modal('hide')
  var newrow = $(row);
        datatable.row.add(newrow).draw();

    if (updateRow !== undefined) {
        datatable.row(updateRow).remove().draw();
        //$('tbody').append(row)
        updateRow = undefined
    } 
    KTMenu.init();
    KTMenu.initGlobalHandlers();

    
}

function OnModelComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator')
   // btn.toggleClass('btn-primary btn-secondary')


}
var headers = $('th');
$.each(headers, function (i) {
    var col = $(this)
    if (!col.hasClass('js-no-export')) {
        expotedColumns.push(i);
    }
})
var KTDatatables = function () {
    // Shared variables
    

    // Private functions
    var initDatatable = function () {
        // Set date data order


        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
         
            'pageLength': 10,
        });
    }

    // Hook export buttons
    var exportButtons = () => {
        const documentTitle = $('.js-DataTables').data('document-title');
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns:expotedColumns
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: expotedColumns
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: expotedColumns
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: expotedColumns
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }
    
    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-DataTables');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();

$(document).ready(function () {
    //disable submit button
    $('form').on('submit', function () {
        if ($('.js-tinymce').length > 0) {
            $('.js-tinymce').each(function () {
                var input = $(this)
                var content = tinymce.get(input.attr('id')).getContent();
                input.val(content)
            });

        }
        var isValid = $(this).valid()
        if(isValid) DisableSubmitButton();
    });
    //handel tinymce
    if ($('.js-tinymce').length>0) {
        //tinymce.init({
        //    selector: ".js-tinymce", height: "480"
        //});
        var options = { selector: ".js-tinymce", height: "387" };

        if (KTThemeMode.getMode() === "dark") {
            options["skin"] = "oxide-dark";
            options["content_css"] = "dark";
        }


        tinymce.init(options);

    }
    //select2
    $('.js-select2').select2({
        placeholder: $(this).data('placeholder'),
        allowClear: true
    });

    $('.js-select2').on('select2:select', function (e) {
        var select = $(this)
        $('form').validate().element('#' + select.attr('id'))
    });
    //datapicker
    $('.js-data-time-picker').flatpickr({
        enableTime: true,
        maxDate: "today"   ,
        dateFormat: "Y-m-d H:i",
    
        monthSelectorType:"dropdown"
    })


    //handle images in preview
    $("#ImageInput").change(function () {
        if (this.files[0]) {
            console.log(this.file)
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#ImagePreview').attr('src', e.target.result);
            }
            reader.readAsDataURL(this.files[0]);
        }
    });


    //handle drop downs in datatable
   

    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });
    
    //var massage = $('#Massage').text()
    //if (massage !== '') {
    //    swal.fire({
    //        icon: "success",
    //        title: "success",
    //        text: massage,
    //        customClass: {
    //            confirmButton:"btn btn-primary"}
    //    });
    //}

    // handel add modal
    $('body').delegate('.js-render-modal', 'click', function () {
        console.log('clicked')
        var btn = $(this)
        var modal = $('#myModal')
        modal.find('#ModalLabel').text(btn.data('title'))
        if (btn.data('update') !== undefined) {
            updateRow = btn.parents('tr')
            //console.log(updateRow)
        }
        $.get({
            url: btn.data('url'),
            success: function (form) {

                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);
            },
            error: function () {
                showfailure()
            }
        });
        modal.modal('show')
    })
    // handel toggle status
    // ToDo change to  delegate
    $('body').delegate('.js-toggle-status', "click", function () {
        var btn = $(this);
        console.log(btn.data('url'))
        bootbox.confirm({
            message: 'are you sure you want to toggle this category?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {

                    $.post({
                        //js-last
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (last) {
                            console.log(btn.data('url'))
                        //    console.log(last);
                            var row = btn.parents('tr')
                            var status = row.find('.js-status');
                            var newstatus = status.text().trim() === 'Disabled' ? 'Active' : 'Disabled';
                            status.text(newstatus).toggleClass('bg-light-success bg-light-danger');
                            status.text(newstatus).toggleClass('text-success text-danger');
                            row.find('.js-last').html(last);
                           // row.addClass('animate__animated animate__wobble');
                        }
                    });

                }
            }
        });






    });
}) 