
namespace suitsTerminal.Compatibility
{
    internal class TerminalStuffMod
    {
        internal static void NetSync(TerminalNode node)
        {
            if (node == null)
                return;

            if (!Plugin.TerminalStuff)
                return;

            if (!TerminalStuff.ConfigSettings.NetworkedNodes.Value || !TerminalStuff.ConfigSettings.ModNetworking.Value)
                return;

            Plugin.X($"Syncing node with TerminalStuff");
            TerminalStuff.EventSub.TerminalParse.NetSync(node);
        }
    }
}
