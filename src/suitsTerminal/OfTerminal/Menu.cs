using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenLib.Common;
using OpenLib.CoreMethods;
using OpenLib.InteractiveMenus;
using suitsTerminal.Suits;
using suitsTerminal.Util;
using UnityEngine;
using static suitsTerminal.Misc.ModConfig;
using static suitsTerminal.Misc.PictureInPicture;
using static suitsTerminal.Suits.RackManager;
using Key = UnityEngine.InputSystem.Key;


namespace suitsTerminal.OfTerminal;

internal class Menu
{
    //BetterMenu Core
    internal static BetterMenu<SuitMenuItem> SuitsMenu = null!;
    internal static Dictionary<Key, Action> ExtraKeyActions = [];
    internal static bool InitOnce = false;

    //Main Pages
    internal static SuitMenuItem HomePage = null!;
    internal static SuitMenuItem FavoritesList = null!;
    internal static SuitMenuItem SuitsList = null!;
    internal static SuitMenuItem HelpPage = null!;

    //MenuItem Pages
    internal static SuitMenuItem SelectSuit = null!;
    internal static SuitMenuItem FavoriteSuit = null!;
    internal static SuitMenuItem SetDefaultSuit = null!;
    internal static SuitMenuItem PurchaseSuitFromStore = null!; //this will probably never be used since suits dont exist till they are bought
    internal static SuitMenuItem SetRandomSuit = null!;

    //Commands
    internal static CommandManager Main = null!;
    internal static CommandManager FavListing = null!;
    internal static CommandManager SuitListing = null!;

    //Tracking between pages
    internal static SuitAttributes PotentialSelection = null!;

    //Old stuff still used
    internal static string togglePiPstring = string.Empty;
    internal static string pipHeightString = string.Empty;
    internal static string pipRotateString = string.Empty;
    internal static string pipZoomString = string.Empty;

    internal static List<MenuItem> CurrentNest = [];

    internal static bool initKeySettings = false;

    //used by terminalstuff, have to leave this here for now until everyone migrates to new version
    public static Color CaretOriginal = Color.green;
    //used by terminalstuff and maybe other mods, have to leave here for a little while
    public static bool specialMenusActive = false;

    internal static void InitBetterMenu()
    {
        if (initKeySettings)
            return;

        initKeySettings = true;
        SetupExtraKeys();
        Loggers.LogDebug("Loading keybinds from config");
        InitOneTime();
        UpdateMainKeys();

        initKeySettings = false;

    }
    private static void InitOneTime()
    {
        if (InitOnce)
            return;
        Plugin.Log.LogMessage("InitOneTime for BetterMenu stuff!");
        AddHintToOther();
        SuitsMenu = new("suitsTerminal", ExtraKeyActions)
        {
            PageSize = 10
        };
        SuitsMenu.OnEnter.AddListener(OnEnterStuff);
        SuitsMenu.OnExit.AddListener(OnExitStuff);
        InitMenuListing();
        InitOnce = true;
    }

    private static void MainMenuStuff()
    {
        TogglePicture(false);
    }

    private static void InitMenuListing()
    {
        HomePage = new("suitsTerminal Home")
        {
            Header = () => $"============= suitsTerminal Menu  =============\r\n",
            Footer = MainFooter,
            OnPageLoad = MainMenuStuff
        };
        SuitsMenu.MainMenu = HomePage;
        SuitsMenu.OnExit.AddListener(RemovePreview);
        FavoritesList = new("Favorites")
        {
            ShowIfEmptyNest = false,
            Header = () => $"============= Favorite Suits  =============\r\n",
            Footer = GetFooter
        };
        FavoritesList.SelectionEvent.AddListener(ShowFavs);
        FavoritesList.SetParentMenu(HomePage);
        SuitsList = new("Change Suits")
        {
            Header = () => $"============= Select a Suit!  =============\r\n",
            Footer = GetFooter
        };

        SuitsList.SelectionEvent.AddListener(ShowNormal);
        SuitsList.SetParentMenu(HomePage);

        SetRandomSuit = new("Random Suit");
        SetRandomSuit.SelectionEvent.AddListener(RandomSuitPage);
        SetRandomSuit.SetParentMenu(HomePage);

        HelpPage = new("Help Page");
        HelpPage.SelectionEvent.AddListener(ShowHelpPage);
        HelpPage.SetParentMenu(HomePage);

        SuitsList.AdjustNestedMenuList.AddListener(SortSuits);
        FavoritesList.AdjustNestedMenuList.AddListener(SortSuits);

        SelectSuit = new("Select Suit")
        {
            OnPageLoad = () =>
            {
                SelectSuit.Name = $"Wear {PotentialSelection.Name}";
            },
        };
        SelectSuit.SelectionEvent.AddListener(() => PotentialSelection.WearSuit());

        FavoriteSuit = new("Favorite Suit")
        {
            OnPageLoad = () =>
            {
                if (PotentialSelection.IsFav)
                    FavoriteSuit.Name = $"Remove Favorite {PotentialSelection.Name}";
                else
                    FavoriteSuit.Name = $"Add Favorite {PotentialSelection.Name}";
            }
        };
        FavoriteSuit.SelectionEvent.AddListener(() => PotentialSelection.ToggleFav());

        SetDefaultSuit = new("Set Default")
        {
            OnPageLoad = () =>
            {
                SuitAttributes.UpdateDefault = true;
                if (PotentialSelection.IsDefault())
                    SetDefaultSuit.Name = $"Remove {PotentialSelection.Name} as Default";
                else
                    SetDefaultSuit.Name = $"Make {PotentialSelection.Name} the default suit";
            }
        };
        SetDefaultSuit.SelectionEvent.AddListener(() => PotentialSelection.ToggleDefault());

        PurchaseSuitFromStore = new("Purchase from store")
        {
            OnPageLoad = () =>
            {
                var UnlockableItems = StartOfRound.Instance.unlockablesList.unlockables;
                PurchaseSuitFromStore.Name = $"Purchase {PotentialSelection.Name} from store {UnlockableItems[PotentialSelection.MenuItem.SuitProps.Suit.syncedSuitID.Value].shopSelectionNode.itemCost}";
            }
        };

        //probably will never be used but cool to keep for potential future updates
        PurchaseSuitFromStore.SelectionEvent.AddListener(() =>
        {
            var UnlockableItems = StartOfRound.Instance.unlockablesList.unlockables;
            SuitsMenu.ExitAction = () => CommonTerminal.LoadNewNode(UnlockableItems[PotentialSelection.MenuItem.SuitProps.Suit.syncedSuitID.Value].shopSelectionNode);
            SuitsMenu.ExitMenu(true);
        });

    }

    private static void SortSuits(ref List<MenuItem> list)
    {
        if (SuitsSortingStyle.Value == Sort.Alphabetical)
        {
            list = [.. list.Cast<SuitMenuItem>().OrderBy(x => x.Name)];
        }
        else if (SuitsSortingStyle.Value == Sort.Numerical)
        {
            list = [.. list.Cast<SuitMenuItem>().OrderBy(x => x.SuitProps.ID)];
        }

        CurrentNest = list;
    }

    private static void ReplaceKey(ref Key current, Key newKey, ref int replacements)
    {
        current = newKey;
        replacements++;
    }

    internal static void RefreshKeys()
    {
        if (SuitsMenu.MenuNode == null)
            return;

        int replacements = 0;

        if (IsValidReplacement(MenuUp.Value, SuitsMenu.upMenu, out Key upKey))
            ReplaceKey(ref SuitsMenu.upMenu, upKey, ref replacements);
        if (IsValidReplacement(MenuDown.Value, SuitsMenu.downMenu, out Key downKey))
            ReplaceKey(ref SuitsMenu.downMenu, downKey, ref replacements);
        if (IsValidReplacement(MenuLeft.Value, SuitsMenu.leftMenu, out Key leftKey))
            ReplaceKey(ref SuitsMenu.leftMenu, leftKey, ref replacements);
        if (IsValidReplacement(MenuRight.Value, SuitsMenu.rightMenu, out Key rightKey))
            ReplaceKey(ref SuitsMenu.rightMenu, rightKey, ref replacements);
        if (IsValidReplacement(LeaveMenu.Value, SuitsMenu.leaveMenu, out Key leaveKey))
            ReplaceKey(ref SuitsMenu.leaveMenu, leaveKey, ref replacements);
        if (IsValidReplacement(SelectMenu.Value, SuitsMenu.selectMenu, out Key selectKey))
            ReplaceKey(ref SuitsMenu.selectMenu, selectKey, ref replacements);

        if (replacements > 0)
            SuitsMenu.UpdateMainActions();

        if (IsAnyExtraKeyDifferent())
        {
            SetupExtraKeys();
            SuitsMenu.OtherActions = ExtraKeyActions;
        }
    }

    private static bool IsAnyExtraKeyDifferent()
    {

        if (CamStyle.Value == PiP.Disabled)
            return false;

        if (!pipHeightString.Equals(TogglePiPHeight.Value, StringComparison.InvariantCultureIgnoreCase))
            return true;

        if (!pipRotateString.Equals(TogglePiPRotation.Value, StringComparison.InvariantCultureIgnoreCase))
            return true;

        if (!pipZoomString.Equals(TogglePiPZoom.Value, StringComparison.InvariantCultureIgnoreCase))
            return true;

        if (!togglePiPstring.Equals(TogglePiP.Value, StringComparison.InvariantCultureIgnoreCase))
            return true;

        return false;
    }

    internal static void CreateBetterCommand()
    {
        Main = new("suitsTerminal Main Menu", MainMenuKWs.GetKeywordValues(isMain: true), CommandHandler.MainSuitsMenu);
        FavListing = new("suitsTerminal Favorites Listing", FavMenuKWs.GetKeywordValues(), CommandHandler.FavsSuitsMenu);
        SuitListing = new("suitsTerminal Suits Listing", SuitsListingKWs.GetKeywordValues(), CommandHandler.SuitsListMenu);
    }

    internal static void AddHintToOther()
    {
        if (LogicHandling.TryGetFromAllNodes("OtherCommands", out TerminalNode otherNode))
        {
            AddingThings.AddToExistingNodeText($"\n>SUITS\nsuitsTerminal advanced menu for changing & viewing suits", ref otherNode);
        }
    }

    private static void SetupExtraKeys()
    {
        ExtraKeyActions = [];
        PiPKeys();
    }

    private static void UpdateMainKeys()
    {
        if (IsValidReplacement(MenuUp.Value, SuitsMenu.upMenu, out Key upKey))
            SuitsMenu.upMenu = upKey;
        if (IsValidReplacement(MenuDown.Value, SuitsMenu.downMenu, out Key downKey))
            SuitsMenu.downMenu = downKey;
        if (IsValidReplacement(MenuLeft.Value, SuitsMenu.leftMenu, out Key leftKey))
            SuitsMenu.leftMenu = leftKey;
        if (IsValidReplacement(MenuRight.Value, SuitsMenu.rightMenu, out Key rightKey))
            SuitsMenu.rightMenu = rightKey;
        if (IsValidReplacement(LeaveMenu.Value, SuitsMenu.leaveMenu, out Key leaveKey))
            SuitsMenu.leaveMenu = leaveKey;
        if (IsValidReplacement(SelectMenu.Value, SuitsMenu.selectMenu, out Key selectKey))
            SuitsMenu.selectMenu = selectKey;

        SuitsMenu.UpdateMainActions();
    }

    internal static string GetFooter()
    {
        string suit = GetEquippedSuitName();
        StringBuilder message = new();
        message.Append($"\r\n\r\n\r\n\r\nCurrently Wearing: {suit}\r\n\r\n");
        message.Append($"Page [{SuitsMenu.leftMenu}] < {SuitsMenu.CurrentPage}/{Mathf.CeilToInt((float)SuitsMenu.DisplayMenuItemsOfType.Count / SuitsMenu.PageSize)} > [{SuitsMenu.rightMenu}]\r\n");
        message.Append($"Leave Menu: [{SuitsMenu.leaveMenu}] Select Suit: [{SuitsMenu.selectMenu}]\r\n");
        return message.ToString();
    }

    internal static string MainFooter()
    {
        StringBuilder message = new();
        message.Append($"\r\n\r\n\r\n\r\nPage [{SuitsMenu.leftMenu}] < {SuitsMenu.CurrentPage}/{Mathf.CeilToInt((float)SuitsMenu.DisplayMenuItemsOfType.Count / SuitsMenu.PageSize)} > [{SuitsMenu.rightMenu}]\r\n");
        message.Append($"Leave Menu: [{SuitsMenu.leaveMenu}] Select Item: [{SuitsMenu.selectMenu}]\r\n");
        return message.ToString();
    }

    private static string GetEquippedSuitName()
    {
        if (CurrentSuit == null)
            return string.Empty;

        return CurrentSuit.Name;
    }

    private static void BindKeys(Action menuAction, Key givenKey, ref string givenKeyString, string defaultKeyString, Key defaultKey)
    {
        Loggers.LogDebug($"Binding {menuAction}");
        if (givenKey != Key.None)
        {
            ExtraKeyActions.Add(givenKey, menuAction);
            Loggers.LogDebug($"{givenKeyString} bound");
        }
        else
        {
            ExtraKeyActions.Add(defaultKey, menuAction);
            givenKeyString = defaultKeyString;
            Loggers.LogDebug($"{givenKeyString} bound");
        }
    }

    private static void CheckKeys(string configString, out Key usingKey, out string keyString)
    {
        if (IsValidKey(configString, out Key validKey))
        {
            usingKey = validKey;
            keyString = validKey.ToString();
        }
        else
        {
            usingKey = Key.None;
            keyString = "FAIL";
        }
    }

    private static void PiPKeys()
    {
        if (CamStyle.Value == PiP.Disabled)
            return;

        CheckKeys(TogglePiPHeight.Value, out Key pipHeight, out pipHeightString);
        BindKeys(PipHeight, pipHeight, ref pipHeightString, "Backslash", Key.Backslash);

        CheckKeys(TogglePiPRotation.Value, out Key pipRotate, out pipRotateString);
        BindKeys(PipRotate, pipRotate, ref pipRotateString, "Equals", Key.Equals);

        CheckKeys(TogglePiPZoom.Value, out Key pipZoom, out pipZoomString);
        BindKeys(PipZoom, pipZoom, ref pipZoomString, "Minus", Key.Minus);

        CheckKeys(TogglePiP.Value, out Key pipKey, out togglePiPstring);
        BindKeys(PipAction, pipKey, ref togglePiPstring, "F12", Key.F12);

    }

    private static bool IsValidReplacement(string key, Key original, out Key validKey)
    {
        List<Key> invalidKeys = [
        Key.Tab
        ];
        if (Enum.TryParse(key, ignoreCase: true, out Key keyFromString))
        {
            if (original == keyFromString)
            {
                validKey = original;
                return false;
            }

            if (invalidKeys.Contains(keyFromString))
            {
                Loggers.WARNING("Tab Key detected, rejecting bind.");
                validKey = Key.None;
                return false;
            }
            else if (SuitsMenu.MainActions.ContainsKey(keyFromString))
            {
                Loggers.WARNING("Key was already bound to something, returning false");
                string allKeys = string.Join(", ", SuitsMenu.MainActions.Keys);
                Loggers.WARNING($"Key list: {allKeys}");
                validKey = Key.None;
                return false;
            }
            else
            {
                Loggers.LogDebug("Valid Key Detected and being assigned");
                validKey = keyFromString;
                return true;
            }
        }
        else
        {
            validKey = Key.None;
            return false;
        }
    }

    private static bool IsValidKey(string key, out Key validKey)
    {
        List<Key> invalidKeys = [
        Key.Tab
        ];
        if (Enum.TryParse(key, ignoreCase: true, out Key keyFromString))
        {
            if (invalidKeys.Contains(keyFromString))
            {
                Loggers.WARNING("Tab Key detected, rejecting bind.");
                validKey = Key.None;
                return false;
            }
            else if (ExtraKeyActions.ContainsKey(keyFromString))
            {
                Loggers.WARNING("Key was already bound to something, returning false");
                string allKeys = string.Join(", ", ExtraKeyActions.Keys);
                Loggers.WARNING($"Key list: {allKeys}");
                validKey = Key.None;
                return false;
            }
            else
            {
                Loggers.LogDebug("Valid Key Detected and being assigned to bind");
                validKey = keyFromString;
                return true;
            }
        }
        else
        {
            validKey = Key.None;
            return false;
        }

    }

    private static Camera GetCam()
    {
        if (OpenLib.Plugin.instance.OpenBodyCamsMod && CamStyle.Value == PiP.OpenBodyCams)
        {
            Loggers.LogDebug("Returning Cam from OpenLib OpenBodyCams Compat!");
            return OpenLib.Compat.OpenBodyCamFuncs.GetCam(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
        }
        else
        {
            PlayerCam = CamStuff.MyCameraHolder.GetComponent<Camera>();
            return PlayerCam!;
        }

    }

    private static void PipAction()
    {
        TogglePicture(!PipActive);
        Loggers.LogDebug($"Toggling PiP to state {!PipActive}");
    }

    private static void PipHeight()
    {
        if (!PipActive)
            return;

        Camera currentCam = GetCam();

        if (currentCam == null)
            return;

        MoveCamera(currentCam.transform, ref HeightStep);
        Loggers.LogDebug($"Changing PiP height to {HeightStep}");
    }

    private static void PipRotate()
    {
        if (!PipActive)
            return;

        Camera currentCam = GetCam();

        if (currentCam == null)
            return;

        RotateCameraAroundPlayer(Plugin.LocalPlayer.meshContainer, currentCam.transform);
        Loggers.LogDebug($"Rotating PiP around player");
    }

    private static void PipZoom()
    {
        if (!PipActive)
            return;

        Camera currentCam = GetCam();

        if (currentCam == null)
            return;

        ChangeCamZoom(currentCam, ref ZoomStep);
        Loggers.LogDebug($"Changing PiP zoom to zoomStep: [{ZoomStep}]");
    }

    private static void RandomSuitPage()
    {
        SuitsMenu.MenuNode.displayText = CommandHandler.RandomSuit();
        SuitsMenu.AcceptAnything = true;
        SuitsMenu.Load();
        Plugin.Terminal.StartCoroutine(SuitsMenuShowPicture(isCentered: true));
    }

    private static void ShowHelpPage()
    {
        //inHelpMenu = true;
        SuitsMenu.MenuNode.displayText = HelpMenuDisplay();
        SuitsMenu.AcceptAnything = true;
        SuitsMenu.Load();
        TogglePicture(false);
    }

    private static void ShowFavs()
    {
        if (FavoritesList.NestedMenus.Count < 1)
        {
            Plugin.Terminal.PlayTerminalAudioServerRpc(1);
            Loggers.LogInfo("Empty favorites menu! Playing error audio for user");
            return;
        }

        Plugin.Terminal.StartCoroutine(SuitsMenuShowPicture());
    }

    private static void ShowNormal()
    {
        Plugin.Terminal.StartCoroutine(SuitsMenuShowPicture());
    }

    private static void OnExitStuff()
    {
        TogglePicture(false);
    }

    private static void OnEnterStuff()
    {
        RotateStep = 0;
        HeightStep = 0;
        ZoomStep = 1;
        SetRandomSuit.ShowIfEmptyNest = RandomSuitMenu.Value;
    }

    internal static IEnumerator SuitsMenuShowPicture(bool isCentered = false)
    {
        yield return new WaitForEndOfFrame();
        TogglePicture(true, isCentered);
        yield break;
    }

    internal static string HelpMenuDisplay()
    {
        Loggers.LogDebug("Help Menu Enabled, showing help information");
        StringBuilder message = new();

        message.Append($"========= AdvancedsuitsMenu Help Page  =========\r\n");
        message.Append("\r\n\r\n");

        message.Append($"Highlight Next Item: [{SuitsMenu.downMenu}]\r\nHighlight Last Item: [{SuitsMenu.upMenu}]\r\n");
        if (CamStyle.Value != PiP.Disabled)
        {
            message.Append($"Toggle Camera Preview: [{togglePiPstring}]\r\nRotate Camera: [{pipRotateString}]\r\nChange Camera Height: [{pipHeightString}]\r\nChange Camera Zoom: [{pipZoomString}]\r\n");
        }
        message.Append($"Leave Suits Menu: [{SuitsMenu.leaveMenu}]\r\nSelect Suit: [{SuitsMenu.selectMenu}]\r\n");
        message.Append($"\r\n>>>\tReturn to Menu: [ANY KEY]\t<<\r\n");
        return message.ToString();
    }
}
