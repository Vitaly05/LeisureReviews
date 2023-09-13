$('#tags-cloud-button').on('click', function (e) {
    e.preventDefault()
    $('#tags-cloud').empty()
    showSpinner()
    UIkit.modal('#tags-cloud-modal').show().then(async function () {
        showTagsCloud(await $.get('/Tag/GetTagsWeights'))
        hideSpinner()
    })
})

function showSpinner() {
    $('#tags-cloud-modal #tags-cloud').hide()
    $('#tags-cloud-modal #spinner').show()
}

function hideSpinner() {
    $('#tags-cloud-modal #spinner').hide()
    $('#tags-cloud-modal #tags-cloud').show()
}

function showTagsCloud(data) {
    $('#tags-cloud').jQCloud(data, {
        width: $('#tags-cloud').width(),
        height: $('#tags-cloud').height(),
        delayedMode: true,
    })
}