using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static suitsTerminal.AdvancedMenu;

namespace suitsTerminal
{
    internal class Misc
    {
        internal static bool keywordsCreated = false;
        internal static bool rackSituated = false;
        internal static int suitsOnRack = 0;
        internal static int weirdSuitNum = 0;
        internal static bool hasLaunched = false;
        internal static bool hintOnce = false;

        internal static bool TryGetGameObject(string location, out GameObject gameobj)
        {
            gameobj = GameObject.Find(location);
            return gameobj != null;
        }

        internal static string HelpMenuDisplay()
        {
            Plugin.X("Help Menu Enabled, showing help information");
            StringBuilder message = new();

            message.Append($"========= AdvancedsuitsMenu Help Page  =========\r\n");
            message.Append("\r\n\r\n");

            message.Append($"Highlight Next Item: [{suitsMenu.downMenu}]\r\nHighlight Last Item: [{suitsMenu.upMenu}]\r\nFavorite Item: [{favItemKeyString}]\r\n");
            if (SConfig.EnablePiPCamera.Value)
            {
                message.Append($"Toggle Camera Preview: [{togglePiPstring}]\r\nRotate Camera: [{pipRotateString}]\r\nChange Camera Height: [{pipHeightString}]\r\nChange Camera Zoom: [{pipZoomString}]\r\n");
            }
            message.Append($"Leave Suits Menu: [{suitsMenu.leaveMenu}]\r\nSelect Suit: [{suitsMenu.selectMenu}]\r\n");
            message.Append($"\r\n>>>\tReturn to Menu: [ANY KEY]\t<<\r\n");
            return message.ToString();
        }

        internal static void SaveToConfig(List<string> stringList, out string configItem)
        {
            configItem = string.Join(", ", stringList);
            Plugin.X($"Saving to config\n{configItem}");
        }

        internal static void SaveFavorites(string saveText)
        {
            if(SConfig.PersonalizedFavorites.Value)
            {
                string favsFilePath = Path.Combine(@"%userprofile%\appdata\locallow\ZeekerssRBLX\Lethal Company", "suitsTerminal") + "\\masterFavsListing.txt";
                favsFilePath = Environment.ExpandEnvironmentVariables(favsFilePath);
                File.WriteAllText(favsFilePath, saveText);
                Plugin.X($"Favorites saved to file at {favsFilePath}");
            }
            else
                SConfig.FavoritesMenuList.Value = saveText;
        }

    }
}
