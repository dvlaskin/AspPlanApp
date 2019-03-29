$(document).ready(function () {
    var date_input = $('input[name="dateCal"]');
    var container = "body";
    var options = {
        format: 'mm/dd/yyyy',
        container: container,
        todayHighlight: true,
        autoclose: true,
    };
    date_input.datepicker(options);
});