using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace suitsTerminal
{
    internal class CommandStuff
    {
        public static Dictionary<TerminalNode, Func<string>> sT = [];

        internal static bool GetNewDisplayText(ref TerminalNode node)
        {
            if (node == null)
                return false;

            if (sT.TryGetValue(node, out Func<string> newDisplayText))
            {
                node.displayText = newDisplayText();
                return true;
            }
            else
            {
                suitsTerminal.X("Not in special nodeListing dictionary");
                return false;
            }

        }

        internal static TerminalNode GetFromAllNodes(string nodeName)
        {
            List<TerminalNode> allNodes = new(Object.FindObjectsOfType<TerminalNode>(true));

            foreach (TerminalNode node in allNodes)
            {
                if (node.name == nodeName)
                {
                    suitsTerminal.X($"{nodeName} found!");
                    return node;
                }
            }

            suitsTerminal.X($"{nodeName} could not be found, result set to null.");

            return null;
        }
    }
}
