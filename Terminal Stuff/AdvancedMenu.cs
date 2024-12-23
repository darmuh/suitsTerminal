﻿using OpenLib.Common;
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

        public static Color CaretOriginal;
        internal static Color transparent = new(0, 0, 0, 0);

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
            {
                if (DynamicBools.TryGetKeyword("suits", out TerminalKeyword suitsKW))
                    menuDisplay = suitsKW.specialKeywordResult;
                else
                    Plugin.ERROR("Unable to get suits node for menuDisplay!!!");
            }

            specialMenusActive = false;
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

            menuDisplay = AddingThings.AddNodeManual("advanced_suitsTerm", "suits", CommandHandler.AdvancedSuitsTerm, true, 0, ConfigSetup.defaultListing);
            if (LogicHandling.TryGetFromAllNodes("OtherCommands", out TerminalNode otherNode))
            {
                AddingThings.AddToExistingNodeText($"\n>SUITS\nsuitsTerminal advanced menu for changing & viewing suits", ref otherNode);
            }
        }

        private static void CollectionOfKeys()
        {
            CheckKeys(MenuUp.Value, out Key upKey, out upString);
            BindKeys("previous_item", upKey, ref upString, "UpArrow", Key.UpArrow);

            CheckKeys(MenuDown.Value, out Key downKey, out downString);
            BindKeys("next_item", downKey, ref downString, "DownArrow", Key.DownArrow);

            CheckKeys(MenuLeft.Value, out Key leftKey, out leftString);
            BindKeys("previous_page", leftKey, ref leftString, "LeftArrow", Key.LeftArrow);

            CheckKeys(MenuRight.Value, out Key rightKey, out rightString);
            BindKeys("next_page", rightKey, ref rightString, "RightArrow", Key.RightArrow);

            CheckKeys(LeaveMenu.Value, out Key leaveKey, out leaveString);
            BindKeys("leave_menu", leaveKey, ref leaveString, "Backspace", Key.Backspace);

            CheckKeys(SelectMenu.Value, out Key selectKey, out selectString);
            BindKeys("menu_select", selectKey, ref selectString, "Enter", Key.Enter);

            CheckKeys(FavItemKey.Value, out Key favItemKey, out favItemKeyString);
            BindKeys("favorite_item", favItemKey, ref favItemKeyString, "F", Key.F);

            CheckKeys(FavMenuKey.Value, out Key favMenuKey, out favMenuKeyString);
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

        // Method to check if any key in the dictionary is pressed
        public static bool AnyKeyIsPressed()
        {

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
                Plugin.X("Returning Cam from OpenLib OpenBodyCams Compat!");
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
            Camera currentCam = null!;
            
            if (pipActive)
                currentCam = GetCam();

            if (value == "previous_page" && !inHelpMenu)
            {
                if (currentPage > 0)
                    currentPage--;

                LoadPage(menuDisplay, AdvancedMenuDisplay(suitListing, activeSelection, 10, ref currentPage));
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "next_page" && !inHelpMenu)
            {
                currentPage++;
                LoadPage(menuDisplay, AdvancedMenuDisplay(suitListing, activeSelection, 10, ref currentPage));
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "previous_item" && !inHelpMenu)
            {
                if (activeSelection > 0)
                    activeSelection--;

                LoadPage(menuDisplay, AdvancedMenuDisplay(suitListing, activeSelection, 10, ref currentPage));
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "next_item" && !inHelpMenu)
            {
                activeSelection++;
                LoadPage(menuDisplay, AdvancedMenuDisplay(suitListing, activeSelection, 10, ref currentPage));
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "leave_menu")
            {
                inHelpMenu = false;
                MenuActive(false);
                return;
            }
            else if (value == "menu_select" && !inHelpMenu)
            {
                SuitAttributes suit = GetMenuItemSuit(suitListing, activeSelection);
                if (suit == null)
                    return;
                CommandHandler.BetterSuitPick(suit);
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                GetCurrentSuitNum();
                LoadPage(menuDisplay, AdvancedMenuDisplay(suitListing, activeSelection, 10, ref currentPage));
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
                if (selectedSuit == null)
                    return;
                if (suitListing.FavList.Contains(selectedSuit.Name))
                {
                    selectedSuit.RemoveFromFavs();
                    suitListing.RefreshFavorites();

                    Plugin.Log.LogInfo($"{selectedSuit.Name} removed from favorites listing");
                    if(suitListing.FavList.Count < 1)
                    {
                        inFavsMenu = false;
                        currentPage = 1;
                        GetCurrentSuitNum();
                        LoadPage(menuDisplay, AdvancedMenuDisplay(suitListing, 0, 10, ref currentPage));
                        SaveToConfig(suitListing.FavList, out string configSave);
                        SaveFavorites(configSave);
                        return;
                    }
                }
                else
                {
                    selectedSuit.AddToFavs();
                    Plugin.Log.LogInfo($"{selectedSuit.Name} added to favorites listing");
                }

                SaveToConfig(suitListing.FavList, out string saveToConfig);
                SaveFavorites(saveToConfig);
                Plugin.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                LoadPage(menuDisplay, AdvancedMenuDisplay(suitListing, activeSelection, 10, ref currentPage));

            }
            else if (value == "favorites_menu")
            {
                if (!inFavsMenu && suitListing.FavList.Count < 1)
                    return;

                if(inHelpMenu)
                {
                    inHelpMenu = false;
                    TogglePiP(true);
                }

                inFavsMenu = !inFavsMenu;
                currentPage = 1;
                GetCurrentSuitNum();
                LoadPage(menuDisplay, AdvancedMenuDisplay(suitListing, 0, 10, ref currentPage));
            }
            else if (value == "help_menu")
            {
                inHelpMenu = !inHelpMenu;
                GetCurrentSuitNum();
                menuDisplay.displayText = HelpMenuDisplay(inHelpMenu);

                TogglePiP(!inHelpMenu);
                LoadPage(menuDisplay);
            }
        }

        internal static void LoadPage(TerminalNode menu, string displayText = "")
        {
            if(displayText.Length > 0)
                menu.displayText = displayText;

            Plugin.Terminal.LoadNewNode(menu);
            if(Plugin.TerminalStuff)
                Compatibility.TerminalStuffMod.NetSync(menu);
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

        internal static void OnTerminalKeyPress()
        {
            if (!specialMenusActive || !AdvancedTerminalMenu.Value)
                return;

            if (AnyKeyIsPressed())
                HandleKeyPress(keyBeingPressed);
        }

        internal static void MenuActive(bool active, bool enableInput = true)
        {
            if (active)
                Plugin.Terminal.StartCoroutine(SuitsMenuStart());
            else
                Plugin.Terminal.StartCoroutine(SuitsMenuExit(enableInput));

        }

        internal static IEnumerator SuitsMenuExit(bool enableInput)
        {
            yield return new WaitForEndOfFrame();
            specialMenusActive = false;
            TogglePiP(false);
            
            yield return new WaitForEndOfFrame();
            Plugin.Terminal.LoadNewNode(Plugin.Terminal.terminalNodes.specialNodes[13]);
            Compatibility.TerminalStuffMod.NetSync(Plugin.Terminal.terminalNodes.specialNodes[13]);
            yield return new WaitForEndOfFrame();
            Plugin.Terminal.screenText.caretColor = CaretOriginal;

            if (enableInput)
            {
                Plugin.Terminal.screenText.ActivateInputField();
                Plugin.Terminal.screenText.interactable = true;
            }

            yield break;
        }

        internal static IEnumerator SuitsMenuStart()
        {
            if (specialMenusActive)
                yield break;

            yield return new WaitForEndOfFrame();
            Plugin.Terminal.screenText.caretColor = transparent;
            specialMenusActive = true;
            GetCurrentSuitNum();
            yield return new WaitForEndOfFrame();
            Plugin.Terminal.screenText.DeactivateInputField();
            Plugin.Terminal.screenText.interactable = false;
            yield return new WaitForEndOfFrame();
            TogglePiP(true);
            yield return new WaitForEndOfFrame();
            PiPSetParent();
            yield break;
        }
    }
}
