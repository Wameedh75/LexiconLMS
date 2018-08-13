$(function () {
    // Reference the auto-generated proxy for the hub.
    var chat = $.connection.chatHub;
    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (name, message) {
        // Add the message to the page.
        var today = new Date();
        var mm = today.getMonth() + 1;
        var dd = today.getDate();
        var yyyy = today.getFullYear();
        if (mm < 10) {
            mm = '0' + mm;
        }
        if (dd < 10) {
            dd = '0' + dd;
        }
        today = dd + '/' + mm + '/' + yyyy;
        $('.main').append('<div class="container">' + '<p>' + name + ' : ' + message + '</p>' + '<span class="time-right">' + today + '</span>');
    };
    // send function
    var send = function () {
        //prevent sending blank message
        if ($('#message').val() == '') {
            alert('you cannot send blank message');
        }
        else {
            // Call the Send method on the hub.
            chat.server.send($('#message').val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        }

    };
    // Set initial focus to message input box.
    $('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            send();
        });
        $('#message').keyup(function (event) {
            if (event.keyCode === 13) {
                send();
            }
        });
    });
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<p>').text(value).html();
    return encodedValue;
}
