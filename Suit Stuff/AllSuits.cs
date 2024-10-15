using suitsTerminal.Suit_Stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static suitsTerminal.Bools;
using static suitsTerminal.Misc;
using static suitsTerminal.ProcessRack;

namespace suitsTerminal
{
    internal class AllSuits
    {
        //internal static List<UnlockableSuit> allSuits = [];
        internal static List<UnlockableItem> UnlockableItems = [];
        internal static Dictionary<int, string> suitNameToID = [];
        internal static SuitListing suitListing = new();

        internal static void GetList()
        {
            suitNameToID.Clear();
            suitListing.NameList.Clear();
            CheckForDuplicateSuitNames();
            FixRack();
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
                    suitListing.NameList.Add(kvp.Value);
                }
                else
                {
                    // If the suit name has been encountered, create a unique suit name with the associated key
                    string uniqueSuitName = $"{kvp.Value}^({kvp.Key})";

                    // Add both the original and unique suit names to suitNames
                    suitListing.NameList.Add(uniqueSuitName);
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
                Plugin.WARNING($"Locked suit [{UnlockableItems[suit.syncedSuitID.Value].unlockableName}] detected, not adding to listings.");
                Plugin.X($"hasBeenUnlockedByPlayer: {UnlockableItems[suit.syncedSuitID.Value].hasBeenUnlockedByPlayer} \nalreadyUnlocked: {UnlockableItems[suit.syncedSuitID.Value].alreadyUnlocked}");
                return false;
            }

            return true;
        }

        private static void RemoveBadSuitIDs()
        {
            // Remove items with negative syncedSuitID

            if (!SConfig.KeepSuitsWithNegativeIDs.Value)
            {
                suitListing.RawSuitsList.RemoveAll(suit => suit.syncedSuitID.Value < 0); //simply remove bad suit IDs
                Plugin.X("Removing suits with negative suitID values.");
            }
            else
            {
                Plugin.X("Attempting to keep suits with negative suitID values.");
                // Remove items with negative syncedSuitID and assign new random numbers
                suitListing.RawSuitsList.RemoveAll(suit =>
                {
                    if (suit.syncedSuitID.Value < 0)
                    {
                        // Generate a new random number
                        int newRandomNumber;
                        do
                        {
                            newRandomNumber = UnityEngine.Random.Range(1, int.MaxValue);
                        } while (suitListing.RawSuitsList.Any(otherSuit => otherSuit.syncedSuitID.Value == newRandomNumber));

                        // Assign the new random number
                        Plugin.X($"suit ID was {suit.syncedSuitID.Value}");
                        suit.syncedSuitID.Value = newRandomNumber;
                        Plugin.X($"suit ID changed to {suit.syncedSuitID.Value}");

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
                Plugin.Log.LogInfo("No sorting requested.");
            }
            else
                Plugin.WARNING("Config failure, no sorting");

        }

        internal static void InitSuitsListing()
        {
            Plugin.X("InitSuitsListing");
            // Use Resources.FindObjectsOfTypeAll to find all instances of UnlockableSuit
            suitListing.RawSuitsList.Clear();
            suitListing.RawSuitsList = [.. Resources.FindObjectsOfTypeAll<UnlockableSuit>()];

            UnlockableItems = StartOfRound.Instance.unlockablesList.unlockables;
            RemoveBadSuitIDs();
            GetList();
        }

        private static void OrderSuitsByName()
        {
            // Order the list by name
            suitListing.RawSuitsList = [.. suitListing.RawSuitsList.OrderBy((UnlockableSuit suit) => UnlockableItems[suit.syncedSuitID.Value].unlockableName)];
        }

        private static void OrderSuitsByID()
        {
            suitListing.RawSuitsList = [.. suitListing.RawSuitsList.OrderBy((UnlockableSuit suit) => suit.syncedSuitID.Value)];
        }

        private static void HideBootsAndRack()
        {
            if (rackSituated)
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
            Plugin.X($"Raw Suit Count: {suitListing.RawSuitsList.Count}");
            Plugin.X($"Unlockables Count: {UnlockableItems.Count}");
            weirdSuitNum = 0;

            HideBootsAndRack();

            foreach (UnlockableSuit item in suitListing.RawSuitsList)
            {
                Plugin.X($"checking - {item.syncedSuitID.Value}");

                if (!AddSuitToList(item))
                    continue;

                AutoParentToShip component = item.gameObject.GetComponent<AutoParentToShip>();
                SuitAttributes suit;

                if (suitListing.Contains(item, out suit))
                {
                    Plugin.X($"SuitAttributes detected valid {suit}, no need to update");
                }
                else if (suitListing.Contains(UnlockableItems[item.syncedSuitID.Value].unlockableName, out suit))
                {
                    suit.UpdateSuit(item, UnlockableItems, ref suitNameToID);
                    Plugin.X($"Updated suit attributes for {suit.Name}");
                }
                else
                {
                    suit = new(item, UnlockableItems, ref suitNameToID);
                    suitListing.SuitsList.Add(suit);
                }

                OldCommands.CreateOldWearCommands(suit);

                if (!SConfig.DontRemove.Value)
                {
                    if (ShouldShowSuit(suit))
                    {
                        suit.IsOnRack = true;
                        ProcessVisibleSuit(component, suitsOnRack);
                    }
                    else
                    {
                        suit.IsOnRack = false;
                        ProcessHiddenSuit(component);
                    }

                    if (suitsOnRack == SConfig.SuitsOnRack.Value && !rackSituated)
                    {
                        rackSituated = true;
                        Plugin.X($"Max suits are on the rack now. rack is situated, yippeee!!!");
                    }
                }
            }

            OldCommands.CreateOldPageCommands();

            InitThisPlugin.initStarted = false;
        }


    }
}
