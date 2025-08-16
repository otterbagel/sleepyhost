using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace SleepyHost
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        const int MAX_TIME = 2600;

        bool goneToSleep = false;
        bool headingToBed = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Set up event handler for a new day starting
            helper.Events.GameLoop.DayStarted -= GameLoop_DayStarted;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            // Set up game loop tick handler
            helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private void GameLoop_DayStarted(object? sender, DayStartedEventArgs e)
        {
            goneToSleep = false;
            headingToBed = false;

            // Only apply mod logic if the game is loaded in multiplayer, and the current player is the host
            if (!Context.IsWorldReady
                || !Context.IsMainPlayer
                || !Context.IsOnHostComputer
                || !Context.IsMultiplayer)
            {
                Monitor.Log("Day has started but player is not hosting a multiplayer game", LogLevel.Debug);
                Monitor.Log("Not auto-heading to bed", LogLevel.Debug);
                return;
            }

            // Checking that we're in the farm house
            if (Game1.currentLocation is not FarmHouse farmHouse)
            {
                Monitor.Log("Day has started but player is not a farmhouse", LogLevel.Debug);
                Monitor.Log($"We are in: {Game1.currentLocation.Name}", LogLevel.Debug);
                Monitor.Log("Not auto-heading to bed", LogLevel.Debug);
                return;
            }

            // Find the player's bed in the house, and move the player to the left of it
            Game1.player.hasMoved = true;
            headingToBed = true;
            var playerBedSpot = farmHouse.GetPlayerBedSpot();
            Game1.player.Position = Utility.PointToVector2(playerBedSpot) * 64f;
            BedFurniture.ShiftPositionForBed(Game1.player);
            Game1.player.position.X -= 32;

            // Face to the right
            Game1.player.FacingDirection = 1; // Face right
            Game1.player.setMovingInFacingDirection();

            Monitor.Log("Auto-heading to bed", LogLevel.Debug);
        }

        private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            // If we're heading to bed, move the player to the right to walk to the bed
            if (headingToBed)
            {
                // If the player is not free to move (ie. the sleep message appeared), stop moving and disable the headingToBed flag
                if (!Context.IsPlayerFree)
                {
                    headingToBed = false;
                    Game1.player.setTrajectory(Vector2.Zero);
                    Game1.player.Halt();

                    if (!SleepPromptDialogueBoxHandler.IsSleepPromptDialogueBoxUp())
                    {
                        Monitor.Log("Player is no longer free, but active menu is not sleep dialog", LogLevel.Debug);
                        Monitor.Log("This is unexpected", LogLevel.Debug);
                        return;
                    }

                    Monitor.Log("Host made it to bed", LogLevel.Debug);
                    Monitor.Log("Will now wait for other multiplayer farmers to go to sleep", LogLevel.Debug);
                    return;
                }

                // If a message has not yet appeared and we're still heading to bed, continue moving to the right towards the bed
                Game1.player.setTrajectory(Vector2.UnitX);
                return;
            }

            if (!goneToSleep
                && SleepPromptDialogueBoxHandler.IsSleepPromptDialogueBoxUp()
                && ShouldGoToSleep())
            {
                if (SleepPromptDialogueBoxHandler.ClickYesIfUp())
                {
                    Monitor.Log("Host is going to sleep", LogLevel.Debug);
                    goneToSleep = true;
                    return;
                }
            }
        }

        private static bool ShouldGoToSleep()
        {
            return AllPlayersSleeping() || IsDayEnding();
        }

        private static bool AllPlayersSleeping()
        {
            var players = Game1.getOnlineFarmers().Where(farmer => farmer != Game1.player).ToList();
            if (players.Count < 1)
            {
                // If there is not at least one farmer, who isn't the current player, don't wait for them
                return false;
            }

            foreach (var player in players)
            {
                if (player.timeWentToBed.Value < 1)
                {
                    // This player has not gone to sleep yet, so keep waiting
                    return false;
                }
            }

            return true;
        }

        private static bool IsDayEnding()
        {
            return Game1.timeOfDay >= MAX_TIME - 1;
        }
    }

    internal class SleepPromptDialogueBoxHandler
    {
        public static bool IsSleepPromptDialogueBoxUp()
        {
            if (Game1.activeClickableMenu is not DialogueBox box
                || !box.isQuestion
                || Game1.currentLocation.lastQuestionKey?.ToLowerInvariant() != "sleep")
            {
                return false;
            }

            if (box.dialogues.Count != 1)
            {
                return false;
            }

            var expectedMessage = Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep");
            if (box.dialogues.FirstOrDefault() != expectedMessage)
            {
                return false;
            }

            if (box.responses.Length != 2 || box.responses[0].responseKey.ToLowerInvariant() != "yes" || box.responses[1].responseKey.ToLowerInvariant() != "no")
            {
                return false;
            }

            return true;
        }

        public static bool ClickYesIfUp()
        {
            if (!IsSleepPromptDialogueBoxUp())
            {
                return false;
            }
            if (Game1.activeClickableMenu is not DialogueBox box)
            {
                return false;
            }
            if (box.responses.FirstOrDefault(r => r.responseKey.ToLowerInvariant() == "yes") is not Response yesResponse)
            {
                return false;
            }

            box.receiveKeyPress(yesResponse.hotkey);
            return true;
        }
    }
}
