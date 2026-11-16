$(document).ready(function () {
    const chatInput = $('#chatInput');
    const chatMessages = $('#chatMessages');
    const sendButton = $('#sendButton');
    const typingIndicator = $('#typingIndicator');

    function appendMessage(sender, text) {
        const msgClass = sender === 'bot' ? 'bot' : 'user';
        chatMessages.append(`<div class="message ${msgClass}">${text}</div>`);
        chatMessages.scrollTop(chatMessages[0].scrollHeight);
    }

    function showTyping(show) {
        typingIndicator.toggle(show);
    }

    sendButton.click(function () {
        const message = chatInput.val();
        if (!message) return;

        appendMessage('user', message);
        chatInput.val('');
        showTyping(true);

        $.ajax({
            url: '/api/chatbot/ask',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ Message: message, UserId: 'guest' }),
            success: function (res) {
                appendMessage('bot', res.response);
            },
            error: function () {
                appendMessage('bot', "Oops! Something went wrong.");
            },
            complete: function () {
                showTyping(false);
            }
        });
    });

    chatInput.keypress(function (e) {
        if (e.which === 13) sendButton.click();
    });
});
