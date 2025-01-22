var chart;

drawRentalChart();
drowdountChart();
$(document).ready(function () {
    var start = moment().subtract(29, "days");
    var end = moment();

    function cb(start, end) {
        $("#dateRangePicker").html(start.format("MMMM D, YYYY") + " - " + end.format("MMMM D, YYYY"));
        
    }

    $("#dateRangePicker").daterangepicker({
        startDate: start,
        endDate: end,
        ranges: {
            "Today": [moment(), moment()],
            "Yesterday": [moment().subtract(1, "days"), moment().subtract(1, "days")],
            "Last 7 Days": [moment().subtract(6, "days"), moment()],
            "Last 30 Days": [moment().subtract(29, "days"), moment()],
            "This Month": [moment().startOf("month"), moment().endOf("month")],
            "Last Month": [moment().subtract(1, "month").startOf("month"), moment().subtract(1, "month").endOf("month")]
        }
    }, cb);

    cb(start, end);
    $("#dateRangePicker").on('DOMSubtreeModified', function () {
        var ht = $(this).html();

        if (ht !== '') {
            var dataRange = ht.split(' - ');
            console.log(dataRange);
            chart.destroy();
            drawRentalChart(dataRange[0], dataRange[1])
        }


    });
});
function drowdountChart() {
    $.get({
        url: `/DashBoard/getSubscribersPerCity`,
        success: function (data) {
            //console.log(data);
            //var options = {
            //    chart: {
            //        type: 'donut'
            //    },
            //    series: data.map(i => i.value),
            //    labels: data.map(i => i.text)
            //}
            //var chart = new ApexCharts(document.querySelector("#donutchart"), options);

            //chart.render();
            const ctx = document.getElementById('donutchart');
           
            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: data.map(i => i.text),
                    datasets: [{
                        label: 'My First Dataset',
                        data: data.map(i => i.value),
                        backgroundColor: [
                            'rgb(255, 99, 132)',
                            'rgb(54, 162, 235)',
                        ],
                        borderRadius: 8,
                        borderColor: '#00000000' ,
                        offset: 7,
                        hoverOffset: 4
                    }]
                },
            });

           
    
        }
    })


}
function drawRentalChart(startDate = null, endDate = null) {


    var element = document.getElementById('RentalsPerDay');

    var height = parseInt(KTUtil.css(element, 'height'));
    var labelColor = KTUtil.getCssVariableValue('--kt-gray-500');
    var borderColor = KTUtil.getCssVariableValue('--kt-gray-200');
    var baseColor = KTUtil.getCssVariableValue('--kt-info');
    var lightColor = KTUtil.getCssVariableValue('--kt-info-light');

    if (!element) {
        return;
    }
    $.get({
        url: `/DashBoard/GetRentalPerDay?startDate=${startDate}&endDate=${endDate}`,
        success: function (data) {

            var options = {
                series: [{
                    name: 'Net Profit',
                    data: data.map(i => i.value)
                }],
                chart: {
                    fontFamily: 'inherit',
                    type: 'area',
                    height: height,
                    toolbar: {
                        show: false
                    }
                },
                plotOptions: {

                },
                legend: {
                    show: false
                },
                dataLabels: {
                    enabled: false
                },
                fill: {
                    type: 'solid',
                    opacity: 1
                },
                stroke: {
                    curve: 'smooth',
                    show: true,
                    width: 3,
                    colors: [baseColor]
                },
                xaxis: {
                    categories: data.map(i => i.text),
                    axisBorder: {
                        show: false,
                    },
                    axisTicks: {
                        show: false
                    },
                    labels: {
                        style: {
                            colors: labelColor,
                            fontSize: '12px'
                        }
                    },
                    crosshairs: {
                        position: 'front',
                        stroke: {
                            color: baseColor,
                            width: 1,
                            dashArray: 3
                        }
                    },
                    tooltip: {
                        enabled: true,
                        formatter: undefined,
                        offsetY: 0,
                        style: {
                            fontSize: '12px'
                        }
                    }
                },
                yaxis: {
                    min: 0,
                    tickAmount: Math.max(...data.map(i => i.value)),
                    labels: {
                        style: {
                            colors: labelColor,
                            fontSize: '12px'
                        }
                    }
                },
                states: {
                    normal: {
                        filter: {
                            type: 'none',
                            value: 0
                        }
                    },
                    hover: {
                        filter: {
                            type: 'none',
                            value: 0
                        }
                    },
                    active: {
                        allowMultipleDataPointsSelection: false,
                        filter: {
                            type: 'none',
                            value: 0
                        }
                    }
                },
                tooltip: {
                    style: {
                        fontSize: '12px'
                    },
                    y: {
                        formatter: function (val) {
                            return 'Books: ' + val
                        }
                    }
                },
                colors: [lightColor],
                grid: {
                    borderColor: borderColor,
                    strokeDashArray: 4,
                    yaxis: {
                        lines: {
                            show: true
                        }
                    }
                },
                markers: {
                    strokeColor: baseColor,
                    strokeWidth: 3
                }
            };

            chart = new ApexCharts(element, options);
            chart.render();
        }
    });


}