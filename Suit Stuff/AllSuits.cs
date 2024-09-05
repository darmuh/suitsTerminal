using System;
using System.Collections.Generic;
using static suitsTerminal.StringStuff;
using static suitsTerminal.Misc;
using static suitsTerminal.ProcessRack;
using static suitsTerminal.Bools;
using System.Linq;
using UnityEngine;
using Component = UnityEngine.Component;
using static OpenLib.Common.CommonStringStuff;

namespace suitsTerminal
{
    internal class AllSuits
    {
        internal static List<UnlockableSuit> allSuits = [];
        internal static List<UnlockableItem> UnlockableItems = [];
        internal static Dictionary<int,string> suitNameToID = [];
        internal static List<string> suitNames = [];
        internal static List<string> favSuits = [];
        internal static List<Page> suitsPages = [];
        internal static bool favSuitsSet = false;

        internal static void GetList()
        {
            suitNameToID.Clear();
            suitNames.Clear();
            weirdSuitNum = 0;
            foreach (UnlockableSuit item in allSuits)
            {
                string SuitName;
                if (item.syncedSuitID.Value >= 0 && AddSuitToList(item))
                {
                    SuitName = UnlockableItems[item.syncedSuitID.Value].unlockableName;

                    if(!SConfig.AdvancedTerminalMenu.Value)
                        SuitName = TerminalFriendlyString(SuitName);
                    if (!suitNameToID.ContainsKey(item.syncedSuitID.Value))
                        suitNameToID.Add(item.syncedSuitID.Value, SuitName);
                    else
                        suitsTerminal.WARNING($"WARNING: duplicate suitID detected: {item.syncedSuitID.Value}\n{SuitName} will not be added to listing");
                }
                else
                {
                    weirdSuitNum++;
                    suitsTerminal.WARNING($"Skipping suit, either weird or locked.");
                }
            }

            CheckForDuplicateSuitNames();
            FixRack();
            InitFavorites();
            OldCommands.MakeRandomSuitCommand();
        }

        private static void CheckForDuplicateSuitNames()
        {
            List<string> fullList = [];
            List<string> duplicatesFound = [];
            // Populate suitNames based on suitNameToID
            foreach (var kvp in suitNameToID)
            {
                // Check if the suit name already exists in suitNames
                if (!fullList.Any(name => string.Equals(name, kvp.Value, StringComparison.OrdinalIgnoreCase)))
                {
                    // If not, add it directly to suitNames
                    fullList.Add(kvp.Value);
                }
                else
                {
                    // If it does, append the integer after ^ to make it unique
                    duplicatesFound.Add(kvp.Value);
                }
            }

            CreateFinalSuitsList(duplicatesFound);
        }

        private static void CreateFinalSuitsList(List<string> duplicateSuitNames)
        {
            // Populate suitNames based on suitNameToID
            foreach (var kvp in suitNameToID)
            {
                // If the suit name has not been encountered yet, add it to suitNames and the set
                if (!duplicateSuitNames.Any(name => string.Equals(name, kvp.Value, StringComparison.OrdinalIgnoreCase)))
                {
                    suitNames.Add(kvp.Value);
                }
                else
                {
                    // If the suit name has been encountered, create a unique suit name with the associated key
                    string uniqueSuitName = $"{kvp.Value}^({kvp.Key})";

                    // Add both the original and unique suit names to suitNames
                    suitNames.Add(uniqueSuitName);
                }
            }
        }

        private static bool AddSuitToList(UnlockableSuit suit)
        {

            if (!SConfig.EnforcePaidSuits.Value)
                return true;

            if (UnlockableItems[suit.syncedSuitID.Value].shopSelectionNode == null)
                return true;

            if (!UnlockableItems[suit.syncedSuitID.Value].spawnPrefab)
            {
                suitsTerminal.WARNING($"Locked suit [{UnlockableItems[suit.syncedSuitID.Value].unlockableName}] detected, not adding to list.");
                suitsTerminal.X($"hasBeenUnlockedByPlayer: {UnlockableItems[suit.syncedSuitID.Value].hasBeenUnlockedByPlayer} \nalreadyUnlocked: {UnlockableItems[suit.syncedSuitID.Value].alreadyUnlocked}");
                return false;
            }
            
            return true;
        }

        private static void RemoveBadSuitIDs()
        {
            // Remove items with negative syncedSuitID

            if (!SConfig.KeepSuitsWithNegativeIDs.Value)
            {
                allSuits.RemoveAll(suit => suit.syncedSuitID.Value < 0); //simply remove bad suit IDs
                suitsTerminal.X("Removing suits with negative suitID values.");
            }
            else
            {
                suitsTerminal.X("Attempting to keep suits with negative suitID values.");
                // Remove items with negative syncedSuitID and assign new random numbers
                allSuits.RemoveAll(suit =>
                {
                    if (suit.syncedSuitID.Value < 0)
                    {
                        // Generate a new random number
                        int newRandomNumber;
                        do
                        {
                            newRandomNumber = UnityEngine.Random.Range(1, int.MaxValue);
                        } while (allSuits.Any(otherSuit => otherSuit.syncedSuitID.Value == newRandomNumber));

                        // Assign the new random number
                        suitsTerminal.X($"suit ID was {suit.syncedSuitID.Value}");
                        suit.syncedSuitID.Value = newRandomNumber;
                        suitsTerminal.X($"suit ID changed to {suit.syncedSuitID.Value}");

                        return true; // Remove the item
                    }

                    return false; // Keep the item
                });
            }
            if (SConfig.SuitsSortingStyle.Value == "alphabetical")
                OrderSuitsByName();
            else if (SConfig.SuitsSortingStyle.Value == "numerical")
            {
                OrderSuitsByID();
            }
            else if (SConfig.SuitsSortingStyle.Value == "none")
            {
                suitsTerminal.Log.LogInfo("No sorting requested, host/client may be desynced.");
            }
            else
                suitsTerminal.WARNING("Config failure, no sorting");

        }

        private static void GetAllSuits()
        {
            // Use Resources.FindObjectsOfTypeAll to find all instances of UnlockableSuit
            allSuits.Clear();
            allSuits = [.. Resources.FindObjectsOfTypeAll<UnlockableSuit>()];

            // Order the list by syncedSuitID.Value
            allSuits = [.. allSuits.OrderBy((UnlockableSuit suit) => suit.suitID)];

            UnlockableItems = StartOfRound.Instance.unlockablesList.unlockables;
            RemoveBadSuitIDs();
        }

        private static void OrderSuitsByName()
        {
            // Order the list by name
            allSuits = [.. allSuits.OrderBy((UnlockableSuit suit) => UnlockableItems[suit.syncedSuitID.Value].unlockableName)];
        }

        private static void OrderSuitsByID()
        {
            allSuits = [.. allSuits.OrderBy((UnlockableSuit suit) => suit.syncedSuitID.Value)];
        }

        internal static void InitSuitsListing()
        {
            GetAllSuits();
            GetList();
        }

        private static void InitFavorites()
        {
            List<string> favConfigList = GetKeywordsPerConfigItem(SConfig.FavoritesMenuList.Value, ',');

            favSuits.Clear();
            if (favConfigList.Count == 0)
                return;

            foreach (string suitName in favConfigList)
            {
                if (suitNames.Contains(suitName) && !favSuits.Contains(suitName))
                {
                    favSuits.Add(suitName);
                    suitsTerminal.X($"Added [{suitName}] to favorites list.");
                }
                else
                    suitsTerminal.WARNING($"[{suitName}] not loaded to favorites. Suit is locked, already added, or not found.");  
            }
        }

        private static void HideBootsAndRack()
        {
            if(rackSituated) 
                return;

            if (SConfig.HideBoots.Value)
            {
                GameObject boots = GetGameObject("Environment/HangarShip/ScavengerModelSuitParts/Circle.004");
                GameObject.Destroy(boots);
            }

            if (SConfig.DontRemove.Value)
                return;

            if (SConfig.HideRack.Value)
            {
                GameObject clothingRack = GetGameObject("Environment/HangarShip/NurbsPath.002");
                GameObject.Destroy(clothingRack);
            }
   
        }

        private static void FixRack()
        {
            suitsTerminal.X($"Suit Count: {allSuits.Count}");
            suitsTerminal.X($"Unlockables Count: {UnlockableItems.Count}");
            weirdSuitNum = 0;
            reorderSuits = 0;
            normSuit = 0;

            List<string> customSuitNames = GetKeywordsPerConfigItem(SConfig.SuitsOnRackOnly.Value, ',');

            HideBootsAndRack();

            foreach (UnlockableSuit item in allSuits)
            {
                AutoParentToShip component = ((Component)item).gameObject.GetComponent<AutoParentToShip>();
                SuitInfo suitInfoComponent = component.gameObject.GetComponent<SuitInfo>();

                List<string> dontAddTerminal = GetListToLower(GetKeywordsPerConfigItem(SConfig.DontAddToTerminal.Value, ','));
                OldCommands.CreateOldWearCommands(item, dontAddTerminal);

                if (!SConfig.DontRemove.Value)
                {
                    isHanging = false;

                    if (suitInfoComponent != null)
                    {
                        string suitTag = suitInfoComponent.suitTag;

                        if (suitTag == "hanging")
                        {
                            isHanging = true;
                            suitsTerminal.X($"Hanging suit detected - {suitTag}");
                        }
                        else if (suitTag == "hidden")
                        {
                            isHanging = false;
                            suitsTerminal.X($"Hidden suit detected - {suitTag}");
                        }
                        else
                        {
                            suitInfoComponent.suitTag = "hidden";
                        }
                    }

                    if (!ShouldShowSuit(item, customSuitNames))
                    {
                        ProcessHiddenSuit(component);
                        normSuit++;
                    }
                    else if (rackSituated && ShouldShowSuit(item, customSuitNames))
                    {
                        ProcessHangingSuit(component);
                    }
                    else if (!rackSituated && ShouldShowSuit(item, customSuitNames))
                    {
                        ProcessVisibleSuit(component, showSuit);
                        normSuit++;
                    }
                    else
                    {
                        ProcessHiddenSuit(component);
                    }

                    if (showSuit == SConfig.SuitsOnRack.Value && !rackSituated)
                    {
                        rackSituated = true;
                        suitsTerminal.X($"Max suits are on the rack now. rack is situated, yippeee!!!");
                    }   
                }  
            }

            InitThisPlugin.initStarted = false;
        }

        
    }
}
