using suitsTerminal.Suit_Stuff;
using System.Linq;
using System.Text;
using UnityEngine;
using static suitsTerminal.AdvancedMenu;
using static suitsTerminal.AllSuits;

namespace suitsTerminal
{
    internal class StringStuff
    {
        internal static string GetNumbers(string selectedSuit)
        {
            int startOfNum = selectedSuit.IndexOf("^");
            string numbersPortion = startOfNum != -1 ? selectedSuit[(startOfNum + 1)..] : string.Empty;
            numbersPortion = numbersPortion.Replace("^", "").Replace("(", "").Replace(")", "");
            return numbersPortion;
        }


        internal static string RemoveNumbers(string selectedSuit)
        {
            int caretIndex = selectedSuit.IndexOf("^");
            string stringWithoutNumbers = caretIndex != -1 ? selectedSuit[..caretIndex] : selectedSuit;
            return stringWithoutNumbers;
        }

        internal static string TerminalFriendlyString(string s)
        {
            StringBuilder stringBuilder = new();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            if (stringBuilder.Length > 14)
            {
                int excessLength = stringBuilder.Length - 14;
                stringBuilder.Remove(14, excessLength);
                //Plugin.X($"terminalFriendlystring: {stringBuilder}");
            }


            return stringBuilder.ToString().ToLower();
        }

        internal static string ChatListing(SuitListing suitListing, int pageSize, int currentPage)
        {
            int listing = suitListing.SuitsList.Count;

            // Ensure currentPage is within valid range
            currentPage = Mathf.Clamp(currentPage, 1, Mathf.CeilToInt((float)listing / pageSize));

            // Calculate the start and end indexes for the current page
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Mathf.Min(startIndex + pageSize, listing);
            StringBuilder message = new();

            message.Append("\r\n");

            // Iterate through each item in the current page
            for (int i = startIndex; i < endIndex; i++)
            {
                SuitAttributes suit = suitListing.SuitsList[i];

                // Append "[EQUIPPED]" line if applicable
                string menuItem = $"{suit.Name}" + (suit.currentSuit ? " [EQUIPPED]" : "");

                // Display the menu item
                message.Append($"'!wear {i}' (" + menuItem + ")\r\n");
            }

            // Display pagination information
            message.Append("\r\n");
            message.Append($"Page {currentPage}/{Mathf.CeilToInt((float)listing / pageSize)}\r\n");

            return message.ToString();
        }

        internal static int GetListing(SuitListing suitListing)
        {
            if (inFavsMenu)
                suitListing.CurrentMenu = 1;
            else
                suitListing.CurrentMenu = 0;

            int listing;
            if (suitListing.CurrentMenu == 0)
                listing = suitListing.NameList.Count;
            else
                listing = suitListing.FavList.Count;

            return listing;
        }

        internal static string AdvancedMenuDisplay(SuitListing suitListing, int activeIndex, int pageSize, ref int currentPage)
        {
            Plugin.X($"activeIndex: {activeIndex}\npageSize: {pageSize}\ncurrentPage: {currentPage}");
            if(suitListing == null)
            {
                Plugin.ERROR("suitsTerminal FATAL ERROR: suitListing is NULL");
                return "suitsTerminal FATAL ERROR: suitListing is NULL";
            }

            int listing = GetListing(suitListing);

            Plugin.X($"listing count: {listing}");

            // Ensure currentPage is within valid range
            currentPage = Mathf.Clamp(currentPage, 1, Mathf.CeilToInt((float)listing / pageSize));

            // Calculate the start and end indexes for the current page
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Mathf.Min(startIndex + pageSize, listing);
            int totalItems = 0;
            int emptySpace;
            StringBuilder message = new();

            message.Append($"============= AdvancedsuitsMenu  =============\r\n");
            message.Append("\r\n");

            // Recalculate activeIndex based on the current page
            // Ensure activeIndex is within the range of items on the current page
            activeIndex = Mathf.Clamp(activeIndex, startIndex, endIndex - 1);
            Plugin.X($"activeSelection: {activeSelection} activeIndex: {activeIndex}");
            Plugin.X("matching values");
            activeSelection = activeIndex;

            // Iterate through each item in the current page
            for (int i = startIndex; i < endIndex; i++)
            {
                SuitAttributes suit;
                if (suitListing.CurrentMenu == 0)
                    suit = suitListing.SuitsList.Where(x => x.MainMenuIndex == i).FirstOrDefault();
                else
                    suit = suitListing.SuitsList.Where(x => x.FavIndex == i).FirstOrDefault();

                // Prepend ">" to the active item and append "[EQUIPPED]" line if applicable

                activeSelection = activeIndex;
                string menuItem;

                if(suit != null)
                {
                    menuItem = (i == activeIndex)
                    ? $"> {suit.Name}" + (suit.currentSuit ? " [EQUIPPED]" : "") + (suit.IsFav ? " (*)" : "")
                    : $"{suit.Name}" + (suit.currentSuit ? " [EQUIPPED]" : "") + (suit.IsFav ? " (*)" : "");
                }
                else
                {
                    menuItem = (i == activeIndex)
                    ? $"> {i} - **MISSING SUIT**"
                    : $"{i} - **MISSING SUIT**";
                    Plugin.WARNING($"Unable to find suit at index [ {i} ] of suitsTerminal suitListing!");
                }

                // Display the menu item
                message.Append(menuItem + "\r\n");
                totalItems++;
            }

            emptySpace = pageSize - totalItems;

            for (int i = 0; i < emptySpace; i++)
            {
                message.Append("\r\n");
            }

            // Display pagination information
            //Page [LeftArrow] < 6/10 > [RightArrow]
            message.Append("\r\n\r\n");
            message.Append($"Currently Wearing: {UnlockableItems[StartOfRound.Instance.localPlayerController.currentSuitID].unlockableName}\r\n\r\n");
            message.Append($"Page [{leftString}] < {currentPage}/{Mathf.CeilToInt((float)listing / pageSize)} > [{rightString}]\r\n");
            message.Append($"Leave Menu: [{leaveString}]\tSelect Suit: [{selectString}]\r\n");
            message.Append($"\r\n>>>\tDisplay Help Page: [{helpMenuKeyString}]\t<<\r\n");
            return message.ToString();
        }

        internal static SuitAttributes GetMenuItemSuit(SuitListing suitListing, int activeIndex)
        {
            SuitAttributes suit;
            if (suitListing.CurrentMenu == 0)
            {
                suit = suitListing.SuitsList.Find(x => x.MainMenuIndex == activeIndex);
                return suit;
            }
            else
            {
                suit = suitListing.SuitsList.Find(x => x.FavIndex == activeIndex);
                return suit;
            }
        }

    }
}
