using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace suitsTerminal
{
    internal class Enums
    {
        internal static IEnumerator ChatHints()
        {
            if (!SConfig.chatHints.Value)
                yield break;

            yield return new WaitForSeconds(5);
            suitsTerminal.X("hint in chat.");

            if (SConfig.terminalCommands.Value)
                HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]: Access more suits by typing 'suits' in the terminal.");

            if (SConfig.chatCommands.Value)
                HUDManager.Instance.AddTextToChatOnServer($"[suitsTerminal]: Access more suits by typing '!suits' in chat.");
        }

        internal static IEnumerator HudHints()
        {
            if (!SConfig.bannerHints.Value)
                yield break;

            yield return new WaitForSeconds(20);
            suitsTerminal.X("hint on hud.");
            if (SConfig.suitsOnRack.Value > 0 && !SConfig.dontRemove.Value && SConfig.terminalCommands.Value && !SConfig.chatCommands.Value)
            {
                HUDManager.Instance.DisplayTip("Suits Access", "Excess suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
            }
            else if (SConfig.suitsOnRack.Value == 0 && !SConfig.dontRemove.Value && SConfig.terminalCommands.Value && !SConfig.chatCommands.Value)
            {
                HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
            }
            else if (SConfig.suitsOnRack.Value == 0 && !SConfig.dontRemove.Value && SConfig.terminalCommands.Value && SConfig.chatCommands.Value)
            {
                HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal or !suits in the chat to access them and change your suit!", false, false, "suitsTerminal-Hint");
            }
            else if (SConfig.suitsOnRack.Value > 0 && !SConfig.dontRemove.Value && SConfig.terminalCommands.Value && SConfig.chatCommands.Value)
            {
                HUDManager.Instance.DisplayTip("Suits Access", "Excess suits have been moved to the terminal for storage. Use command 'suits' in the terminal or !suits in the chat to access them and change your suit!", false, false, "suitsTerminal-Hint");
            }
        }
    }
}
