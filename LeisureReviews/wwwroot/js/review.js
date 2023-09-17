const reviewId = $('#review-info').data('id')

const converter = new showdown.Converter()
$('#content').html(converter.makeHtml($('#content').html()))

$('.get-image').each(async function () {
    $(this).parents('.uk-slideshow').show()
    $(this).parents('.illustration').find('#illustraion-spinner').show()
    const fileId = $(this).data('fileId')
    if (fileId.length === 0) {
        return
    }
    const self = this
    await $.get(`/Review/GetIllustration?fileId=${fileId}`).always(function () {
        $(self).parents('.illustration').find('#illustraion-spinner').hide()
    }).done(function (file) {
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

$('.rating-button').on('mouseenter', function () {
    updateRatingButtonsClass($(this).data('value'), 'yellow-svg-temp')
})

$('.rating-button').on('mouseleave', function () {
    $('.rating-button').removeClass('yellow-svg-temp')
    $('.rating-button').removeClass('clear-svg-temp')
})

$('.rating-button').on('click', async function () {
    await $.post(`/Review/${reviewId}/Rate/${$(this).data('value')}`).done(function (data) {
        updateRatingButtonsClass(data.value, 'yellow-svg')
        $('#average-rate').text(`(${data.average})`)
        UIkit.notification('Your rate saved', { status: 'success', pos: 'bottom-center' })
    }).fail(response => {
        if (response.status === 401) {
            UIkit.notification('Please sign in to rate', { status: 'warning', pos: 'bottom-center' })
        }
    })
})

function updateRatingButtonsClass(currentRate, activeClass) {
    for (let i = 1; i <= currentRate; i++) {
        $(`[data-value="${i}"]`).removeClass('clear-svg-temp')
        $(`[data-value="${i}"]`).addClass(activeClass)
    }
    for (let i = currentRate + 1; i <= 5; i++) {
        $(`[data-value="${i}"]`).removeClass(activeClass)
        $(`[data-value="${i}"]`).addClass('clear-svg-temp')
    }
}

function createCommentClone(authorName, createTime, text) {
    const commentClone = $('#comment-template').clone()
    commentClone.find('#text').text(text)
    commentClone.find('#create-time').text(createTime)
    commentClone.find('#profile-link').text(authorName)
    commentClone.find('#profile-link').attr('href', `/Profile/${authorName}`)
    return commentClone
}