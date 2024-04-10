using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Key = UnityEngine.InputSystem.Key;
using System.Collections;
using UnityEngine;
using System.Linq;
using static suitsTerminal.SConfig;
using static suitsTerminal.StringStuff;
using static suitsTerminal.AllSuits;
using static TerminalApi.TerminalApi;
using GameNetcodeStuff;

namespace suitsTerminal
{
    public class AdvancedMenu
    {
        // Define a dictionary to map keys to actions
        internal static Dictionary<Key, string> keyActions = new Dictionary<Key, string>();
        internal static Key keyBeingPressed;
        internal static Key leaveMenu;
        internal static Key selectMenu;
        internal static Key togglePiP;
        internal static string leaveString;
        internal static string selectString;
        internal static string togglePiPstring;
        internal static int currentPage;
        internal static int activeSelection;
        internal static int currentlyWearing;
        internal static bool exitSpecialMenu = false;
        public static bool specialMenusActive = false;
        internal static TerminalNode menuDisplay = null;

        internal static void InitSettings()
        {
            suitsTerminal.X("Loading keybinds from config");
            keyActions.Clear();
            keyActions.Add(Key.LeftArrow, "previous_page");
            keyActions.Add(Key.RightArrow, "next_page");
            keyActions.Add(Key.UpArrow, "previous_item");
            keyActions.Add(Key.DownArrow, "next_item");
            LeaveMenu();
            SelectKey();
            TogglePiPKey();
            CreateMenuCommand();
            if(menuDisplay == null )
                menuDisplay = CreateTerminalNode("", true);
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

            CommandHandler.AddCommand("Advanced Menus Corotuine", true, otherNodes, "suits", true, "advanced_suitsTerm", "Other", "Access suitsTerminal menu for selecting a new suit to wear.", CommandHandler.AdvancedSuitsTerm);
        }

        private static void LeaveMenu()
        {
            if(IsValidKey(SConfig.leaveMenu.Value, out Key validKey))
            {
                leaveMenu = validKey;
                keyActions.Add(leaveMenu, "leave_menu");
                leaveString = SConfig.leaveMenu.Value;
            }   
            else
            {
                keyActions.Add(Key.Backspace, "leave_menu");
                leaveString = "Backspace";
            }
                
        }

        private static void TogglePiPKey()
        {
            if (!enablePiPCamera.Value)
                return;

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

        private static void SelectKey()
        {
            if (IsValidKey(SConfig.selectMenu.Value, out Key validKey))
            {
                selectMenu = validKey;
                keyActions.Add(selectMenu, "menu_select");
                selectString = SConfig.selectMenu.Value;
            }   
            else
            {
                keyActions.Add(Key.Enter, "menu_select");
                selectString = "Enter";
            }
        }

        private static bool IsValidKey(string key, out Key validKey)
        {
            List<Key> invalidKeys = new List<Key>() {
            Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J,
            Key.K, Key.L, Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T,
            Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z, Key.Space
            };
            if (Enum.TryParse(key, ignoreCase: true, out Key keyFromString))
            {
                if (invalidKeys.Contains(keyFromString))
                {
                    suitsTerminal.X("Alphabetical Key detected, rejecting bind.");
                    validKey = Key.None;
                    return false;
                }
                else if (keyActions.ContainsKey(keyFromString))
                {
                    suitsTerminal.X("Key was already bound to something, returning false");
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

        // Method to check if any key in the dictionary is pressed
        public static bool AnyKeyIsPressed()
        {
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

        internal static void UpdatePicture()
        {
            suitsTerminal.Terminal.terminalImage.enabled = true;
            suitsTerminal.Terminal.displayingPersistentImage = GetSuitTexture(activeSelection);
            suitsTerminal.Terminal.terminalImage.texture = GetSuitTexture(activeSelection); 
        }

        internal static void RemovePicture()
        {
            suitsTerminal.Terminal.terminalImage.enabled = false;
            suitsTerminal.Terminal.displayingPersistentImage = null;
            suitsTerminal.Terminal.terminalImage.texture = null;
        }

        private static void HandleKeyAction(string value)
        {
            if (value == "previous_page")
            {
                if(currentPage > 0)
                    currentPage--;

                menuDisplay.displayText = AdvancedMenuDisplay(suitNames, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "next_page")
            {
                currentPage++;
                menuDisplay.displayText = AdvancedMenuDisplay(suitNames, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "previous_item")
            {
                if(activeSelection > 0)
                    activeSelection--;

                menuDisplay.displayText = AdvancedMenuDisplay(suitNames, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "next_item")
            {
                activeSelection++;
                menuDisplay.displayText = AdvancedMenuDisplay(suitNames, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                return;
            }
            else if (value == "leave_menu")
            {
                exitSpecialMenu = true;
                return;
            }
            else if (value == "menu_select")
            {
                string selectedSuit = GetActiveMenuItem(suitNames, activeSelection, 10, currentPage);
                currentlyWearing = activeSelection;
                CommandHandler.AdvancedSuitPick(selectedSuit);
                suitsTerminal.X($"Current Page: {currentPage}\n Current Item: {activeSelection}");
                menuDisplay.displayText = AdvancedMenuDisplay(suitNames, activeSelection, 10, currentPage);
                suitsTerminal.Terminal.LoadNewNode(menuDisplay);
                return;
            }
            else if (value == "toggle_pip")
            {
                PictureInPicture.TogglePiP(!PictureInPicture.pipActive);
                suitsTerminal.X($"Toggling PiP to state {!PictureInPicture.pipActive}");
                return;
            }
        }

        internal static Texture GetSuitTexture(int activeSuit)
        {
            if (allSuits[activeSuit] == null)
                return null;

            return allSuits[activeSuit].suitMaterial.mainTexture;

        }

        internal static void GetCurrentSuitID()
        {
            foreach (UnlockableSuit suit in allSuits)
            {
                if (suit.suitID == StartOfRound.Instance.localPlayerController.currentSuitID ||
                    suit.syncedSuitID.Value == StartOfRound.Instance.localPlayerController.currentSuitID)
                {
                    currentlyWearing = allSuits.IndexOf(suit);  // Assign directly to currentlyWearing
                    break;  // Exit the loop once a match is found
                }
            }
        }



        internal static IEnumerator ActiveMenu()
        {
            if (specialMenusActive)
                yield break;

            specialMenusActive = true;
            GetCurrentSuitNum();
            PictureInPicture.TogglePiP(true);
            yield return new WaitForSeconds(0.2f);
            suitsTerminal.Terminal.screenText.interactable = false;
            suitsTerminal.X("ActiveMenu Coroutine");
            currentPage = 1;
            activeSelection = 0;

            while (suitsTerminal.Terminal.terminalInUse && advancedTerminalMenu.Value && !exitSpecialMenu)
            {
                if (AnyKeyIsPressed())
                {
                    HandleKeyPress(keyBeingPressed);
                    yield return new WaitForSeconds(0.1f);
                }
                else
                    yield return new WaitForSeconds(0.1f);
            }

            if (!suitsTerminal.Terminal.terminalInUse)
                suitsTerminal.X("Terminal no longer in use");

            if (exitSpecialMenu)
                exitSpecialMenu = false;

            specialMenusActive = false;

            PictureInPicture.TogglePiP(false);
            //RemovePicture();
            suitsTerminal.Terminal.screenText.interactable = true;
            yield return new WaitForSeconds(0.1f);
            suitsTerminal.Terminal.screenText.ActivateInputField();
            suitsTerminal.Terminal.LoadNewNode(suitsTerminal.Terminal.terminalNodes.specialNodes[13]);
            yield break;
        }
    }
}
