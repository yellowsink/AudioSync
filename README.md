# AudioSync

AudioSync is a cross-platform music player that syncs. Yep it's really that simple.

[boring demo](https://youtu.be/gTOYC8AN3ug)

## Why?

This probably needs some explaining, as there isn't an obvious use for this, except for setting up a listening part with some friends if you don't all have Spotify premium and/or Discord, I guess.

Me and my friends sometimes have a voice call where we share music we like etc. It's quite fun to do, but sadly just using Pipewire or equivalent to send the output of a media player into your voice call will result in a pretty meh sounding result for everyone else.

This tries to solve this issue by allowing the person playing the music to instead join a server as the "master", and then be able to control music playback on all the other "clients". They can play, pause, edit the queue, etc.

## How does it work?

The server (`AudioSync.Server`) exposes a SignalR hub, which is connected to by the client (`AudioSync.Client`).

Opening a SignalR connection is not enough, however. The client must now identify itself to the server, giving a name and whether it intends to become the master client.

One of the clients declares itself the "master", and if none has done this already and the name given is not taken, it will become the server master, else it will be sent a failure and nothing happens on the server. The client would then fall back to connect normally.

Other clients connect as a normal "client", and as long as the name given is not taken, it will be allowed to join the server, else a failure is returned.

The master can ask the server for actions such as adding and removing songs, playing and pausing, etc. by invoking the relevant action on the server's hub.

The server will ensure the user is indeed a master, though the client does also check this before making a request, and if so will perform the relevant server-side actions, and then if needed, invoke the relevant actions on all the connected clients, except the sender, usually the master but in some cases not necessarily.

The other clients will then receive this and perform the appropriate action.

Songs can be added from most major streaming sites, and the Soundcloud URL, or if unavailable YouTube, will be automatically fetched and cached on demand, to be used with youtube-dl. This allows us to get almost any streaming link to play for everyone.

### Can I write a custom client, such as for a phone?

YES!!!! You can.

Any programming language that has a SignalR client can be used to make a client. See [Technical deep-dive](https://github.com/yellowsink/AudioSync/blob/master/deep-dive.md).

## Technology

- .NET 5 & C# - the programming language and framework this project uses to work.

- Avalonia UI - the framework the graphical client of AudioSync is built in. It's cross-platform too, which is rare for UI frameworks (except GTK and Qt, I guess, but they're the exceptions).
- ASP.NET Core - the framework that makes setting up a http server so simple, so I can focus on coding the thing.
- SignalR - the library that makes real-time two-way communication between clients and a server so nice, essentially sending whole objects magically between the client and server, and allowing them to invoke code on each other.
