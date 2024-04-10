using System;
using static suitsTerminal.StringStuff;
using static suitsTerminal.AllSuits;
using static suitsTerminal.CommandHandler;
using static suitsTerminal.AdvancedMenu;
using static UnityEngine.InputSystem.InputRemoting;

namespace suitsTerminal
{
    internal class ChatHandler
    {
        internal static void HandleChatMessage(string command)
        {
            //Set fov with chat command.
            if (command.StartsWith("!suits"))
            {
                string displayText = string.Empty;
                string[] args = command.Split(' ');
                if (args.Length == 1)
                {
                    GetCurrentSuitID();
                    string message = ChatListing(suitNames, 6, 1);
                    HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t {message}");
                    return;
                }
                else if (args.Length > 1)
                {
                    string pageNum = args[1];
                    if (int.TryParse(pageNum, out int pageNumVal))
                    {
                        string message = ChatListing(suitNames, 6, pageNumVal);
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t {message}");
                        return;
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid page number format: {pageNum}");
                        suitsTerminal.X($"Invalid page number format: {pageNum}");
                        return;
                    }
                }


            }
            else if (command.StartsWith("!wear"))
            {
                string[] args = command.Split(' ');
                if (args.Length == 1)
                {
                    HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t No suit specified...");
                }
                else if (args.Length > 1)
                {
                    string suitNum = args[1];
                    if (int.TryParse(suitNum, out int suitNumVal))
                    {
                        if (suitNumVal >= 0 && suitNumVal < suitNames.Count)
                        {
                            string suitName = suitNames[suitNumVal];
                            suitsTerminal.X($"wear command");
                            AdvancedSuitPick(suitName);
                            GetCurrentSuitID();
                            return;
                        }
                        else
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid suit number: {suitNum}");
                            suitsTerminal.X($"Invalid suit number: {suitNum}");
                            return;
                        }
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid suit number format: {suitNum}");
                        suitsTerminal.X($"Invalid suit number format: {suitNum}");
                        return;
                    }
                }

            }
            else if (command.StartsWith("!clear"))
            {
                HUDManager.Instance.chatText.text.Remove(0, HUDManager.Instance.chatText.text.Length);
                HUDManager.Instance.ChatMessageHistory.Clear();
            }
        }
    }
}
