using suitsTerminal.Suit_Stuff;
using System.Text;
using UnityEngine;

namespace suitsTerminal
{
    internal class StringStuff
    {
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

    }
}
