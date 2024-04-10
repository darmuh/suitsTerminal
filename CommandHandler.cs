using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerminalApi.Classes;
using static suitsTerminal.AllSuits;
using static suitsTerminal.StringStuff;
using static TerminalApi.TerminalApi;

namespace suitsTerminal
{
    internal class CommandHandler
    {
        public static List<CommandInfo> suitsTerminalCommands = new List<CommandInfo>();
        public delegate void CommandDelegate(out string displayText);
        internal static string RandomSuit()
        {
            suitsTerminal.X($"Suit Count: {allSuits.Count}");
            suitsTerminal.X($"Unlockables Count: {UnlockableItems.Count}");

            int playerID = GetPlayerID();
            string displayText = string.Empty;


            if (UnlockableItems != null)
            {
                for (int i = 0; i < UnlockableItems.Count; i++)
                {
                    // Get a random index
                    int randomIndex = UnityEngine.Random.Range(0, allSuits.Count);
                    string SuitName;

                    // Get the UnlockableSuit at the random index
                    UnlockableSuit randomSuit = allSuits[randomIndex];
                    if (randomSuit != null && UnlockableItems[randomSuit.syncedSuitID.Value] != null)
                    {
                        SuitName = UnlockableItems[randomSuit.syncedSuitID.Value].unlockableName;
                        UnlockableSuit.SwitchSuitForPlayer(StartOfRound.Instance.allPlayerScripts[playerID], randomSuit.syncedSuitID.Value, true);
                        randomSuit.SwitchSuitServerRpc(playerID);
                        randomSuit.SwitchSuitClientRpc(playerID);
                        displayText = $"Changing suit to {SuitName}!\r\n";
                        return displayText;
                    }
                    else
                    {
                        suitsTerminal.X($"Random suit ID was invalid or null");
                    }
                }
            }

            displayText = $"Unable to set suit random suit.\r\n";
            return displayText;
        }

        internal static void SuitPickCommand(out string displayText)
        {
            displayText = PickSuit();
            return;
        }

        internal static void RandomSuitCommand(out string displayText)
        {
            displayText = RandomSuit();
            return;
        }

        internal static void AdvancedSuitsTerm(out string displayText)
        {
            suitsTerminal.Terminal.StartCoroutine(AdvancedMenu.ActiveMenu());
            displayText = AdvancedMenuDisplay(suitNames, 0, 10, 1);
            return;
        }

        internal static void AdvancedSuitPick(string selectedSuit)
        {
            int playerID = GetPlayerID();
            string displayText = string.Empty;
            //suitsTerminal.X("1.");

            if (UnlockableItems != null && selectedSuit != string.Empty)
            {
                //suitsTerminal.X("2.");
                foreach (UnlockableSuit suit in allSuits)
                {
                    if (suit.syncedSuitID != null && suit.syncedSuitID.Value >= 0)
                    {
                        suitsTerminal.X("3.");
                        if (UnlockableItems.Count < suit.syncedSuitID.Value)
                        {
                            suitsTerminal.Log.LogError("suit change encountered error with suitID");
                            return;
                        }

                        string SuitName = UnlockableItems[suit.syncedSuitID.Value].unlockableName;
                        if (SuitName.Equals(selectedSuit))
                        {
                            suitsTerminal.X("4.");
                            UnlockableSuit.SwitchSuitForPlayer(StartOfRound.Instance.allPlayerScripts[playerID], suit.syncedSuitID.Value, true);
                            suit.SwitchSuitServerRpc(playerID);
                            suit.SwitchSuitClientRpc(playerID);
                            displayText = $"Changing suit to {UnlockableItems[suit.syncedSuitID.Value].unlockableName}\r\n";
                            suitsTerminal.X(displayText);
                            return;
                        }
                    }
                }
            }

            displayText = $"Unable to set suit to match command: {selectedSuit}";
            suitsTerminal.X(displayText);
            return;
        }

        internal static string PickSuit()
        {
            suitsTerminal.X($"Suit Count: {allSuits.Count}");
            suitsTerminal.X($"Unlockables Count: {UnlockableItems.Count}");

            string cleanedText = GetScreenCleanedText(suitsTerminal.Terminal);
            int playerID = GetPlayerID();
            string displayText = string.Empty;

            if (UnlockableItems != null)
            {
                foreach (UnlockableSuit suit in allSuits)
                {
                    if (suit.syncedSuitID.Value >= 0)
                    {
                        string SuitName = UnlockableItems[suit.syncedSuitID.Value].unlockableName.ToLower();
                        SuitName = TerminalFriendlyString(SuitName);
                        if (cleanedText.Equals("wear " + SuitName))
                        {
                            UnlockableSuit.SwitchSuitForPlayer(StartOfRound.Instance.allPlayerScripts[playerID], suit.syncedSuitID.Value, true);
                            suit.SwitchSuitServerRpc(playerID);
                            suit.SwitchSuitClientRpc(playerID);
                            displayText = $"Changing suit to {UnlockableItems[suit.syncedSuitID.Value].unlockableName}\r\n";
                            return displayText;
                        }
                        else
                            suitsTerminal.X($"SuitName: {SuitName} doesn't match Cleaned Text: {cleanedText}");
                    }
                    else
                    {
                        suitsTerminal.X($"suit ID was {suit.syncedSuitID.Value}");
                    }
                }
            }

            displayText = $"Unable to set suit to match command: {cleanedText}";
            return displayText;
        }

        internal static int GetPlayerID()
        {
            List<PlayerControllerB> allPlayers = new List<PlayerControllerB>();
            string myName = GameNetworkManager.Instance.localPlayerController.playerUsername;
            int returnID = -1;
            allPlayers = StartOfRound.Instance.allPlayerScripts.ToList();
            allPlayers = allPlayers.OrderBy((PlayerControllerB player) => player.playerClientId).ToList();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (StartOfRound.Instance.allPlayerScripts[i].playerUsername == myName)
                {
                    suitsTerminal.X("Found my playerID");
                    returnID = i;
                    break;
                }
            }
            if (returnID == -1)
                suitsTerminal.X("Failed to find ID");
            return returnID;
        }

        private static string GetScreenCleanedText(Terminal __instance)
        {
            string s = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            return RemovePunctuation(s);
        }

        private static string RemovePunctuation(string s) //copied from game files
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().ToLower();
        }

        internal static void AddCommand(string textFail, bool clearText, List<TerminalNode> nodeGroup, string keyWord, bool isVerb, string nodeName, string category, string description, CommandDelegate methodName)
        {
            TerminalNode node = CreateTerminalNode(textFail, clearText);
            TerminalKeyword termWord = CreateTerminalKeyword(keyWord, isVerb, node);

            CommandInfo commandInfo = new CommandInfo()
            {
                TriggerNode = node,
                DisplayTextSupplier = () =>
                {
                    methodName(out string displayText);
                    return displayText;
                },
                Category = category,
                Description = description
            };
            suitsTerminalCommands.Add(commandInfo);

            AddTerminalKeyword(termWord, commandInfo);

            node.name = nodeName;
            nodeGroup.Add(node);
        }

    }
}
