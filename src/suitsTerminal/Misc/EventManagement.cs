using OpenLib.Events;
using suitsTerminal.OfTerminal;
using suitsTerminal.Suits;
using suitsTerminal.Util;

namespace suitsTerminal.Misc;

internal class EventManagement
{
    internal static void Subscribe()
    {
        EventManager.TerminalDisable.AddListener(OnTerminalDisable);
        EventManager.TerminalStart.AddListener(OnStartRound);
        EventManager.PlayerSpawn.AddListener(OnPlayerSpawn);
        EventManager.ShipReset.AddListener(OnShipReset);

        //Unique
        OpenLib.TerminalUpdatePatch.usePatch = true; //needed for below event listener
                                                     //EventManager.TerminalKeyPressed.AddListener(AdvancedMenu.OnTerminalKeyPress);

    }

    private static void OnShipReset()
    {
        StartOfRound.Instance.StartCoroutine(Coroutines.DelayStartOnReset());
    }

    private static void OnTerminalDisable()
    {
        Plugin.HintOnce = false;
        PictureInPicture.PiPCreated = false;
        Menu.specialMenusActive = false;
        Menu.PotentialSelection = null!;
        Plugin.EquipDefault = true;
        Loggers.LogDebug("set initial variables");
        ResetSuitPlacementVars();

    }

    internal static void ResetSuitPlacementVars()
    {
        RackManager.RealCurrentID = 0;
        if (RackManager.AllSuits.Count == 0)
            return;

        RackManager.AllSuits.ForEach(s => s.Reset());
    }

    private static void OnStartRound()
    {
        Menu.InitBetterMenu();
        RackManager.InitFavoritesListing();
    }

    private static void OnPlayerSpawn()
    {
        Loggers.LogDebug("OnPlayerSpawn");
        if (Plugin.HintOnce)
            return;
        Loggers.LogDebug("First spawn detected!");
        RackManager.HideBootsAndRack();
        Plugin.ShowHint();
        PiPStuff();
        Plugin.Terminal.StartCoroutine(Coroutines.SetDefaultSuit());
    }

    private static void PiPStuff()
    {
        PictureInPicture.ShadowDefault = Plugin.LocalPlayer.thisPlayerModel.shadowCastingMode;
        PictureInPicture.ModelLayerDefault = Plugin.LocalPlayer.thisPlayerModel.gameObject.layer;
    }
}
