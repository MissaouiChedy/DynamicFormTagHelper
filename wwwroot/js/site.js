// Write your Javascript code.
$(function () {
    $('.datetimepicker').each(function (index, elem) {
        $(elem).datetimepicker({
            format: "YYYY-MM-DD[T]HH:mm:ss.SSS[Z]",
            showClear: true,
            showTodayButton: true
        });
    });
});