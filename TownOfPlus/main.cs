using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnhollowerBaseLib;
using Hazel;
using System;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace TownOfPlus
{
    [BepInPlugin(Id, Name, Version)]
    [BepInProcess("Among Us.exe")]
    public class main : BasePlugin
    {
        //Modの詳細
        public const string Id = "com.tugaru.TownOfPlus";
        public const string Name = "TownOfPlus";
        public const string Version = "1.0.0";
        public Harmony Harmony { get; } = new Harmony(Id);

        //Modの関数
        public static string ModNameText = "\r\n" + Name + " v" + Version;

        public static ConfigEntry<bool> HideLobbyCodes { get; private set; }
        public static ConfigEntry<bool> LobbyTimer { get; private set; }
        public static ConfigEntry<bool> RandomMaps { get; private set; }
        public static ConfigEntry<bool> RainbowSkin { get; private set; }
        public static ConfigEntry<bool> RainbowVisor { get; private set; }
        public static ConfigEntry<bool> RainbowName { get; private set; }
        public static ConfigEntry<bool> TranslucentName { get; private set; }
        public static ConfigEntry<bool> FakeLevel { get; private set; }
        public static ConfigEntry<bool> HideNameplates { get; private set; }
        public static ConfigEntry<bool> Zoom { get; private set; }
        public static ConfigEntry<bool> OPkick { get; private set; }

        //Mod詳細設定
        public static bool AddTheSkeld = true;
        public static bool AddMIRAHQ = true;
        public static bool AddPolus = true;
        public static bool AddAirShip = true;
        public static string LobbyCode = Name;
        public static int SetLevel = 101;

        public override void Load()
        {
            //設定項目
            HideLobbyCodes = Config.Bind("Client Options", "HideLobbyCodes", false);
            LobbyTimer = Config.Bind("Client Options", "LobbyTimer", false);
            RandomMaps = Config.Bind("Client Options", "RandomMapsMode", false);
            RainbowSkin = Config.Bind("Client Options", "RainbowSkin", false);
            RainbowVisor = Config.Bind("Client Options", "RainbowVisor", false);
            RainbowName = Config.Bind("Client Options", "RainbowName", false);
            TranslucentName = Config.Bind("Client Options", "TranslucentName", false);
            FakeLevel = Config.Bind("Client Options", "FakeLevel", false);
            HideNameplates = Config.Bind("Client Options", "HideNameplates", false);
            Zoom = Config.Bind("Client Options", "Zoom", false);
            OPkick = Config.Bind("Client Options", "OPkick", false);

            Harmony.PatchAll();
        }
    }
}
