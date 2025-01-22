$(document).ready(function () {
    var books = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
     /*   prefetch: '/Search/Find?Typedm=%QUERY',*/
        remote: {
            url: '/Search/Find?typedm=%QUERY',
            wildcard: '%QUERY'
        }
    });

    $('#remote .typeahead').typeahead(null, {
        name: 'best-pictures',
        display: 'title',
        source: books,
        templates: {
            empty: [
                '<div class="empty-message">',
                'there is no book or Author with that name.',
                '</div>'
            ].join('\n'),
            suggestion: Handlebars.compile('<div><strong>{{title}}</strong> – {{author}}</div>')

        }
    }).on('typeahead:select', function (e, book) {
        window.location.replace(`/Search/Details?Key=${book.key}`);
    });
});