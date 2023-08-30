$('.delete-review-button').on('click', async function () {
    var currentButton = $(this)
    let reviewId = $(this).parents('.review-card').data('id')
    //@ts-ignore
    return await UIkit.modal.confirm('Are you sure you want to delete the review?').then(async function () {
        showButtonSpinner(currentButton)
        return await $.ajax(`/Review/Delete?reviewId=${reviewId}`, {
            method: 'DELETE'
        }).done(function (reviewId) {
            $(document).find(`.review-card[data-id="${reviewId}"]`).fadeOut('slow')
            //@ts-ignore
            UIkit.notification({ message: 'The review was successfully deleted', status: 'success', pos: 'bottom-center' })
        }).fail(function () {
            //@ts-ignore
            UIkit.notification({ message: 'An error occurred during review deleiton', status: 'danger', pos: 'bottom-center' })
        }).always(function () {
            hideButtonSpinner(currentButton)
        })
    })
})

function showButtonSpinner(button: JQuery) {
    button.prop('disabled', true)
    button.find('#button-text').hide()
    button.find('#button-spinner').show()
}

function hideButtonSpinner(button: JQuery) {
    button.prop('disabled', false)
    button.find('#button-text').show()
    button.find('#button-spinner').hide()
}