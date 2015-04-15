var betId = 0;
var startSize = 300;
var currentSize = startSize;
var startCoundDown = 3;
var countDown = startCoundDown;
var countdownTimer;
var pauseTime = 7000;
var timeLeftInPause = pauseTime;
var timerInterval = 10;
var staticTextSize = 100;

$(document).ready(function () {
    var hub = $.connection.nurfUsHub;
    var authenticated = false;

    if (getCookie("clientName").length >= 3)
    {
        authenticated = true;
        toggleBet(true);
    }
    else {
        toggleBet(false);
    }

    $('#betAmount').spinedit({
        minimum: 1,
        maximum: 1000000000,
        step: 100,
        value: 1000,
        numberOfDecimals: 0
    });

    function countdown() {
        timeLeftInPause = pauseTime;
        if (countdownTimer > 0) {
            clearInterval(countdownTimer);
        }

        countdownTimer = setInterval(function () { countdownElapsed() }, timerInterval);
    }

    function countdownElapsed() {
        timeLeftInPause -= timerInterval;

        if (Modernizr.mq('(min-width: 800px)')) {
            startSize = 300;
            staticTextSize = 100;
        } else {
            startSize = 100;
            staticTextSize = 24;
        }

        var fontSizeStep = startSize/100;

        if (timeLeftInPause > pauseTime - 1000) {
            $("#countDown").attr("style", "z-index: 0; font-size: " + staticTextSize + "pt; position: fixed; top: 20%; left: 25%; font-weight: bold; color: red; ");
            $("#countDown").text("Get Ready...");
        }
        else if (timeLeftInPause > startCoundDown * 1000) {
            $("#countDown").attr("style", "z-index: 0; font-size: " + staticTextSize + "pt; position: fixed; top: 20%; left: 18%; font-weight: bold; color: red; ");
            $("#countDown").text("Round Starting");
        }
        else {
            if (currentSize <= 0) {
                currentSize = startSize;
                if (countDown > 0) {
                    countDown--;
                }
                else {
                    countDown = startCoundDown;
                    $("#countDown").text("");
                    clearInterval(countdownTimer);
                    $("#betArena").show();
                    return;
                }
            }
            else {
                currentSize -= fontSizeStep;
            }

            if (countDown == 0) {
                $("#countDown").attr("style", "z-index: 0; font-size: " + startSize + "pt; position: fixed; top: 40%; left: 8%; font-weight: bold; color: red; ");
                $("#countDown").text("URF!!!");
            }
            else {
                $("#countDown").attr("style", "z-index: 0; font-size: " + currentSize + "pt; position: fixed; top: 40%; left: 45%; font-weight: bold; color: red; ");
                $("#countDown").text(countDown);
            }
        }
    }

    var timeLeftTemplate = '<div class="badge timer" data-x-started="{SecondsElapsed}"></div>';

    hub.client.newMatch = function (gameDisplay) {
        betId = 0;
        if (getCookie("clientName").length >= 3)
        {
            $("#betArena").hide();
            countdown();
        }

        $(".checkShow").removeClass("checkShow");

        new Audio("/Audio/UrfIntro.mp3").play();

        $("#gameDetail").empty().append(timeLeftTemplate.supplant(gameDisplay));
        $("#betQuestion").empty().append(gameDisplay.BetQuestion);

        var checkMark = '<div class="checkMark"><img src="/Images/Checkmark.png" /></div>';
        var preImage = '<img src="/Images/Champion/';

        if (gameDisplay.BetType == 1) {
            preImage = checkMark + preImage;
        }

        $("#participant1").empty().append(preImage + gameDisplay.BlueTeam[0].ChampionImage + '"\>');
        $("#participant2").empty().append(preImage + gameDisplay.BlueTeam[1].ChampionImage + '"\>');
        $("#participant3").empty().append(preImage + gameDisplay.BlueTeam[2].ChampionImage + '"\>');
        $("#participant4").empty().append(preImage + gameDisplay.BlueTeam[3].ChampionImage + '"\>');
        $("#participant5").empty().append(preImage + gameDisplay.BlueTeam[4].ChampionImage + '"\>');
        $("#participant6").empty().append(preImage + gameDisplay.PurpleTeam[0].ChampionImage + '"\>');
        $("#participant7").empty().append(preImage + gameDisplay.PurpleTeam[1].ChampionImage + '"\>');
        $("#participant8").empty().append(preImage + gameDisplay.PurpleTeam[2].ChampionImage + '"\>');
        $("#participant9").empty().append(preImage + gameDisplay.PurpleTeam[3].ChampionImage + '"\>');
        $("#participant10").empty().append(preImage + gameDisplay.PurpleTeam[4].ChampionImage + '"\>');

        $("#participant1").attr("data-x-betId", gameDisplay.BlueTeam[0].ParticipantId)
        $("#participant2").attr("data-x-betId", gameDisplay.BlueTeam[1].ParticipantId)
        $("#participant3").attr("data-x-betId", gameDisplay.BlueTeam[2].ParticipantId)
        $("#participant4").attr("data-x-betId", gameDisplay.BlueTeam[3].ParticipantId)
        $("#participant5").attr("data-x-betId", gameDisplay.BlueTeam[4].ParticipantId)
        $("#participant6").attr("data-x-betId", gameDisplay.PurpleTeam[0].ParticipantId)
        $("#participant7").attr("data-x-betId", gameDisplay.PurpleTeam[1].ParticipantId)
        $("#participant8").attr("data-x-betId", gameDisplay.PurpleTeam[2].ParticipantId)
        $("#participant9").attr("data-x-betId", gameDisplay.PurpleTeam[3].ParticipantId)
        $("#participant10").attr("data-x-betId", gameDisplay.PurpleTeam[4].ParticipantId)

        if (gameDisplay.BetType == 0) {
            $(".blueTeam, .purpleTeam, .summoner, .selectable").off();
            $(".summoner").removeClass("selectable");
            $(".blueTeam, .purpleTeam").addClass("selectable");
        }
        else {
            $(".blueTeam, .purpleTeam, .summoner, .selectable").off();
            $(".blueTeam, .purpleTeam").removeClass("selectable");
            $(".summoner").addClass("selectable");
        }

        $(".selectable").click(function () {
            betSelected(this);
        });

    };

    hub.client.userResponse = function (newClient) {
        if (newClient.Valid) {
            setCookie("clientName", newClient.Name, 365);
            setCookie("clientKey", newClient.Key, 365);
            authenticated = true;

            toggleBet(true);
        }
        else {
            $("#guestName").val('');
            alert(newClient.Message);
        }
    };

    hub.client.broadcastMessage = function (name, message) {
        // Html encode display name and message. 
        var encodedName = htmlEncode(name);
        var encodedMsg = htmlEncode(message);
        // Add the message to the page. 
        $('#discussion').prepend('<li><strong><div class="badge">' + encodedName
            + '</div></strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };

    hub.client.applause = function () {
        new Audio("/Audio/Applause.mp3").play();
    };

    hub.client.fart = function () {
        new Audio("/Audio/Fart.mp3").play();
    };

    hub.client.displayCurrency = function (amount) {
        $('#currentCurrency').text(amount);
    };

    //This is where we will poll the server to add the bet info.
    function betSelected(element) {
        $(".checkShow").removeClass("checkShow");
        if (betId != $(element).attr("data-x-betId")) {
            betId = $(element).attr("data-x-betId");
            $(element).children(".checkMark").addClass("checkShow");
            //--------------------- 
            hub.server.addUserBet(getCookie("clientKey"), $("#betAmount").val(), betId);
        }
        else {
            betId = 0;
            hub.server.removeUserBet(getCookie("clientKey"));
        }
    }

    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();

            if (authenticated)
            {
                $('#sendmessage').click();
            }
            else {
                hub.server.newGuest($("#guestName").val());
            }
            return false;
        }
    });

    $.connection.hub.start().done(function () {
        hub.server.getCurrentMatch();

        $('#applause').click(function () {
            hub.server.applause(getCookie("clientName"), getCookie("clientKey"));
        });

        $('#fart').click(function () {
            hub.server.fart(getCookie("clientName"), getCookie("clientKey"));
        });

        $('#sendmessage').click(function () {
            // Call the Send method on the hub. 
            hub.server.send(getCookie("clientName"), getCookie("clientKey"), $('#message').val()).done(function (result) {
                if (result)
                {
                    // Clear text box and reset focus for next comment. 
                    $('#message').val('').focus();
                }
                else {
                    authenticated = false;
                    setCookie("clientName", "", -365);
                    setCookie("clientKey", "", -365);

                    toggleBet(false);
                }
            });
        });
    });
});

function toggleBet(authenticate)
{
    if (authenticate)
    {
        $("#guestEntry").hide();
        $("#howtoplay").hide();
        $("#head").show();
        $("#betArena").show();
        // Set initial focus to message input box.  
        $('#message').focus();
    }
    else {
        $("#betArena").hide();
        $("#guestEntry").show();

        if (getCookie("clientName").length >= 3)
        {
            // Set initial focus to message input box.  
            $('#guestName').focus();
        }
        else {
            $("#head").hide();
            $("#howtoplay").show();
            $("#playWizard").show();
        }
    }
}