using OpenLib.Common;
using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using suitsTerminal.Suit_Stuff;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static suitsTerminal.AllSuits;
using static suitsTerminal.Misc;
using static suitsTerminal.PictureInPicture;
using static suitsTerminal.SConfig;
using static suitsTerminal.StringStuff;
using Key = UnityEngine.InputSystem.Key;

namespace suitsTerminal
{
    public class AdvancedMenu
    {
        // Define a dictionary to map keys to actions
        internal static Dictionary<Key, string> keyActions = [];
        internal static Key keyBeingPressed;
        internal static Key leaveMenu;
        internal static Key selectMenu;
        internal static Key togglePiP;
        internal static string upString;
        internal static string downString;
        internal static string leftString;
        internal static string rightString;
        internal static string leaveString;
        internal static string selectString;
        internal static string favItemKeyString;
        internal static string favMenuKeyString;
        internal static string helpMenuKeyString;
        internal static string togglePiPstring;
        internal static string pipHeightString;
        internal static string pipRotateString;
        internal static string pipZoomString;

        internal static bool inFavsMenu = false;
        internal static bool inHelpMenu = false;
        internal static int currentPage = 1;
        internal static int activeSelection = 0;
        internal static bool exitSpecialMenu = false;

        public static bool specialMenusActive = false;

        internal static TerminalNode menuDisplay = null!;

        internal static bool initKeySettings = false;

        internal static void InitSettings()
        {
            if (initKeySettings)
                return;

            initKeySettings = true;
            keyActions.Clear();
            Plugin.X("Loading keybinds from config");
            CollectionOfKeys();
            TogglePiPKey();
            CreateMenuCommand();
            if (menuDisplay == null)
                menuDisplay = AddingThings.CreateDummyNode("suitsTerminal AdvancedMenu", true, "");

            initKeySettings = false;
        }

        private static void GetCurrentSuitNum()
        {
            if (StartOfRound.Instance.localPlayerController.currentSuitID < 0)
                return;

            GetCurrentSuitID();
            Plugin.X($"currentSuitID: {StartOfRound.Instance.localPlayerController.currentSuitID}\n UnlockableItems: {UnlockableItems.Count}");
        }

        internal static void CreateMenuCommand()
        {
            if (!AdvancedTerminalMenu.Value)
                return;

            CommandHandler.AddCommand(true, "suits", "advanced_suitsTerm", CommandHandler.AdvancedSuitsTerm, ConfigSetup.defaultListing, "other", "suitsTerminal advanced menu for changing suits");
        }

        private static void CollectionOfKeys()
        {
            CheckKeys(SConfig.MenuUp.Value, out Key upKey, out upString);
            BindKeys("previous_item", upKey, ref upString, "UpArrow", Key.UpArrow);

            CheckKeys(SConfig.MenuDown.Value, out Key downKey, out downString);
            BindKeys("next_item", downKey, ref downString, "DownArrow", Key.DownArrow);

            CheckKeys(SConfig.MenuLeft.Value, out Key leftKey, out leftString);
            BindKeys("previous_page", leftKey, ref leftString, "LeftArrow", Key.LeftArrow);

            CheckKeys(SConfig.MenuRight.Value, out Key rightKey, out rightString);
            BindKeys("next_page", rightKey, ref rightString, "RightArrow", Key.RightArrow);

            CheckKeys(SConfig.LeaveMenu.Value, out Key leaveKey, out leaveString);
            BindKeys("leave_menu", leaveKey, ref leaveString, "Backspace", Key.Backspace);

            CheckKeys(SConfig.SelectMenu.Value, out Key selectKey, out selectString);
            BindKeys("menu_select", selectKey, ref selectString, "Enter", Key.Enter);

            CheckKeys(SConfig.FavItemKey.Value, out Key favItemKey, out favItemKeyString);
            BindKeys("favorite_item", favItemKey, ref favItemKeyString, "F", Key.F);

            CheckKeys(SConfig.FavMenuKey.Value, out Key favMenuKey, out favMenuKeyString);
            BindKeys("favorites_menu", favMenuKey, ref favMenuKeyString, "F1", Key.F1);

            CheckKeys(HelpMenu.Value, out Key helpMenuKey, out helpMenuKeyString);
            BindKeys("help_menu", helpMenuKey, ref helpMenuKeyString, "H", Key.H);
        }

        private static void BindKeys(string menuAction, Key givenKey, ref string givenKeyString, string defaultKeyString, Key defaultKey)
        {
            Plugin.X($"Binding {menuAction}");
            if (givenKey != Key.None)
            {
                keyActions.Add(givenKey, menuAction);
                Plugin.X($"{givenKeyString} bound to {menuAction}");
            }
            else
            {
                keyActions.Add(defaultKey, menuAction);
                givenKeyString = defaultKeyString;
                Plugin.X($"{givenKeyString} bound to {menuAction}");
            }
        }

        private static void CheckKeys(string configString, out Key usingKey, out string keyString)
        {
            if (IsValidKey(configString, out Key validKey))
            {
                usingKey = validKey;
                keyString = configString;
            }
            else
            {
                usingKey = Key.None;
                keyString = "FAIL";
            }
        }

        private static void TogglePiPKey()
        {
            if (!EnablePiPCamera.Value)
                return;

            CheckKeys(TogglePiPHeight.Value, out Key pipHeight, out pipHeightString);
            BindKeys("pip_height", pipHeight, ref pipHeightString, "Backslash", Key.Backslash);

            CheckKeys(TogglePiPRotation.Value, out Key pipRotate, out pipRotateString);
            BindKeys("pip_rotate", pipRotate, ref pipRotateString, "Equals", Key.Equals);

            CheckKeys(TogglePiPZoom.Value, out Key pipZoom, out pipZoomString);
            BindKeys("pip_zoom", pipZoom, ref pipZoomString, "Minus", Key.Minus);

            if (IsValidKey(SConfig.TogglePiP.Value, out Key validKey))
            {
                togglePiP = validKey;
                keyActions.Add(togglePiP, "toggle_pip");
                togglePiPstring = SConfig.TogglePiP.Value;
            }
            else
            {
                keyActions.Add(Key.F12, "toggle_pip");
                togglePiPstring = "F12";
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
                else if (keyActions.ContainsKey(keyFromString))
                {
                    Plugin.WARNING("Key was already bound to something, returning false");
                    string allKeys = string.Join(", ", keyActions.Keys);
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

        internal static void TerminalInputEnabled(bool state)
        {
            if (state == false)
            {
                Plugin.Terminal.screenText.interactable = false;
                Plugin.Terminal.currentNode.maxCharactersToType = 0;
            }
            else
            {
                Plugin.Terminal.screenText.interactable = true;
                Plugin.Terminal.currentNode.maxCharactersToType = 25;
            }
        }

        // Method to check if any key in the dictionary is pressed
        public static bool AnyKeyIsPressed()
        {
            TerminalInputEnabled(false);

            foreach (var keyAction in keyActions)
            {
                if (Keyboard.current[keyAction.Key].isPressed)
                {
                    keyBeingPressed = keyAction.Key;
                    Plugin.X($"Key detected in use: {keyAction.Key}");
                    return true;
                }
            }
            return false;
        }

        // Method to handle key presses
        private static void HandleKeyPress(Key key)
        {
            // Check if the key exists in the dictionary
            if (keyActions.ContainsKey(key))
            {
                // Get the keyword associated to the key
                keyActions.TryGetValue(key, out string value);

                // Execute the action corresponding to the key
                Plugin.X($"Attempting to match given key to action: {value}");

                HandleKeyAction(value);
                return;
            }
            else
                Plugin.Log.LogError("Shortcut KeyActions list not updating properly");
        }

        internal static Camera GetCam()
        {
            if (OpenLib.Plugin.instance.OpenBodyCamsMod && UseOpenBodyCams.Value)
            {
                Camera Cam = OpenLib.Compat.OpenBodyCamFuncs.GetCam(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
                if (Cam == null)
                {
                    OpenLib.Compat.OpenBodyCamFuncs.OpenBodyCamsMirrorStatus(true, ObcResolution.Value, 0.1f, false, ref CamStuff.ObcCameraHolder);
                }

                return OpenLib.Compat.OpenBodyCamFuncs.GetCam(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
            }
            else
            {
                playerCam = CamStuff.MyCameraHolder.GetComponent<Camera>();
                return playerCam!;
            }

        }

        private static void HandleKeyAction(string value)
        {
            Camera currentCam = GetCam();

            if (value == "previous_page" && !inHelpMenu)
            {
                if (currentPage > 0)
                    currentPage--;

                menuDisplay.displayText = AdvancedMenuDisplay(suitListing, activeSelection, 10, currentPage);
                Plugin.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "next_page" && !inHelpMenu)
            {
                currentPage++;
                menuDisplay.displayText = AdvancedMenuDisplay(suitListing, activeSelection, 10, currentPage);
                Plugin.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "previous_item" && !inHelpMenu)
            {
                if (activeSelection > 0)
                    activeSelection--;

                menuDisplay.displayText = AdvancedMenuDisplay(suitListing, activeSelection, 10, currentPage);
                Plugin.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "next_item" && !inHelpMenu)
            {
                activeSelection++;
                menuDisplay.displayText = AdvancedMenuDisplay(suitListing, activeSelection, 10, currentPage);
                Plugin.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "leave_menu")
            {
                exitSpecialMenu = true;
                TerminalInputEnabled(true);
                return;
            }
            else if (value == "menu_select" && !inHelpMenu)
            {
                SuitAttributes suit = GetMenuItemSuit(suitListing, activeSelection);
                CommandHandler.BetterSuitPick(suit);
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                GetCurrentSuitNum();
                menuDisplay.displayText = AdvancedMenuDisplay(suitListing, activeSelection, 10, currentPage);
                Plugin.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                return;
            }
            else if (value == "toggle_pip" && !inHelpMenu)
            {
                TogglePiP(!pipActive);
                Plugin.X($"Toggling PiP to state {!pipActive}");
                return;
            }
            else if (value == "pip_height" && !inHelpMenu)
            {
                if (currentCam == null)
                    return;

                MoveCamera(currentCam.transform, ref heightStep);
                Plugin.X($"Changing PiP height to {heightStep}");
            }
            else if (value == "pip_rotate" && !inHelpMenu)
            {
                if (currentCam == null)
                    return;

                RotateCameraAroundPlayer(StartOfRound.Instance.localPlayerController.meshContainer, currentCam.transform);
                Plugin.X($"Rotating PiP around player");
            }
            else if (value == "pip_zoom" && !inHelpMenu)
            {
                if (currentCam == null)
                    return;

                ChangeCamZoom(currentCam, ref zoomStep);
                Plugin.X($"Changing PiP zoom to zoomStep: [{zoomStep}]");
            }
            else if (value == "favorite_item" && !inHelpMenu)
            {
                SuitAttributes selectedSuit = GetMenuItemSuit(suitListing, activeSelection);
                if (suitListing.FavList.Contains(selectedSuit.Name))
                {
                    selectedSuit.RemoveFromFavs();
                    foreach (SuitAttributes fav in suitListing.SuitsList)
                        fav.RefreshFavIndex();

                    Plugin.Log.LogInfo($"{selectedSuit.Name} removed from favorites listing");
                }
                else
                {
                    selectedSuit.AddToFavs();
                    Plugin.Log.LogInfo($"{selectedSuit.Name} added to favorites listing");
                }

                SaveToConfig(suitListing.FavList, out string saveToConfig);
                FavoritesMenuList.Value = saveToConfig;
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                menuDisplay.displayText = AdvancedMenuDisplay(suitListing, activeSelection, 10, currentPage);
                Plugin.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
            }
            else if (value == "favorites_menu" && !inHelpMenu)
            {
                inFavsMenu = !inFavsMenu;
                currentPage = 1;
                GetCurrentSuitNum();
                menuDisplay.displayText = AdvancedMenuDisplay(suitListing, 0, 10, currentPage);
                Plugin.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
            }
            else if (value == "help_menu")
            {
                inHelpMenu = !inHelpMenu;
                currentPage = 1;
                GetCurrentSuitNum();
                menuDisplay.displayText = HelpMenuDisplay(inHelpMenu);

                TogglePiP(!inHelpMenu);
                Plugin.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
            }
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

        internal static IEnumerator ActiveMenu()
        {
            if (specialMenusActive)
                yield break;

            specialMenusActive = true;
            inFavsMenu = false;
            rotateStep = 0;
            heightStep = 0;
            zoomStep = 1;
            GetCurrentSuitNum();
            TogglePiP(true);
            Plugin.Terminal.screenText.DeactivateInputField();
            Plugin.Terminal.screenText.interactable = false;
            yield return new WaitForSeconds(0.2f);
            Plugin.X("ActiveMenu Coroutine");
            currentPage = 1;
            activeSelection = 0;
            PiPSetParent();

            while (Plugin.Terminal.terminalInUse && AdvancedTerminalMenu.Value && !exitSpecialMenu)
            {
                if (AnyKeyIsPressed())
                {
                    HandleKeyPress(keyBeingPressed);
                    yield return new WaitForSeconds(MenuKeyPressDelay.Value);
                }
                else
                    yield return new WaitForSeconds(MenuPostSelectDelay.Value);
            }

            if (!Plugin.Terminal.terminalInUse)
                Plugin.X("Terminal no longer in use");

            if (exitSpecialMenu)
                exitSpecialMenu = false;

            specialMenusActive = false;

            TogglePiP(false);
            yield return new WaitForSeconds(0.1f);
            Plugin.Terminal.screenText.ActivateInputField();
            Plugin.Terminal.screenText.interactable = true;
            Plugin.Terminal.LoadNewNode(Plugin.Terminal.terminalNodes.specialNodes[13]);
            yield break;
        }
    }
}
