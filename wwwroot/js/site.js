function initializeAutoComplete() {
    // initialize auto complete fields
    $('input.autocomplete').each(function (index, elem) {
        var suggestionEngine = null;

        if ($(elem).data('source-local')) {
            // initialize from local source
            suggestionEngine = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.whitespace,
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                local: $(elem).data('source-local')
            });

        }
        else if ($(elem).data('source-ajax')) {
            // initialize from remote source
            suggestionEngine = new Bloodhound({
                datumTokenizer: function (datum) {
                    return Bloodhound.tokenizers.whitespace(datum);
                },
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                remote: {
                    url: $(elem).data('source-ajax') + '?typed=%QUERY',
                    wildcard: '%QUERY'
                }
            });
        }

        if (suggestionEngine !== null) {
            suggestionEngine.initialize();
            $(elem).typeahead({
                hint: true,
                highlight: true,
                minLength: 1
            }, {
                    source: suggestionEngine.ttAdapter(),
                    autoSelect: true
                });
        }
    });
}

$(document).ready(function () {
    $('.datetimepicker').each(function (index, elem) {
        $(elem).datetimepicker({
            format: "YYYY-MM-DD[T]HH:mm:ss.SSS[Z]",
            showClear: true,
            showTodayButton: true
        });
    });

    initializeAutoComplete();
});
