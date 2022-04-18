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
        public const string Version = "1.6.2";
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
        public static ConfigEntry<bool> ChatCommand { get; private set; }
        public static ConfigEntry<bool> KeyCommand { get; private set; }
        public static ConfigEntry<bool> NameOutline { get; private set; }
        public static ConfigEntry<bool> CPS { get; private set; }
        public static ConfigEntry<bool> RoomOption { get; private set; }
        public static ConfigEntry<bool> NokillCool { get; private set; }
        public static ConfigEntry<bool> DateTimeSetting { get; private set; }

        //Mod詳細設定
        //ランダムマップ
        public static ConfigEntry<bool> AddTheSkeld { get; private set; }
        public static ConfigEntry<bool> AddMIRAHQ { get; private set; }
        public static ConfigEntry<bool> AddPolus { get; private set; }
        public static ConfigEntry<bool> AddAirShip { get; private set; }

        //ロビーコード
        public static ConfigEntry<string> SetLobbyCode { get; private set; }
        public static ConfigEntry<string> SetCodeColor { get; private set; }

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

        //プラットフォームKick
        public static ConfigEntry<string> SetOPkick { get; private set; }

        //CPS
        public static ConfigEntry<float> CPSpositionX { get; private set; }
        public static ConfigEntry<float> CPSpositionY { get; private set; }

        public static bool SettingCPS = false;

        //DateTime
        public static ConfigEntry<float> DateTimepositionX { get; private set; }
        public static ConfigEntry<float> DateTimepositionY { get; private set; }

        public static bool SettingDateTime = false;


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
            Zoom = Config.Bind("Client Options", "Zoom", true);
            OPkick = Config.Bind("Client Options", "OPkick", false);
            SendJoinPlayer = Config.Bind("Client Options", "SendJoinPlayer", false);
            DoubleName = Config.Bind("Client Options", "DoubleName", false);
            ChangeGameName = Config.Bind("Client Options", "ChangeGameName", false);
            AutoCopyCode = Config.Bind("Client Options", "AutoCopyCode", false);
            RainbowOutline = Config.Bind("Client Options", "RainbowOutline", false);
            CrewColorOutline = Config.Bind("Client Options", "CrewColorOutline", false);
            RainbowVent = Config.Bind("Client Options", "RainbowVent", false);
            CrewColorVent = Config.Bind("Client Options", "CrewColorVent", false);
            ChatCommand = Config.Bind("Client Options", "ChatCommand", true);
            KeyCommand = Config.Bind("Client Options", "KeyCommand", true);
            NameOutline = Config.Bind("Client Options", "OutlineName", false);
            CPS = Config.Bind("Client Options", "CPS", false);
            RoomOption = Config.Bind("Client Options", "RoomOption", true);
            NokillCool = Config.Bind("Client Options", "NokillCool", false);
            DateTimeSetting = Config.Bind("Client Options", "DateTimeSetting", false);


            //ランダムマップ
            AddTheSkeld = Config.Bind("RandomMaps Options", "AddTheSkeld", true);
            AddMIRAHQ = Config.Bind("RandomMaps Options", "AddMIRAHQ", true);
            AddPolus = Config.Bind("RandomMaps Options", "AddPolus", true);
            AddAirShip = Config.Bind("RandomMaps Options", "AddAirShip", true);

            //ロビーコード
            SetLobbyCode = Config.Bind("LobbyCode Options", "SetLobbyCode", Name);
            SetCodeColor = Config.Bind("LobbyCode Options", "SetCodeColor", "FFFFFF");

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

            //プラットフォームKick
            SetOPkick = Config.Bind("SetOPkick Options", "SetOPkick","3,4,5,6,7,8,9,10,");

            //CPS
            CPSpositionX = Config.Bind("CPS Options", "CPSpositionX", 0f);
            CPSpositionY = Config.Bind("CPS Options", "CPSpositionY", 2.75f);

            //DateTime
            DateTimepositionX = Config.Bind("DateTime Options", "DateTimepositionX", 0f);
            DateTimepositionY = Config.Bind("DateTime Options", "DateTimepositionY", 2.75f);

            Harmony.PatchAll();
        }
    }
}
