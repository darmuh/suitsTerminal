using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using suitsTerminal.Misc;
using suitsTerminal.Util;
using UnityEngine;
using static OpenLib.Common.CommonStringStuff;

namespace suitsTerminal.Suits;

internal class RackManager
{
    internal static List<SuitAttributes> AllSuits = [];
    internal static List<string> FavsList = [];
    internal static bool IsRackFull
    {
        get
        {
            int rackCount = AllSuits.FindAll(c => c.IsOnRack).Count;
            return rackCount >= ModConfig.SuitsOnRack.Value;
        }
    }
    private static int _realcurrent = 0;
    internal static int RealCurrentID
    {
        get
        {
            return _realcurrent;
        }
        set
        {
            _realcurrent = value;
        }
    }
    private static SuitAttributes _current = null!;
    internal static SuitAttributes CurrentSuit
    {
        get
        {
            if (AllSuits.Count > 0)
                _current = AllSuits.FirstOrDefault(x => x.Suit.suitID == RealCurrentID);

            return _current!;
        }
        set
        {
            _current = value;
        }
    }

    internal static int FavCount
    {
        get
        {
            return AllSuits.FindAll(x => x.IsFav).Count;
        }
    }

    internal static List<string> FavList
    {
        get
        {
            return AllSuits.FindAll(x => x.IsFav).ConvertAll(x => x.Name);
        }
    }

    internal static bool RackDisabled
    {
        get
        {
            if (ClothingRack == null)
                return true;

            return ModConfig.RackSettings.Value == ModConfig.Removal.OnlyRackAndExtraSuits || ModConfig.RackSettings.Value == ModConfig.Removal.Everything;
        }
    }

    internal static int RackCount
    {
        get
        {
            return AllSuits.FindAll(x => x.IsOnRack).Count;
        }
    }

    private static GameObject _clothingRack = null!;
    internal static GameObject ClothingRack
    {
        get
        {
            if (_clothingRack == null)
            {
                if (Plugin.TryGetGameObject("Environment/HangarShip/NurbsPath.002", out _clothingRack))
                    return _clothingRack;
                else
                    return null!;
            }

            return _clothingRack;
        }
    }

    public static void SuitSpawn(UnlockableSuit unlockableSuit)
    {
        AutoParentToShip component = unlockableSuit.gameObject.GetComponent<AutoParentToShip>();
        SuitAttributes suit = unlockableSuit.gameObject.GetComponent<SuitAttributes>();

        if (Plugin.TooManySuits)
            return;

        if (ModConfig.RackSettings.Value == ModConfig.Removal.DontRemoveAnything)
        {
            Loggers.WARNING("suitsTerminal is NOT touching the rack!!!");
            return;
        }

        if (RackDisabled)
        {
            ProcessHiddenSuit(component);
            Loggers.LogDebug($"Hiding suit - [ {suit.Name} ]");
            suit.IsOnRack = false;
        }
        else
        {
            if (ShouldShowSuit(suit, RackCount))
            {
                ProcessVisibleSuit(component, RackCount);
                Loggers.LogDebug($"Showing suit - [ {suit.Name} ]");
                suit.IsOnRack = true;
            }
            else
            {
                ProcessHiddenSuit(component);
                Loggers.LogDebug($"Hiding suit - [ {suit.Name} ]");
                suit.IsOnRack = false;
            }
        }
    }

    internal static void HideBootsAndRack()
    {
        bool removeBoots = ModConfig.RackSettings.Value == ModConfig.Removal.OnlyBootsAndExtraSuits || ModConfig.RackSettings.Value == ModConfig.Removal.Everything;
        bool removeRack = ModConfig.RackSettings.Value == ModConfig.Removal.OnlyRackAndExtraSuits || ModConfig.RackSettings.Value == ModConfig.Removal.Everything;

        if (removeBoots && Plugin.TryGetGameObject("Environment/HangarShip/ScavengerModelSuitParts/Circle.004", out GameObject boots))
            UnityEngine.Object.Destroy(boots);

        if (removeRack && Plugin.TryGetGameObject("Environment/HangarShip/NurbsPath.002", out GameObject clothingRack))
            UnityEngine.Object.Destroy(clothingRack);
    }

    internal static void RemovePreview()
    {
        if (CurrentSuit == null)
        {
            Loggers.WARNING("Unable to re-equip the correct suit!");
            return;
        }

        UnlockableSuit.SwitchSuitForPlayer(Plugin.LocalPlayer, CurrentSuit.Suit.suitID, false);
    }

    internal static void InitFavoritesListing()
    {
        if (ModConfig.PersonalizedFavorites.Value)
        {
            string favsFilePath = Plugin.PersonalFilesPath();

            if (!File.Exists(favsFilePath + @"\masterFavsListing.txt"))
                File.WriteAllText(favsFilePath + @"\masterFavsListing.txt", ModConfig.FavoritesMenuList.Value);

            string favoritesText = File.ReadAllText(favsFilePath + @"\masterFavsListing.txt");
            Loggers.LogDebug($"favoritesText: {favoritesText}");
            FavsList = GetKeywordsPerConfigItem(favoritesText, ',');
            foreach (string fav in FavsList)
            {
                Loggers.LogDebug($"-- {fav} --");
            }
        }
        else
            FavsList = GetKeywordsPerConfigItem(ModConfig.FavoritesMenuList.Value, ',');
    }

    internal static void ProcessHiddenSuit(AutoParentToShip component)
    {
        //Plugin.X("processhiddensuit method");
        component.disableObject = true;
        component.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        component.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    internal static void ProcessVisibleSuit(AutoParentToShip component, int suitNumber)
    {
        //Plugin.X("processvisiblesuit");
        component.overrideOffset = true;

        float offsetModifier = ModConfig.RackOffset.Value;

        component.positionOffset = StartOfRound.Instance.rightmostSuitPosition.localPosition + StartOfRound.Instance.rightmostSuitPosition.forward * offsetModifier * (suitNumber - 1);
        component.rotationOffset = new Vector3(0f, 90f, 0f);
    }

    internal static bool ShouldShowSuit(SuitAttributes suit, int count)
    {
        if (RackDisabled)
            return false;

        List<string> suitsOnRackOnly = GetKeywordsPerConfigItem(ModConfig.SuitsOnRackOnly.Value, ',');
        List<string> neverAddToRack = GetListToLower(GetKeywordsPerConfigItem(ModConfig.DontAddToRack.Value, ','));

        if (neverAddToRack.Contains(suit.Name.ToLower()))
            return false;

        if (!IsRackFull)
        {
            if (suitsOnRackOnly.Count == 0 && count < ModConfig.SuitsOnRack.Value)
                return true;

            if (suit.Name.Length > 1 && suitsOnRackOnly.Count > 0 && suitsOnRackOnly.Any(s => s.Equals(suit.Name, StringComparison.InvariantCultureIgnoreCase)))
                return true;
        }
        else
        {
            if (suit.IsOnRack)
                return true;
        }

        return false;
    }
}
