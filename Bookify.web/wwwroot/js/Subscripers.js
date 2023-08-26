$(document).ready(function () {
    $('#GovernorateId').on('change', function () {
        console.log('asdfas------------------->');
        var governorate = $(this).val();
        if (governorate !== '') {

            var Areas = $('#AreaId')
            Areas.empty();
            Areas.append('<option ></option>');
            $.ajax({
                url: '/Subscripers/GetAreas/' + governorate,
                success: function (areas) {
                  
                    $.each(areas,function(i,area ){
                        Areas.append($('<option></option>').attr("value",area.value).text(area.text)) 
                    })
                },
                error: function () {
                    showsuccess()

                }
            })
        }
    })


    

});