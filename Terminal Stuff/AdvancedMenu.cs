using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Key = UnityEngine.InputSystem.Key;
using System.Collections;
using UnityEngine;
using static suitsTerminal.SConfig;
using static suitsTerminal.StringStuff;
using static suitsTerminal.AllSuits;
using static suitsTerminal.PictureInPicture;
using static suitsTerminal.Misc;
using static suitsTerminal.TerminalHook;
using GameNetcodeStuff;
using System.Text;

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
        internal static int currentPage;
        internal static int activeSelection;
        internal static int currentlyWearing;
        internal static bool exitSpecialMenu = false;

        public static bool specialMenusActive = false;
        
        internal static TerminalNode menuDisplay = null;

        internal static bool initKeySettings = false;

        internal static void InitSettings()
        {
            if (initKeySettings)
                return;

            initKeySettings = true;
            keyActions.Clear();
            suitsTerminal.X("Loading keybinds from config");
            CollectionOfKeys();
            TogglePiPKey();
            CreateMenuCommand();
            if(menuDisplay == null )
                menuDisplay = CreateDummyNode("suitsTerminal AdvancedMenu", true, "");

            initKeySettings = false;
        }

        private static void GetCurrentSuitNum()
        {
            if (StartOfRound.Instance.localPlayerController.currentSuitID < 0)
                return;

            GetCurrentSuitID();
            suitsTerminal.X($"Currently Wearing: {currentlyWearing}\ncurrentSuitID: {StartOfRound.Instance.localPlayerController.currentSuitID}\n UnlockableItems: {UnlockableItems.Count}");
        }

        internal static void CreateMenuCommand()
        {
            if (!advancedTerminalMenu.Value)
                return;

            CommandHandler.AddCommand("Advanced Menus Corotuine", true, "suits", false, "advanced_suitsTerm", CommandHandler.AdvancedSuitsTerm, CommandStuff.sT);
        }

        private static void CollectionOfKeys()
        {
            CheckKeys(SConfig.menuUp.Value, out Key upKey, out upString);
            BindKeys("previous_item", upKey, ref upString, "UpArrow", Key.UpArrow);

            CheckKeys(SConfig.menuDown.Value, out Key downKey, out downString);
            BindKeys("next_item", downKey, ref downString, "DownArrow", Key.DownArrow);

            CheckKeys(SConfig.menuLeft.Value, out Key leftKey, out leftString);
            BindKeys("previous_page", leftKey, ref leftString, "LeftArrow", Key.LeftArrow);

            CheckKeys(SConfig.menuRight.Value, out Key rightKey, out rightString);
            BindKeys("next_page", rightKey, ref rightString, "RightArrow", Key.RightArrow);

            CheckKeys(SConfig.leaveMenu.Value, out Key leaveKey, out leaveString);
            BindKeys("leave_menu", leaveKey, ref leaveString, "Backspace", Key.Backspace);

            CheckKeys(SConfig.selectMenu.Value, out Key selectKey, out selectString);
            BindKeys("menu_select", selectKey, ref selectString, "Enter", Key.Enter);

            CheckKeys(SConfig.favItemKey.Value, out Key favItemKey, out favItemKeyString);
            BindKeys("favorite_item", favItemKey, ref favItemKeyString, "F", Key.F);

            CheckKeys(SConfig.favMenuKey.Value, out Key favMenuKey, out favMenuKeyString);
            BindKeys("favorites_menu", favMenuKey, ref favMenuKeyString, "F1", Key.F1);

            CheckKeys(helpMenu.Value, out Key helpMenuKey, out helpMenuKeyString);
            BindKeys("help_menu", helpMenuKey, ref helpMenuKeyString, "H", Key.H);
        }

        private static void BindKeys(string menuAction, Key givenKey, ref string givenKeyString, string defaultKeyString, Key defaultKey)
        {
            suitsTerminal.X($"Binding {menuAction}");
            if(givenKey != Key.None)
            {
                keyActions.Add(givenKey, menuAction);
                suitsTerminal.X($"{givenKeyString} bound to {menuAction}");
            }
            else
            {
                keyActions.Add(defaultKey, menuAction);
                givenKeyString = defaultKeyString;
                suitsTerminal.X($"{givenKeyString} bound to {menuAction}");
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
            if (!enablePiPCamera.Value)
                return;

            CheckKeys(togglePiPHeight.Value, out Key pipHeight, out pipHeightString);
            BindKeys("pip_height", pipHeight, ref pipHeightString, "Backslash", Key.Backslash);

            CheckKeys(togglePiPRotation.Value, out Key pipRotate, out pipRotateString);
            BindKeys("pip_rotate", pipRotate, ref pipRotateString, "Equals", Key.Equals);

            CheckKeys(togglePiPZoom.Value, out Key pipZoom, out pipZoomString);
            BindKeys("pip_zoom", pipZoom, ref pipZoomString, "Minus", Key.Minus);

            if (IsValidKey(SConfig.togglePiP.Value, out Key validKey))
            {
                togglePiP = validKey;
                keyActions.Add(togglePiP, "toggle_pip");
                togglePiPstring = SConfig.togglePiP.Value;
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
                    suitsTerminal.X("Tab Key detected, rejecting bind.");
                    validKey = Key.None;
                    return false;
                }
                else if (keyActions.ContainsKey(keyFromString))
                {
                    suitsTerminal.X("Key was already bound to something, returning false");
                    string allKeys = string.Join(", ", keyActions.Keys);
                    suitsTerminal.X($"Key list: {allKeys}");
                    validKey = Key.None;
                    return false;
                }
                else
                {
                    suitsTerminal.X("Valid Key Detected and being assigned to bind");
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
            if(state == false)
            {
                suitsTerminal.Terminal.screenText.interactable = false;
            }
            else
            {
                suitsTerminal.Terminal.screenText.interactable = true;
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
                    suitsTerminal.X($"Key detected in use: {keyAction.Key}");
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
                suitsTerminal.X($"Attempting to match given key to action: {value}");

                HandleKeyAction(value);
                return;
            }
            else
                suitsTerminal.Log.LogError("Shortcut KeyActions list not updating properly");
        }

        private static Camera GetCam()
        {
            if(suitsTerminal.OpenBodyCams && useOpenBodyCams.Value)
            {
                return OpenBodyCams.GetMirrorCam();
            }
            else
                return playerCam;
        }

        private static void HandleKeyAction(string value)
        {
            List<string> currentMenu = GetString();
            Camera currentCam = GetCam();

            if (value == "previous_page" && !inHelpMenu)
            {
                if (currentPage > 0)
                    currentPage--;

                menuDisplay.displayText = AdvancedMenuDisplay(currentMenu, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "next_page" && !inHelpMenu)
            {
                currentPage++;
                menuDisplay.displayText = AdvancedMenuDisplay(currentMenu, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "previous_item" && !inHelpMenu)
            {
                if (activeSelection > 0)
                    activeSelection--;

                menuDisplay.displayText = AdvancedMenuDisplay(currentMenu, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "next_item" && !inHelpMenu)
            {
                activeSelection++;
                menuDisplay.displayText = AdvancedMenuDisplay(currentMenu, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
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
                string selectedSuit = GetActiveMenuItem(currentMenu, activeSelection, 10, currentPage);
                CommandHandler.AdvancedSuitPick(selectedSuit);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                GetCurrentSuitNum();
                menuDisplay.displayText = AdvancedMenuDisplay(currentMenu, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                suitsTerminal.Terminal.StartCoroutine(TransformHotfix(currentCam));
                TerminalInputEnabled(false);
                return;
            }
            else if (value == "toggle_pip" && !inHelpMenu)
            {
                TogglePiP(!pipActive);
                suitsTerminal.X($"Toggling PiP to state {!pipActive}");
                return;
            }
            else if (value == "pip_height" && !inHelpMenu)
            {
                MoveCamera(currentCam.transform, ref heightStep);
                suitsTerminal.X($"Changing PiP height to {heightStep}");
            }
            else if (value == "pip_rotate" && !inHelpMenu)
            {
                RotateCameraAroundPlayer(StartOfRound.Instance.localPlayerController.transform, currentCam.transform);
                suitsTerminal.X($"Rotating PiP around player");
            }
            else if (value == "pip_zoom" && !inHelpMenu)
            {
                ChangeCamZoom(currentCam, ref zoomStep);
                suitsTerminal.X($"Changing PiP zoom to zoomStep: [{zoomStep}]");
            }
            else if (value == "favorite_item" && !inHelpMenu)
            {
                string selectedSuit = GetActiveMenuItem(suitNames, activeSelection, 10, currentPage);
                if(favSuits.Contains(selectedSuit))
                    favSuits.Remove(selectedSuit);
                else
                    favSuits.Add(selectedSuit);

                SaveToConfig(favSuits, out string saveToConfig);
                favoritesMenuList.Value = saveToConfig;
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                menuDisplay.displayText = AdvancedMenuDisplay(suitNames, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
            }
            else if (value == "favorites_menu" && !inHelpMenu)
            {
                inFavsMenu = !inFavsMenu;
                List<string> newMenu = GetString();
                currentPage = 1;
                activeSelection = 0;
                GetCurrentSuitNum();
                menuDisplay.displayText = AdvancedMenuDisplay(newMenu, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
                DisplayAllMenuItemsLog(newMenu);
            }
            else if (value == "help_menu")
            {
                inHelpMenu = !inHelpMenu;
                currentPage = 1;
                activeSelection = 0;
                GetCurrentSuitNum();
                menuDisplay.displayText = HelpMenuDisplay(inHelpMenu, currentMenu);

                TogglePiP(!inHelpMenu);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                TerminalInputEnabled(false);
            }
        }

        internal static IEnumerator TransformHotfix(Camera currentCam)
        {
            if (!suitsTerminal.OpenBodyCams)
                yield break;
            if (!useOpenBodyCams.Value)
                yield break;

            yield return new WaitForEndOfFrame();
            CamInit(currentCam);
        }

        private static void DisplayAllMenuItemsLog(List<string> menuListing)
        {
            StringBuilder fullList = new();
            foreach(string item in menuListing)
            {
                fullList.Append(item + "\n");
            }
                suitsTerminal.X($"{fullList}");
        }

        internal static void GetCurrentSuitID()
        {
            foreach (UnlockableSuit suit in allSuits)
            {
                if (suit.suitID == StartOfRound.Instance.localPlayerController.currentSuitID ||
                    suit.syncedSuitID.Value == StartOfRound.Instance.localPlayerController.currentSuitID)
                {
                    if (!inFavsMenu)
                        currentlyWearing = allSuits.IndexOf(suit);  // Assign directly to currentlyWearing
                    else
                    {
                        string unlockableName = UnlockableItems[suit.syncedSuitID.Value].unlockableName;
                        int indexOf = favSuits.IndexOf(unlockableName);
                        if (indexOf == -1)
                            currentlyWearing = favSuits.IndexOf(unlockableName + $"^({suit.syncedSuitID.Value})");
                        else
                            currentlyWearing = indexOf;
                    }
                        
                    break;  // Exit the loop once a match is found
                }
            }
        }

        private static void PiPSetParent()
        {
            if (!enablePiPCamera.Value)
                return;

            pipRawImage.transform.SetParent(suitsTerminal.Terminal.screenText.image.transform);
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
            suitsTerminal.Terminal.screenText.DeactivateInputField();
            suitsTerminal.Terminal.screenText.interactable = false;
            yield return new WaitForSeconds(0.2f);
            suitsTerminal.X("ActiveMenu Coroutine");
            currentPage = 1;
            activeSelection = 0;
            PiPSetParent();

            while (suitsTerminal.Terminal.terminalInUse && advancedTerminalMenu.Value && !exitSpecialMenu)
            {
                if (AnyKeyIsPressed())
                {
                    HandleKeyPress(keyBeingPressed);
                    yield return new WaitForSeconds(menuKeyPressDelay.Value);
                }
                else
                    yield return new WaitForSeconds(menuPostSelectDelay.Value);
            }

            if (!suitsTerminal.Terminal.terminalInUse)
                suitsTerminal.X("Terminal no longer in use");

            if (exitSpecialMenu)
                exitSpecialMenu = false;

            specialMenusActive = false;

            TogglePiP(false);
            yield return new WaitForSeconds(0.1f);
            suitsTerminal.Terminal.screenText.ActivateInputField();
            suitsTerminal.Terminal.screenText.interactable = true;
            suitsTerminal.Terminal.LoadNewNode(suitsTerminal.Terminal.terminalNodes.specialNodes[13]);
            yield break;
        }
    }
}
