$('#default-form button[type="submit"]').on('click', function () {
    if ($('#default-form').valid()) {
        $(this).prop('disabled', true)
        $(this).find('#button-text').hide()
        $(this).find('#button-spinner').show()
        $('#default-form').trigger('submit')
    }
})