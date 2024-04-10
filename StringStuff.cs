using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static suitsTerminal.AdvancedMenu;

namespace suitsTerminal
{
    internal class StringStuff
    {
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
            StringBuilder message = new StringBuilder();

            message.Append($" === suitsTerminal Advanced Suits Menu ===\r\n=== Use Arrow Keys to Navigate All Suits === \r\n");
            message.Append($" === Select a Suit by pressing [{selectString}] === \r\n");
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
                // Prepend ">" to the active item and append "[EQUIPPED]" line if applicable
                string menuItem = (i == activeIndex)
                    ? "> " + ((i == currentlyWearing) ? menuItems[i] + " [EQUIPPED]" : menuItems[i])
                    : ((i == currentlyWearing) ? menuItems[i] + " [EQUIPPED]" : menuItems[i]);


                // Display the menu item
                message.Append(menuItem + "\r\n");
            }

            // Display pagination information
            message.Append("\r\n");
            message.Append($"Page {currentPage}/{Mathf.CeilToInt((float)menuItems.Count / pageSize)}\r\n");

            if(SConfig.enablePiPCamera.Value)
                message.Append($"\r\n === Toggle Mirror-Cam by pressing [{togglePiPstring}] === \r\n");

            message.Append($" === Leave this menu by pressing [{leaveString}] === \r\n");

            //AdvancedMenu.UpdatePicture();
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
