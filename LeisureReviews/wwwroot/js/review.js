const converter = new showdown.Converter()
$('#content').html(converter.makeHtml($('#content').html()))

$('.get-image').each(async function () {
    const fileId = $(this).data('fileId')
    if (fileId.length === 0) {
        $(this).parents('.uk-cover-container').hide()
        return
    }
    $('#illustraion-spinner').show()
    const self = this
    await $.get(`/Review/GetIllustration?fileId=${fileId}`).always(function () {
        $('#illustraion-spinner').hide()
    }).done(function (file) {
        $(self).parents('.uk-cover-container').show()
        $(self).attr('src', file)
    })
})