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
        public static main Instance;
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
        public static ModOption ChangeLobbyCodes;
        public static ModOption SetLobbyCode;
        public static ModOption SetCodeColor;

        public static ModOption LobbyTimer;

        public static ModOption RandomMaps;
        public static ModOption AddTheSkeld;
        public static ModOption AddMIRAHQ;
        public static ModOption AddPolus;
        public static ModOption AddAirShip;

        public static ModOption RainbowHat;

        public static ModOption RainbowVisor;

        public static ModOption RainbowSkin;

        public static ModOption RainbowPet;

        public static ModOption RainbowName;

        public static ModOption TranslucentName;
        public static ModOption SetTranslucentName;

        public static ModOption FakeLevel;

        public static ModOption HideNameplates;

        public static ModOption Zoom;

        public static ModOption OPkick;
        public static ModOption AddUnknown;
        public static ModOption AddEpicPC;
        public static ModOption AddSteamPC;
        public static ModOption AddMac;
        public static ModOption AddWin10;
        public static ModOption AddItch;
        public static ModOption AddIPhone;
        public static ModOption AddAndroid;
        public static ModOption AddSwitch;
        public static ModOption AddXbox;
        public static ModOption AddPlaystation;

        public static ModOption SendJoinPlayer;
        public static ModOption SetSendJoinChat;

        public static ModOption DoubleName;
        public static ModOption SetDoubleName;

        public static ModOption ChangeGameName;
        public static ModOption SetGameName;

        public static ModOption AutoCopyCode;

        public static ModOption RainbowOutline;

        public static ModOption CrewColorOutline;

        public static ModOption RainbowVent;

        public static ModOption CrewColorVent;

        public static ModOption ChatCommand;
        public static ModOption ComTab;
        public static ModOption ComCancel;
        public static ModOption ChangeComWord;

        public static ModOption KeyCommand;
        public static ModOption KeyDelete;
        public static ModOption KeyUndoAndRedo;
        public static ModOption KeyCopy;
        public static ModOption KeyCut;
        public static ModOption KeyPaste;
        public static ModOption NameOutline;

        public static ModOption CPS;
        public static ConfigEntry<float> CPSpositionX { get; private set; }
        public static ConfigEntry<float> CPSpositionY { get; private set; }
        public static bool SettingCPS = false;

        public static ModOption RoomOption;
        public static ModOption RemoveReset;
        public static ModOption AdvancedNum;
        public static ModOption ShowMapSelect;

        public static ModOption DateTimeSetting;
        public static ConfigEntry<float> DateTimepositionX { get; private set; }
        public static ConfigEntry<float> DateTimepositionY { get; private set; }
        public static bool SettingDateTime = false;

        public static ModOption CustomOverlay;
        public static ModOption ShowRolesSetting;
        public static ModOption ShowFriendText;
        public static ModOption ShowLevelText;
        public static ModOption ShowHostColor;
        public static ModOption ShowColorName;
        public static ModOption FirstColorName;
        public static ModOption ShowBlockedPlayer;
        public static ModOption CustomOverlayKeyBind;

        public static ModOption OldPingPositon;

        public static ModOption CrewColorChat;

        public static ModOption TranslucentChat;
        public static ModOption SetTranslucentChat;

        public static ModOption CrewColorVoteArea;

        public static ModOption FPS;
        public static ConfigEntry<float> FPSpositionX { get; private set; }
        public static ConfigEntry<float> FPSpositionY { get; private set; }
        public static bool SettingFPS = false;

        public static ModOption FakePing;
        public static ModOption SetFakePing;

        public static ModOption SkipLogo;

        public static ModOption ChangeNameBox;

        public static ModOption AlwaysChat;

        public static ModOption ChatLimitPlus;

        public static ModOption CancelChatMap;

        public static ModOption ShowHost;

        public static ModOption PasteCodeText;

        public static ModOption CodeTextPlus;

        public static ModOption AlwaysStart;

        public static ModOption StartCount;

        public static ModOption ResetCount;

        public static ModOption StartCountText;

        public static ModOption FixSkinBug;

        public static ModOption FixLeftPlayer;

        public static ModOption FixPlayerColor;

        public static ModOption WallWalk;
        public static ModOption WallWalkKeyBind;

        public static ModOption EndGame;
        public static ModOption EndGameKeyBindFirst;
        public static ModOption EndGameKeyBindSecond;
        public static ModOption EndGameKeyBindThird;

        public static ModOption CrewColorText;

        public static ModOption AutoBanBlockedPlayer;

        public override void Load()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("TownOfPlus");
            Instance = this;

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
            CPSpositionX = Config.Bind("CPS Options", "CPSpositionX", 0f);
            CPSpositionY = Config.Bind("CPS Options", "CPSpositionY", 2.75f);

            DateTimepositionX = Config.Bind("DateTime Options", "DateTimepositionX", 0f);
            DateTimepositionY = Config.Bind("DateTime Options", "DateTimepositionY", 2.75f);

            FPSpositionX = Config.Bind("FPS Options", "FPSpositionX", 0f);
            FPSpositionY = Config.Bind("FPS Options", "FPSpositionY", 2.75f);

            ModSetting.Load();

            Harmony.PatchAll();
        }
    }
}
