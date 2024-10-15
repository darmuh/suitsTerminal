using System.Collections;
using UnityEngine;

namespace suitsTerminal
{
    internal class Enums
    {
        internal static IEnumerator ChatHints()
        {
            if (!SConfig.ChatHints.Value)
                yield break;

            yield return new WaitForSeconds(5);
            Plugin.X("hint in chat.");

            if (SConfig.TerminalCommands.Value)
                HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]: Access more suits by typing 'suits' in the terminal.");

            if (SConfig.ChatCommands.Value)
                HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]: Access more suits by typing '!suits' in chat.");
        }

        internal static IEnumerator HudHints()
        {
            if (!SConfig.BannerHints.Value)
                yield break;

            yield return new WaitForSeconds(20);
            Plugin.X("hint on hud.");
            if (SConfig.SuitsOnRack.Value > 0 && !SConfig.DontRemove.Value && SConfig.TerminalCommands.Value && !SConfig.ChatCommands.Value)
            {
                HUDManager.Instance.DisplayTip("Suits Access", "Excess suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
            }
            else if (SConfig.SuitsOnRack.Value == 0 && !SConfig.DontRemove.Value && SConfig.TerminalCommands.Value && !SConfig.ChatCommands.Value)
            {
                HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
            }
            else if (SConfig.SuitsOnRack.Value == 0 && !SConfig.DontRemove.Value && SConfig.TerminalCommands.Value && SConfig.ChatCommands.Value)
            {
                HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal or !suits in the chat to access them and change your suit!", false, false, "suitsTerminal-Hint");
            }
            else if (SConfig.SuitsOnRack.Value > 0 && !SConfig.DontRemove.Value && SConfig.TerminalCommands.Value && SConfig.ChatCommands.Value)
            {
                HUDManager.Instance.DisplayTip("Suits Access", "Excess suits have been moved to the terminal for storage. Use command 'suits' in the terminal or !suits in the chat to access them and change your suit!", false, false, "suitsTerminal-Hint");
            }
        }
    }
}
