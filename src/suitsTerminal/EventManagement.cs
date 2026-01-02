using OpenLib.Events;

namespace suitsTerminal;

internal class EventManagement
{
    internal static void Subscribe()
    {
        EventManager.TerminalDisable.AddListener(OnTerminalDisable);
        EventManager.TerminalLoadIfAffordable.AddListener(OnLoadAffordable);
        EventManager.TerminalStart.AddListener(OnStartRound);
        EventManager.PlayerSpawn.AddListener(OnPlayerSpawn);
        EventManager.ShipReset.AddListener(OnShipReset);

        //Unique
        OpenLib.TerminalUpdatePatch.usePatch = true; //needed for below event listener
                                                     //EventManager.TerminalKeyPressed.AddListener(AdvancedMenu.OnTerminalKeyPress);

    }

    internal static void OnLoadAffordable(TerminalNode node)
    {
        if (node.shipUnlockableID < 0 || node.shipUnlockableID > StartOfRound.Instance.unlockablesList.unlockables.Count)
            return;

        if (StartOfRound.Instance.unlockablesList.unlockables[node.shipUnlockableID].unlockableType == 0) //suit purchase detected
        {
            Loggers.LogDebug("new suit detected!");
            //RackManager.FixRack();
        }
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
        //RackManager.AllSuits.Clear();
        ResetSuitPlacementVars();
        
    }

    internal static void ResetSuitPlacementVars()
    {
        RackManager.RackSetupComplete = false;
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
        //AdvancedMenu.CaretOriginal = Plugin.Terminal.screenText.caretColor;
        
        if (RackManager.RackSetupComplete) //the below should only be run once per save
            return;

        Loggers.LogDebug("RackSetupComplete is FALSE");
        RackManager.RackLaunch();
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
