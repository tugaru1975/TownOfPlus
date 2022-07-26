using HarmonyLib;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfPlus
{
    public enum ModType
    {
        None,
        Toggle,
        File,
        KeyBind,
        TextBox,
    }

    public enum TextType
    {
        None,
        ColorText,
        nameText,
        Percent,
    }

    public enum ModTag
    {
        None,
        ChangeLobbyCodes,
        LobbyTimer,
        RoomOption,
        RandomMap,
        SkinColor,
        NameColor,
        Outline,
        FakeLevel,
        HideNameplates,
        Zoom,
        OPkick,
        SendJoinPlayer,
        DoubleName,
        ChangeGameName,
        AutoCopyCode,
        ChatCommand,
        KeyCommand,
        CPS,
        DateTimeSetting,
        CustomOverlay,
        OldPingPositon,
        CrewColorChat,
        TranslucentChat,
        CrewColorVoteArea,
        FPS,
        FakePing,
        SkipLogo,
        ChangeNameBox,
        ChatButtonPlus,
        ShowHost,
        CodeText,
        StartButton,
        FixBug,
        WallWalk,
        EndGame,
        CrewColorText,
        AutoBanBlockedPlayer,
    }

    [HarmonyPatch]
    public static class ModSetting
    {
        public static void Load()
        {
            main.ChangeLobbyCodes = new ModOption("部屋コード", "\n部屋のコードの文字を変更できます。", ModTag.ChangeLobbyCodes);
                main.SetCodeColor = new ModOption("コードカラーの変更", "FFFFFF", "\nカラーコードを入力して部屋コードの色を変更します", TextType.ColorText, ModTag.ChangeLobbyCodes);
                main.SetLobbyCode = new ModOption("コードの変更", main.Name, "\n部屋コードを変更します", TextType.None, ModTag.ChangeLobbyCodes);

            main.LobbyTimer = new ModOption("ロビータイマー", "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\nロビーが落ちるまでの目安となるタイマーを表示します。", ModTag.LobbyTimer);

            main.RoomOption = new ModOption("部屋設定の拡張", "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\nオンライン状態でのマップ切り替え、設定のキルクールやタスク量の上限を変更します。", ModTag.RoomOption);
                main.RemoveReset = new ModOption("初期化の削除", "\n初期設定に戻す項目を削除します。", ModTag.RoomOption, true);
                main.AdvancedNum = new ModOption("数値の詳細設定", "\n数値をCtrlキーを押しながら変更すると、細かい単位で変更出来ます。", ModTag.RoomOption, true);
                main.ShowMapSelect = new ModOption("マップ選択", "\nオンライン状態でのマップ選択を有効にします。", ModTag.RoomOption, true);

            main.RandomMaps = new ModOption("ランダムマップ", "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\n毎試合マップをランダムに変更します。", ModTag.RandomMap);
                main.AddTheSkeld = new ModOption("TheSkeld", "\nランダムマップにTheSkeldを追加します。", ModTag.RandomMap, true);
                main.AddMIRAHQ = new ModOption("MIRAHQ", "\nランダムマップにMIRAHQを追加します。", ModTag.RandomMap, true);
                main.AddPolus = new ModOption("Polus", "\nランダムマップにPolusを追加します。", ModTag.RandomMap, true);
                main.AddAirShip = new ModOption("AirShip", "\nランダムマップにAirShipを追加します。", ModTag.RandomMap, true);

            ModOption.CreateFile("スキンカラーの変更", "\nスキンの色を変更します。", ModTag.SkinColor);
                main.RainbowHat = new ModOption("虹色の帽子", "\n帽子を虹色に変更します。", ModTag.SkinColor, true);
                main.RainbowVisor = new ModOption("虹色のバイザー", "\nバイザーを虹色に変更します。", ModTag.SkinColor, true);
                main.RainbowSkin = new ModOption("虹色のスキン", "\nスキンを虹色に変更します。", ModTag.SkinColor, true);
                main.RainbowPet = new ModOption("虹色のペット", "\nペットを虹色に変更します。", ModTag.SkinColor, true);

            ModOption.CreateFile("ネームカラーの変更", "\n名前の色を変更します。", ModTag.NameColor);
                main.RainbowName = new ModOption("虹色の名前", "\n名前を虹色に変更します。", ModTag.NameColor, true);
                main.TranslucentName = new ModOption("半透明の名前", "\n名前の透明度を変更します。", ModTag.NameColor, true);
                main.SetTranslucentName = new ModOption("名前の透明度変更", 75, "\n名前の透明度を変更します。", TextType.Percent, ModTag.NameColor);
                main.NameOutline = new ModOption("名前のアウトライン", "\n名前の虹色、半透明になる部分を名前の縁に変更します。\n虹色の名前か半透明の名前が有効でないと機能しません。", ModTag.NameColor, true);

            ModOption.CreateFile("アウトライン", "\n様々なアウトラインの色を変更します。", ModTag.Outline);
                main.RainbowOutline = new ModOption("虹色のアウトライン", "\nキルターゲットの縁取りの色を虹色にします。", ModTag.Outline, true);
                main.CrewColorOutline = new ModOption("クルー色のアウトライン", "\nキルターゲットの縁取りの色を相手のクルーの色に変更します。", ModTag.Outline, true);
                main.RainbowVent = new ModOption("虹色のベント", "\nベントの縁取りを虹色に変更します。", ModTag.Outline, true);
                main.CrewColorVent = new ModOption("クルー色のベント", "\nベントの縁取りを自身の色に変更します。", ModTag.Outline, true);

            main.FakeLevel = new ModOption("偽のレベル", "\n自身のレベルをランダムにします。", ModTag.FakeLevel);

            main.HideNameplates = new ModOption("ネームプレート非表示", "\n全員のネームプレートを真っ白にします。", ModTag.HideNameplates);

            main.Zoom = new ModOption("拡大縮小機能", "\n死亡時にマップ全体を見渡せるようにします。\nフリープレイ時は拡大ができます。", ModTag.Zoom);


            main.OPkick = new ModOption("特定の機種を追い出す", "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\n特定の機種を追い出すことができます。", ModTag.OPkick);
                main.AddUnknown = new ModOption("不明な機種", "\n追い出す機種に不明な機種を追加します", ModTag.OPkick, true);
                main.AddEpicPC = new ModOption("EpicPC", "\n追い出す機種にEpicPCを追加します", ModTag.OPkick, true);
                main.AddSteamPC = new ModOption("SteamPC", "\n追い出す機種にSteamPCを追加します", ModTag.OPkick, true);
                main.AddMac = new ModOption("Mac", "\n追い出す機種にMacを追加します", ModTag.OPkick, true);
                main.AddWin10 = new ModOption("Win10", "\n追い出す機種にWin10を追加します", ModTag.OPkick, true);
                main.AddItch = new ModOption("Itch", "\n追い出す機種にItchを追加します", ModTag.OPkick, true);
                main.AddIPhone = new ModOption("IPhone", "\n追い出す機種にIPhoneを追加します", ModTag.OPkick, true);
                main.AddAndroid = new ModOption("Android", "\n追い出す機種にAndroidを追加します", ModTag.OPkick, true);
                main.AddSwitch = new ModOption("Switch", "\n追い出す機種にSwitchを追加します", ModTag.OPkick, true);
                main.AddXbox = new ModOption("Xbox", "\n追い出す機種にXboxを追加します", ModTag.OPkick, true);
                main.AddPlaystation = new ModOption("Playstation", "\n追い出す機種にPlaystationを追加します", ModTag.OPkick, true);

            main.SendJoinPlayer = new ModOption("参加者にチャットを送る", "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\n入ってきたプレイヤーに任意のチャットを送ることができます。", ModTag.SendJoinPlayer);
                main.SetSendJoinChat = new ModOption("メッセージの変更", "TownOfPlusを使用しています", "\n送るチャット内容を変更します。", TextType.None, ModTag.SendJoinPlayer);

            main.DoubleName = new ModOption("二段の名前", "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\n名前を二段に変更できます。", ModTag.DoubleName);
                main.SetDoubleName = new ModOption("二段目の名前の変更", "二段目", "\n二段目の名前を変更します", TextType.None, ModTag.DoubleName);

            main.ChangeGameName = new ModOption("ゲーム中の名前", "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\nゲーム中の名前を任意の名前に変更できます。", ModTag.ChangeGameName);
                main.SetGameName = new ModOption("ゲーム中の名前の変更", "部屋主", "\nゲーム中の名前を変更します", TextType.nameText, ModTag.ChangeGameName);

            main.AutoCopyCode = new ModOption("自動コードコピー", "\n部屋コードを自動でコピーします。", ModTag.AutoCopyCode);

            main.ChatCommand = new ModOption("チャットコマンド", "\nチャットコマンド[" + TextPlus.ComWord + "コマンド名]を有効、無効にできます。", ModTag.ChatCommand);
                main.ComTab = new ModOption("コマンド補完", "\nTabのコマンド補完を有効にできます。", ModTag.ChatCommand, true);
                main.ComCancel = new ModOption("コマンドキャンセル", "\nコマンドが存在しない場合でも全体送信をキャンセルします。", ModTag.ChatCommand, true);
                main.ChangeComWord = new ModOption("コマンド文字変更", "\nコマンドの文字を「.」から「/」に変更します。", ModTag.ChatCommand, true);

            main.KeyCommand = new ModOption("キーコマンド", "\nキーコマンドを有効、無効にできます。", ModTag.KeyCommand);
                main.KeyDelete = new ModOption("全消し", "\nCtrl+BackSpace(全消し)を有効にします。", ModTag.KeyCommand, true);
                main.KeyUndoAndRedo = new ModOption("戻す、取り消し", "\nCtrl+Z(戻す) Ctrl+Y(取り消し)を有効にします。", ModTag.KeyCommand, true);
                main.KeyCopy = new ModOption("コピー", "\nCtrl+C(コピー)を有効にします。", ModTag.KeyCommand, true);
                main.KeyCut = new ModOption("カット", "\nCtrl+X(カット)を有効にします。", ModTag.KeyCommand, true);
                main.KeyPaste = new ModOption("ペースト", "\nCtrl+V(ペースト) Ctrl+Shift+V(直接ペースト)を有効にします。", ModTag.KeyCommand, true);

            main.CPS = new ModOption("CPS", "\n一秒間のクリック数を表示します。" + CommandList.ChatComHelp(CommandTag.CPS), ModTag.CPS);

            main.DateTimeSetting = new ModOption("DateTime", "\n今日の日付、時間を表示します。" + CommandList.ChatComHelp(CommandTag.DateTime), ModTag.DateTimeSetting);


            main.CustomOverlay = new ModOption("部屋設定&プレイヤー一覧", "\n部屋設定とプレイヤー一覧を表示します。", ModTag.CustomOverlay);
                main.ShowRolesSetting = new ModOption("役職の詳細設定", "\n設定の役職設定を表示します。", ModTag.CustomOverlay, true);
                main.ShowFriendText = new ModOption("フレンドコード", "\nプレイヤー一覧のフレンドコードを表示します。", ModTag.CustomOverlay, true);
                main.ShowLevelText = new ModOption("レベル", "\nプレイヤー一覧のレベルを表示します。", ModTag.CustomOverlay, true);
                main.ShowHostColor = new ModOption("部屋主の表示", "\nプレイヤー一覧の部屋主の色を変更します。", ModTag.CustomOverlay, true);
                main.ShowColorName = new ModOption("色の名前表示", "\nプレイヤー一覧の色の名前を表示します。", ModTag.CustomOverlay, true);
                main.FirstColorName = new ModOption("色の一文字目表示", "\nプレイヤー一覧の色の名前を一文字目だけ表示します。", ModTag.CustomOverlay, true);
                main.ShowBlockedPlayer = new ModOption("ブロックプレイヤー", "\nブロックしているプレイヤーを赤く表示します。", ModTag.CustomOverlay, true);
                main.CustomOverlayKeyBind = new ModOption("キー設定", KeyCode.F3, "\n表示するキーを設定します。", ModTag.CustomOverlay);

            main.OldPingPositon = new ModOption("旧Ping位置", "\nPingの表示位置を旧バージョンの表示位置に変更します。", ModTag.OldPingPositon);

            main.CrewColorChat = new ModOption("クルー色のチャット", "\nチャットの色をクルー色にします。", ModTag.CrewColorChat);

            main.TranslucentChat = new ModOption("半透明のチャット", "\nチャットを半透明にします。", ModTag.TranslucentChat);
                main.SetTranslucentChat = new ModOption("透明度の変更", 75, "\n透明度を変更します。\n範囲 1% ～ 100%", TextType.Percent, ModTag.TranslucentChat);

            main.CrewColorVoteArea = new ModOption("クルー色のネームプレート", "\nネームプレートの色をクルー色にします。", ModTag.CrewColorVoteArea);
            
            main.FPS = new ModOption("FPS", "\n一秒間のフレームレートを表示します。" + CommandList.ChatComHelp(CommandTag.FPS), ModTag.FPS);


            main.FakePing = new ModOption("FakePing", "\nPingの数値を変更します。", ModTag.FakePing);
                main.SetFakePing = new ModOption("Pingの値変更", 100, "\nPingの数値を変更します。", TextType.None, ModTag.FakePing);

            main.SkipLogo = new ModOption("スキップロゴ", "\n開始時のロゴをスキップします。", ModTag.SkipLogo);

            main.ChangeNameBox = new ModOption("ネームボックス", "\n部屋作成時と部屋検索時に名前変更を出来るようにします。", ModTag.ChangeNameBox);

            ModOption.CreateFile("チャットボタンPlus", "\nチャットボタンの設定を変更します。", ModTag.ChatButtonPlus);
                main.AlwaysChat = new ModOption("フリープレイ時のチャット表示", "\nフリープレイ時にチャットボタンを常時表示します。", ModTag.ChatButtonPlus, true);
                main.ChatLimitPlus = new ModOption("チャットの最大文字数変更", "\nチャットの最大文字数を120文字に変更します。", ModTag.ChatButtonPlus, true);
                main.CancelChatMap = new ModOption("マップ非表示", "\nチャット入力時にマップを非表示にします。", ModTag.ChatButtonPlus, true);

            main.ShowHost = new ModOption("部屋主の名前表示", "\nロビーに部屋主の名前を表示します。", ModTag.ShowHost);

            ModOption.CreateFile("コード入力", "\nコード入力を便利にします。", ModTag.CodeText);
                main.PasteCodeText = new ModOption("自動ペースト", "\n自動でコードをペーストします。", ModTag.CodeText, true);
                main.CodeTextPlus = new ModOption("コード補完", "\nコードの最後の文字を補完します。", ModTag.CodeText, true);

            ModOption.CreateFile("開始ボタン", "\n開始ボタンの設定。", ModTag.StartButton);
                main.AlwaysStart = new ModOption("常時有効", "\n開始ボタンを常時有効化します。", ModTag.StartButton, true);
                main.StartCount = new ModOption("即開始", KeyCode.LeftShift, "\nカウントダウン中にすぐ開始します。", ModTag.StartButton);
                main.ResetCount = new ModOption("開始のキャンセル", KeyCode.C, "\nカウントダウン中に開始をキャンセルします。", ModTag.StartButton);
                main.StartCountText = new ModOption("チャット時のカウントダウン表示", "\nチャットを開いててもカウントダウンを見えるようにします。", ModTag.StartButton, true);


            main.WallWalk = new ModOption("壁ぬけ", "\nロビーかフリープレイで壁を歩けるようにします。", ModTag.WallWalk);
                main.WallWalkKeyBind = new ModOption("壁ぬけのキー設定", KeyCode.LeftControl, "\n壁抜けのキーを設定します", ModTag.WallWalk);


            main.EndGame = new ModOption("廃村", "\n廃村コマンドを追加します。", ModTag.EndGame);
                main.EndGameKeyBindFirst = new ModOption("廃村コマンド1", KeyCode.LeftShift, "\n廃村コマンドの1つ目を変更します", ModTag.EndGame);
                main.EndGameKeyBindSecond = new ModOption("廃村コマンド2", KeyCode.H, "\n廃村コマンドの2つ目を変更します", ModTag.EndGame);
                main.EndGameKeyBindThird = new ModOption("廃村コマンド3", KeyCode.Return, "\n廃村コマンドの3つ目を変更します", ModTag.EndGame);
            
            main.CrewColorText = new ModOption("カラーテキスト", "\n色のテキストに色をつけます。", ModTag.CrewColorText);

            ModOption.CreateFile("バグの修正", "\nバグを修正します。", ModTag.FixBug);
                main.FixSkinBug = new ModOption("スキンの修正", "\nスキンの位置がズレてるバグを修正します。", ModTag.FixBug, true);
                main.FixLeftPlayer = new ModOption("会議バグの修正", "\n抜けたプレイヤーが黒くならないバグを修正します。", ModTag.FixBug, true);
                main.FixPlayerColor = new ModOption("プレイヤーカラーの修正", "\nスキンメニューでプレイヤーの色が違うバグを修正します。", ModTag.FixBug, true);

            main.AutoBanBlockedPlayer = new ModOption("ブロックBAN", "\nブロックしているプレイヤーを自動でBANします。", ModTag.AutoBanBlockedPlayer);
        }

        public static SmallButton[] SNSButtons =>
            new SmallButton[]
            {
                new SmallButton("GitHub",
                    "TownOfPlusのGitHubを開きます",
                    ()=>
                    {
                        Process.Start("https://github.com/tugaru1975/TownOfPlus");
                    }),

                new SmallButton("Bug",
                    "バグ報告のテンプレートをコピーし、報告ページを開きます\nCtrlキーを押しながらクリックでページを開くのをキャンセルします",
                    ()=>
                    {
                        Log.DumpLog(true);
                        if (!Input.GetKey(KeyCode.LeftControl)) Process.Start("https://marshmallow-qa.com/tugaruyukkuri");
                    }),

                new SmallButton("Twitter",
                    "公式ツイッターアカウントを開きます",
                    ()=>
                    {
                        Process.Start("https://twitter.com/tugaru_topmod");
                    }),
            };

        private static GameObject popUp;
        private static TextMeshPro titleText;
        private static GameObject textbox;

        private static TextMeshPro Note;
        private static TextMeshPro SettingMenuTitle;
        public static List<TOPOptions> modButtonsList;
        private static Dictionary<ToggleButtonBehaviour, bool> OtherButtonsList;

        private static ToggleButtonBehaviour buttonPrefab;
        private static int page = 1;


        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static void Postfix(MainMenuManager __instance)
        {
            // Prefab for the title
            var tmp = __instance.Announcement.transform.Find("Title_Text").gameObject.GetComponent<TextMeshPro>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.transform.localPosition += Vector3.left * 0.2f;
            titleText = Object.Instantiate(tmp);
            Object.Destroy(titleText.GetComponent<TextTranslatorTMP>());
            titleText.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(titleText);

            var ob = Object.Instantiate(DestroyableSingleton<AccountManager>.Instance.accountTab.editNameScreen.nameText.nameSource.gameObject);
            Object.Destroy(ob.GetComponent<NameTextBehaviour>());
            Object.Destroy(ob.transform.FindChild("Background").gameObject);
            ob.transform.SetPos(0, 0);
            var col = ob.GetComponent<BoxCollider2D>();
            ob.layer = 5;
            ob.SetActive(false);
            var tb = ob.GetComponent<TextBoxTMP>();
            tb.ForceUppercase = tb.Hidden = tb.IpMode = tb.ClearOnFocus = false;
            tb.AllowPaste = true;
            tb.characterLimit = 120;
            tb.outputText.enableWordWrapping = tb.outputText.enableAutoSizing = true;
            tb.outputText.rectTransform.position = ob.transform.position;
            tb.outputText.rectTransform.sizeDelta = col.size = new(4.5f, .7f);
            Object.DontDestroyOnLoad(ob);
            textbox = ob;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
        public static void Prefix(OptionsMenuBehaviour __instance)
        {
            if (!__instance.CensorChatButton) return;

            if (!popUp)
            {
                CreateCustomPOP(__instance);
            }

            if (!buttonPrefab)
            {
                buttonPrefab = Object.Instantiate(__instance.CensorChatButton);
                Object.DontDestroyOnLoad(buttonPrefab);
                buttonPrefab.name = "CensorChatPrefab";
                buttonPrefab.gameObject.SetActive(false);
            }

            SetUpPOPOptions();
            InitializeMoreButton(__instance);
        }

        private static void CreateCustomPOP(OptionsMenuBehaviour prefab)
        {
            popUp = Object.Instantiate(prefab.gameObject);
            Object.DontDestroyOnLoad(popUp);
            popUp.transform.SetPos(z: -810f);
            popUp.name = "TownOfPlusMenu";

            Object.Destroy(popUp.GetComponent<OptionsMenuBehaviour>());
            foreach (var gObj in popUp.gameObject.GetAllChilds())
            {
                switch (gObj.name)
                {
                    case "Background": gObj.transform.SetSc(x: 1.55f);
                        break;

                    case "CloseButton": gObj.transform.SetPos(x: -4.5f);
                        break;

                    default:  Object.Destroy(gObj);
                        break;
                }
            }

            popUp.SetActive(false);
        }
        private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
        {
            var transform = __instance.MouseAndKeyboardOptions.transform;
            var moreOptions = Object.Instantiate(buttonPrefab, transform.parent);
            moreOptions.transform.localPosition = transform.localPosition;
            moreOptions.transform.localScale = transform.localScale * 0.7f;
            moreOptions.gameObject.SetActive(true);
            moreOptions.Text.text = moreOptions.name = "TownOfPlusOptions";
            var moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
            moreOptionsButton.OnClick = new();
            moreOptionsButton.OnClick.AddListener((Action)(() =>
            {
                if (!popUp) return;

                if (__instance.transform.parent && __instance.transform.parent == HudManager.Instance.transform)
                {
                    popUp.transform.SetParent(HudManager.Instance.transform);
                    popUp.transform.localPosition = new(0, 0, -800f);
                }
                else
                {
                    popUp.transform.SetParent(null);
                    Object.DontDestroyOnLoad(popUp);
                }
                RefreshPOPOpen();
                ModButtonsActive();
            }));
        }

        private static void RefreshPOPOpen()
        {
            popUp.gameObject.SetActive(false);
            popUp.gameObject.SetActive(true);
            SetUpPOPOptions();
        }

        private static void SetNote()
        {
            Note = Object.Instantiate(titleText, popUp.transform);
            Note.GetComponent<RectTransform>().SetPos(2.25f, 1.75f , -5f);
            Note.GetComponent<RectTransform>().SetSc(1.25f, 1.25f);
            Note.gameObject.SetActive(true);
            Note.alignment = TextAlignmentOptions.Top;
            Note.fontSizeMin = Note.fontSize = 2f;
            Note.text = "";
            Note.name = "noteText";
        }

        private static void SetSettingMenuTitle()
        {
            SettingMenuTitle = Object.Instantiate(titleText, popUp.transform);
            SettingMenuTitle.GetComponent<RectTransform>().SetPos(-1.25f, 2.3f, -5f);
            SettingMenuTitle.GetComponent<RectTransform>().SetSc(1.25f, 1.25f);
            SettingMenuTitle.gameObject.SetActive(true);
            SettingMenuTitle.alignment = TextAlignmentOptions.TopLeft;
            SettingMenuTitle.fontSizeMax = SettingMenuTitle.fontSizeMin = SettingMenuTitle.fontSize = 2.5f;
            SettingMenuTitle.enableWordWrapping = false;
            SettingMenuTitle.text = "";
            SettingMenuTitle.name = "SettingMenuTitle";
        }

        private static void ResetNoteText(string note = "", bool haschild = false)
        {
            Note.text = (haschild ? "Ctrlキーで詳細設定を開く" : "") + "\n" + note;
        }

        private static void ResetSettingMenuTitle(string title)
        {
            SettingMenuTitle.text = title != null ? title + "の詳細設定".SetSize(1.5f) : "";
        }

        private static void SetUpPOPOptions()
        {
            if (popUp.transform.GetComponentInChildren<ToggleButtonBehaviour>()) return;

            OtherButton();
            SetUpSNSButtons();
            SetNote();
            SetSettingMenuTitle();

            modButtonsList = new();

            int count = -1;
            int childcount = -1;
            var modtype = ModTag.None;
            var Options = ModOption.AllOptions;
            foreach (var info in Options)
            {
                var button = Object.Instantiate(buttonPrefab, popUp.transform);

                GameObject nameText = null;

                var HasChild = Options.Count(c => c.Tag == info.Tag) != 1 && !info.IsChild;

                var size = new Vector2(2.2f, .7f);

                Vector3 pos;
                if (info.IsChild)
                {
                    if (modtype != info.Tag)
                    {
                        childcount = 0;
                        modtype = info.Tag;
                    }
                    else childcount++;

                    if (childcount % 2 != 0 && info.ModType == ModType.TextBox) childcount++;

                    pos = new(childcount % 2 == 0 ? -2.92f : -0.58f, 1.75f - childcount / 2 * 0.8f, -.5f);

                    if (info.ModType == ModType.TextBox)
                    {
                        nameText = Object.Instantiate(textbox, button.transform);
                        nameText.name = "TextBox";
                        pos = new(-1.75f, pos.y, -.5f);
                        size = nameText.GetComponent<BoxCollider2D>().size;
                        var box = nameText.GetComponent<TextBoxTMP>();
                        var passivenameText = nameText.GetComponent<PassiveButton>();
                        Action onchange = null;
                        string ResetText = "";
                        box.SetText(info.Config.Value);

                        ResetText = info.Config.DefaultValue.ToString();
                        if (info.IsInt)
                        {
                            box.IpMode = true;
                            if (info.TextType == TextType.Percent)
                            {
                                box.characterLimit = 3;
                                onchange = () =>
                                {
                                    box.SetText(box.text.TrimStart('0'));
                                    if (int.TryParse(box.text, out var result))
                                    {
                                        if (result <= 100) info.Config.Value = result.ToString();
                                        else box.SetText("100");
                                    }
                                };
                            }
                            box.characterLimit = 10;
                            onchange ??= () =>
                            {
                                if (int.TryParse(box.text, out var result)) info.Config.Value = result.ToString();
                                else if (box.text != "") box.SetText(int.MaxValue.ToString());
                            };
                        }
                        else
                        {
                            switch (info.TextType)
                            {
                                case TextType.ColorText:
                                    box.characterLimit = 8;
                                    if (ColorUtility.TryParseHtmlString(info.Config.Value, out var color))
                                    {
                                        button.Background.color = color;
                                    }
                                    onchange = () =>
                                    {
                                        var code = "#" + box.text;
                                        if (ColorUtility.TryParseHtmlString(code, out var color))
                                        {
                                            info.Config.Value = code;
                                            button.Background.color = color;
                                        }
                                    };
                                    break;

                                case TextType.nameText:
                                    box.characterLimit = 10;
                                    break;

                                default:
                                    box.allowAllCharacters = box.AllowEmail = box.AllowSymbols = true;
                                    break;
                            }
                            onchange ??= () =>
                            {
                                info.Config.Value = box.text;
                            };
                        }

                        box.OnChange = new();

                        box.OnChange.AddListener((Action)(() =>
                        {
                            onchange();
                        }));

                        passivenameText.OnClick = new();
                        passivenameText.OnMouseOver = new();
                        passivenameText.OnMouseOut = new();

                        passivenameText.OnClick.AddListener((Action)(() =>
                        {
                            if (Input.GetKey(KeyCode.LeftControl)) box.SetText(ResetText);
                        }));

                        passivenameText.OnMouseOver.AddListener((Action)(() =>
                        {
                            ResetNoteText($"{info.Title.SetSize(3)}\n{info.Note}");
                            if (info.TextType != TextType.ColorText) button.Background.color = new Color32(34, 139, 34, byte.MaxValue);
                        }));

                        passivenameText.OnMouseOut.AddListener((Action)(() =>
                        {
                            ResetNoteText();
                            if (info.TextType != TextType.ColorText) button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                        }));

                        nameText.SetActive(true);
                        childcount++;
                    }
                }
                else
                {
                    count++;
                    int Y = count - ((int)Math.Floor((decimal)count / 12)) * 12;
                    pos = new(count % 2 == 0 ? -2.92f : -0.58f, 1.75f - Y / 2 * 0.8f, -.5f);
                }

                button.transform.localPosition = pos;

                button.name = info.Tag.ToString() + (info.IsChild ? "Child" : "") + info.Title.Trim();

                button.onState = info.Getbool();

                button.Background.color = info.ModType switch
                {
                    ModType.Toggle => button.onState ? Color.green : Palette.ImpostorRed,
                    ModType.TextBox => info.TextType switch
                    {
                        TextType.ColorText => button.Background.color,
                        _ => Palette.ImpostorRed,
                    },
                    _ => Palette.ImpostorRed,
                };

                if (info.ModType == ModType.TextBox) Object.Destroy(button.transform.FindChild("Text_TMP").gameObject);
                else
                {
                    button.Text.text = info.ModType == ModType.KeyBind ? (info.Title + "\n" + info.Config.Value).SetSize(1.75f) : info.Title;
                    button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
                    button.Text.font = Object.Instantiate(titleText.font);
                    button.Text.GetComponent<RectTransform>().sizeDelta = new(2, 2);
                }
                var colliderButton = button.GetComponent<BoxCollider2D>();

                button.gameObject.SetActive(false);
                
                if (info.ModType != ModType.TextBox)
                {
                    var passiveButton = button.GetComponent<PassiveButton>();

                    colliderButton.size = size; 
                    passiveButton.OnClick = new();
                    passiveButton.OnMouseOut = new();
                    passiveButton.OnMouseOver = new();

                    passiveButton.OnClick.AddListener((Action)(() =>
                    {
                        switch (info.ModType)
                        {
                            case ModType.Toggle:
                                if (Input.GetKey(KeyCode.LeftControl) && HasChild) ModButtonsActive(info.Tag, info.Title);
                                else
                                {
                                    info.Config.Value = (button.onState = !info.Getbool()).ToString();
                                    button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                                }
                                break;

                            case ModType.File:
                                ModButtonsActive(info.Tag, info.Title);
                                break;

                            case ModType.KeyBind:
                                button.Text.text = (info.Title + "\n" + (info.Config.Value = Helpers.GetKey().ToString())).SetSize(1.75f);
                                break;
                        }
                    }));

                    passiveButton.OnMouseOver.AddListener((Action)(() =>
                    {
                        ResetNoteText($"{info.Title.SetSize(3)}\n{info.Note}", HasChild && info.ModType != ModType.File);
                        if (info.ModType != ModType.File) button.Background.color = new Color32(34, 139, 34, byte.MaxValue);
                    }));

                    passiveButton.OnMouseOut.AddListener((Action)(() =>
                    {
                        ResetNoteText();
                        if (info.ModType != ModType.File) button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                    }));

                }
                else Object.Destroy(colliderButton);

                foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    spr.size = size;

                modButtonsList.Add(new() { Info = info, Button = button});
                
            }
        }

        private static void OtherButton()
        {
            OtherButtonsList = new();
            for (var i = 0; i < 3; i++)
            {
                var button = Object.Instantiate(buttonPrefab, popUp.transform);

                var ButtonType = i;

                button.transform.localPosition = ButtonType switch
                {
                    0 => new(-0.58f, 2.4f),
                    1 => new(-2.92f, 2.4f),
                    2 => new(-3.48f, 2.4f),
                    _ => new(),
                };

                button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

                button.Text.text = button.name = ButtonType switch
                {
                    0 => "右ページ",
                    1 => "左ページ",
                    2 => "戻る",
                    _ => "",
                };

                button.Text.fontSizeMin = button.Text.fontSizeMax = 2.5f;
                button.Text.font = Object.Instantiate(titleText.font);
                button.Text.GetComponent<RectTransform>().sizeDelta = new(2, 2);

                button.gameObject.SetActive(true);

                var passiveButton = button.GetComponent<PassiveButton>();
                var colliderButton = button.GetComponent<BoxCollider2D>();

                var size = ButtonType switch
                {
                    0 or 1 => new Vector2(2.2f, .4f),
                    2 => new Vector2(1.1f, .4f),
                    _ => new Vector2(),
                }; ;

                colliderButton.size = size;

                passiveButton.OnClick = new();
                passiveButton.OnMouseOut = new();
                passiveButton.OnMouseOver = new();
                passiveButton.OnClick.AddListener((Action)(() =>
                {
                    switch (ButtonType)
                    {
                        case 0:
                            if (page < AllOptionsPage()) page++;
                            else page = 1;
                            break;

                        case 1:
                            if (page > 1) page--;
                            else page = AllOptionsPage();
                            break;
                    }
                    ModButtonsActive();
                }));
                passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
                passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));
                foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    spr.size = size;

                OtherButtonsList.Add(button, ButtonType is 0 or 1);

            }
        }

        private static int AllOptionsPage()
        {
            return (int)Math.Ceiling((decimal)modButtonsList.Count(c => !c.Info.IsChild) / 12);
        }


        private static void SetUpSNSButtons()
        {
            for (var i = 0; i < SNSButtons.Length; i++)
            {
                var info = SNSButtons[i];
                var button = Object.Instantiate(buttonPrefab, popUp.transform);
                button.transform.localPosition = new(3.75f - i * 0.5f, 2.4f);

                button.Background.color = Palette.LightBlue;

                button.Text.text = button.name = info.Title;
                button.Text.fontSizeMin = button.Text.fontSizeMax = 2.5f;
                button.Text.font = Object.Instantiate(titleText.font);
                button.Text.GetComponent<RectTransform>().sizeDelta = new(2, 2);

                button.gameObject.SetActive(true);

                var passiveButton = button.GetComponent<PassiveButton>();
                var colliderButton = button.GetComponent<BoxCollider2D>();

                var size = new Vector2(.5f, .5f);
                colliderButton.size = size;

                passiveButton.OnClick = new();
                passiveButton.OnMouseOut = new();
                passiveButton.OnMouseOver = new();
                passiveButton.OnClick.AddListener((Action)(() =>
                {
                    info.Click();
                }));

                passiveButton.OnMouseOver.AddListener((Action)(() =>
                {
                    ResetNoteText($"{info.Title.SetSize(3)}\n{info.Note}");
                    button.Background.color = Palette.CrewmateBlue;
                }));

                passiveButton.OnMouseOut.AddListener((Action)(() =>
                {
                    ResetNoteText();
                    button.Background.color = Palette.LightBlue;
                }));
                foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    spr.size = size;
            }
        }

        public static void UpdateColor()
        {
            foreach (var mod in modButtonsList)
            {
                try
                {
                    var button = mod.Button;
                    var info = mod.Info;
                    if (info.ModType != ModType.Toggle) continue;
                    if (button.onState = info.Getbool()) button.Background.color = Color.green;
                    else button.Background.color = Palette.ImpostorRed;
                }
                catch { }
            }
        }

        private static void ModButtonsActive(ModTag type = ModTag.None, string title = null)
        {
            var i = -1;
            foreach (var mod in modButtonsList)
            {
                var button = mod.Button;
                var info = mod.Info;
                if (!info.IsChild) i++;
                var IsDef = (page - 1) * 12 <= i && i < page * 12 && !info.IsChild;
                var IsChild = info.Tag == type && info.IsChild;
                if (type == ModTag.None ? IsDef : IsChild)
                {
                    if (info.ModType == ModType.File) button.Background.color = info.GetFileColor();
                    button.gameObject.SetActive(true);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
            ResetSettingMenuTitle(title);
            OtherButtonsActive(type == ModTag.None);
        }

        private static void OtherButtonsActive(bool IsDef)
        {
            foreach (var other in OtherButtonsList)
            {
                other.Key.gameObject.SetActive(other.Value.Equals(IsDef));
            }
        }

        private static IEnumerable<GameObject> GetAllChilds(this GameObject Go)
        {
            for (var i = 0; i < Go.transform.childCount; i++)
            {
                yield return Go.transform.GetChild(i).gameObject;
            }
        }
    }

    public class ModOption
    {
        public static List<ModOption> AllOptions = new();
        public string Title = "";
        public ConfigEntry<string> Config;
        public string Note = "";
        public ModTag Tag = ModTag.None;
        public ModType ModType = ModType.None;
        public TextType TextType = TextType.None;
        public bool IsInt = false;
        public bool IsChild = false;

        public ModOption(string title, string note, ModTag tag, ModType type)
        {
            Title = title;
            Note = note;
            Tag = tag;
            ModType = type;
            AllOptions.Add(this);
        }

        public ModOption(string title, KeyCode defaultvalue, string note, ModTag tag)
        {
            Title = title;
            Note = note + "\nキー押しながらクリックでキー設定を変更\nデフォルトのキー設定\n".SetSize(1.5f) + defaultvalue.ToString().SetSize(3f);
            Tag = tag;
            IsChild = true;
            ModType = ModType.KeyBind;
            Config = main.Instance.Config.Bind($"{tag} Option", title, defaultvalue.ToString());
            AllOptions.Add(this);
        }

        public ModOption(string title, object defaultvalue, string note, TextType texttype, ModTag tag)
        {
            Title = title;
            Note = note + "\nCtrlキーを押しながらクリックで値をリセット".SetSize(1.5f);
            TextType = texttype;
            Tag = tag;
            IsInt = defaultvalue is int;
            IsChild = true;
            ModType = ModType.TextBox;
            Config = main.Instance.Config.Bind($"{tag} Option", title, defaultvalue.ToString());
            AllOptions.Add(this);
        }

        public ModOption(string title, string note, ModTag tag, bool ischild = false)
        {
            Title = title;
            Note = note;
            Tag = tag;
            IsChild = ischild;
            ModType = ModType.Toggle;
            var defaultvalue = ischild && !IsFileChild;
            Config = main.Instance.Config.Bind($"{tag} Option", title, defaultvalue.ToString());
            AllOptions.Add(this);
        }

        public static void CreateFile(string title, string note, ModTag tag) => new ModOption(title, note, tag, ModType.File);
        public Color GetFileColor() => AllOptions.Any(w => w.Tag == Tag && w.IsChild && w.Getbool()) ? new Color32(255, 255, 0, 255) : Palette.Orange;
        public bool IsFileChild => AllOptions.Any(a => a.Tag == Tag && a.ModType == ModType.File);
        public bool Getbool() => bool.TryParse(Config?.Value, out var result) && result;
        public string Getstring() => Config.Value;
        public int Getint() => int.TryParse(Config.Value, out int result) ? result : 0;
        public KeyCode Getkeycode() => Enum.TryParse<KeyCode>(Config.Value, out var result) ? result : KeyCode.None;
    }

    public class TOPOptions
    {
        public ModOption Info { get; set; }
        public ToggleButtonBehaviour Button { get; set; }
    }

    public class SmallButton
    {
        public string Title;
        public string Note;
        public Action Click;

        public SmallButton(string title, string note, Action click)
        {
            Title = title;
            Note = note;
            Click = click;
        }
    }

}
