$('#save-review-button').on('click', async function (e) {
    e.preventDefault()
    if ($('#edit-review-form').valid()) {
        $(this).prop('disabled', true)
        $(this).find('#button-text').hide()
        $(this).find('#button-spinner').show()
        return await saveReview()
    }
})

$('.review-field').on('input', function () {
    $(`[name="Review.${$(this).data('field')}"]`).val($(this).val())
})

async function saveReview() {
    return await $.post('/SaveReview', getData(), function (data) {
        $('[name="Review.Id"]').val(data.id)
        $('[name="Review.AuthorId"]').val(data.authorId)
        //@ts-ignore
        UIkit.modal($('#successful-save-modal')).show()
    }).always(function () {
        hideSpinner()
        $('.validation-summary-errors ul').empty()
    })
}

function hideSpinner() {
    $('#save-review-button').prop('disabled', false)
    $('#save-review-button').find('#button-text').show()
    $('#save-review-button').find('#button-spinner').hide()
}

function getData(): any {
    return {
        title: $('[name="Review.Title"]').val(),
        leisure: $('[name="Review.Leisure"]').val(),
        group: $('[name="Review.Group"]').val(),
        authorRate: $('[name="Review.AuthorRate"]').val(),
        content: $('[name="Review.Content"]').val(),
        authorId: $('[name="Review.AuthorId"]').val(),
        id: $('[name="Review.Id"]').val(),
        createTime: $('[name="Review.CreateTime"]').val(),
    }
}