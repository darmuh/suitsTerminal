using OpenLib.Common;
using OpenLib.CoreMethods;
using OpenLib.Events;
using OpenLib.InteractiveMenus;
using suitsTerminal.Suit_Stuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static suitsTerminal.AllSuits;
using static suitsTerminal.Misc;
using static suitsTerminal.PictureInPicture;
using static suitsTerminal.SConfig;
using Key = UnityEngine.InputSystem.Key;

namespace suitsTerminal
{
    public class AdvancedMenu
    {
        //BetterMenu Core
        internal static BetterMenu<SuitMenuItem> suitsMenu = null!;
        internal static Dictionary<Key, Action> ExtraKeyActions = [];
        internal static bool InitOnce = false;

        //BetterMenu Pages
        internal static SuitMenuItem HomePage = null!;
        internal static SuitMenuItem FavoritesList = null!;
        internal static SuitMenuItem SuitsList = null!;
        internal static SuitMenuItem HelpPage = null!;

        //BetterMenu Page Events
        internal static Events.CustomEvent OpenFavs = new();
        internal static Events.CustomEvent ShowAllSuits = new();
        internal static Events.CustomEvent ShowHelp = new();

        //BetterMenu Misc
        internal static CommandManager Command = null!;

        //Old stuff still used
        internal static string favItemKeyString = string.Empty;
        internal static string togglePiPstring = string.Empty;
        internal static string pipHeightString = string.Empty;
        internal static string pipRotateString = string.Empty;
        internal static string pipZoomString = string.Empty;

        

        private static TerminalNode menuDisplay = null!;

        internal static bool initKeySettings = false;

        //used by terminalstuff, have to leave this here for now until everyone migrates to new version
        public static Color CaretOriginal = Color.green;
        //used by terminalstuff and maybe other mods, have to leave here for a little while
        public static bool specialMenusActive = false;


        internal static void InitBetterMenu()
        {
            if (initKeySettings || !AdvancedTerminalMenu.Value)
                return;

            initKeySettings = true;
            SetupExtraKeys();
            Plugin.X("Loading keybinds from config");
            if (menuDisplay == null)
                menuDisplay = Command.terminalNode;
            InitOneTime();
            UpdateMainKeys();

            suitsMenu.MenuNode = menuDisplay!;
            
            initKeySettings = false;

        }

        private static void InitOneTime()
        {
            if (InitOnce)
                return;
            Plugin.Log.LogMessage("InitOneTime for BetterMenu stuff!");
            AddHintToOther();
            suitsMenu = new("suitsTerminal", ExtraKeyActions);
            suitsMenu.PageSize = 10;
            suitsMenu.OnEnter.AddListener(OnEnterStuff);
            suitsMenu.OnExit.AddListener(OnExitStuff);
            InitMenuListing();
            InitOnce = true;
        }

        private static void MainMenuStuff()
        {
            TogglePiP(false);
        }

        private static void InitMenuListing()
        {
            HomePage = new("suitsTerminal Home");
            HomePage.Header = () => $"============= AdvancedsuitsMenu  =============\r\n";
            HomePage.Footer = MainFooter;
            HomePage.OnPageLoad = MainMenuStuff;
            suitsMenu.MainMenu = HomePage;
            FavoritesList = new("Favorites", OpenFavs);
            FavoritesList.ShowIfEmptyNest = false;
            FavoritesList.Header = () => $"============= Favorite Suits  =============\r\n";
            FavoritesList.Footer = GetFooter;
            OpenFavs.AddListener(ShowFavs);
            FavoritesList.SetParentMenu(HomePage);
            SuitsList = new("Change Suits", ShowAllSuits);
            SuitsList.Header = () => $"============= Select a Suit!  =============\r\n";
            SuitsList.Footer = GetFooter;
            ShowAllSuits.AddListener(ShowNormal);
            SuitsList.SetParentMenu(HomePage);
            HelpPage = new("Help Page", ShowHelp);
            ShowHelp.AddListener(ShowHelpPage);
            HelpPage.SetParentMenu(HomePage);

            SuitMenuItem.AddListToBetterMenu([HomePage, FavoritesList, SuitsList, HelpPage]);
        }

        private static void ReplaceKey(ref Key current, Key newKey, ref int replacements)
        {
            current = newKey;
            replacements++;
        }

        internal static void RefreshKeys()
        {
            if (suitsMenu.MenuNode == null)
                return;

            int replacements = 0;

            if (IsValidReplacement(MenuUp.Value, suitsMenu.upMenu, out Key upKey))
                ReplaceKey(ref suitsMenu.upMenu, upKey, ref replacements);
            if (IsValidReplacement(MenuDown.Value, suitsMenu.downMenu, out Key downKey))
                ReplaceKey(ref suitsMenu.downMenu, downKey, ref replacements);
            if (IsValidReplacement(MenuLeft.Value, suitsMenu.leftMenu, out Key leftKey))
                ReplaceKey(ref suitsMenu.leftMenu, leftKey, ref replacements);
            if (IsValidReplacement(MenuRight.Value, suitsMenu.rightMenu, out Key rightKey))
                ReplaceKey(ref suitsMenu.rightMenu, rightKey, ref replacements);
            if (IsValidReplacement(LeaveMenu.Value, suitsMenu.leaveMenu, out Key leaveKey))
                ReplaceKey(ref suitsMenu.leaveMenu, leaveKey, ref replacements);
            if (IsValidReplacement(SelectMenu.Value, suitsMenu.selectMenu, out Key selectKey))
                ReplaceKey(ref suitsMenu.selectMenu, selectKey, ref replacements);

            if (replacements > 0)
                suitsMenu.UpdateMainActions();

            if (IsAnyExtraKeyDifferent())
            {
                SetupExtraKeys();
                suitsMenu.OtherActions = ExtraKeyActions;
            }

        }

        private static bool IsAnyExtraKeyDifferent()
        {
            if (favItemKeyString.ToLowerInvariant() != FavItemKey.Value.ToLowerInvariant())
                return true;

            if (!EnablePiPCamera.Value)
                return false;

            if (pipHeightString.ToLowerInvariant() != TogglePiPHeight.Value.ToLowerInvariant())
                return true;

            if (pipRotateString.ToLowerInvariant() != TogglePiPRotation.Value.ToLowerInvariant())
                return true;

            if (pipZoomString.ToLowerInvariant() != TogglePiPZoom.Value.ToLowerInvariant())
                return true;

            if (togglePiPstring.ToLowerInvariant() != SConfig.TogglePiP.Value.ToLowerInvariant())
                return true;

            return false;
        }

        private static void GetCurrentSuitNum()
        {
            if (StartOfRound.Instance.localPlayerController.currentSuitID < 0)
                return;

            GetCurrentSuitID();
            Plugin.X($"currentSuitID: {StartOfRound.Instance.localPlayerController.currentSuitID}\n UnlockableItems: {UnlockableItems.Count}");
        }

        internal static void CreateBetterCommand()
        {
            Command = new("suitsTerminal", AdvancedTerminalMenu, ["suits", "suits menu"], CommandHandler.AdvancedSuitsTerm);
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
            CheckKeys(FavItemKey.Value, out Key favItemKey, out favItemKeyString);
            BindKeys(FavItem, favItemKey, ref favItemKeyString, "F", Key.F);
            PiPKeys();
        }

        private static void UpdateMainKeys()
        {
            if (IsValidReplacement(MenuUp.Value, suitsMenu.upMenu, out Key upKey))
                suitsMenu.upMenu = upKey;
            if (IsValidReplacement(MenuDown.Value, suitsMenu.downMenu, out Key downKey))
                suitsMenu.downMenu = downKey;
            if (IsValidReplacement(MenuLeft.Value, suitsMenu.leftMenu, out Key leftKey))
                suitsMenu.leftMenu = leftKey;
            if (IsValidReplacement(MenuRight.Value, suitsMenu.rightMenu, out Key rightKey))
                suitsMenu.rightMenu = rightKey;
            if (IsValidReplacement(LeaveMenu.Value, suitsMenu.leaveMenu, out Key leaveKey))
                suitsMenu.leaveMenu = leaveKey;
            if (IsValidReplacement(SelectMenu.Value, suitsMenu.selectMenu, out Key selectKey))
                suitsMenu.selectMenu = selectKey;

            suitsMenu.UpdateMainActions();
        }

        internal static string GetFooter()
        {
            string suit = GetSuitName();
            StringBuilder message = new();
            message.Append($"\r\n\r\n\r\n\r\nCurrently Wearing: {suit}\r\n\r\n");
            message.Append($"Page [{suitsMenu.leftMenu}] < {suitsMenu.CurrentPage}/{Mathf.CeilToInt((float)suitsMenu.DisplayMenuItemsOfType.Count / suitsMenu.PageSize)} > [{suitsMenu.rightMenu}]\r\n");
            message.Append($"Leave Menu: [{suitsMenu.leaveMenu}] Select Suit: [{suitsMenu.selectMenu}]\r\n");
            return message.ToString();
        }

        internal static string MainFooter()
        {
            StringBuilder message = new();
            message.Append($"\r\n\r\n\r\n\r\nPage [{suitsMenu.leftMenu}] < {suitsMenu.CurrentPage}/{Mathf.CeilToInt((float)suitsMenu.DisplayMenuItemsOfType.Count / suitsMenu.PageSize)} > [{suitsMenu.rightMenu}]\r\n");
            message.Append($"Leave Menu: [{suitsMenu.leaveMenu}] Select Item: [{suitsMenu.selectMenu}]\r\n");
            return message.ToString();
        }

        private static string GetSuitName()
        {
            string fail = string.Empty;
            if (UnlockableItems.Count == 0)
                return fail;

            if(StartOfRound.Instance == null)
                return fail;

            if (StartOfRound.Instance.localPlayerController == null)
                return fail;

            if (StartOfRound.Instance.localPlayerController.currentSuitID > UnlockableItems.Count)
                return fail;

            return UnlockableItems[StartOfRound.Instance.localPlayerController.currentSuitID].unlockableName;
        }

        private static void BindKeys(Action menuAction, Key givenKey, ref string givenKeyString, string defaultKeyString, Key defaultKey)
        {
            Plugin.X($"Binding {menuAction}");
            if (givenKey != Key.None)
            {
                ExtraKeyActions.Add(givenKey, menuAction);
                Plugin.X($"{givenKeyString} bound");
            }
            else
            {
                ExtraKeyActions.Add(defaultKey, menuAction);
                givenKeyString = defaultKeyString;
                Plugin.X($"{givenKeyString} bound");
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
            if (!EnablePiPCamera.Value)
                return;

            CheckKeys(TogglePiPHeight.Value, out Key pipHeight, out pipHeightString);
            BindKeys(PipHeight, pipHeight, ref pipHeightString, "Backslash", Key.Backslash);

            CheckKeys(TogglePiPRotation.Value, out Key pipRotate, out pipRotateString);
            BindKeys(PipRotate, pipRotate, ref pipRotateString, "Equals", Key.Equals);

            CheckKeys(TogglePiPZoom.Value, out Key pipZoom, out pipZoomString);
            BindKeys(PipZoom, pipZoom, ref pipZoomString, "Minus", Key.Minus);

            CheckKeys(SConfig.TogglePiP.Value, out Key pipKey, out togglePiPstring);
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
                    Plugin.WARNING("Tab Key detected, rejecting bind.");
                    validKey = Key.None;
                    return false;
                }
                else if (suitsMenu.MainActions.ContainsKey(keyFromString))
                {
                    Plugin.WARNING("Key was already bound to something, returning false");
                    string allKeys = string.Join(", ", suitsMenu.MainActions.Keys);
                    Plugin.WARNING($"Key list: {allKeys}");
                    validKey = Key.None;
                    return false;
                }
                else
                {
                    Plugin.X("Valid Key Detected and being assigned");
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
                    Plugin.WARNING("Tab Key detected, rejecting bind.");
                    validKey = Key.None;
                    return false;
                }
                else if (ExtraKeyActions.ContainsKey(keyFromString))
                {
                    Plugin.WARNING("Key was already bound to something, returning false");
                    string allKeys = string.Join(", ", ExtraKeyActions.Keys);
                    Plugin.WARNING($"Key list: {allKeys}");
                    validKey = Key.None;
                    return false;
                }
                else
                {
                    Plugin.X("Valid Key Detected and being assigned to bind");
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
            if (OpenLib.Plugin.instance.OpenBodyCamsMod && UseOpenBodyCams.Value)
            {
                Plugin.X("Returning Cam from OpenLib OpenBodyCams Compat!");
                return OpenLib.Compat.OpenBodyCamFuncs.GetCam(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
            }
            else
            {
                playerCam = CamStuff.MyCameraHolder.GetComponent<Camera>();
                return playerCam!;
            }

        }

        private static void PipAction()
        {
            TogglePiP(!pipActive);
            Plugin.X($"Toggling PiP to state {!pipActive}");
        }

        private static void PipHeight()
        {
            if (!pipActive)
                return;

            Camera currentCam = GetCam();

            if (currentCam == null)
                return;

            MoveCamera(currentCam.transform, ref heightStep);
            Plugin.X($"Changing PiP height to {heightStep}");
        }

        private static void PipRotate()
        {
            if (!pipActive)
                return;

            Camera currentCam = GetCam();

            if (currentCam == null)
                return;

            RotateCameraAroundPlayer(StartOfRound.Instance.localPlayerController.meshContainer, currentCam.transform);
            Plugin.X($"Rotating PiP around player");
        }

        private static void PipZoom()
        {
            if (!pipActive)
                return;

            Camera currentCam = GetCam();

            if (currentCam == null)
                return;

            ChangeCamZoom(currentCam, ref zoomStep);
            Plugin.X($"Changing PiP zoom to zoomStep: [{zoomStep}]");
        }

        private static void ShowHelpPage()
        {
            //inHelpMenu = true;
            suitsMenu.MenuNode.displayText = HelpMenuDisplay();
            suitsMenu.AcceptAnything = true;
            suitsMenu.Load();
            TogglePiP(false);
        }

        private static void ShowFavs()
        {
            if (FavoritesList.NestedMenus.Count < 1)
            {
                Plugin.Terminal.PlayTerminalAudioServerRpc(1);
                Plugin.Log.LogInfo("Empty favorites menu! Playing error audio for user");
                return;
            }
                

            Plugin.Terminal.StartCoroutine(SuitsMenuStart());
        }

        private static void ShowNormal()
        {
            Plugin.Terminal.StartCoroutine(SuitsMenuStart());
        }

        private static void FavItem()
        {
            if (!FavoritesList.IsActive && !SuitsList.IsActive)
            {
                Plugin.X("Current menu does not accept favorites as input!");
                return;
            }

            MenuItem selection = null!;
            selection = suitsMenu.DisplayMenuItemsOfType[suitsMenu.ActiveSelection];

            if (selection == null)
            {
                Plugin.WARNING($"SELECTION NULL AT FAVITEM\nActiveSelection - {suitsMenu.ActiveSelection}\nFavList Count - {FavoritesList.NestedMenus.Count}");
                return;
            }
  
            SuitAttributes selectedSuit = suitListing.SuitsList.FirstOrDefault(x => x.Name == selection.Name);

            if (selectedSuit == null)
                return;

            if (selectedSuit.IsFav)
            {
                selectedSuit.RemoveFromFavs();

                Plugin.Log.LogInfo($"{selectedSuit.Name} removed from favorites listing");
                if (suitListing.FavList.Count < 1)
                {
                    if (FavoritesList.IsActive)
                    {
                        SaveToConfig(suitListing.FavList, out string configSave);
                        SaveFavorites(configSave);
                        GetCurrentSuitNum();
                        suitsMenu.ExitInTerminal();
                        return;
                    }  
                }
            }
            else
            {
                selectedSuit.AddToFavs();
                Plugin.Log.LogInfo($"{selectedSuit.Name} added to favorites listing");
            }

            SaveToConfig(suitListing.FavList, out string saveToConfig);
            SaveFavorites(saveToConfig);
            suitsMenu.Load();
        }

        internal static void GetCurrentSuitID()
        {
            foreach (SuitAttributes item in suitListing.SuitsList)
            {
                if (item.Suit.suitID == StartOfRound.Instance.localPlayerController.currentSuitID ||
                    item.Suit.syncedSuitID.Value == StartOfRound.Instance.localPlayerController.currentSuitID)
                {
                    item.currentSuit = true;
                    Plugin.X($"Detected wearing - {item.Name}");
                }
                else
                    item.currentSuit = false;
            }
        }

        private static void PiPSetParent()
        {
            if (!EnablePiPCamera.Value)
                return;

            pipRawImage.transform.SetParent(Plugin.Terminal.screenText.image.transform);
        }

        private static void OnExitStuff()
        {
            TogglePiP(false);
        }

        private static void OnEnterStuff()
        { 
            rotateStep = 0;
            heightStep = 0;
            zoomStep = 1;
            GetCurrentSuitNum();
        }

        internal static IEnumerator SuitsMenuStart()
        {
            GetCurrentSuitNum();
            yield return new WaitForEndOfFrame();
            TogglePiP(true);
            yield return new WaitForEndOfFrame();
            PiPSetParent();
            yield break;
        }
    }
}
