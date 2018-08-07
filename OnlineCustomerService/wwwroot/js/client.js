let messageList;
let messageInput;
let sendButton;
let quitButton;
let socket;

let scheme = document.location.protocol == "https:" ? "wss" : "ws";
let url = document.location.hostname == "" ? "localhost" : document.location.hostname;
let port = document.location.port ? (":" + document.location.port) : "";
let query = window.location.search;

function addMessage(message) {
    messageList.append('<li class="list-group-item">' + message + '</li>');
    messageList.parent().scrollTop(messageList.parent().height());
}

function connection() {
    socket = new WebSocket(scheme + "://" + url + port + "/client" + query);

    socket.onopen = function (event) {
        addMessage("系統：歡迎使用線上客服系統，請等待客服人員處理您的需求。");
    };

    socket.onclose = function (event) {
        addMessage("系統：對話已結束，感謝您的使用。");
        sendButton.prop("disabled", true);
    };

    socket.onerror = function (event) {
        addMessage("系統：出現錯誤，請重新使用此系統。");
    };

    socket.onmessage = function (event) {
        try {
            let d = JSON.parse(event.data);
            if (d.Start) {
                sendButton.prop("disabled", false);
                addMessage("系統：客服人員已可處理您的需求。");
                return;
            } else if (d.End) {
                sendButton.prop("disabled", true);
                addMessage("系統：客服人員已離開。");
                socket.close();
                return;
            }
        } catch (e) {
            
        }
        
        addMessage("客服人員：" + event.data);
    };  
}

$(function () {
    messageList = $("#message-list");
    messageInput = $("#message-input");
    sendButton = $("#send-button");
    quitButton = $("#quit-button");

    sendButton.click(function () {
        if (!$(this).prop("disabled")) {
            socket.send(messageInput.val());
            addMessage("您：" + messageInput.val());
            messageInput.val("");
        }
    });

    messageInput.keypress(function (e) {
        let code = (e.keyCode ? e.keyCode : e.which);

        if (code == 13) {
            sendButton.click();
        }
    });

    quitButton.click(function () {
        if (confirm("確定要離開嗎？")) {
            location.href = "/";
        }
    });

    connection();
});