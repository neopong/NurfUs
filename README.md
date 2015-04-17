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

User accounts are managed by the ASP.NET Identity framework which we adapted to
allow "Guest" users.

To get the data, we threw together a quick application to start grabbing as many matchids
from the URF Endpoint as possible. Once we captured as many matchids as we could
we had access to all the match data through the Riot API. This application is 
located under the NurfUsMatchPuller/ directory.

## Our Challenges

While this was a fun project, it was not without it's challenges. We wanted to
create something that brought community interaction and presented the URF data
in a fun way that had some reward to it. Sticking to the constraints of the URF 
Endpoint, we had to figure out what to do with a plethora of anonymous match data.
We also found that two weeks is a tight deadline when you're working a full-time job.


## The Team

Scott Karbel 
	Summoner Name: neopong 
	github: [neopong](https://github.com/neopong)

Tyler Thomas 
	Summoner Name: MMMOverkill 
	github: [tt9](https://github.com/tt9)

