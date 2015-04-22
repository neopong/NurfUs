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
var currentBetAmount = 0;
var submittingName = false;

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

    $('[data-toggle="tooltip"]').tooltip();

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
                    $("#footer").show();
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

    //Used for any generic debug logging you want to dump from the server to the client
    hub.client.serverLog = function(logInfo) {
        if (window.console) {
            window.console.log(logInfo);
        }
    };

    hub.client.newMatch = function (gameDisplay) {
        //Hide the modal if it appears
        $('#resultDialog').modal('hide');

        betId = 0;
        currentBetAmount = 0;

        if (getCookie("clientName").length >= 3)
        {
            $("#betArena").hide();
            $("#footer").hide();

            countdown();
        }

        $("#betAmount").removeAttr("disabled");
        $(".checkShow").removeClass("checkShow");

        playAudio("/Audio/UrfIntro.mp3");

        $("#gameDetail").empty().append(timeLeftTemplate.supplant(gameDisplay));
        $("#betQuestion").empty().append(gameDisplay.BetQuestion);

        var checkMark = '<div class="checkMark"><img src="/Images/Checkmark.png" /></div>';
        var preImage = '<img src="/Images/Champion/';

        $("#payout").empty();

        if (gameDisplay.BetType == 1) {
            preImage = checkMark + preImage;
            $("#payout").addClass("bling");
            $("#payout").append("Payout 5:1");
        } else {
            $("#payout").removeClass("bling");
            $("#payout").append("Payout 1:1");
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

        submittingName = false;
        $("#guestLogin").removeAttr("disabled");
        $("#guestName").removeAttr("disabled");
    };

    /*

    {
        BetType BetType 
        List<int> CorrectAnswerIds
        int UserAnswer 
        int UserBetAmount
        bool UserCorrect
        UserFallsBelowThreshold
    }

    */
    var clearModal = function () {
        $("#choiceList").empty();
        $('#playerChoice').empty();
        $('#moneyOutcome').empty();
        $('#threshHoldMessage').empty();
    }

    hub.client.displayPostGameResult = function (gameResult)
    {
        clearModal();
        //Team Bet Type
        if (gameResult.BetType == 0)
        {
            //blue team
            if (gameResult.CorrectAnswerIds[0] == 100) {
                $("#choiceList").append('<span class="blueText">Blue Team</span>');
            }
            //purple team
            else {
                $("#choiceList").append('<span class="purpleText">Purple Team</span>');
            }

            if(gameResult.UserAnswer == 100)
            {
                $('#playerChoice').append('<span class="blueText">Blue Team</span>');
            }
            else
            {
                $('#playerChoice').append('<span class="purpleText">Purple Team</span>');
            }

        }
        //Summoner Bet Type
        else if (gameResult.BetType == 1)
        {
            var playerChoice = $('[data-x-betId="' + betId + '"]').children('img').first().clone();

            if (betId <= 5) {
                $(playerChoice).addClass("blueBorder");
            } else {
                $(playerChoice).addClass("purpleBorder");
            }

            $('#playerChoice').append(playerChoice);

            for (var i = 0 ; i < gameResult.CorrectAnswerIds.length; i++) {
                var correctChoiceImg = $('[data-x-betId="' + gameResult.CorrectAnswerIds[i] + '"]').children('img').first().clone();

                if (gameResult.CorrectAnswerIds[i] <= 5) {
                    $(correctChoiceImg).addClass("blueBorder");
                } else {
                    $(correctChoiceImg).addClass("purpleBorder");
                }

                $("#choiceList").append(correctChoiceImg);
            }
        }

        //UserAmountDelta
        $('#moneyOutcome').append('<img src="/Images/MoneyFish50x50.png" class="currency" /> ' + gameResult.UserAmountDelta);

        if (gameResult.UserFallsBelowThreshold == true) {
            $('#threshHoldMessage').append('Urf the manatee feels bad for your loss, He gifts you some more fish!! <img src="/Images/MoneyFish50x50.png" class="currency" />');
        }
        $('#resultDialog').modal('show');

        if (gameResult.UserCorrect) {
            playAudio("/Audio/Victory.mp3");
        } else {
            playAudio("/Audio/Defeat.mp3");
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
        playAudio("/Audio/Applause.mp3");
    };

    hub.client.fart = function () {
        playAudio("/Audio/Fart.mp3");
    };

    hub.client.displayCurrency = function (amount) {
        $('#currentCurrency').text(amount);
    };

    //This is where we will poll the server to add the bet info.
    function betSelected(element) {
        var newBetId = $(element).attr("data-x-betId");
        playAudio("/Audio/lockinchampion.mp3");

        if (betId != newBetId) {
            var betAmountDelta = $("#betAmount").val() / 1 - currentBetAmount;

            if (betAmountDelta > getCurrentCurrency()) {
                alert("You don't have enough fish to place this bet. Please adjust the amount of fish you're wagering.");
                $("#betAmount").focus();
            } else {
                hub.server.addUserBet(getCookie("clientKey"), $("#betAmount").val(), newBetId).done(function(result) {
                    if (result) {
                        betId = newBetId;
                        currentBetAmount = $("#betAmount").val() / 1;
                        hub.client.displayCurrency(getCurrentCurrency() - betAmountDelta);
                        $(".checkShow").removeClass("checkShow");
                        $(element).children(".checkMark").addClass("checkShow");
                        $("#betAmount").attr("disabled", "disabled");
                    } else {
                        alert("You don't have enough fish to place this bet. Please adjust the amount of fish you're wagering.");
                        $("#betAmount").focus();
                    }
                });
            }
        }
        else {
            $("#betAmount").removeAttr("disabled");
            betId = 0;
            hub.client.displayCurrency(getCurrentCurrency() + currentBetAmount);
            currentBetAmount = 0;
            $(".checkShow").removeClass("checkShow");
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
                $('#guestLogin').click();
            }
            return false;
        }
    });

    $.connection.hub.start().done(function () {
        hub.server.getCurrentMatch();

        $('#applause').click(function () {
            if (getCurrentCurrency() - 20000 >= currentBetAmount) {
                hub.server.applause(getCookie("clientName"), getCookie("clientKey")).done(function(result) {
                    if (result) {
                        hub.client.displayCurrency(getCurrentCurrency() - 20000);
                    } else {
                        alert("The mighty Urf demands 20K feesh to applaude someone.  He won't even wake up for anything less than 7K feesh!");
                    }
                });
            } else {
                if (betId > 0) {
                    alert("The cost to get Urf to applaude the site (20K) wouldn't leave you with enough to complete your bet. Remove your bet by unchecking your current selection, change your bet amount and then place your bet again.");
                } else {
                    alert("The mighty Urf demands 20K feesh to applaude someone.  He won't even wake up for anything less than 7K feesh!");
                }
            }
        });

        $('#fart').click(function () {
            if (getCurrentCurrency() - 30000 >= currentBetAmount) {
                hub.server.fart(getCookie("clientName"), getCookie("clientKey")).done(function(result) {
                    if (result) {
                        hub.client.displayCurrency(getCurrentCurrency() - 30000);
                    } else {
                        alert("You can't have fish farts if you haven't eaten at least 30K fish!  Win some more and then let em rip!");
                    }
                });
            } else {
                if (betId > 0) {
                    alert("Attempting to fart away this many fish (30K) wouldn't leave you with enough to complete your bet. Remove your bet by unchecking your current selection, change your bet amount and then place your bet again.");
                } else {
                    alert("You can't have fish farts if you haven't eaten at least 30K fish!  Win some more and then let em rip!");
                }
            }
        });

        $('#guestLogin').click(function () {
            if (!submittingName) {
                submittingName = true;
                $("#guestLogin").attr("disabled", "disabled");
                $("#guestName").attr("disabled", "disabled");

                hub.server.newGuest($("#guestName").val());
            }
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
        //$('#message').focus();
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

function playAudio(fileToPlay) {
    if (getCookie("muteAudio").length == 0) {
        var audio = new Audio(fileToPlay);
        audio.volume = .75;
        audio.play();
    }
}

function getCurrentCurrency() {
    return $('#currentCurrency').text() / 1;
}