$(document).ready(function () {
    $('.datetimepicker').each(function (index, elem) {
        $(elem).datetimepicker({
            format: "YYYY-MM-DD[T]HH:mm:ss.SSS[Z]",
            showClear: true,
            showTodayButton: true
        });
    });

    $('input.autocomplete').each(function (index, elem) {
        if ($(elem).data('source-local')) {
            var suggestionEngine = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.whitespace,
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                local: $(elem).data('source-local')
            });
            suggestionEngine.initialize();
            $(elem).typeahead({
                items: 4,
                source: suggestionEngine.ttAdapter(),
                autoSelect: true
            });
        }
        else if ($(elem).data('source-ajax')) {

        }
    });
});
