var currentCopies = [];
var selectedCopies = [];
var IsEditMode = false;

function onAddCopySuccess(copy) {
    $('#Value').val('');
    var sbookId = $(copy).find('.js-copy').data('book-id')
    if (selectedCopies.find(c => c.bookId === sbookId)) {
        showfailure("you can't have more than one editions of the same book")
        return;
    }
    $('#Copies_Form').prepend(copy);
    $('#Copies_Form').find(':submit').removeClass('d-none');
    RepairInput();
}
function RepairInput() {
    var copies = $('.js-copy');
    selectedCopies = [];
    $.each(copies, function (i, input) {
        var input = $(input);
        selectedCopies.push({ serial: input.val(), bookId: input.data('book-id') });
        input.attr('name', 'SelectedCopies[' + i + ']').attr('id', 'SelectedCopies_' + i + '_');

    })

}
$(document).ready(function () {
    if ($('.js-copy').length > 0) {
        IsEditMode = true;
        RepairInput();
        currentCopies = selectedCopies;
    }
    $('.js-search').on('click', function (e) {
        e.preventDefault();
        var serial = $('#Value').val();
        if (selectedCopies.find(c => c.serial === serial)) {
            showfailure("you can't add the same copy")
            return;
        }
        if ($('.js-copy').length >= maxAllowedCopies) {
            showfailure("you can't add more than " + maxAllowedCopies + " book(s)")
            return;

        }
        $('#searchForm').submit();

    });
    $('body').delegate('.js-remove', 'click', function () {
        var btn = $(this);
        var container = btn.parents('.js-copy-container');
        if (IsEditMode) {
            btn.toggleClass('btn-danger btn-success js-remove js-readd').text('Re-add');
            container.find('img').css('opacity', '0.2');
            container.find('h4').css('text-decoration', 'line-through');
            container.find('.js-copy').toggleClass('js-copy js-removed').removeAttr('name').removeAttr('id');
        } else {
            container.remove();
        }

        RepairInput()
        if ($.isEmptyObject(selectedCopies) || JSON.stringify(currentCopies) == JSON.stringify(selectedCopies))
            $('#Copies_Form').find(':submit').addClass('d-none');
        else
            $('#Copies_Form').find(':submit').removeClass('d-none');
    });

    $('body').delegate('.js-readd', 'click', function () {
        var btn = $(this);
        var container = btn.parents('.js-copy-container');

        btn.toggleClass('btn-danger btn-success js-remove js-readd').text('Remove');
        container.find('img').css('opacity', '1');
        container.find('h4').css('text-decoration', 'none');
        container.find('.js-removed').toggleClass('js-copy js-removed');


        RepairInput()
        if (JSON.stringify(currentCopies) == JSON.stringify(selectedCopies))
            $('#Copies_Form').find(':submit').addClass('d-none');
        else
            $('#Copies_Form').find(':submit').removeClass('d-none');
    });
}); 