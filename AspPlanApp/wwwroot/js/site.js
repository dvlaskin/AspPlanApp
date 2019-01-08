$(document).ready(function () {
    var date_input = $('input[name="date"]'); //our date input has the name "date"
    var container = "body";
    var options = {
        format: 'mm/dd/yyyy',
        container: container,
        todayHighlight: true,
        autoclose: true,
    };
    date_input.datepicker(options);
})