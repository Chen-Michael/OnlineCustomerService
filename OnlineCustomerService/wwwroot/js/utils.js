let queryString = {};

function ajax(url, method, data, done, fail) {
    $.ajax({
        method: method,
        url: url,
        data: data,
        contentType: "application/json",
        dataType: "json"
    })
    .done(function (msg) {
        if (typeof done == 'function') {
            done(msg);
        }
    })
    .fail(function (msg) {
        if (typeof fail == 'function') {
            fail(msg);
        }
    });
}

let p = window.location.search.substring(1).split("&");

for (let i = 0; i < p.length; i++) {
    let v = p[i].split("=");
    queryString[v[0]] = v[1];
}