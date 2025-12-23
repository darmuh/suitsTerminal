using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace suitsTerminal;

[BepInAutoPlugin]
[BepInDependency("darmuh.OpenLib", OpenLib.MyPluginInfo.PLUGIN_VERSION)] //OpenLib requires latest version!
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static bool TooManySuits
    {
        get
        {
            return Chainloader.PluginInfos.ContainsKey("TooManySuits");
        }
    }

    internal static bool SuitSaver
    {
        get
        {
            return Chainloader.PluginInfos.ContainsKey("Hexnet.lethalcompany.suitsaver");
        }
    }

    internal static Terminal Terminal
    {
        get
        {
            return OpenLib.Plugin.instance.Terminal;
        }
    }

    internal static PlayerControllerB LocalPlayer
    {
        get
        {
            return StartOfRound.Instance.localPlayerController;
        }
    }

    internal static bool HintOnce = false;
    internal static bool EquipDefault = true;
    private static string _personalFiles = string.Empty;

    private void Awake()
    {
        Log = Logger;

        Log.LogInfo($"Plugin {Name} is loaded with version {Version}!");
        Log.LogInfo($"Built on v73 of Lethal Company ;)");
        ModConfig.Init(Config);
        EventManagement.Subscribe();
        Menu.CreateBetterCommand();
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    internal static void ShowHint()
    {
        if (HintOnce)
            return;

        if (Terminal == null)
        {
            Log.LogError("~~ FATAL ERROR ~~");
            Log.LogError("Terminal instance is NULL");
            Log.LogError("~~ FATAL ERROR ~~");
            return;
        }
        Terminal.StartCoroutine(Coroutines.ChatHints());
        Terminal.StartCoroutine(Coroutines.HudHints());
        HintOnce = true;
    }

    internal static bool TryGetGameObject(string location, out GameObject gameobj)
    {
        gameobj = GameObject.Find(location);
        return gameobj != null;
    }

    internal static void SaveToConfig(List<string> stringList, out string configItem)
    {
        configItem = string.Join(", ", stringList);
        Loggers.LogDebug($"Saving to config\n{configItem}");
    }

    internal static string PersonalFilesPath()
    {
        if (!string.IsNullOrEmpty(_personalFiles))
            return _personalFiles;

        _personalFiles = Path.Combine(@"%userprofile%\appdata\locallow\ZeekerssRBLX\Lethal Company", "suitsTerminal");
        _personalFiles = Environment.ExpandEnvironmentVariables(_personalFiles);
        if (!Directory.Exists(_personalFiles))
            Directory.CreateDirectory(_personalFiles);

        Loggers.LogDebug($"suitsTerminal files path - {_personalFiles}");

        return _personalFiles;
    }

    internal static void SaveFavorites(string saveText)
    {
        if (ModConfig.PersonalizedFavorites.Value)
        {
            string favsFilePath = Path.Combine(@"%userprofile%\appdata\locallow\ZeekerssRBLX\Lethal Company", "suitsTerminal") + "\\masterFavsListing.txt";
            favsFilePath = Environment.ExpandEnvironmentVariables(favsFilePath);
            File.WriteAllText(favsFilePath, saveText);
            Loggers.LogDebug($"Favorites saved to file at {favsFilePath}");
        }
        else
            ModConfig.FavoritesMenuList.Value = saveText;
    }

    internal static void SaveDefault(string saveText)
    {
        if (ModConfig.PersonalizedDefault.Value)
        {
            string FilePath = Path.Combine(@"%userprofile%\appdata\locallow\ZeekerssRBLX\Lethal Company", "suitsTerminal") + "\\defaultSuit.txt";
            FilePath = Environment.ExpandEnvironmentVariables(FilePath);
            File.WriteAllText(FilePath, saveText);
            Loggers.LogDebug($"Personalized default saved to file at {FilePath}");
        }
        else
            ModConfig.DefaultSuit.Value = saveText;
    }

    internal static string TerminalFriendlyString(string s)
    {
        StringBuilder stringBuilder = new();
        foreach (char c in s)
        {
            if (!char.IsPunctuation(c))
            {
                stringBuilder.Append(c);
            }
        }

        if (stringBuilder.Length > 14)
        {
            int excessLength = stringBuilder.Length - 14;
            stringBuilder.Remove(14, excessLength);
            //Plugin.X($"terminalFriendlystring: {stringBuilder}");
        }


        return stringBuilder.ToString().ToLower();
    }

    internal static string ChatListing(int pageSize, int currentPage)
    {
        int listing = RackManager.AllSuits.Count;

        // Ensure currentPage is within valid range
        currentPage = Mathf.Clamp(currentPage, 1, Mathf.CeilToInt((float)listing / pageSize));

        // Calculate the start and end indexes for the current page
        int startIndex = (currentPage - 1) * pageSize;
        int endIndex = Mathf.Min(startIndex + pageSize, listing);
        StringBuilder message = new();

        message.Append("\r\n");

        // Iterate through each item in the current page
        for (int i = startIndex; i < endIndex; i++)
        {
            SuitAttributes suit = RackManager.AllSuits[i];

            // Append "[EQUIPPED]" line if applicable
            string menuItem = $"{suit.Name}" + (suit.IsCurrent() ? " [EQUIPPED]" : "");

            // Display the menu item
            message.Append($"'!wear {i}' (" + menuItem + ")\r\n");
        }

        // Display pagination information
        message.Append("\r\n");
        message.Append($"Page {currentPage}/{Mathf.CeilToInt((float)listing / pageSize)}\r\n");

        return message.ToString();
    }
}
