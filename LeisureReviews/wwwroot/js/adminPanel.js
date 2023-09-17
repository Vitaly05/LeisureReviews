$('.change-status-button').on('click', async function (e) {
    e.preventDefault()
    if (await modalConfirm(`Are you sure you want to ${$(this).data('action')} this user?`)) {
        await changeStatusAsync($(this), $(this).closest('.user-card'))
    }
})

$('.change-role-button').on('click', async function (e) {
    e.preventDefault()
    if (await modalConfirm(`Are you sure you want to add to this user role "${$(this).data('role-name')}"?`)) {
        await changeRoleAsync($(this), $(this).closest('.user-card'))
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

async function changeRoleAsync(button, card) {
    await $.ajax('/Account/ChangeRole', {
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(getChangeRoleData(button))
    }).fail(function () {
        UIkit.notification('An error occurred when changing the user role', { status: 'danger', pos: 'bottom-center' })
    }).done(function (result) {
        card.find('#role').text(result)
        UIkit.notification('User role successfully changed', { status: 'success', pos: 'bottom-center' })
    })
}

function getChangeStatusData(button) {
    return {
        userName: getUserName(button),
        status: button.data('status')
    }
}

function getChangeRoleData(button) {
    return {
        userName: getUserName(button),
        role: button.data('role')
    }
}

function getUserName(button) {
    return button.closest('.user-card').data('user-name').toString()
}

function deleteUser(card) {
    card.fadeOut('slow')
    UIkit.notification('User successfully deleted', { status: 'success', pos: 'bottom-center' })
}

function showStatus(card, status) {
    card.find('#status').text(status)
    UIkit.notification('User status successfully changed', { status: 'success', pos: 'bottom-center' })
}