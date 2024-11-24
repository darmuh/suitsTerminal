using Steamworks.Ugc;
using suitsTerminal.Suit_Stuff;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static suitsTerminal.Bools;
using static suitsTerminal.Misc;
using static suitsTerminal.ProcessRack;
using static OpenLib.Common.CommonStringStuff;
using System;

namespace suitsTerminal
{
    internal class AllSuits
    {
        //internal static List<UnlockableSuit> allSuits = [];
        internal static List<UnlockableItem> UnlockableItems = [];
        internal static Dictionary<int, string> suitNameToID = [];
        internal static SuitListing suitListing = new();
        internal static List<string> favsList = [];

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
            Plugin.X("Removing suits with negative suitID values.");
            // Remove items with negative syncedSuitID
            foreach (UnlockableSuit suit in suitListing.RawSuitsList)
            {
                if (suit.syncedSuitID.Value < 0)
                    Plugin.X($"Negative value [ {suit.syncedSuitID.Value} ] detected for suit\nRemoving from suitsTerminal listing");
            }

            suitListing.RawSuitsList.RemoveAll(suit => suit.syncedSuitID.Value < 0); //simply remove bad suit IDs

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

        internal static void InitFavoritesListing(bool checkList = false)
        {       
            if (SConfig.PersonalizedFavorites.Value)
            {
                string favsFilePath = Path.Combine(@"%userprofile%\appdata\locallow\ZeekerssRBLX\Lethal Company", "suitsTerminal");
                favsFilePath = Environment.ExpandEnvironmentVariables(favsFilePath);
                if (!Directory.Exists(favsFilePath))
                    Directory.CreateDirectory(favsFilePath);

                Plugin.X($"Favorites file path - {favsFilePath}");

                if (!File.Exists(favsFilePath + @"\masterFavsListing.txt"))
                    File.WriteAllText(favsFilePath + @"\masterFavsListing.txt", SConfig.FavoritesMenuList.Value);

                string favoritesText = File.ReadAllText(favsFilePath + @"\masterFavsListing.txt");
                Plugin.X($"favoritesText: {favoritesText}");
                favsList = GetKeywordsPerConfigItem(favoritesText, ',');
                foreach(string fav in favsList)
                {
                    Plugin.X($"-- {fav} --");
                }
            }
            else
                favsList = GetKeywordsPerConfigItem(SConfig.FavoritesMenuList.Value, ',');

            suitListing?.RefreshFavorites(checkList);
        }

        internal static void InitSuitsListing()
        {
            Plugin.X("InitSuitsListing");
            // Use Resources.FindObjectsOfTypeAll to find all instances of UnlockableSuit
            suitListing.RawSuitsList.Clear();
            suitListing.RawSuitsList = [.. Resources.FindObjectsOfTypeAll<UnlockableSuit>()];
            suitNameToID.Clear();
            suitListing.NameList.Clear();


            UnlockableItems = StartOfRound.Instance.unlockablesList.unlockables;
            RemoveBadSuitIDs();
            FixRack();
            OldCommands.MakeRandomSuitCommand();
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

            List<string> names = []; //for old terminal commands to track duplicates

            foreach (UnlockableSuit item in suitListing.RawSuitsList)
            {
                Plugin.X($"checking - {item.syncedSuitID.Value}");

                if (!AddSuitToList(item))
                    continue;

                AutoParentToShip component = item.gameObject.GetComponent<AutoParentToShip>();

                if (suitListing.Contains(item, out SuitAttributes suit))
                {
                    suit.UpdateSuit(item, UnlockableItems, ref suitNameToID);
                    Plugin.X($"Updated suit attributes for {suit.Name} with ID {suit.UniqueID}");
                }
                else if (suitListing.Contains(item.syncedSuitID.Value, out suit))
                {
                    suit.UpdateSuit(item, UnlockableItems, ref suitNameToID);
                    Plugin.X($"Updated suit attributes for {suit.Name} with ID {suit.UniqueID}");
                }
                else
                {
                    suit = new(item, UnlockableItems, ref suitNameToID);
                    suitListing.SuitsList.Add(suit);
                    Plugin.X($"Added suit attributes for {suit.Name} with ID {suit.UniqueID}");
                }

                OldCommands.CreateOldWearCommands(suit, ref names);

                if (!SConfig.DontRemove.Value)
                {
                    if (ShouldShowSuit(suit))
                    {
                        suit.IsOnRack = true;
                        ProcessVisibleSuit(component, suitsOnRack);
                        Plugin.X($"Showing suit - [ {suit.Name} ]");
                    }
                    else
                    {
                        suit.IsOnRack = false;
                        ProcessHiddenSuit(component);
                        Plugin.X($"Hiding suit - [ {suit.Name} ]");
                    }

                    if (suitsOnRack == SConfig.SuitsOnRack.Value && !rackSituated)
                    {
                        rackSituated = true;
                        Plugin.X($"Max suits are on the rack now. rack is situated, yippeee!!!");
                    }
                }
            }

            if(SConfig.DontRemove.Value)
                Plugin.WARNING("suitsTerminal is NOT touching the rack!!!");

            Plugin.X($"Main list count: {suitListing.NameList.Count}\nFav list count: {suitListing.FavList.Count}");

            OldCommands.CreateOldPageCommands();

            InitThisPlugin.initStarted = false;
        }


    }
}
