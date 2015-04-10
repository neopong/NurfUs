$(document).ready(function () {
    var hub = $.connection.nurfUsHub;
    var authenticated = false;

    if (getCookie("clientName").length >= 3)
    {
        authenticated = true;
    }

    var timeLeftTemplate = '<div class="badge timer" data-x-started="{SecondsElapsed}"></div>';

    hub.client.newMatch = function (gameDisplay) {
        $("#gameDetail").empty().append(timeLeftTemplate.supplant(gameDisplay));
        $("#participant1").empty().append('<img src="/Images/Champion/' + gameDisplay.BlueTeam[0].ChampionImage + '"\>');
        $("#participant2").empty().append('<img src="/Images/Champion/' + gameDisplay.BlueTeam[1].ChampionImage + '"\>');
        $("#participant3").empty().append('<img src="/Images/Champion/' + gameDisplay.BlueTeam[2].ChampionImage + '"\>');
        $("#participant4").empty().append('<img src="/Images/Champion/' + gameDisplay.BlueTeam[3].ChampionImage + '"\>');
        $("#participant5").empty().append('<img src="/Images/Champion/' + gameDisplay.BlueTeam[4].ChampionImage + '"\>');
        $("#participant6").empty().append('<img src="/Images/Champion/' + gameDisplay.PurpleTeam[0].ChampionImage + '"\>');
        $("#participant7").empty().append('<img src="/Images/Champion/' + gameDisplay.PurpleTeam[1].ChampionImage + '"\>');
        $("#participant8").empty().append('<img src="/Images/Champion/' + gameDisplay.PurpleTeam[2].ChampionImage + '"\>');
        $("#participant9").empty().append('<img src="/Images/Champion/' + gameDisplay.PurpleTeam[3].ChampionImage + '"\>');
        $("#participant10").empty().append('<img src="/Images/Champion/' + gameDisplay.PurpleTeam[4].ChampionImage + '"\>');

        if (gameDisplay.BetType == 0) {
            $(".blueTeam").off();
            $(".purpleTeam").off();

            $(".blueTeam").click(function () {
                alert("Blue team go!");
            });

            $(".purpleTeam").click(function () {
                alert("Purple team go!");
            });
        }
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
        $('#discussion').prepend('<li><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };

    if (authenticated)
    {
        toggleBet(true);
    }
    else {
        toggleBet(false);
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
        $("#betArena").show();
        // Set initial focus to message input box.  
        $('#message').focus();
    }
    else {
        $("#guestEntry").show();
        $("#betArena").hide();
        // Set initial focus to message input box.  
        $('#guestName').focus();
    }
}