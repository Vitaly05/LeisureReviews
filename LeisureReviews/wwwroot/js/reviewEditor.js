const converter = new showdown.Converter()

$('#markdown-preview').html(converter.makeHtml(getReviewContent()))

$('#markdown-input').on('input', function () {
    $('#markdown-preview').html(converter.makeHtml(getReviewContent()))
    $(`[name="Review.Content"]`).val(getReviewContent())
})

$('#save-review-button').on('click', async function (e) {
    e.preventDefault()
    if ($('#edit-review-form').valid()) {
        showButtonSpinner($(this))
        await saveReview()
    }
})

$('.review-field').on('input', function () {
    $(`[name="Review.${$(this).data('field')}"]`).val($(this).val())
})

$(`[name="Review.Group"]`).val($('.review-field[data-field="Group"]').val())

const tagsInput = new Tokenfield({
    el: document.querySelector('#tags-input'),
    items: getTagsInputItems('#used-tags'),
    setItems: getTagsInputItems('#tags-input'),
    minChars: 0,
    delimiters: [',']
})

var illustrationFile

var illustratoinChanged = false

$('.get-image').each(async function () {
    const fileId = $(this).data('fileId')
    if (fileId.length === 0) return
    const self = this
    $('#illustraion-spinner').show()
    $('#upload-illustration-panel').hide()
    await $.get(`/Review/GetIllustration?fileId=${fileId}`).always(function () {
        $('#illustraion-spinner').hide()
    }).fail(function () {
        $('#upload-illustration-panel').show()
    }).done(function (file) {
        $('#illustration-block').show()
        $(self).attr('src', file)
    })
})

$('#delete-illustration-button').on('click', function (e) {
    e.preventDefault()
    illustrationFile = null
    illustratoinChanged = true
    $('#illustration-block').hide()
    $('#upload-illustration-panel').show()
})

$('#upload-illustration-panel').on('dragover', function (e) {
    e.preventDefault()
    e.stopPropagation()
    $(this).addClass('uk-dragover')
})

$('#upload-illustration-panel').on('dragleave', function (e) {
    e.preventDefault()
    e.stopPropagation()
    $(this).removeClass('uk-dragover')
})

$('#upload-illustration-panel').on('drop', function (e) {
    $(this).removeClass('uk-dragover')
    setImage(e.originalEvent.dataTransfer.files[0], e)
})

$('#illustration-file-input').on('change', function (e) {
    setImage($(this).prop('files')[0], e)
})

function setImage(image, e) {
    e.preventDefault()
    e.stopPropagation()
    if (image.type.startsWith('image/')) {
        illustrationFile = image
        illustratoinChanged = true
        showImage(image)
    }
    else {
        $('#upload-illustration-panel').addClass('uk-animation-shake')
        setTimeout(() => $('#upload-illustration-panel').removeClass('uk-animation-shake'), 1000)
        UIkit.notification('Please select an image', { status: 'warning', pos: 'bottom-center' })
    }
}

function showImage(image) {
    const reader = new FileReader()
    reader.addEventListener('load', function () {
        $('#illustration-image').attr('src', reader.result)
        $('#illustration-block').show()
        $('#upload-illustration-panel').hide()
    })
    reader.readAsDataURL(image)
}

function getTagsInputItems(selector) {
    const tagsInputValues= $(selector).val().split(',')
    return tagsInputValues.every(v => v.length == 0) ? [] : tagsInputValues.map((v, i) => ({ id: i++, name: v }))
}

async function saveReview() {
    await $.ajax({
        url: '/Review/Save',
        type: 'POST',
        data: getData(),
        processData: false,
        contentType: false
    }).always(function () {
        hideButtonSpinner($('#save-review-button'))
        $('.validation-summary-errors ul').empty()
    }).done(function (data) {
        $('[name="Review.Id"]').val(data.id)
        $('[name="Review.AuthorId"]').val(data.authorId)
        $('#illustration-image').attr('data-file-id', data.illustrationId)
        illustratoinChanged = false
        UIkit.modal($('#successful-save-modal')).show()
    })
}

function getData() {
    var formData = new FormData()
    appendMainReviewInfo(formData)
    appendReviewTags(formData)
    appendIllustrationInfo(formData)
    return formData
}

function appendMainReviewInfo(formData) {
    formData.append('title', $('[name="Review.Title"]').val())
    formData.append('leisure', $('[name="Review.Leisure"]').val())
    formData.append('group', $('[name="Review.Group"]').val())
    formData.append('authorRate', $('[name="Review.AuthorRate"]').val())
    formData.append('content', getReviewContent())
    formData.append('authorId', $('[name="Review.AuthorId"]').val())
    formData.append('id', $('[name="Review.Id"]').val())
    formData.append('createTime', $('[name="Review.CreateTime"]').val())
}

function appendReviewTags(formData) {
    const tagsNames = tagsInput.getItems().map(i => i.name)
    tagsNames.length === 0 || tagsNames.forEach(t => formData.append('tagsNames[]', t))
}

function appendIllustrationInfo(formData) {
    formData.append('illustration', illustrationFile)
    const illustrationId = $('#illustration-image').attr('data-file-id');
    if (illustrationId !== undefined && illustrationId.length !== 0) {
        formData.append('illustrationId', illustrationId);
    }
    formData.append('illustrationChanged', illustratoinChanged)
}

function getReviewContent() {
    return $('#markdown-input').html().replace(/<br>/g, "\n")
}