using static suitsTerminal.StringStuff;
using static suitsTerminal.AllSuits;
using static suitsTerminal.CommandHandler;
using static suitsTerminal.AdvancedMenu;

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
                    string message = ChatListing(suitNames, 6, 1);
                    HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t {message}");
                    lastCommandRun = command;
                    return;
                }
                else if (args.Length > 1)
                {
                    string pageNum = args[1];
                    if (int.TryParse(pageNum, out int pageNumVal))
                    {
                        string message = ChatListing(suitNames, 6, pageNumVal);
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t {message}");
                        lastCommandRun = command;
                        return;
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid page number format: {pageNum}");
                        suitsTerminal.WARNING($"Invalid page number format: {pageNum}");
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
                        if (suitNumVal >= 0 && suitNumVal < suitNames.Count)
                        {
                            string suitName = suitNames[suitNumVal];
                            suitsTerminal.X($"wear command");
                            AdvancedSuitPick(suitName);
                            GetCurrentSuitID();
                            lastCommandRun = command;
                            return;
                        }
                        else
                        {
                            HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid suit number: {suitNum}");
                            suitsTerminal.WARNING($"Invalid suit number: {suitNum}");
                            lastCommandRun = command;
                            return;
                        }
                    }
                    else
                    {
                        HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]:\t Invalid suit number format: {suitNum}");
                        suitsTerminal.WARNING($"Invalid suit number format: {suitNum}");
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
