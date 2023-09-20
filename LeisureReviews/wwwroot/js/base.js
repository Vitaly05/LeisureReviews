function showButtonSpinner(button) {
    button.prop('disabled', true)
    button.find('#button-text').hide()
    button.find('#button-spinner').show()
}

function hideButtonSpinner(button) {
    button.prop('disabled', false)
    button.find('#button-text').show()
    button.find('#button-spinner').hide()
}

$('.search-button')?.on('click', function (e) {
    e.preventDefault()
    UIkit.modal($('#search-modal')).show()
})

$('#theme-switcher')?.on('click', function (e) {
    e.preventDefault()
    if ($('#theme-link').attr('href').length === 0) {
        localStorage.setItem('themeHref', '/lib/uikit/themes/dark.css')
    } else {
        localStorage.setItem('themeHref', '')
    }
    $('#theme-link').attr('href', localStorage.getItem('themeHref'))
})

$('#theme-link').attr('href', localStorage.getItem('themeHref') ?? '')

const searchClient = algoliasearch('MS6370NITB', 'ddd1467673a13e7a1031cd9aff4a0130')

const search = instantsearch({
    indexName: 'reviews',
    searchClient
})

search.addWidgets([
    instantsearch.widgets.hits({
        container: '#hits',
        templates: {
            item: displayHit,
            empty: '<div class="text-center py-4">No results<div>',
        },
    }),
    instantsearch.widgets.pagination({
        container: '#pagination',
        showFirst: false,
        showLast: false,
        padding: 2
    })
])

search.start()

function displayHit(hit) {
    const reviewCard = $('#hit-template').clone()
    fillReviewCard(reviewCard, hit)
    reviewCard.show()
    return reviewCard.html()
}

function fillReviewCard(card, hit) {
    card.find('a').attr('href', `/Review?reviewId=${hit.objectID}`)
    card.find('#title').html(hit._highlightResult.title.value)
    card.find('#leisure').html(hit._highlightResult.leisure.value)
}

$('#search-input').on('input', function () {
    search.helper.setQuery($(this).val()).search()
})