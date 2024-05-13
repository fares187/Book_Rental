$(document).ready(function () {
    $('.js-renew').on('click', function () {
        var subscrinerKey = $(this).data('key')
        bootbox.confirm({
            message: 'Are you sure you need to renew this subscription?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
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
                        url: '/Subscripers/RenewSubscription?key=' + subscrinerKey,
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (row) {
                            var icon = $('#ActiveStatusIcon')
                            icon.removeClass('d-none');
                            $('#Rental-Btn').removeClass('d-none');
                            icon.siblings('svg').remove();
                            icon.parents('.card').removeClass('bg-warning').addClass('bg-success');

                            $('#cardstatus').text('Active Subscriber');
                            $('#StatusBadge').text('Active Subscriber');
                            $('#StatusBadge').removeClass('badge-light-warning').addClass('badge-light-success');
                            $('#SubscriptionsTable').find('tbody').append(row);
                            showsuccess();



                        },
                        error: function () {
                            showfailure()
                        }
                    });

                }
            }
        });
    });
    $('.js-cancel-rental').on('click', function () {
        var btn = $(this)
        bootbox.confirm({
            message: 'Are you sure you need to cancel this rental?',
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
                        url: '/Rentals/MarkAsDeleted/' + btn.data('id'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function () {
                            var numberOfCopies = parseInt(btn.parents('tr').find('.js-numberOfCopies').text().replace(/\s+/g, ''));
                            
                            btn.parents('tr').remove();
                            if ($('#RentalsTable tbody tr').length === 0) {
                                $('#RentalsTable').fadeOut(function () {
                                    $('#Alert').fadeIn();
                                });

                            }
                            
                            var numberOfRentals = parseInt($('.js-numberof').text().replace(/\s+/g, ''))
                            $('.js-numberof').text(numberOfRentals - numberOfCopies);
                        },
                        error: function () {
                            showfailure()
                        }
                    });

                }
            }
        });
    });

});