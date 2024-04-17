using Steamworks.Ugc;
using System;
using System.Collections.Generic;
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
            string numbersPortion = startOfNum != -1 ? selectedSuit.Substring(startOfNum + 1) : string.Empty;
            numbersPortion = numbersPortion.Replace("^", "").Replace("(", "").Replace(")", "");
            return numbersPortion;
        }


        internal static string RemoveNumbers(string selectedSuit)
        {
            int caretIndex = selectedSuit.IndexOf("^");
            string stringWithoutNumbers = caretIndex != -1 ? selectedSuit.Substring(0, caretIndex) : selectedSuit;
            return stringWithoutNumbers;
        }

        internal static string TerminalFriendlyString(string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
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
                //suitsTerminal.X($"terminalFriendlystring: {stringBuilder}");
            }


            return stringBuilder.ToString().ToLower();
        }

        internal static string ChatListing(List<string> menuItems, int pageSize, int currentPage)
        {
            // Ensure currentPage is within valid range
            currentPage = Mathf.Clamp(currentPage, 1, Mathf.CeilToInt((float)menuItems.Count / pageSize));

            // Calculate the start and end indexes for the current page
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Mathf.Min(startIndex + pageSize, menuItems.Count);
            StringBuilder message = new StringBuilder();

            message.Append("\r\n");

            // Iterate through each item in the current page
            for (int i = startIndex; i < endIndex; i++)
            {
                // Append "[EQUIPPED]" line if applicable
                string menuItem = (i == currentlyWearing) ? menuItems[i] + " [EQUIPPED]" : menuItems[i];

                // Display the menu item
                message.Append($"'!wear {i}' (" + menuItem + ")\r\n");
            }

            // Display pagination information
            message.Append("\r\n");
            message.Append($"Page {currentPage}/{Mathf.CeilToInt((float)menuItems.Count / pageSize)}\r\n");

            return message.ToString();
        }

        internal static string AdvancedMenuDisplay(List<string> menuItems, int activeIndex, int pageSize, int currentPage)
        {
            // Ensure currentPage is within valid range
            currentPage = Mathf.Clamp(currentPage, 1, Mathf.CeilToInt((float)menuItems.Count / pageSize));

            // Calculate the start and end indexes for the current page
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Mathf.Min(startIndex + pageSize, menuItems.Count);
            int totalItems = 0;
            int emptySpace;
            StringBuilder message = new StringBuilder();

            message.Append($"============= AdvancedsuitsMenu  =============\r\n");
            message.Append("\r\n");

            // Recalculate activeIndex based on the current page
            // Ensure activeIndex is within the range of items on the current page
            activeIndex = Mathf.Clamp(activeIndex, startIndex, endIndex - 1);
            suitsTerminal.X($"activeSelection: {activeSelection} activeIndex: {activeIndex}");
            suitsTerminal.X("matching values");
            activeSelection = activeIndex;

            // Iterate through each item in the current page
            for (int i = startIndex; i < endIndex; i++)
            {
                // Check if the menuItem is in favSuits
                bool isFavorite = favSuits.Contains(menuItems[i]);

                // Prepend ">" to the active item and append "[EQUIPPED]" line if applicable
                string menuItem = (i == activeIndex)
                    ? "> " + ((i == currentlyWearing) ? menuItems[i] + " [EQUIPPED]" : menuItems[i]) + (isFavorite ? " (*)" : "")
                    : ((i == currentlyWearing) ? menuItems[i] + " [EQUIPPED]" : menuItems[i]) + (isFavorite ? " (*)" : "");

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
            message.Append($"Page [{leftString}] < {currentPage}/{Mathf.CeilToInt((float)menuItems.Count / pageSize)} > [{rightString}]\r\n");
            //message.Append($"============= See controls below =============\r\n");
            message.Append($"Next Item [{downString}]\tLast Item [{upString}]\r\nFavorite Item [{favItemKeyString}] \tFavorites Menu [{favMenuKeyString}]\r\n");
            if(SConfig.enablePiPCamera.Value)
            {
                message.Append($"Toggle Cam [{togglePiPstring}]\tRotate Cam [{pipRotateString}]\t\r\nCam Height [{pipHeightString}]\tCam Zoom [{pipZoomString}]\r\n");
            }
            message.Append($"Leave Menu [{leaveString}]\tSelect Suit [{selectString}]\r\n");
            return message.ToString();
        }

        internal static string GetActiveMenuItem(List<string> menuItems, int activeIndex, int pageSize, int currentPage)
        {
            suitsTerminal.X("GetActiveMenuItem");
            // Calculate the start and end indexes for the current page
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Mathf.Min(startIndex + pageSize, menuItems.Count);

            // Ensure activeIndex is within the bounds of the current page
            if (activeIndex < startIndex || activeIndex >= endIndex)
            {
                // Adjust currentPage to bring activeIndex within the current page range
                currentPage = (activeIndex / pageSize) + 1;
                startIndex = (currentPage - 1) * pageSize;
                endIndex = Mathf.Min(startIndex + pageSize, menuItems.Count);
            }

            // Retrieve the active menu item
            string menuItem = menuItems[activeIndex];
            suitsTerminal.X($"Selecting Active Menu Item: {menuItem}");

            return menuItem;
        }


    }
}
