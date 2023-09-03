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