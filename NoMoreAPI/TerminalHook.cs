using System;
using System.Collections.Generic;
using UnityEngine;

namespace suitsTerminal
{
    internal class TerminalHook
    {
        internal static List<UnlockableItem> suitsTermUnlockables = [];
        internal static void AddKeywordToExistingNode(string keyWord, TerminalNode existingNode)
        {
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = keyWord + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = false;
            terminalKeyword.specialKeywordResult = existingNode;

            List<CompatibleNoun> existingNounList = [.. existingNode.terminalOptions];

            CompatibleNoun noun = new()
            {
                noun = terminalKeyword,
                result = existingNode
            };
            existingNounList.Add(noun);
            existingNode.terminalOptions = [.. existingNounList];

            allKeywordsList.Add(terminalKeyword);
            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeDynamicCommand(string nodeName, string keyWord, string failtext, bool clearText, bool acceptAnything, Func<string> commandFunc, Dictionary<TerminalNode, string> specialNodeList, Dictionary<TerminalNode, Func<string>> nodeListing)
        {
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            CheckForAndDeleteKeyWord(keyWord.ToLower());

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = failtext;
            terminalNode.clearPreviousText = clearText;
            terminalNode.acceptAnything = acceptAnything;
            //terminalNode.overrideOptions = true;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = false;
            terminalKeyword.specialKeywordResult = terminalNode;

            nodeListing.Add(terminalNode, commandFunc);
            specialNodeList.Add(terminalNode, keyWord.ToLower());

            allKeywordsList.Add(terminalKeyword);
            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText)
        {
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            CheckForAndDeleteKeyWord(keyWord);

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;


            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;
            _ = new
            CompatibleNoun()
            {
                noun = terminalKeyword,
                result = terminalNode
            };

            allKeywordsList.Add(terminalKeyword);
            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }
        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, Func<string> commandAction, Dictionary<TerminalNode, Func<string>> nodeListing, int specialNumber, Dictionary<TerminalNode, int> specialNodeList)
        {
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            CheckForAndDeleteKeyWord(keyWord);

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            //terminalNode.overrideOptions = true;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            nodeListing.Add(terminalNode, commandAction);
            specialNodeList.Add(terminalNode, specialNumber);

            allKeywordsList.Add(terminalKeyword);
            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, Func<string> commandAction, Dictionary<TerminalNode, Func<string>> nodeListing, int specialNumber, string specialName, Dictionary<TerminalNode, int> specialNodeList, Dictionary<int, string> reverseNodeNumList)
        {
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            CheckForAndDeleteKeyWord(keyWord);

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            //terminalNode.overrideOptions = true;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            nodeListing.Add(terminalNode, commandAction);
            specialNodeList.Add(terminalNode, specialNumber);
            if (!reverseNodeNumList.ContainsKey(specialNumber))
                reverseNodeNumList.Add(specialNumber, specialName);

            allKeywordsList.Add(terminalKeyword);
            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }


        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, Func<string> commandAction, Dictionary<TerminalNode, Func<string>> nodeListing)
        {
            //suitsTerminal.X("Making command");
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            //suitsTerminal.X("List retrieved.");
            CheckForAndDeleteKeyWord(keyWord);

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            //terminalNode.overrideOptions = true;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            nodeListing.Add(terminalNode, commandAction);

            allKeywordsList.Add(terminalKeyword);
            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, bool needsConfirm, bool acceptAnything, string confirmResultName, string denyResultName, string confirmDisplayText, string denyDisplayText, int price, Func<string> commandAction, Func<string> confirmAction, Func<string> denyAction, Dictionary<TerminalNode, Func<string>> nodeListing)
        {
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            CheckForAndDeleteKeyWord(keyWord);

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            terminalNode.itemCost = price;
            terminalNode.overrideOptions = true;
            terminalNode.acceptAnything = acceptAnything;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            if (needsConfirm && (confirmAction != null && denyAction != null))
            {
                MakeConfirmationNode(confirmResultName, denyResultName, confirmAction, denyAction, confirmDisplayText, denyDisplayText, price, nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny);
                terminalNode.terminalOptions = [confirm, deny];
                allKeywordsList.Add(terminalKeyword);
                allKeywordsList.Add(confirm.noun);
                allKeywordsList.Add(deny.noun);
                nodeListing.Add(terminalNode, commandAction);
                suitsTerminal.X($"Node/Keyword added with confirmation nodes for {keyWord}");
            }
            else
            {
                allKeywordsList.Add(terminalKeyword);
                nodeListing.Add(terminalNode, commandAction);
                suitsTerminal.X($"Node/Keyword added without confirmation nodes for {keyWord}");
            }


            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];

        }

        internal static void MakeCommand(string nodeName, string keyWord, string displayText, bool isVerb, bool clearText, bool needsConfirm, bool acceptAnything, string confirmResultName, string denyResultName, string confirmDisplayText, string denyDisplayText, int price, Func<string> commandAction, Func<string> confirmAction, Func<string> denyAction, Dictionary<TerminalNode, string> specialNodeList, Dictionary<TerminalNode, Func<string>> nodeListing)
        {
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            CheckForAndDeleteKeyWord(keyWord);

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = displayText;
            terminalNode.clearPreviousText = clearText;
            terminalNode.itemCost = price;
            terminalNode.overrideOptions = true;
            terminalNode.acceptAnything = acceptAnything;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;
            specialNodeList.Add(terminalNode, keyWord);

            if (needsConfirm && (confirmAction != null && denyAction != null))
            {
                MakeConfirmationNode(confirmResultName, denyResultName, confirmAction, denyAction, confirmDisplayText, denyDisplayText, price, nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny);
                terminalNode.terminalOptions = [confirm, deny];
                allKeywordsList.Add(terminalKeyword);
                allKeywordsList.Add(confirm.noun);
                allKeywordsList.Add(deny.noun);
                nodeListing.Add(terminalNode, commandAction);
                suitsTerminal.X($"Node/Keyword added with confirmation nodes for {keyWord}");
            }
            else
            {
                allKeywordsList.Add(terminalKeyword);
                nodeListing.Add(terminalNode, commandAction);
                suitsTerminal.X($"Node/Keyword added without confirmation nodes for {keyWord}");
            }


            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];

        }

        private static void AddToBuyWord(ref TerminalKeyword buyKeyword, ref TerminalKeyword terminalKeyword, UnlockableItem item)
        {
            terminalKeyword.defaultVerb = buyKeyword;
            suitsTerminal.X($"Added buy verb to {buyKeyword}");
            CompatibleNoun wordIsCompatNoun = new()
            {
                noun = terminalKeyword,
                result = item.shopSelectionNode
            };
            List<CompatibleNoun> buyKeywordList = [.. buyKeyword.compatibleNouns];
            buyKeywordList.Add(wordIsCompatNoun);
            buyKeyword.compatibleNouns = [.. buyKeywordList];

        }

        internal static void MakeStoreCommand(string nodeName, string keyWord, string storeName, bool isVerb, bool clearText, bool acceptAnything, string confirmResultName, string denyResultName, string confirmDisplayText, string denyDisplayText, int price, Func<string> commandAction, Func<string> confirmAction, Dictionary<TerminalNode, Func<string>> nodeListing)
        {
            List<TerminalKeyword> allKeywordsList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];
            CheckForAndDeleteKeyWord(keyWord);

            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.displayText = storeName;
            terminalNode.clearPreviousText = clearText;
            terminalNode.itemCost = price;
            terminalNode.overrideOptions = true;
            terminalNode.acceptAnything = acceptAnything;

            TerminalKeyword terminalKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            terminalKeyword.name = nodeName + "_keyword";
            terminalKeyword.word = keyWord.ToLower();
            terminalKeyword.isVerb = isVerb;
            terminalKeyword.specialKeywordResult = terminalNode;

            if (confirmAction != null)
            {
                MakeConfirmationNode(confirmResultName, denyResultName, confirmAction, confirmDisplayText, denyDisplayText, price, nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny);
                terminalNode.terminalOptions = [confirm, deny];
                UnlockableItem storeItem = AddUnlockable(confirm.result, true, $"{storeName}");
                StartOfRound.Instance.unlockablesList.unlockables.Add(storeItem);
                int unlockableID = StartOfRound.Instance.unlockablesList.unlockables.IndexOf(storeItem);
                terminalNode.shipUnlockableID = unlockableID;
                confirm.result.shipUnlockableID = unlockableID;
                confirm.result.buyUnlockable = true;
                confirm.result.itemCost = price;

                if (TryGetKeyword("buy", out TerminalKeyword buy))
                {
                    AddToBuyWord(ref buy, ref terminalKeyword, storeItem);
                }
                allKeywordsList.Add(terminalKeyword);
                allKeywordsList.Add(confirm.noun);
                allKeywordsList.Add(deny.noun);
                nodeListing.Add(terminalNode, commandAction);
                suitsTerminal.X($"Node/Keyword added with confirmation nodes for {keyWord}");
            }
            else
            {
                suitsTerminal.Log.LogError($"Shop nodes NEED confirmation, but confirmAction is null for {keyWord}!");
            }

            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. allKeywordsList];
        }

        //store confirm
        internal static void MakeConfirmationNode(string confirmResultName, string denyResultName, Func<string> confirmAction, string confirmDisplayText, string denyDisplayText, int price, Dictionary<TerminalNode, Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny)
        {
            confirm = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            confirm.noun.word = "confirm";
            confirm.noun.isVerb = true;

            confirm.result = ScriptableObject.CreateInstance<TerminalNode>();
            confirm.result.name = confirmResultName;
            confirm.result.displayText = confirmDisplayText;
            confirm.result.clearPreviousText = true;
            confirm.result.itemCost = price;
            nodeListing.Add(confirm.result, confirmAction);

            deny = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            deny.noun.word = "deny";
            deny.noun.isVerb = true;

            deny.result = ScriptableObject.CreateInstance<TerminalNode>();
            deny.result.name = denyResultName;
            deny.result.clearPreviousText = true;
            deny.result.displayText = denyDisplayText;
        }

        internal static void MakeConfirmationNode(string confirmResultName, string denyResultName, Func<string> confirmAction, Func<string> denyAction, string confirmDisplayText, string denyDisplayText, int price, Dictionary<TerminalNode, Func<string>> nodeListing, out CompatibleNoun confirm, out CompatibleNoun deny)
        {
            confirm = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            confirm.noun.word = "confirm";
            confirm.noun.isVerb = true;

            confirm.result = ScriptableObject.CreateInstance<TerminalNode>();
            confirm.result.name = confirmResultName;
            confirm.result.displayText = confirmDisplayText;
            confirm.result.clearPreviousText = true;
            confirm.result.itemCost = price;
            nodeListing.Add(confirm.result, confirmAction);


            deny = new CompatibleNoun
            {
                noun = ScriptableObject.CreateInstance<TerminalKeyword>()
            };
            deny.noun.word = "deny";
            deny.noun.isVerb = true;

            deny.result = ScriptableObject.CreateInstance<TerminalNode>();
            deny.result.name = denyResultName;
            deny.result.displayText = denyDisplayText;
            deny.result.clearPreviousText = true;
            nodeListing.Add(deny.result, denyAction);
        }

        internal static void CheckForAndDeleteKeyWord(string keyWord)
        {
            //suitsTerminal.X($"Checking for {keyWord}");
            List<TerminalKeyword> keyWordList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];

            for (int i = keyWordList.Count - 1; i >= 0; i--)
            {
                if (keyWordList[i].word.Equals(keyWord))
                {
                    keyWordList.RemoveAt(i);
                    //suitsTerminal.X($"Keyword: [{keyWord}] removed");
                    break;
                }
            }

            suitsTerminal.Terminal.terminalNodes.allKeywords = [.. keyWordList];
            //suitsTerminal.X("keyword list adjusted");
            return;
        }

        internal static void AddLogicToCommand(TerminalNode terminalNode, Func<string> commandAction, Dictionary<TerminalNode, Func<string>> nodeListing)
        {
            nodeListing.Add(terminalNode, commandAction);
            suitsTerminal.X($"associating {terminalNode.name} to specified commandAction");
        }

        internal static bool TryGetKeyword(string keyWord)
        {
            List<TerminalKeyword> keyWordList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keyWordList)
            {
                if (keyword.word.Equals(keyWord))
                {
                    //suitsTerminal.X($"Keyword: [{keyWord}] found!");
                    return true;
                }
            }

            return false;
        }

        internal static bool TryGetKeyword(string keyWord, out TerminalKeyword terminalKeyword)
        {
            List<TerminalKeyword> keyWordList = [.. suitsTerminal.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keyWordList)
            {
                if (keyword.word.Equals(keyWord))
                {
                    suitsTerminal.X($"Keyword: [{keyWord}] found!");
                    terminalKeyword = keyword;
                    return true;
                }
            }

            terminalKeyword = null;
            return false;
        }

        internal static TerminalNode CreateDummyNode(string nodeName, bool clearPrevious, string displayText)
        {
            TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
            terminalNode.name = nodeName;
            terminalNode.clearPreviousText = clearPrevious;
            terminalNode.displayText = displayText;
            return terminalNode;
        }

        internal static UnlockableItem AddUnlockable(TerminalNode shopNode, bool alwaysInStock, string unlockableName)
        {
            UnlockableItem item = new()
            {
                unlockableType = 1,
                unlockableName = unlockableName,
                shopSelectionNode = shopNode,
                alwaysInStock = alwaysInStock,
                IsPlaceable = false,
                spawnPrefab = false,
                maxNumber = 1
            };
            suitsTerminal.Terminal.ShipDecorSelection.Add(shopNode);
            suitsTermUnlockables.Add(item);
            return item;
        }

        internal static void SetTerminalInput(string terminalInput)
        {
            suitsTerminal.Terminal.TextChanged(suitsTerminal.Terminal.currentText[..^suitsTerminal.Terminal.textAdded] + terminalInput);
            suitsTerminal.Terminal.screenText.text = suitsTerminal.Terminal.currentText;
            suitsTerminal.Terminal.textAdded = terminalInput.Length;
        }
    }
}
