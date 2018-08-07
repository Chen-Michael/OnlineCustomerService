$(function () {
    $(document).on("submit", "#login", function (event) {
        event.preventDefault();
        $("#login-button").prop("disabled", true);

        let data = JSON.stringify({ "Account": $("#account").val(), "Password": $("#password").val() });

        ajax("/Admin/Login", "post", data, done, fail);
    });
});

function done(data) {
    if (data.success) {
        location.href = "service.html?token=" + data.result;
    } else {
        alert("登入失敗");
        $("#login-button").prop("disabled", false);
    }
}

function fail(data) {
    alert("系統出錯");
    $("#login-button").prop("disabled", false);
}