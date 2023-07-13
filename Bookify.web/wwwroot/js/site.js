$(document).ready(function () {
    var massage = $('#Massage').text()
    if (massage !== '') {
        swal.fire({
            icon: "success",
            title: "success",
            text: massage,
            customClass: {
                confirmButton:"btn btn-primary"}
        });
    }

})