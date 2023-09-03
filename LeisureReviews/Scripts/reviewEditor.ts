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

//@ts-ignore
const tagsInput = new Tokenfield({
    el: document.querySelector('#tags-input'),
    items: getTagsInputItems('#used-tags'),
    setItems: getTagsInputItems('#tags-input'),
    minChars: 0,
    delimiters: [',']
})

function getTagsInputItems(selector: string): Array<object> {
    const tagsInputValues: Array<string> = ($(selector).val() as string).split(',')
    return tagsInputValues.every(v => v.length == 0) ? [] : tagsInputValues.map((v, i) => ({ id: i++, name: v }))
}

async function saveReview() {
    return await $.ajax({
        url: '/Review/Save',
        type: 'POST',
        data: getData(),
        processData: false,
        contentType: false
    }).always(function () {
        hideSpinner()
        $('.validation-summary-errors ul').empty()
    }).done(function (data) {
        $('[name="Review.Id"]').val(data.id)
        $('[name="Review.AuthorId"]').val(data.authorId)
        //@ts-ignore
        UIkit.modal($('#successful-save-modal')).show()
    })
}

function hideSpinner() {
    $('#save-review-button').prop('disabled', false)
    $('#save-review-button').find('#button-text').show()
    $('#save-review-button').find('#button-spinner').hide()
}

function getData(): any {
    var formData = new FormData()
    formData.append('title', $('[name="Review.Title"]').val() as string)
    formData.append('leisure', $('[name="Review.Leisure"]').val() as string)
    formData.append('group', $('[name="Review.Group"]').val() as string)
    formData.append('authorRate', $('[name="Review.AuthorRate"]').val() as string)
    formData.append('content', $('[name="Review.Content"]').val() as string)
    formData.append('authorId', $('[name="Review.AuthorId"]').val() as string)
    const tagsNames: Array<string> = tagsInput.getItems().map(i => i.name)
    tagsNames.length === 0 || tagsNames.forEach(t => formData.append('tagsNames[]', t))
    formData.append('id', $('[name="Review.Id"]').val() as string)
    formData.append('createTime',$('[name="Review.CreateTime"]').val() as string)
    formData.append('illustration', $('#illustration-file-input').prop('files')[0])
    return formData
}