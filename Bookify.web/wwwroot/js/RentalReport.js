
var start= undefined;
var end = undefined;
$(document).ready(function () {
    //console.log($('#Filters input[Id=EndDate]').val())
    //if ($('#Filters input[Id=EndDate]').val() == '') {
    //    var start = moment().subtract(29, 'days');
    //    var end = moment();
    //} else {
    //    var start = moment($('#Filters input[Id=StartDate]').val()) ;
    //    var end = moment($('#Filters input[Id=EndDate]').val());
    //}
        
   
    //function CD(start, end) {
    //    $('#DateRangePicker span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
    //    $("#StartDate").val(start.format('YYYY-MM-DD'))
    //    $("#EndDate").val(end.format('YYYY-MM-DD'))
    //}
    //$('#DateRangePicker').daterangepicker({
    //    startDate: start,
    //    endDate: end,
    //    autoApply: true,
    //    maxDate: new Date(),
    //    minDate: new Date("2020-01-01")
    //}, CD);
    //CD(start,end);
    //$('#RentalReportForm').on('submit', function () {
    //    console.log($('#DateRangePicker').html());
    //});

    $("#DateRangePicker").daterangepicker({
        autoApply: true,

        minDate: new Date("2020-01-01"),
        maxDate: moment().startOf("hour"),
        //startDate: moment().startOf("hour"),
        //endDate: moment().startOf("hour").add(32, "hour")
        startDate: new Date("2020-01-01"),
        endDate: moment().startOf("hour"),
       
    });
    $('#DateRangePicker').on('apply.daterangepicker', function (ev, picker) {
        console.log($('DateRangePicker').val());

    });

})