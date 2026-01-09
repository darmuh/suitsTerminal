using System.Collections;
using UnityEngine;

namespace suitsTerminal;

internal class Coroutines
{
    internal static IEnumerator DelayStartOnReset()
    {
        RackManager.RealCurrentID = 0;
        Menu.specialMenusActive = false;
        Loggers.LogDebug("ShipResetStuff!");
        Plugin.EquipDefault = true;
        yield return SetDefaultSuit();
    }

    internal static IEnumerator SetDefaultSuit()
    {
        if (!Plugin.EquipDefault)
            yield break;

        while (Plugin.LocalPlayer.inSpecialInteractAnimation)
            yield return new WaitForSeconds(1f);

        Debug.Log("Setting default suit!");
        SuitAttributes.WearDefault();
    }

    internal static IEnumerator ChatHints()
    {
        if (ModConfig.HintStyle.Value != ModConfig.Hint.ChatOnly && ModConfig.HintStyle.Value != ModConfig.Hint.AllHints)
            yield break;

        yield return new WaitForSeconds(5);
        if (HUDManager.Instance == null)
        {
            Loggers.WARNING("Unable to hint on chat! HUDManager instance is null!");
            yield break;
        }

        Loggers.LogMessage("displaying hint in chat.");

        HUDManager.Instance.AddChatMessage($"[suitsTerminal]: Access more suits by typing 'suits' in the terminal.");

        if (ModConfig.ChatCommands.Value)
            HUDManager.Instance.AddChatMessage($"[suitsTerminal]: Access more suits by typing '!suits' in chat.");
    }

    internal static IEnumerator HudHints()
    {
        if (ModConfig.HintStyle.Value != ModConfig.Hint.BannerOnly && ModConfig.HintStyle.Value != ModConfig.Hint.AllHints)
            yield break;

        yield return new WaitForSeconds(20);
        Loggers.LogDebug("hint on hud.");

        bool rackExists = ModConfig.RackSettings.Value != ModConfig.Removal.OnlyRackAndExtraSuits && ModConfig.RackSettings.Value != ModConfig.Removal.Everything;

        if (ModConfig.SuitsOnRack.Value > 0 && rackExists && !ModConfig.ChatCommands.Value)
        {
            HUDManager.Instance.DisplayTip("Suits Access", "Excess suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
        }
        else if (ModConfig.SuitsOnRack.Value == 0 && !rackExists && !ModConfig.ChatCommands.Value)
        {
            HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal to access them and change your suit!", false, false, "suitsTerminal-Hint");
        }
        else if (ModConfig.SuitsOnRack.Value == 0 && !rackExists && ModConfig.ChatCommands.Value)
        {
            HUDManager.Instance.DisplayTip("Suits Access", "All suits have been moved to the terminal for storage. Use command 'suits' in the terminal or !suits in the chat to access them and change your suit!", false, false, "suitsTerminal-Hint");
        }
        else if (ModConfig.SuitsOnRack.Value > 0 && rackExists && ModConfig.ChatCommands.Value)
        {
            HUDManager.Instance.DisplayTip("Suits Access", "Excess suits have been moved to the terminal for storage. Use command 'suits' in the terminal or !suits in the chat to access them and change your suit!", false, false, "suitsTerminal-Hint");
        }
    }
}
