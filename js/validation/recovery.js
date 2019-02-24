function clearMessage() {
    document.getElementById("recover-password-error").innerHTML = "";
}

function changeInputType() {
    var e = document.getElementById("usertype");
    var usertype = e.options[e.selectedIndex].value;

    var placeholder = "Email Address/Username";
    if (usertype == "admin") {
        placeholder = "Username";
    }
    else if (usertype == "teacher") {
        placeholder = "Email Address";
    }
    document.getElementById("inputEmail").placeholder = placeholder;
}

function recoverPassword() {
    var e = document.getElementById("usertype");
    var usertype = e.options[e.selectedIndex].value;
    var email = document.getElementById("inputEmail").value;

    if (usertype.length == 0 || email.length == 0) {
        document.getElementById("recover-password-error").innerHTML = "Please fill up the recovery information.";
    }
    else {
        document.getElementById("recover-password-error").innerHTML = "Checking Validity...";
        $.ajax({
            type: 'GET',
            url: "../Home/RecoverPassword",
            dataType: 'json',
            data: 'usertype=' + encodeURIComponent(usertype) + '&email=' + encodeURIComponent(email),
            success: function (data) {
                debugger;
                document.getElementById("recover-password-error").innerHTML = data;
            },
            error: function (ex) {
                var r = jQuery.parseJSON(response.responseText);
                alert("Message: " + r.Message);
                alert("StackTrace: " + r.StackTrace);
                alert("ExceptionType: " + r.ExceptionType);
                document.getElementById("recover-password-error").innerHTML = r.Message;
            }
        });
    }
}