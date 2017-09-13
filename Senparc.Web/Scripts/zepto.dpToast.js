/*
	DP Toast jQuery Plugin, Version 1.1
	Copyright (C) Dustin Poissant 2014
	See http://htmlpreview.github.io/?https://github.com/dustinpoissant/jquery.dpToast/blob/master/License.html
	for more information reguarding usage.
*/
; function dpToast() {
    var container;
    if ($("#dp-toasts").length < 1) {
        $("body").append("<div id='dp-toasts'></div>");
        container = $("#dp-toasts");
        container[0].count = 0;
        container.css({
            position: "fixed",
            display: "inline-block",
            bottom: "50px",
            left: "0px",
            width: "100%",
            textAlign: "center",
            margin: "0 auto",
            zIndex: '100000'
        });
    } else {
        container = $("#dp-toasts");
    }
    var toastNumber = container[0].count + 1;
    var message = "Error: No Toast Message";
    var timeout = 3000;
    for (var i = 0; i < arguments.length; i++) {
        if (typeof (arguments[i]) == "string" && arguments[i].length > 0) message = arguments[i];
        else if (typeof (arguments[i]) == "number") timeout = arguments[i];
    }
    container.prepend("<div class='dp-toast dp-toast-" + toastNumber + "'>" + message + "</div><div class='dp-toast-" + toastNumber + "-br'></div>");
    var toast = $(".dp-toast-" + toastNumber);
    toast.css({
        display: "inline-block",
        backgroundColor: "rgba(0,0,0,0.9)",
        color: "white",
        padding: "10px 16px",
        borderRadius: "3px",
        margin: "5px auto",
        boxShadow: "0 0 5px rgba(0,0,0,0.5), 0 0 2px rgba(0,0,0,0.5)"
    });
    toast.show();
    window.setTimeout(function () {
        toast.hide();
        toast.add(".toast-" + toastNumber + "-br").remove();
        if (container.children().size() == 0) container.remove();
    }, timeout);
    container[0].count = toastNumber;
}