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
    socket = new WebSocket(scheme + "://" + url + port + "/service" + query);

    socket.onopen = function (event) {
        addMessage("系統：歡迎登入線上客服系統，請等待用戶請求。");
    };

    socket.onclose = function (event) {
        addMessage("系統：對話已結束，感謝您的使用。");
    };

    socket.onerror = function (event) {
        addMessage("系統：出現錯誤，請重新使用此系統。");
    };

    socket.onmessage = function (event) {
        try {
            let d = JSON.parse(event.data);
            if (d.websocketId) {
                sendButton.prop("disabled", false);
                addMessage("系統：用戶已連接，相關資料如下。");
                addMessage("&emsp;姓名：" + d.name);
                addMessage("&emsp;信箱：" + d.mail);
                addMessage("&emsp;電話：" + d.phone);
                addMessage("&emsp;問題：" + d.question);
                return;
            } else if (d.End) {
                sendButton.prop("disabled", true);
                addMessage("系統：用戶已離開，十秒後等待下個用戶請求。");
                return;
            }
        } catch (e) {
            
        }
        
        addMessage("用戶：" + event.data);
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
        if (confirm("確定要結束對話嗎？")) {
            sendButton.prop("disabled", true);
            socket.send('{"End": true}');
        }
    });

    connection();
});