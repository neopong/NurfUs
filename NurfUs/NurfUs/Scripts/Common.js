$(document).ready(function () {
    if (!String.prototype.supplant) {
        String.prototype.supplant = function (o) {
            return this.replace(/{([^{}]*)}/g,
                function (a, b) {
                    var r = o[b];
                    return typeof r === 'string' || typeof r === 'number' ? htmlEncode(r) : htmlEncode(a);
                }
            );
        };
    }

});

function htmlEncode(message) {
    return $('<div />').text(message).html();
}

function describeTime(milliseconds) {
    milliseconds = milliseconds / 1;

    var totalSeconds = Math.floor(milliseconds / 1000);
    var seconds = totalSeconds % 60;
    var totalMinutes = Math.floor(totalSeconds / 60);
    var minutes = totalMinutes % 60;
    var hours = Math.floor(totalMinutes / 60);

    var timeDisplay = "";

    if (hours <= 10) {
        var pad = "00";

        var timePart;

        if (hours > 0) {
            timePart = "" + hours;
            timeDisplay = pad.substring(0, pad.length - timePart.length) + timePart + ":";
        }

        timePart = "" + minutes;
        timeDisplay += pad.substring(0, pad.length - timePart.length) + timePart + ":";

        timePart = "" + seconds;
        timeDisplay += pad.substring(0, pad.length - timePart.length) + timePart;
    } else {
        timeDisplay = "--**Waiting for new match**--";
    }

    return timeDisplay;
}

var pageTimer = setInterval(function () { myTimer() }, 1000);

function myTimer() {
    $(".timer").each(function (index, element) {
        var milliseconds = element.getAttribute("data-x-started") / 1;

        $(element).text(describeTime(milliseconds));

        element.setAttribute("data-x-started", milliseconds + 1000);
    });
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}