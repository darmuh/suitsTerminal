using System;
using System.Collections.Generic;
using System.Text;
using static suitsTerminal.StringStuff;
using static suitsTerminal.Misc;
using static suitsTerminal.ProcessRack;
using System.Linq;
using UnityEngine;
using Component = UnityEngine.Component;
using Steamworks.Ugc;
using System.Collections;

namespace suitsTerminal
{
    internal class AllSuits
    {
        internal static List<UnlockableSuit> allSuits = new List<UnlockableSuit>();
        internal static List<UnlockableItem> UnlockableItems = new List<UnlockableItem>();
        internal static List<TerminalNode> suitPages = new List<TerminalNode>();
        internal static List<TerminalNode> otherNodes = new List<TerminalNode>();
        internal static List<string> suitNames = new List<string>();
        internal static List<Page> suitsPages = new List<Page>();

        internal static void GetList()
        {
            suitNames.Clear();
            weirdSuitNum = 0;
            foreach (UnlockableSuit item in allSuits)
            {
                string SuitName = "";
                if (item.syncedSuitID.Value >= 0 && AddSuitToList(item))
                {
                    SuitName = UnlockableItems[item.syncedSuitID.Value].unlockableName;

                    if(!SConfig.advancedTerminalMenu.Value)
                        SuitName = TerminalFriendlyString(SuitName);
                    
                    suitNames.Add(SuitName);
                }
                else
                {
                    weirdSuitNum++;
                    suitsTerminal.X($"Skipping suit, either weird or locked.");
                }
            }
        }

        private static bool AddSuitToList(UnlockableSuit suit)
        {
            if (!SConfig.enforcePaidSuits.Value)
                return true;

            if (!UnlockableItems[suit.syncedSuitID.Value].hasBeenUnlockedByPlayer && UnlockableItems[suit.syncedSuitID.Value].shopSelectionNode != null)
            {
                suitsTerminal.X("Locked suit detected, not adding to list.");
                return false;
            }
            
            return true;
        }

        private static void RemoveBadSuitIDs()
        {
            // Remove items with negative syncedSuitID

            if (!SConfig.keepSuitsWithNegativeIDs.Value)
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

            OrderSuitsByName();
        }

        private static void GetAllSuits()
        {
            // Use Resources.FindObjectsOfTypeAll to find all instances of UnlockableSuit
            allSuits.Clear();
            allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().ToList();

            // Order the list by syncedSuitID.Value
            allSuits = allSuits.OrderBy((UnlockableSuit suit) => suit.suitID).ToList();

            UnlockableItems = StartOfRound.Instance.unlockablesList.unlockables;
            RemoveBadSuitIDs();
        }

        private static void OrderSuitsByName()
        {
            // Order the list by name
            allSuits = allSuits.OrderBy((UnlockableSuit suit) => UnlockableItems[suit.syncedSuitID.Value].unlockableName).ToList();
        }

        internal static void InitSuitsListing()
        {
            GetAllSuits();
            GetList();
            FixRack();
            OldCommands.MakeRandomSuitCommand();
        }

        private static void FixRack()
        {
            suitsTerminal.X($"Suit Count: {allSuits.Count}");
            suitsTerminal.X($"Unlockables Count: {UnlockableItems.Count}");
            weirdSuitNum = 0;
            reorderSuits = 0;
            normSuit = 0;

            foreach (UnlockableSuit item in allSuits)
            {
                AutoParentToShip component = ((Component)item).gameObject.GetComponent<AutoParentToShip>();
                SuitInfo suitInfoComponent = component.gameObject.GetComponent<SuitInfo>();

                OldCommands.CreateOldWearCommands(item);

                if (!SConfig.dontRemove.Value)
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

                    if (normSuit >= SConfig.suitsOnRack.Value && !rackSituated || (rackSituated && !isHanging))
                    {
                        ProcessHiddenSuit(component);
                        normSuit++;
                    }
                    else if (rackSituated && isHanging)
                    {
                        ProcessHangingSuit(component);
                    }
                    else if (normSuit < SConfig.suitsOnRack.Value && !rackSituated)
                    {
                        ProcessVisibleSuit(component, normSuit);
                        normSuit++;
                    }
                    else
                    {
                        ProcessHiddenSuit(component);
                    }

                    if (showSuit == SConfig.suitsOnRack.Value && !rackSituated)
                    {
                        rackSituated = true;
                        suitsTerminal.X($"Max suits are on the rack now. rack is situated \n^ ^\n .");
                    }
                }
            }

            InitThisPlugin.initStarted = false;
        }
    }
}
