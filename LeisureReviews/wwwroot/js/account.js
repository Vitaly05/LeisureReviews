$('#default-form button[type="submit"]').on('click', function () {
    if ($('#default-form').valid()) {
        showButtonSpinner($(this))
        $('#default-form').trigger('submit')
    }
})