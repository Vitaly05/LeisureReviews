const reviewId = $('#review-info').data('id')

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

$('#like-review-button').on('click', async function () {
    showButtonSpinner($('#like-review-button'))
    await $.post(`/Review/${reviewId}/Like`).done(function () {
        hideButtonSpinner($('#like-review-button'))
        $('#like-panel').fadeOut('slow')
    })
})

const commentsHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/Comments")
    .build();

commentsHubConnection.on('NewComment', function (authorName, createTime, text) {
    hideButtonSpinner($('#send-comment-button'))
    const comment = createCommentClone(authorName, createTime, text)
    $('#comments').prepend(comment)
    comment.slideDown('slow')
    $('#no-comments-text').hide()
})

$('#send-comment-button').on('click', function () {
    if ($('#comment-input').val().length === 0) {
        UIkit.notification('Please write a text', { status: 'warning', pos: 'bottom-center' })
        return
    }
    showButtonSpinner($('#send-comment-button'))
    commentsHubConnection.invoke('send', $('#comment-input').val(), reviewId)
    $('#comment-input').val('')
})

commentsHubConnection.start().then(function () {
    commentsHubConnection.invoke('Init', reviewId)
})

function createCommentClone(authorName, createTime, text) {
    const commentClone = $('#comment-template').clone()
    commentClone.find('#text').text(text)
    commentClone.find('#create-time').text(createTime)
    commentClone.find('#profile-link').text(authorName)
    commentClone.find('#profile-link').attr('href', `/Profile/${authorName}`)
    return commentClone
}