# Sleepy Host (Stardew Valley Multiplayer Mod)

## What is this?

Sleepy Host is a simple solution to a problem that has bugged me about Stardew Valley as a long-time player. Multiplayer in Stardew Valley is very fun, but coordinating a time to play in a group, especially a large group, can be very challenging.

Compare the multiplayer experience in Stardew Valley to Minecraft's. With a standalone server in Minecraft, coordinating times to play is not necessary, as players can hop in and out of the server as they please. When no players are in the server, time does not pass. So the world is only affected when at least one player is logged in, and players can come and go as they please.

Stardew Valley's multiplayer experience is different because the host of the multiplayer session is, themselves, a player. So in order for a group to play together in the same game, the host of the farm must be playing.

What if Stardew Valley's multiplayer experience could resemble something closer to Minecraft's? That's what this mod helps achieve.

## How does it work?

It's deceptively simple. This is what the mod does:

1. At the start of a new day, the host will immediately move to the bed, and bring up the "Go to sleep for the night?" dialog.
1. It will remain in bed with this dialog open until either:
   * All other players in the session (assuming there are any) have gone to sleep.
	* The day is about to hit 2:00 AM.
1. After either of these conditions are met, the "Yes" option will automatically be chosen to go to sleep.

If the mod is installed, but the current game is not a multiplayer game or the player is not hosting the game, then the automated behavior will not happen. Only the host of a multiplayer game will see any behavior occur.

## How does this change the multiplayer experience?

Here is the idea: The host of the game who originally started the farm, will _not_ be a playable farmer. Instead, the host is meant to simply keep the game running for everybody else.

A dedicated instance of the game should be set up in such a way to keep the game running, hosting a multiplayer session, 24/7 (similar to a dedicated Minecraft server).

In addition, and this is important: **_A separate auto-pausing multiplayer mod should be used by all players and the host._** An example of such a mod is [WAIT by aurpine](https://www.nexusmods.com/stardewvalley/mods/20661) (not affiliated). This mod, and similar mods, change the behavior of the clock in the game by pausing time if _all_ logged-in players are paused, but time will continue passing if at least one player is unpaused.

Combining Sleepy Host with a mod like WAIT will mean that if no players (besides the host) are logged in, then time will stay paused, and the farm will be in stasis. But once someone logs in, time will resume, and players can play the game as normal. As soon as everyone logs out, time will pause again. But crucially, the host is always hosting, so players can come and go as they please.

When non-host players are playing the game, and choose to go to sleep for the night to end the day, Sleepy Host will only tell the host to sleep once all other players have also gone to sleep first. However, if 2:00 AM is about to approach and all other players have not yet gone to sleep, Sleepy Host will also tell the host to go to sleep, so as to never keep the game from proceeding to the next day.
