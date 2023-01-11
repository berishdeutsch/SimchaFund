$(() => {
    $("#new-contributor").on('click', function () {
        $(".new-contrib").modal()
    })

    $(".deposit-button").on('click', function () {
        $('[name="contributorId"]').val($(this).data('contribid'))
        $(".deposit").modal()
    })

    $(".edit-contrib").on('click', function () {
        $("#contributor_id").val($(this).data('id'))
        $("#contributor_first_name").val($(this).data('first-name'))
        $("#contributor_last_name").val($(this).data('last-name'))
        $("#contributor_cell").val($(this).data('cell'))
        $("#initialDepositDiv").remove()
        $("#contributor_date").val($(this).data('date'))
        $("#contributor_always_include").prop('checked', $(this).data('alwaysInclude') === "True")
        $("#add-update-contributor").attr('action', '/contributors/update')
        $(".new-contrib").modal()
    })
})