$(document).ready(function () {
    $('.page-link').on('click', function () {


        var btn = $(this)
        var data = btn.data('page-number')
        console.log('-------------------->hey');
        if (btn.parent().hasClass('active'))
            return;
        console.log('-------------------->hey2');
        $('#PageNumber').val(data);

        $('#Filters').submit();
    });
});