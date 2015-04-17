# NurfUs
An application devoted to the Riot API challenge and the new NURF mode... sure to be the future of League

## Live Link

[Nurf.Us](http://Nurf.us)

## What is it?

[Nurf.Us](http://Nurf.us) is a League of Legends based betting arena.

The betting arena consists of rounds where the players are presented with a 
question based on actual URF data that was pulled from the new Riot URF endpoint.
Each round gives you 60 seconds to think over the choices and place your bet.
If you guess correctly you will earn fish, but if you guess wrong you will lose the fish you bet.
Once you have won a sufficient amount of fish you can spend them on sound effects in the
global chat and impress your competitors with your incredible wealth.

## Some Technical Stuff

This project is an ASP.NET MVC website that leverages websocket technology through
the [SignalR API](http://signalr.net/). This allows all connections to be
updated in real time without the need to repeatedly send requests to the web server.

The project requires the [.Net framework 4.5.1](http://www.microsoft.com/en-us/download/details.aspx?id=40779)

User accounts are managed by the ASP.NET Identity framework which we adapted to
allow "Guest" users.

To get the data, we threw together a quick application to start grabbing as many matchids
from the URF Endpoint as possible. Once we captured as many matchids as we could
we had access to all the match data through the Riot API. This application is 
located under the NurfUsMatchPuller/ directory.

The site is also designed using [Bootstrap 3](http://getbootstrap.com/) which means it should function
well even on mobile devices, laptops and tablets... yay!

## Setup

### NurfUsMatchPuller 

This project pulls all of the match Id's and match details from the Rito API. This info is cached and used by the website.

It uses the following endpoints:
* [api-challenge-v4.1](https://developer.riotgames.com/api/methods#!/980/3340)
* [match-v2.2](https://developer.riotgames.com/api/methods#!/967/3313)

You will want to configure the following pieces of information in the App.config:
* [Root directory to store Match Buckets](https://github.com/neopong/NurfUs/blob/master/NurfUsMatchPuller/NurfUsMatchPuller/App.config#L8)
* [Subdirectory name to store Match Details](https://github.com/neopong/NurfUs/blob/master/NurfUsMatchPuller/NurfUsMatchPuller/App.config#L9)
* [Flag to pull Match Id buckets](https://github.com/neopong/NurfUs/blob/master/NurfUsMatchPuller/NurfUsMatchPuller/App.config#L10)
* [Flag to pull Match Details](https://github.com/neopong/NurfUs/blob/master/NurfUsMatchPuller/NurfUsMatchPuller/App.config#L11)
* [SQL connection string](https://github.com/neopong/NurfUs/blob/master/NurfUsMatchPuller/NurfUsMatchPuller/App.config#L27)

Enter your API Key in the [Program.cs](https://github.com/neopong/NurfUs/blob/master/NurfUsMatchPuller/NurfUsMatchPuller/Program.cs#L20)

You can run the executable as you wish or set it up as a scheduled task (I recommend setting up a scheduled task)

### NurfUs - This is the front-end for the website and holds all of the logic for working with the users. 

Most of the server side logic is in the [NurfUsHub.cs](https://github.com/neopong/NurfUs/blob/master/NurfUs/NurfUs/Hubs/NurfUsHub.cs)
and the client side logic is in the [Main.js](https://github.com/neopong/NurfUs/blob/master/NurfUs/NurfUs/Scripts/Page/Main.js)

You will want to change the following information in the Web.config:
* GoogleAnalyticsKey
* MatchDirectory (this should be the combination of the directory and subdirectory that you setup in the NurfUsMatchPuller)
* SQL Connection string

You should also enter your API key into the APIKey.txt file in the root directory of the project

If you want to create a new question just implement the [IBetQuestion interface](https://github.com/neopong/NurfUs/blob/master/NurfUs/NurfUs/Classes/Betting/Questions/IBetQuestion.cs).
Then add it to the [array of possible questions in the NurfUsHub](https://github.com/neopong/NurfUs/blob/master/NurfUs/NurfUs/Hubs/NurfUsHub.cs#L69)

## Our Challenges

While this was a fun project, it was not without it's challenges. We wanted to
create something that brought community interaction and presented the URF data
in a fun way that had some reward to it. Sticking to the constraints of the URF 
Endpoint, we had to figure out what to do with a plethora of anonymous match data.
We also found that two weeks is a tight deadline when you're working a full-time job.

Learning SignalR had a little overhead in the learning curve as we had both barely used it prior to this.
I must say SignalR is pretty awesome and I think websockets will end up being the next step in web development, regardless of platform.
All in all I'm really glad we got to work with SignalR as investing your time in learning new technology is always WORTH!

Last but not least, we also had some trouble with making the front-end look decent as we are not designers.
In fact we are about the furthest thing from designers as we mainly deal with backend and middle-tiers but that wasn't going to stop us!
We hope that the interface feels intuitive and is at least somewhat appealing! /crossesfingers

I just want to take a moment to thank Twitter for Bootstrap. You make our crappy designing skills not look nearly as bad as it should!

## The Team

Scott Karbel 
<br />
	Summoner Name: neopong 
<br />
	github: [neopong](https://github.com/neopong)


Tyler Thomas 
<br />
	Summoner Name: MMMOverkill 
<br />
	github: [tt9](https://github.com/tt9)

