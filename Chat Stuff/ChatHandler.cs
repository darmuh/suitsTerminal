using suitsTerminal.Suit_Stuff;
using static suitsTerminal.AdvancedMenu;
using static suitsTerminal.AllSuits;
using static suitsTerminal.CommandHandler;
using static suitsTerminal.StringStuff;

namespace suitsTerminal
{
    internal class ChatHandler
    {
        internal static string lastCommandRun = "";
        internal static void HandleChatMessage(string command)
        {
            if (lastCommandRun == command)
                return;

            //Set fov with chat command.
            if (command.StartsWith("!suits"))
            {
                string[] args = command.Split(' ');
                if (args.Length == 1)
                {
                    GetCurrentSuitID();
                    string message = ChatListing(suitListing, 6, 1);
                    HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t {message}");
                    lastCommandRun = command;
                    return;
                }
                else if (args.Length > 1)
                {
                    string pageNum = args[1];
                    if (int.TryParse(pageNum, out int pageNumVal))
                    {
                        string message = ChatListing(suitListing, 6, pageNumVal);
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t {message}");
                        lastCommandRun = command;
                        return;
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid page number format: {pageNum}");
                        Plugin.WARNING($"Invalid page number format: {pageNum}");
                        lastCommandRun = command;
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
                    lastCommandRun = command;
                }
                else if (args.Length > 1)
                {
                    string suitNum = args[1];
                    if (int.TryParse(suitNum, out int suitNumVal))
                    {
                        if (suitNumVal >= 0 && suitNumVal < suitListing.SuitsList.Count)
                        {
                            SuitAttributes suit = suitListing.SuitsList[suitNumVal];
                            Plugin.X($"wear command");
                            BetterSuitPick(suit);
                            GetCurrentSuitID();
                            lastCommandRun = command;
                            return;
                        }
                        else
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid suit number: {suitNum}");
                            Plugin.WARNING($"Invalid suit number: {suitNum}");
                            lastCommandRun = command;
                            return;
                        }
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid suit number format: {suitNum}");
                        Plugin.WARNING($"Invalid suit number format: {suitNum}");
                        lastCommandRun = command;
                        return;
                    }
                }

            }
            else if (command.StartsWith("!clear"))
            {
                _ = HUDManager.Instance.chatText.text.Remove(0, HUDManager.Instance.chatText.text.Length);
                HUDManager.Instance.ChatMessageHistory.Clear();
                lastCommandRun = command;
            }
        }
    }
}
