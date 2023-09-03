$('.delete-review-button').on('click', async function () {
    const currentButton = $(this)
    const reviewId = $(this).parents('.review-card').data('id')
    await UIkit.modal.confirm('Are you sure you want to delete the review?').then(async function () {
        showButtonSpinner(currentButton)
        return await $.ajax(`/Review/Delete?reviewId=${reviewId}`, {
            method: 'DELETE'
        }).done(function (reviewId) {
            $(document).find(`.review-card[data-id="${reviewId}"]`).fadeOut('slow')
            UIkit.notification({ message: 'The review was successfully deleted', status: 'success', pos: 'bottom-center' })
        }).fail(function () {
            UIkit.notification({ message: 'An error occurred during review deleiton', status: 'danger', pos: 'bottom-center' })
        }).always(function () {
            hideButtonSpinner(currentButton)
        })
    })
})