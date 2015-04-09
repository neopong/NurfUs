$(document).ready(function () {
    var hub = $.connection.nurfUsHub;

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
    };

    hub.client.broadcastMessage = function (name, message) {
        // Html encode display name and message. 
        var encodedName = htmlEncode(name);
        var encodedMsg = htmlEncode(message);
        // Add the message to the page. 
        $('#discussion').prepend('<li><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };
    // Get the user name and store it to prepend to messages.
    $('#displayname').val("Guest( " + prompt('Enter your name:', '') + " )");
    // Set initial focus to message input box.  
    $('#message').focus();

    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            $('#sendmessage').click();
            return false;
        }
    });

    $.connection.hub.start().done(function () {
        hub.server.getCurrentMatch();

        $('#sendmessage').click(function () {
            // Call the Send method on the hub. 
            hub.server.send($('#displayname').val(), $('#message').val());
            // Clear text box and reset focus for next comment. 
            $('#message').val('').focus();
        });
    });
});