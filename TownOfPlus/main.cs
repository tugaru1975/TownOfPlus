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
        public const string Version = "1.3.0";
        public static System.Version VersionId = System.Version.Parse(Version);

        public Harmony Harmony { get; } = new Harmony(Id);

        //Modの名前
        public static string ModNameText = "\r\n" + Name + " v" + Version;

        //Modアップデート
        public static ConfigEntry<string> ShowPopUpVersion { get; set; }

        //帽子URL
        public static ConfigEntry<string> HatURL { get; private set; }

        //Mod設定
        public static ConfigEntry<bool> HideLobbyCodes { get; private set; }
        public static ConfigEntry<bool> LobbyTimer { get; private set; }
        public static ConfigEntry<bool> RandomMaps { get; private set; }
        public static ConfigEntry<bool> RainbowHat { get; private set; }
        public static ConfigEntry<bool> RainbowVisor { get; private set; }
        public static ConfigEntry<bool> RainbowName { get; private set; }
        public static ConfigEntry<bool> TranslucentName { get; private set; }
        public static ConfigEntry<bool> FakeLevel { get; private set; }
        public static ConfigEntry<bool> HideNameplates { get; private set; }
        public static ConfigEntry<bool> Zoom { get; private set; }
        public static ConfigEntry<bool> OPkick { get; private set; }
        public static ConfigEntry<bool> SendJoinPlayer { get; private set; }
        public static ConfigEntry<bool> DoubleName { get; private set; }
        public static ConfigEntry<bool> ChangeGameName { get; private set; }
        public static ConfigEntry<bool> AutoCopyCode { get; private set; }
        public static ConfigEntry<bool> RainbowOutline { get; private set; }
        public static ConfigEntry<bool> CrewColorOutline { get; private set; }
        public static ConfigEntry<bool> RainbowVent { get; private set; }
        public static ConfigEntry<bool> CrewColorVent { get; private set; }

        //Mod詳細設定
        //ランダムマップ
        public static ConfigEntry<bool> AddTheSkeld { get; private set; }
        public static ConfigEntry<bool> AddMIRAHQ { get; private set; }
        public static ConfigEntry<bool> AddPolus { get; private set; }
        public static ConfigEntry<bool> AddAirShip { get; private set; }

        //ロビーコード
        public static ConfigEntry<string> SetLobbyCode { get; private set; }

        //偽のレベル
        public static ConfigEntry<int> SetLevel { get; private set; }

        //参加者にチャットを送る
        public static ConfigEntry<string> SetSendJoinChat { get; private set; }

        //二段の名前
        public static ConfigEntry<string> SetDoubleName { get; private set; }

        //ゲーム中の名前
        public static ConfigEntry<string> SetGameName { get; private set; }

        //半透明の名前
        public static ConfigEntry<int> SetTranslucentName { get; private set; }

        public static string NewHatURL = "";

        public override void Load()
        {
            //Hatファイル作成
            Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + @"\TOPHats\");
            Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + @"\TOPHats\Test\");

            //アップデート
            ShowPopUpVersion = Config.Bind("Update", "Show PopUp", "0");

            //帽子URL
            HatURL = Config.Bind("HatURL", "HatURL", "https://raw.githubusercontent.com/tugaru1975/TOPHats/master,https://raw.githubusercontent.com/ユーザー名/プロジェクト名/master");

            NewHatURL = HatURL.Value;

            //設定項目
            HideLobbyCodes = Config.Bind("Client Options", "HideLobbyCodes", false);
            LobbyTimer = Config.Bind("Client Options", "LobbyTimer", false);
            RandomMaps = Config.Bind("Client Options", "RandomMapsMode", false);
            RainbowHat = Config.Bind("Client Options", "RainbowHat", false);
            RainbowVisor = Config.Bind("Client Options", "RainbowVisor", false);
            RainbowName = Config.Bind("Client Options", "RainbowName", false);
            TranslucentName = Config.Bind("Client Options", "TranslucentName", false);
            FakeLevel = Config.Bind("Client Options", "FakeLevel", false);
            HideNameplates = Config.Bind("Client Options", "HideNameplates", false);
            Zoom = Config.Bind("Client Options", "Zoom", false);
            OPkick = Config.Bind("Client Options", "OPkick", false);
            SendJoinPlayer = Config.Bind("Client Options", "SendJoinPlayer", false);
            DoubleName = Config.Bind("Client Options", "DoubleName", false);
            ChangeGameName = Config.Bind("Client Options", "ChangeGameName", false);
            AutoCopyCode = Config.Bind("Client Options", "AutoCopyCode", false);
            RainbowOutline = Config.Bind("Client Options", "RainbowOutline", false);
            CrewColorOutline = Config.Bind("Client Options", "CrewColorOutline", false);
            RainbowVent = Config.Bind("Client Options", "RainbowVent", false);
            CrewColorVent = Config.Bind("Client Options", "CrewColorVent", false);

            //ランダムマップ
            AddTheSkeld = Config.Bind("RandomMaps Options", "AddTheSkeld", true);
            AddMIRAHQ = Config.Bind("RandomMaps Options", "AddMIRAHQ", true);
            AddPolus = Config.Bind("RandomMaps Options", "AddPolus", true);
            AddAirShip = Config.Bind("RandomMaps Options", "AddAirShip", true);

            //ロビーコード
            SetLobbyCode = Config.Bind("LobbyCode Options", "SetLobbyCode", Name);

            //偽のレベル
            SetLevel = Config.Bind("FakeLevel Options", "SetLevel", 101);

            //参加者にチャットを送る
            SetSendJoinChat = Config.Bind("SendJoinPlayer Options", "SetSendJoinChat", "TownOfPlusを使用しています");
            
            //二段の名前
            SetDoubleName = Config.Bind("SetDoubleName Options", "SetDoubleName", "二段目");

            //ゲーム中の名前
            SetGameName = Config.Bind("SetGameName Options", "SetGameName", "部屋主");

            //半透明の名前
            SetTranslucentName = Config.Bind("SetTranslucentName Options", "SetTranslucentName", 75);

            Harmony.PatchAll();
        }
    }
}
