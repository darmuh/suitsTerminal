using GameNetcodeStuff;
using OpenBodyCams;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static suitsTerminal.AdvancedMenu;
using static suitsTerminal.AllSuits;
using static suitsTerminal.PictureInPicture;

namespace suitsTerminal
{
    internal class Misc
    {
        internal static bool keywordsCreated = false;
        internal static bool rackSituated = false;
        internal static int normSuit = 0;
        internal static int showSuit = 0;
        internal static int weirdSuitNum = 0;
        internal static int reorderSuits = 0;
        internal static bool hasLaunched = false;
        internal static bool hintOnce = false;
        internal static bool isHanging = false;

        internal static List<string> GetString()
        {
            if(favSuits.Count == 0)
            {
                suitsTerminal.Log.LogWarning("Favorite suits empty, displaying regular menu");
                inFavsMenu = false;
                return suitNames;
            }

            if (inFavsMenu)
                return favSuits;
            else
                return suitNames;
        }

        internal static GameObject GetGameObject(string location)
        {
            return GameObject.Find(location);
        }

        internal static string HelpMenuDisplay(bool inHelpMenu, List<string> currentMenu)
        {
            if (inHelpMenu)
            {
                suitsTerminal.X("Help Menu Enabled, showing help information");
                StringBuilder message = new();

                message.Append($"========= AdvancedsuitsMenu Help Page  =========\r\n");
                message.Append("\r\n\r\n");

                message.Append($"Highlight Next Item: [{downString}]\r\nHighlight Last Item: [{upString}]\r\nFavorite Item: [{favItemKeyString}]\r\nShow Favorites Menu: [{favMenuKeyString}]\r\n");
                if (SConfig.enablePiPCamera.Value)
                {
                    message.Append($"Toggle Camera Preview: [{togglePiPstring}]\r\nRotate Camera: [{pipRotateString}]\r\nChange Camera Height: [{pipHeightString}]\r\nChange Camera Zoom: [{pipZoomString}]\r\n");
                }
                message.Append($"Leave Suits Menu: [{leaveString}]\r\nSelect Suit: [{selectString}]\r\n");
                message.Append($"\r\n>>>\tReturn to Suit Selection: [{helpMenuKeyString}]\t<<\r\n");
                return message.ToString();
            }
            else
            {
                suitsTerminal.X("Help Menu disabled, returning to menu selection...");
                return StringStuff.AdvancedMenuDisplay(currentMenu, activeSelection, 10, currentPage);
            }
        }

        internal static void SaveToConfig(List<string> stringList, out string configItem)
        {
            configItem = string.Join(", ", stringList);
            suitsTerminal.X($"Saving to config\n{configItem}");
        }

        internal static PlayerControllerB GetPlayerUsingTerminal()
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (!player.isPlayerDead && player.currentTriggerInAnimationWith == suitsTerminal.Terminal.terminalTrigger)
                {
                    suitsTerminal.X($"Player: {player.playerUsername} detected using terminal.");
                    return player;
                }
            }
            return null;
        }


    }
}
