using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using UnityEngine;


namespace TownOfPlus
{
    [BepInPlugin(Id, Name, Version)]
    [BepInProcess("Among Us.exe")]
    public class main : BasePlugin
    {
        //Modの詳細
        public const string Id = "com.tugaru.TownOfPlus";
        public const string Name = "TownOfPlus";
        public const string Version = "1.8.0";
        public static Version VersionId = System.Version.Parse(Version);
        internal static ManualLogSource Logger;
        public Harmony Harmony { get; } = new Harmony(Id);

        //Modの名前
        public static string ModNameText = Name + " v" + Version;

        //TownOfPlusのURL
        public static readonly string TOPUrl = Path.GetDirectoryName(Application.dataPath) + @"\TownOfPlus\";

        //Modアップデート
        public static ConfigEntry<bool> DebugMode { get; set; }

        //Modアップデート
        public static ConfigEntry<string> ShowPopUpVersion { get; set; }

        //帽子URL
        public static ConfigEntry<string> HatURL { get; private set; }
        public static string NewHatURL = "";

        //バイザーURL
        public static ConfigEntry<string> VisorURL { get; private set; }
        public static string NewVisorURL = "";

        //ネームプレートURL
        public static ConfigEntry<string> NamePlateURL { get; private set; }
        public static string NewNamePlateURL = "";

        //Mod設定
        public static ConfigEntry<bool> ChangeLobbyCodes { get; private set; }
            public static ConfigEntry<string> SetLobbyCode { get; private set; }
            public static ConfigEntry<string> SetCodeColor { get; private set; }

        public static ConfigEntry<bool> LobbyTimer { get; private set; }

        public static ConfigEntry<bool> RandomMaps { get; private set; }
            public static ConfigEntry<bool> AddTheSkeld { get; private set; }
            public static ConfigEntry<bool> AddMIRAHQ { get; private set; }
            public static ConfigEntry<bool> AddPolus { get; private set; }
            public static ConfigEntry<bool> AddAirShip { get; private set; }

        public static ConfigEntry<bool> RainbowHat { get; private set; }

        public static ConfigEntry<bool> RainbowVisor { get; private set; }

        public static ConfigEntry<bool> RainbowSkin { get; private set; }

        public static ConfigEntry<bool> RainbowPet { get; private set; }

        public static ConfigEntry<bool> RainbowName { get; private set; }

        public static ConfigEntry<bool> TranslucentName { get; private set; }
            public static ConfigEntry<int> SetTranslucentName { get; private set; }

        public static ConfigEntry<bool> FakeLevel { get; private set; }

        public static ConfigEntry<bool> HideNameplates { get; private set; }

        public static ConfigEntry<bool> Zoom { get; private set; }

        public static ConfigEntry<bool> OPkick { get; private set; }
            public static ConfigEntry<bool> AddUnknown { get; private set; }
            public static ConfigEntry<bool> AddEpicPC { get; private set; }
            public static ConfigEntry<bool> AddSteamPC { get; private set; }
            public static ConfigEntry<bool> AddMac { get; private set; }
            public static ConfigEntry<bool> AddWin10 { get; private set; }
            public static ConfigEntry<bool> AddItch { get; private set; }
            public static ConfigEntry<bool> AddIPhone { get; private set; }
            public static ConfigEntry<bool> AddAndroid { get; private set; }
            public static ConfigEntry<bool> AddSwitch { get; private set; }
            public static ConfigEntry<bool> AddXbox { get; private set; }
            public static ConfigEntry<bool> AddPlaystation { get; private set; }

        public static ConfigEntry<bool> SendJoinPlayer { get; private set; }
            public static ConfigEntry<string> SetSendJoinChat { get; private set; }

        public static ConfigEntry<bool> DoubleName { get; private set; }
            public static ConfigEntry<string> SetDoubleName { get; private set; }

        public static ConfigEntry<bool> ChangeGameName { get; private set; }
            public static ConfigEntry<string> SetGameName { get; private set; }

        public static ConfigEntry<bool> AutoCopyCode { get; private set; }

        public static ConfigEntry<bool> RainbowOutline { get; private set; }

        public static ConfigEntry<bool> CrewColorOutline { get; private set; }

        public static ConfigEntry<bool> RainbowVent { get; private set; }

        public static ConfigEntry<bool> CrewColorVent { get; private set; }

        public static ConfigEntry<bool> ChatCommand { get; private set; }
            public static ConfigEntry<bool> ComTab { get; private set; }
            public static ConfigEntry<bool> ComCancel { get; private set; }
            public static ConfigEntry<bool> ChangeComWord { get; private set; }

        public static ConfigEntry<bool> KeyCommand { get; private set; }
            public static ConfigEntry<bool> KeyDelete { get; private set; }
            public static ConfigEntry<bool> KeyUndoAndRedo { get; private set; }
            public static ConfigEntry<bool> KeyCopy { get; private set; }
            public static ConfigEntry<bool> KeyCut { get; private set; }
            public static ConfigEntry<bool> KeyPaste { get; private set; }
        public static ConfigEntry<bool> NameOutline { get; private set; }

        public static ConfigEntry<bool> CPS { get; private set; }
            public static ConfigEntry<float> CPSpositionX { get; private set; }
            public static ConfigEntry<float> CPSpositionY { get; private set; }
            public static bool SettingCPS = false;

        public static ConfigEntry<bool> RoomOption { get; private set; }
            public static ConfigEntry<bool> AdvancedNum { get; private set; }
            public static ConfigEntry<bool> ShowMapSelect { get; private set; }

        public static ConfigEntry<bool> DateTimeSetting { get; private set; }
            public static ConfigEntry<float> DateTimepositionX { get; private set; }
            public static ConfigEntry<float> DateTimepositionY { get; private set; }
            public static bool SettingDateTime = false;

        public static ConfigEntry<bool> CustomOverlay { get; private set; }
            public static ConfigEntry<bool> ShowRoleSetting { get; private set; }
            public static ConfigEntry<bool> ShowFriendText { get; private set; }
            public static ConfigEntry<bool> ShowLevelText { get; private set; }
            public static ConfigEntry<bool> ShowHostColor { get; private set; }
            public static ConfigEntry<bool> ShowColorName { get; private set; }
            public static ConfigEntry<bool> FirstColorName { get; private set; }
            public static ConfigEntry<KeyCode> CustomOverlayKeyBind { get; private set; }

        public static ConfigEntry<bool> OldPingPositon { get; private set; }

        public static ConfigEntry<bool> CrewColorChat { get; private set; }

        public static ConfigEntry<bool> TranslucentChat { get; private set; }
            public static ConfigEntry<int> SetTranslucentChat { get; private set; }

        public static ConfigEntry<bool> CrewColorVoteArea { get; private set; }

        public static ConfigEntry<bool> FPS { get; private set; }
            public static ConfigEntry<float> FPSpositionX { get; private set; }
            public static ConfigEntry<float> FPSpositionY { get; private set; }
            public static bool SettingFPS = false;

        public static ConfigEntry<bool> FakePing { get; private set; }
            public static ConfigEntry<int> SetFakePing { get; private set; }

        public static ConfigEntry<bool> SkipLogo { get; private set; }

        public static ConfigEntry<bool> ChangeNameBox { get; private set; }

        public static ConfigEntry<bool> AlwaysChat { get; private set; }

        public static ConfigEntry<bool> ChatLimitPlus { get; private set; }

        public static ConfigEntry<bool> CancelChatMap { get; private set; }

        public static ConfigEntry<bool> ShowHost { get; private set; }

        public static ConfigEntry<bool> PasteCodeText { get; private set; }

        public static ConfigEntry<bool> CodeTextPlus { get; private set; }

        public static ConfigEntry<bool> AlwaysStart { get; private set; }

        public static ConfigEntry<KeyCode> StartCount { get; private set; }

        public static ConfigEntry<KeyCode> ResetCount { get; private set; }

        public static ConfigEntry<bool> StartCountText { get; private set; }

        public static ConfigEntry<bool> FixSkinBag { get; private set; }

        public static ConfigEntry<bool> FixLeftPlayer { get; private set; }

        public static ConfigEntry<bool> FixPlayerColor { get; private set; }

        public static ConfigEntry<bool> WallWalk { get; private set; }
            public static ConfigEntry<KeyCode> WallWalkKeyBind { get; private set; }

        public static ConfigEntry<bool> EndGame { get; private set; }
            public static ConfigEntry<KeyCode> EndGameKeyBindFirst { get; private set; }
            public static ConfigEntry<KeyCode> EndGameKeyBindSecond { get; private set; }
            public static ConfigEntry<KeyCode> EndGameKeyBindThird { get; private set; }

        public static ConfigEntry<bool> CrewColorText { get; private set; }

        public override void Load()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("TownOfPlus");

            //TOPファイル作成
            Directory.CreateDirectory(TOPUrl);

            //テストファイル作成
            Directory.CreateDirectory(TOPUrl + @"SkinTest");

            //テストHatファイル作成
            Directory.CreateDirectory(TOPUrl + @"SkinTest\HatTest");

            //テストVisorファイル作成
            Directory.CreateDirectory(TOPUrl + @"SkinTest\VisorTest");

            //テストNamePlateファイル作成
            Directory.CreateDirectory(TOPUrl + @"SkinTest\NamePlateTest");

            //Hatファイル作成
            Directory.CreateDirectory(TOPUrl + @"TOPHats");

            //Visorファイル作成
            Directory.CreateDirectory(TOPUrl + @"TOPVisors");

            //NamePlateファイル作成
            Directory.CreateDirectory(TOPUrl + @"TOPNamePlates");

            //SkinDataファイル作成
            Directory.CreateDirectory(TOPUrl + @"SkinData");

            //Logファイル作成
            Directory.CreateDirectory(TOPUrl + @"Log");

            //アップデート
            DebugMode = Config.Bind("Debug", "DebugMode", false);

            //アップデート
            ShowPopUpVersion = Config.Bind("Update", "Show PopUp", "0");

            //帽子URL
            HatURL = Config.Bind("HatURL", "HatURL", "https://raw.githubusercontent.com/ユーザー名/プロジェクト名/master");
            HatURL.Value = HatURL.Value.TrimAll("https://raw.githubusercontent.com/tugaru1975/TOPHats/master,");
            NewHatURL = "https://raw.githubusercontent.com/tugaru1975/TOPHats/master," + HatURL.Value.Replace("github.com", "raw.githubusercontent.com");

            //バイザーURL
            VisorURL = Config.Bind("VisorURL", "VisorURL", "https://raw.githubusercontent.com/ユーザー名/プロジェクト名/master");
            VisorURL.Value = VisorURL.Value.TrimAll("https://raw.githubusercontent.com/tugaru1975/TOPVisors/master,");
            NewVisorURL = "https://raw.githubusercontent.com/tugaru1975/TOPVisors/master," + VisorURL.Value.Replace("github.com", "raw.githubusercontent.com");

            //ネームプレートURL
            NamePlateURL = Config.Bind("NamePlateURL", "NamePlateURL", "https://raw.githubusercontent.com/ユーザー名/プロジェクト名/master");
            NamePlateURL.Value = NamePlateURL.Value.TrimAll("https://raw.githubusercontent.com/tugaru1975/TOPNamePlates/master,");
            NewNamePlateURL = "https://raw.githubusercontent.com/tugaru1975/TOPNamePlates/master," + NamePlateURL.Value.Replace("github.com", "raw.githubusercontent.com");

            //設定項目
            ChangeLobbyCodes = Config.Bind("Client Options", "HideLobbyCodes", false);
                SetLobbyCode = Config.Bind("LobbyCode Options", "SetLobbyCode", Name);
                SetCodeColor = Config.Bind("LobbyCode Options", "SetCodeColor", "FFFFFF");

            LobbyTimer = Config.Bind("Client Options", "LobbyTimer", false);

            RandomMaps = Config.Bind("Client Options", "RandomMapsMode", false);
                AddTheSkeld = Config.Bind("RandomMaps Options", "AddTheSkeld", true);
                AddMIRAHQ = Config.Bind("RandomMaps Options", "AddMIRAHQ", true);
                AddPolus = Config.Bind("RandomMaps Options", "AddPolus", true);
                AddAirShip = Config.Bind("RandomMaps Options", "AddAirShip", true);

            RainbowHat = Config.Bind("Client Options", "RainbowHat", false);

            RainbowVisor = Config.Bind("Client Options", "RainbowVisor", false);

            RainbowSkin = Config.Bind("Client Options", "RainbowSkin", false);

            RainbowPet = Config.Bind("Client Options", "RainbowPet", false);

            RainbowName = Config.Bind("Client Options", "RainbowName", false);

            TranslucentName = Config.Bind("Client Options", "TranslucentName", false);
                SetTranslucentName = Config.Bind("SetTranslucentName Options", "SetTranslucentName", 75);

            FakeLevel = Config.Bind("Client Options", "FakeLevel", false);

            HideNameplates = Config.Bind("Client Options", "HideNameplates", false);

            Zoom = Config.Bind("Client Options", "Zoom", false);

            OPkick = Config.Bind("Client Options", "OPkick", false);
                AddUnknown = Config.Bind("OPkick Options", "AddUnknown", true);
                AddEpicPC = Config.Bind("OPkick Options", "AddEpicPC", false);
                AddSteamPC = Config.Bind("OPkick Options", "AddSteamPC", false);
                AddMac = Config.Bind("OPkick Options", "AddMac", true);
                AddWin10 = Config.Bind("OPkick Options", "AddWin10", true);
                AddItch = Config.Bind("OPkick Options", "AddItch", true);
                AddIPhone = Config.Bind("OPkick Options", "AddIPhone", true);
                AddAndroid = Config.Bind("OPkick Options", "AddAndroid", true);
                AddSwitch = Config.Bind("OPkick Options", "AddSwitch", true);
                AddXbox = Config.Bind("OPkick Options", "AddXbox", true);
                AddPlaystation = Config.Bind("OPkick Options", "AddPlaystation", true);

            SendJoinPlayer = Config.Bind("Client Options", "SendJoinPlayer", false);
                SetSendJoinChat = Config.Bind("SendJoinPlayer Options", "SetSendJoinChat", "TownOfPlusを使用しています");

            DoubleName = Config.Bind("Client Options", "DoubleName", false);
                SetDoubleName = Config.Bind("SetDoubleName Options", "SetDoubleName", "二段目");

            ChangeGameName = Config.Bind("Client Options", "ChangeGameName", false);
                SetGameName = Config.Bind("SetGameName Options", "SetGameName", "部屋主");

            AutoCopyCode = Config.Bind("Client Options", "AutoCopyCode", false);

            RainbowOutline = Config.Bind("Client Options", "RainbowOutline", false);

            CrewColorOutline = Config.Bind("Client Options", "CrewColorOutline", false);

            RainbowVent = Config.Bind("Client Options", "RainbowVent", false);

            CrewColorVent = Config.Bind("Client Options", "CrewColorVent", false);

            ChatCommand = Config.Bind("Client Options", "ChatCommand", false);
                ComTab = Config.Bind("ChatCommand Options", "KeyTab", true);
                ComCancel = Config.Bind("ChatCommand Options", "ComCancel", true);
                ChangeComWord = Config.Bind("ChatCommand Options", "ChangeComWord", false);

            KeyCommand = Config.Bind("Client Options", "KeyCommand", false);
                KeyDelete = Config.Bind("KeyCommand Options", "KeyDelete", true);
                KeyUndoAndRedo = Config.Bind("KeyCommand Options", "KeyUndoAndRedo", true);
                KeyCopy = Config.Bind("KeyCommand Options", "KeyCopy", true);
                KeyCut = Config.Bind("KeyCommand Options", "KeyCut", true);
                KeyPaste = Config.Bind("KeyCommand Options", "KeyPaste", true);

            NameOutline = Config.Bind("Client Options", "OutlineName", false);

            CPS = Config.Bind("Client Options", "CPS", false);
                CPSpositionX = Config.Bind("CPS Options", "CPSpositionX", 0f);
                CPSpositionY = Config.Bind("CPS Options", "CPSpositionY", 2.75f);

            RoomOption = Config.Bind("Client Options", "RoomOption", false);
                AdvancedNum = Config.Bind("RoomOption Options", "AdvancedNum", true);
                ShowMapSelect = Config.Bind("RoomOption Options", "ShowMapSelect", true);

            DateTimeSetting = Config.Bind("Client Options", "DateTimeSetting", false);
                DateTimepositionX = Config.Bind("DateTime Options", "DateTimepositionX", 0f);
                DateTimepositionY = Config.Bind("DateTime Options", "DateTimepositionY", 2.75f);

            CustomOverlay = Config.Bind("Client Options", "CustomOverlay", false);
                ShowRoleSetting = Config.Bind("CustomOverlay Options", "ShowRoleSetting", true);
                ShowFriendText = Config.Bind("CustomOverlay Options", "ShowFriendText", true);
                ShowLevelText = Config.Bind("CustomOverlay Options", "ShowLevelText", true);
                ShowHostColor = Config.Bind("CustomOverlay Options", "ShowHostColor", true);
                ShowColorName = Config.Bind("CustomOverlay Options", "ShowColorName", true);
                FirstColorName = Config.Bind("CustomOverlay Options", "FirstColorName", true);
                CustomOverlayKeyBind = Config.Bind("CustomOverlay Options", "CustomOverlayKeyBind", KeyCode.F3);

            OldPingPositon = Config.Bind("Client Options", "OldPingPositon", false);

            CrewColorChat = Config.Bind("Client Options", "CrewColorChat", false);

            TranslucentChat = Config.Bind("Client Options", "TranslucentChat", false);
                SetTranslucentChat = Config.Bind("SetTranslucentChat Options", "SetTranslucentChat", 75);

            CrewColorVoteArea = Config.Bind("Client Options", "CrewColorVoteArea", false);

            FPS = Config.Bind("Client Options", "FPS", false);
                FPSpositionX = Config.Bind("FPS Options", "FPSpositionX", 0f);
                FPSpositionY = Config.Bind("FPS Options", "FPSpositionY", 2.75f);

            FakePing = Config.Bind("Client Options", "FakePing", false);
                SetFakePing = Config.Bind("FakePing Options", "SetFakePing", 100);

            SkipLogo = Config.Bind("Client Options", "SkipLogo", false);

            ChangeNameBox = Config.Bind("Client Options", "ChangeNameBox", false);

            AlwaysChat = Config.Bind("Client Options", "AlwaysChat", false);

            ChatLimitPlus = Config.Bind("Client Options", "ChatLimitPlus", false);

            CancelChatMap = Config.Bind("Client Options", "CancelChatMap", false);

            ShowHost = Config.Bind("Client Options", "ShowHost", false);

            PasteCodeText = Config.Bind("Client Options", "CodeTextPlus", false);

            CodeTextPlus = Config.Bind("Client Options", "CodeTextPlus", false);

            AlwaysStart = Config.Bind("Client Options", "AlwaysStart", false);

            StartCount = Config.Bind("Client Options", "StartCount", KeyCode.LeftShift);

            ResetCount = Config.Bind("Client Options", "ResetCount", KeyCode.C);

            StartCountText = Config.Bind("Client Options", "StartCountText", false);

            FixSkinBag = Config.Bind("Client Options", "FixSkinBag", false);

            FixLeftPlayer = Config.Bind("Client Options", "FixLeftPlayer", false);

            FixPlayerColor = Config.Bind("Client Options", "FixPlayerColor", false);

            WallWalk = Config.Bind("Client Options", "WallWalk", false);
                WallWalkKeyBind = Config.Bind("WallWalk Options", "WallWalkKeyBind", KeyCode.LeftControl);

            EndGame = Config.Bind("Client Options", "EndGame", false);
                EndGameKeyBindFirst = Config.Bind("EndGame Options", "EndGameKeyBindFirst", KeyCode.LeftShift);
                EndGameKeyBindSecond = Config.Bind("EndGame Options", "EndGameKeyBindSecond", KeyCode.H);
                EndGameKeyBindThird = Config.Bind("EndGame Options", "EndGameKeyBindThird", KeyCode.Return);

            CrewColorText = Config.Bind("Client Options", "CrewColorText", false);

            Harmony.PatchAll();
        }
    }
}
