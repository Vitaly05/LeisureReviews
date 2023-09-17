$('.change-status-button').on('click', async function (e) {
    e.preventDefault()
    if (await modalConfirm(`Are you sure you want to ${$(this).data('action')} this user?`)) {
        await changeStatusAsync($(this), $(this).closest('.user-card'))
    }
})

async function modalConfirm(message) {
    return await UIkit.modal.confirm(message).then(() => true, () => false)
}

async function changeStatusAsync(button, card) {
    await $.ajax('/Account/ChangeStatus', {
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(getChangeStatusData(button))
    }).fail(function () {
        UIkit.notification('An error occurred when changing the user status', { status: 'danger', pos: 'bottom-center' })
    }).done(function (result) {
        result === "Deleted" ? deleteUser(card) : showStatus(card, result)
    })
}

function getChangeStatusData(button) {
    return {
        userName: button.closest('.user-card').data('user-name').toString(),
        status: button.data('status')
    }
}

function deleteUser(card) {
    card.fadeOut('slow')
    UIkit.notification('User successfully deleted', { status: 'success', pos: 'bottom-center' })
}

function showStatus(card, status) {
    card.find('#status').text(status)
    UIkit.notification('User status successfully changes', { status: 'success', pos: 'bottom-center' })
}