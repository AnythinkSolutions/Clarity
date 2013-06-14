function confirmAction(header, body, continueAction, cancelAction) {
    $dialog = $('#confirmModal');
    $header = $('#confirmHeader');
    $body = $('#confirmBody');
    $cancel = $('#confirmCancelButton');
    $continue = $('#confirmContinueButton');

    $header.text(header);
    $body.text(body);

    $continue.click(function () {
        if (continueAction) continueAction();
        $dialog.modal('hide');
    });
    $cancel.click(function () {
        if (cancelAction) cancelAction();
        $dialog.modal('hide');
    });

    $dialog.modal('show');
}