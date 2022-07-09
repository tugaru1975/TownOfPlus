using HarmonyLib;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace TownOfPlus
{
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
        FixBag,
        WallWalk,
        EndGame,
        CrewColorText,
    }

    [HarmonyPatch]
    public static class ModOptionSetting
    {
        public static SelectionBehaviour[] AllOptions =>
            new SelectionBehaviour[]
            {
                new SelectionBehaviour("部屋コード",
                    main.ChangeLobbyCodes,
                    "\n部屋のコードの文字を変更できます。" + CommandList.ChatComHelp(CommandTag.ChangeLobbyCode, CommandTag.ChangeCodeColor),
                    ModTag.ChangeLobbyCodes),

                new SelectionBehaviour("ロビータイマー",
                    main.LobbyTimer,
                    "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\nロビーが落ちるまでの目安となるタイマーを表示します。",
                    ModTag.LobbyTimer),

                new SelectionBehaviour("部屋設定の拡張",
                    main.RoomOption,
                    "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\nオンライン状態でのマップ切り替え、設定のキルクールやタスク量の上限を変更します。",
                    ModTag.RoomOption),

                        new SelectionBehaviour("数値の詳細設定",
                            main.AdvancedNum,
                            "数値をCtrlキーを押しながら変更すると、細かい単位で変更出来ます。",
                            ModTag.RoomOption, true),

                        new SelectionBehaviour("マップ選択",
                            main.ShowMapSelect,
                            "オンライン状態でのマップ選択を有効にします。",
                            ModTag.RoomOption, true),

                new SelectionBehaviour("ランダムマップ",
                    main.RandomMaps,
                    "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\n毎試合マップをランダムに変更します。",
                    ModTag.RandomMap),

                        new SelectionBehaviour("TheSkeld",
                            main.AddTheSkeld,
                            "ランダムマップにTheSkeldを追加します。",
                            ModTag.RandomMap, true),

                        new SelectionBehaviour("MIRAHQ",
                            main.AddMIRAHQ,
                            "ランダムマップにMIRAHQを追加します。",
                            ModTag.RandomMap, true),

                        new SelectionBehaviour("Polus",
                            main.AddPolus,
                            "ランダムマップにPolusを追加します。",
                            ModTag.RandomMap, true),

                        new SelectionBehaviour("AirShip",
                            main.AddAirShip,
                            "ランダムマップにAirShipを追加します。",
                            ModTag.RandomMap, true),

                new SelectionBehaviour("スキンカラーの変更",
                    "\nスキンの色を変更します。",
                    ModTag.SkinColor),

                    new SelectionBehaviour("虹色の帽子",
                        main.RainbowHat,
                        "\n帽子を虹色に変更します。",
                        ModTag.SkinColor,
                        true),

                    new SelectionBehaviour("虹色のバイザー",
                        main.RainbowVisor,
                        "\nバイザーを虹色に変更します。",
                        ModTag.SkinColor,
                        true),

                    new SelectionBehaviour("虹色のスキン",
                        main.RainbowSkin,
                        "\nスキンを虹色に変更します。",
                        ModTag.SkinColor,
                        true),

                    new SelectionBehaviour("虹色のペット",
                        main.RainbowPet,
                        "\nペットを虹色に変更します。",
                        ModTag.SkinColor,
                        true),

                new SelectionBehaviour("ネームカラーの変更",
                    "\n名前の色を変更します。",
                    ModTag.NameColor),

                        new SelectionBehaviour("虹色の名前",
                            main.RainbowName,
                            "\n名前を虹色に変更します。",
                            ModTag.NameColor,
                            true),

                        new SelectionBehaviour("半透明の名前",
                            main.TranslucentName,
                            "\n名前の透明度を変更します。" + CommandList.ChatComHelp(CommandTag.TranslucentName),
                            ModTag.NameColor,
                            true),

                        new SelectionBehaviour("名前のアウトライン",
                            main.NameOutline,
                            "\n名前の虹色、半透明になる部分を名前の縁に変更します。" +
                            "\n虹色の名前か半透明の名前が有効でないと機能しません。",
                            ModTag.NameColor,
                            true),

                new SelectionBehaviour("アウトライン",
                    "\n様々なアウトラインの色を変更します。",
                    ModTag.Outline),

                        new SelectionBehaviour("虹色のアウトライン",
                            main.RainbowOutline,
                            "\nキルターゲットの縁取りの色を虹色にします。", 
                            ModTag.Outline,
                            true),

                        new SelectionBehaviour("クルー色のアウトライン",
                            main.CrewColorOutline,
                            "\nキルターゲットの縁取りの色を相手のクルーの色に変更します。",
                            ModTag.Outline,
                            true),

                        new SelectionBehaviour("虹色のベント",
                            main.RainbowVent,
                            "\nベントの縁取りを虹色に変更します。",
                            ModTag.Outline,
                            true),

                        new SelectionBehaviour("クルー色のベント",
                            main.CrewColorVent,
                            "\nベントの縁取りを自身の色に変更します。",
                            ModTag.Outline,
                            true),

                new SelectionBehaviour("偽のレベル",
                    main.FakeLevel,
                    "\n自身のレベルをランダムにします。",
                    ModTag.FakeLevel),

                new SelectionBehaviour("ネームプレート非表示",
                    main.HideNameplates,
                    "\n全員のネームプレートを真っ白にします。",
                    ModTag.HideNameplates),

                new SelectionBehaviour("拡大縮小機能",
                    main.Zoom,
                    "\n死亡時にマップ全体を見渡せるようにします。" +
                    "\nフリープレイ時は拡大ができます。",
                    ModTag.Zoom),

                new SelectionBehaviour("特定の機種を追い出す",
                    main.OPkick,
                    "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\n特定の機種を追い出すことができます。",
                    ModTag.OPkick),

                        new SelectionBehaviour("不明な機種",
                            main.AddUnknown,
                            "追い出す機種に不明な機種を追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("EpicPC",
                            main.AddEpicPC,
                            "追い出す機種にEpicPCを追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("SteamPC",
                            main.AddSteamPC,
                            "追い出す機種にSteamPCを追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("Mac",
                            main.AddMac,
                            "追い出す機種にMacを追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("Win10",
                            main.AddWin10,
                            "追い出す機種にWin10を追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("Itch",
                            main.AddItch,
                            "追い出す機種にItchを追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("IPhone",
                            main.AddIPhone,
                            "追い出す機種にIPhoneを追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("Android",
                            main.AddAndroid,
                            "追い出す機種にAndroidを追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("Switch",
                            main.AddSwitch,
                            "追い出す機種にSwitchを追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("Xbox",
                            main.AddXbox,
                            "追い出す機種にXboxを追加します",
                            ModTag.OPkick,
                            true),

                        new SelectionBehaviour("Playstation",
                            main.AddPlaystation,
                            "追い出す機種にPlaystationを追加します",
                            ModTag.OPkick,
                            true),

                new SelectionBehaviour("参加者にチャットを送る",
                    main.SendJoinPlayer,
                    "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\n入ってきたプレイヤーに任意のチャットを送ることができます。" + CommandList.ChatComHelp(CommandTag.SendChat),
                    ModTag.SendJoinPlayer),

                new SelectionBehaviour("二段の名前",
                    main.DoubleName,
                    "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\n名前を二段に変更できます。" + CommandList.ChatComHelp(CommandTag.DoubleName),
                    ModTag.DoubleName),

                new SelectionBehaviour("ゲーム中の名前",
                    main.ChangeGameName,
                    "部屋主限定".SetColor("00BFFF").SetSize(2.5f) + "\nゲーム中の名前を任意の名前に変更できます。" + CommandList.ChatComHelp(CommandTag.ChangeGameName),
                    ModTag.ChangeGameName),

                new SelectionBehaviour("自動コードコピー",
                    main.AutoCopyCode,
                    "\n部屋コードを自動でコピーします。",
                    ModTag.AutoCopyCode),

                new SelectionBehaviour("チャットコマンド",
                    main.ChatCommand,
                    "\nチャットコマンド[" + TextPlus.ComWord + "コマンド名]を有効、無効にできます。",
                    ModTag.ChatCommand),

                        new SelectionBehaviour("コマンド補完",
                            main.ComTab,
                            "\nTabのコマンド補完を有効にできます。",
                            ModTag.ChatCommand,
                            true),

                        new SelectionBehaviour("コマンドキャンセル",
                            main.ComCancel,
                            "\nコマンドが存在しない場合でも全体送信をキャンセルします。",
                            ModTag.ChatCommand,
                            true),

                        new SelectionBehaviour("コマンド文字変更",
                            main.ChangeComWord,
                            "\nコマンドの文字を「/」から「.」に変更します。",
                            ModTag.ChatCommand,
                            true),

                new SelectionBehaviour("キーコマンド",
                    main.KeyCommand,
                    "\nキーコマンドを有効、無効にできます。",
                    ModTag.KeyCommand),

                        new SelectionBehaviour("全消し",
                            main.KeyDelete,
                            "\nCtrl+BackSpace(全消し)を有効にします。",
                            ModTag.KeyCommand,
                            true),

                        new SelectionBehaviour("戻す、取り消し",
                            main.KeyUndoAndRedo,
                            "\nCtrl+Z(戻す) Ctrl+Y(取り消し)を有効にします。",
                            ModTag.KeyCommand,
                            true),

                        new SelectionBehaviour("コピー",
                            main.KeyCopy,
                            "\nCtrl+C(コピー)を有効にします。",
                            ModTag.KeyCommand,
                            true),

                        new SelectionBehaviour("カット",
                            main.KeyCut,
                            "\nCtrl+X(カット)を有効にします。",
                            ModTag.KeyCommand,
                            true),

                        new SelectionBehaviour("ペースト",
                            main.KeyPaste,
                            "\nCtrl+V(ペースト) Ctrl+Shift+V(直接ペースト)を有効にします。",
                            ModTag.KeyCommand,
                            true),

                new SelectionBehaviour("CPS",
                    main.CPS,
                    "\n一秒間のクリック数を表示します。" + CommandList.ChatComHelp(CommandTag.CPS),
                    ModTag.CPS),

                new SelectionBehaviour("DateTime",
                    main.DateTimeSetting,
                    "\n今日の日付、時間を表示します。" + CommandList.ChatComHelp(CommandTag.DateTime),
                    ModTag.DateTimeSetting),

                new SelectionBehaviour("部屋設定&プレイヤー一覧",
                    main.CustomOverlay,
                    "\n部屋設定とプレイヤー一覧を表示します。",
                    ModTag.CustomOverlay),

                        new SelectionBehaviour("役職の詳細設定",
                            main.ShowRoleSetting,
                            "\n設定の役職設定を表示します。",
                            ModTag.CustomOverlay,
                            true),

                        new SelectionBehaviour("フレンドコード",
                            main.ShowFriendText,
                            "\nプレイヤー一覧のフレンドコードを表示します。",
                            ModTag.CustomOverlay,
                            true),

                        new SelectionBehaviour("レベル",
                            main.ShowLevelText,
                            "\nプレイヤー一覧のレベルを表示します。",
                            ModTag.CustomOverlay,
                            true),

                        new SelectionBehaviour("部屋主の表示",
                            main.ShowHostColor,
                            "\nプレイヤー一覧の部屋主の色を変更します。",
                            ModTag.CustomOverlay,
                            true),

                        new SelectionBehaviour("色の名前表示",
                            main.ShowColorName,
                            "\nプレイヤー一覧の色の名前を表示します。",
                            ModTag.CustomOverlay,
                            true),

                        new SelectionBehaviour("色の一文字目表示",
                            main.FirstColorName,
                            "\nプレイヤー一覧の色の名前を一文字目だけ表示します。",
                            ModTag.CustomOverlay,
                            true),

                        new SelectionBehaviour("キー設定",
                            main.CustomOverlayKeyBind,
                            "",
                            ModTag.CustomOverlay),

                new SelectionBehaviour("旧Ping位置",
                    main.OldPingPositon,
                    "\nPingの表示位置を旧バージョンの表示位置に変更します。",
                    ModTag.OldPingPositon),

                new SelectionBehaviour("クルー色のチャット",
                    main.CrewColorChat,
                    "\nチャットの色をクルー色にします。",
                    ModTag.CrewColorChat),

                new SelectionBehaviour("半透明のチャット",
                    main.TranslucentChat,
                    "\nチャットを半透明にします。" + CommandList.ChatComHelp(CommandTag.TranslucentChat),
                    ModTag.TranslucentChat),

                new SelectionBehaviour("クルー色のネームプレート",
                    main.CrewColorVoteArea,
                    "\nネームプレートの色をクルー色にします。",
                    ModTag.CrewColorVoteArea),

                new SelectionBehaviour("FPS",
                    main.FPS,
                    "\n一秒間のフレームレートを表示します。" + CommandList.ChatComHelp(CommandTag.FPS),
                    ModTag.FPS),

                new SelectionBehaviour("FakePing",
                    main.FakePing,
                    "\nPingの数値を変更します。" + CommandList.ChatComHelp(CommandTag.FakePing),
                    ModTag.FakePing),

                new SelectionBehaviour("スキップロゴ",
                    main.SkipLogo,
                    "\n開始時のロゴをスキップします。",
                    ModTag.SkipLogo),

                new SelectionBehaviour("ネームボックス",
                    main.ChangeNameBox,
                    "\n部屋作成時と部屋検索時に名前変更を出来るようにします。",
                    ModTag.ChangeNameBox),

                new SelectionBehaviour("チャットボタンPlus",
                    "\nチャットボタンの設定を変更します。",
                    ModTag.ChatButtonPlus),

                        new SelectionBehaviour("フリープレイ時のチャット表示",
                            main.AlwaysChat,
                            "\nフリープレイ時にチャットボタンを常時表示します。",
                            ModTag.ChatButtonPlus,
                            true),

                        new SelectionBehaviour("チャットの最大文字数変更",
                            main.ChatLimitPlus,
                            "\nチャットの最大文字数を120文字に変更します。",
                            ModTag.ChatButtonPlus,
                            true),

                        new SelectionBehaviour("マップ非表示",
                            main.CancelChatMap,
                            "\nチャット入力時にマップを非表示にします。",
                            ModTag.ChatButtonPlus,
                            true),

                new SelectionBehaviour("部屋主の名前表示",
                    main.ShowHost,
                    "\nロビーに部屋主の名前を表示します。",
                    ModTag.ShowHost),

                new SelectionBehaviour("コード入力",
                    "\nコード入力を便利にします。",
                    ModTag.CodeText),

                        new SelectionBehaviour("ペースト",
                            main.PasteCodeText,
                            "\nCtrl+V(ペースト) を有効にします。",
                            ModTag.CodeText,
                            true),

                        new SelectionBehaviour("コード補完",
                            main.CodeTextPlus,
                            "\nコードの最後の文字を補完します。",
                            ModTag.CodeText,
                            true),

                new SelectionBehaviour("開始ボタン",
                    "\n開始ボタンの設定。",
                    ModTag.StartButton),

                        new SelectionBehaviour("常時有効",
                            main.AlwaysStart,
                            "\n開始ボタンを常時有効化します。",
                            ModTag.StartButton,
                            true),

                        new SelectionBehaviour("即開始",
                            main.StartCount,
                            "カウントダウン中にすぐ開始します。",
                            ModTag.StartButton),

                        new SelectionBehaviour("開始のキャンセル",
                            main.ResetCount,
                            "カウントダウン中に開始をキャンセルします。",
                            ModTag.StartButton),

                        new SelectionBehaviour("チャット時のカウントダウン表示",
                            main.StartCountText,
                            "\nチャットを開いててもカウントダウンを見えるようにします。",
                            ModTag.StartButton,
                            true),

                new SelectionBehaviour("壁ぬけ",
                    main.WallWalk,
                    "\nロビーで壁を歩けるようにします。",
                    ModTag.WallWalk),

                        new SelectionBehaviour("壁ぬけのキー設定",
                            main.WallWalkKeyBind,
                            "",
                            ModTag.WallWalk),

                new SelectionBehaviour("廃村",
                    main.EndGame,
                    "\n廃村コマンドを追加します。",
                    ModTag.EndGame),

                        new SelectionBehaviour("廃村コマンド1",
                            main.EndGameKeyBindFirst,
                            "廃村コマンドの1つ目を変更します",
                            ModTag.EndGame),

                        new SelectionBehaviour("廃村コマンド2",
                            main.EndGameKeyBindSecond,
                            "廃村コマンドの2つ目を変更します",
                            ModTag.EndGame),

                        new SelectionBehaviour("廃村コマンド3",
                            main.EndGameKeyBindThird,
                            "廃村コマンドの3つ目を変更します",
                            ModTag.EndGame),

                new SelectionBehaviour("カラーテキスト",
                    main.CrewColorText,
                    "\n色のテキストに色をつけます。",
                    ModTag.CrewColorText),

                new SelectionBehaviour("バグの修正",
                    "\nバグを修正します。",
                    ModTag.FixBag),

                        new SelectionBehaviour("スキンの修正",
                            main.FixSkinBag,
                            "\nスキンの位置がズレてるバグを修正します。",
                            ModTag.FixBag,
                            true),

                        new SelectionBehaviour("会議バグの修正",
                            main.FixLeftPlayer,
                            "\n抜けたプレイヤーが黒くならないバグを修正します。",
                            ModTag.FixBag,
                            true),

                        new SelectionBehaviour("プレイヤーカラーの修正",
                            main.FixPlayerColor,
                            "\nスキンメニューでプレイヤーの色が違うバグを修正します。",
                            ModTag.FixBag,
                            true),
            };

        public static SmallButton[] SNSButtons =>
            new SmallButton[]
            {
                new SmallButton("GitHub",
                    "TownOfPlusのGitHubを開きます",
                    ()=>
                    {
                        Process.Start("https://github.com/tugaru1975/TownOfPlus");
                    }),

                new SmallButton("Bag",
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

        private static TextMeshPro Note;
        private static TextMeshPro SettingMenuTitle;
        private static Dictionary<ToggleButtonBehaviour, SelectionBehaviour> modButtonsList;
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
            moreOptionsButton.OnClick = new ButtonClickedEvent();
            moreOptionsButton.OnClick.AddListener((Action)(() =>
            {
                if (!popUp) return;

                if (__instance.transform.parent && __instance.transform.parent == HudManager.Instance.transform)
                {
                    popUp.transform.SetParent(HudManager.Instance.transform);
                    popUp.transform.localPosition = new Vector3(0, 0, -800f);
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
            var Options = AllOptions;
            foreach (var info in Options)
            {
                var button = Object.Instantiate(buttonPrefab, popUp.transform);

                var HasChild = Options.Count(c => c.Tag.Equals(info.Tag)) != 1 && !info.IsChild;

                Vector3 pos;
                if (info.IsChild)
                {
                    if (modtype != info.Tag)
                    {
                        childcount = 0;
                        modtype = info.Tag;
                    }
                    else childcount++;
                    pos = new Vector3(childcount % 2 == 0 ? -2.92f : -0.58f, 1.75f - childcount / 2 * 0.8f, -.5f);
                }
                else
                {
                    count++;
                    int Y = count - ((int)Math.Floor((decimal)count / 12)) * 12;
                    pos = new Vector3(count % 2 == 0 ? -2.92f : -0.58f, 1.75f - Y / 2 * 0.8f, -.5f);
                }

                button.transform.localPosition = pos;

                button.onState = info.Config?.Value is true;

                button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

                button.Text.text = info.IsKeyBind ? (info.Title + "\n" + info.KeyName.Value).SetSize(1.75f) : info.Title;
                button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
                button.Text.font = Object.Instantiate(titleText.font);
                button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

                button.name = info.Tag.ToString() + (info.IsChild ? "Child" : "") + info.Title.Trim();

                var passiveButton = button.GetComponent<PassiveButton>();
                var colliderButton = button.GetComponent<BoxCollider2D>();

                button.gameObject.SetActive(false);

                var size = new Vector2(2.2f, .7f);
                colliderButton.size = size;

                passiveButton.OnClick = new ButtonClickedEvent();
                passiveButton.OnMouseOver = new UnityEvent();
                passiveButton.OnMouseOut = new UnityEvent();

                passiveButton.OnClick.AddListener((Action)(() =>
                {
                    if (info.IsFile || (Input.GetKey(KeyCode.LeftControl) && HasChild))
                    {
                        ModButtonsActive(info.Tag, info.Title);
                        return;
                    }
                    if (info.IsKeyBind)
                    {
                        button.Text.text = (info.Title + "\n" + (info.KeyName.Value = Helpers.GetKey())).SetSize(1.75f);
                        return;
                    }
                    button.onState = info.Config.Value = !info.GetOption().Config.Value;
                    button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                }));

                passiveButton.OnMouseOver.AddListener((Action)(() =>
                {
                    ResetNoteText($"{info.Title.SetSize(3)}\n{info.Note}", HasChild && !info.IsFile);
                    if (!info.IsFile) button.Background.color = new Color32(34, 139, 34, byte.MaxValue);
                }));

                passiveButton.OnMouseOut.AddListener((Action)(() =>
                {
                    ResetNoteText();
                    if (!info.IsFile) button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                }));

                foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    spr.size = size;
                modButtonsList.Add(button, info);
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
                button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

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

                passiveButton.OnClick = new ButtonClickedEvent();
                passiveButton.OnMouseOut = new UnityEvent();
                passiveButton.OnMouseOver = new UnityEvent();
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
            return (int)Math.Ceiling((decimal)modButtonsList.Count(c => !c.Value.IsChild) / 12);
        }


        private static void SetUpSNSButtons()
        {
            for (var i = 0; i < SNSButtons.Length; i++)
            {
                var info = SNSButtons[i];
                var button = Object.Instantiate(buttonPrefab, popUp.transform);
                button.transform.localPosition = new Vector3(3.75f - i * 0.5f, 2.4f);

                button.Background.color = Palette.LightBlue;

                button.Text.text = button.name = info.Title;
                button.Text.fontSizeMin = button.Text.fontSizeMax = 2.5f;
                button.Text.font = Object.Instantiate(titleText.font);
                button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

                button.gameObject.SetActive(true);

                var passiveButton = button.GetComponent<PassiveButton>();
                var colliderButton = button.GetComponent<BoxCollider2D>();

                var size = new Vector2(.5f, .5f);
                colliderButton.size = size;

                passiveButton.OnClick = new ButtonClickedEvent();
                passiveButton.OnMouseOut = new UnityEvent();
                passiveButton.OnMouseOver = new UnityEvent();
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
                    var button = mod.Key;
                    var info = mod.Value;
                    if (info.IsFile || info.IsKeyBind) continue;
                    if (button.onState = info.GetOption().Config?.Value is true) button.Background.color = Color.green;
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
                var button = mod.Key;
                var info = mod.Value;

                if (!info.IsChild) i++;
                var IsDef = (page - 1) * 12 <= i && i < page * 12 && !info.IsChild;
                var IsChild = info.Tag == type && info.IsChild;
                if (type.Equals(ModTag.None) ? IsDef : IsChild)
                {
                    if (info.IsFile) button.Background.color = GetFileColor(info.Tag);
                    button.gameObject.SetActive(true);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
            ResetSettingMenuTitle(title);
            OtherButtonsActive(type.Equals(ModTag.None));
        }

        private static void OtherButtonsActive(bool IsDef)
        {
            foreach (var other in OtherButtonsList)
            {
                other.Key.gameObject.SetActive(other.Value.Equals(IsDef));
            }
        }

        private static Color GetFileColor(ModTag type)
        {
            var IsColor = AllOptions.Any(w => w.Tag == type && w.IsChild && w.Config?.Value is true);
            return IsColor ? new Color32(255, 255, 0, 255) : Palette.Orange;
        }

        private static SelectionBehaviour GetOption(this SelectionBehaviour info) =>
            AllOptions.FirstOrDefault(f => f.Title == info.Title && f.Tag == info.Tag && f.IsChild == info.IsChild && f.IsFile == info.IsFile && f.IsKeyBind == info.IsKeyBind);


        private static IEnumerable<GameObject> GetAllChilds(this GameObject Go)
        {
            for (var i = 0; i < Go.transform.childCount; i++)
            {
                yield return Go.transform.GetChild(i).gameObject;
            }
        }

        public class SelectionBehaviour
        {
            public string Title = "";
            public ConfigEntry<bool> Config;
            public string Note = "";
            public ModTag Tag = ModTag.None;
            public ConfigEntry<KeyCode> KeyName;
            public ConfigEntry<string> WordName;
            public bool IsChild = false;
            public bool IsFile = false;
            public bool IsKeyBind = false;

            public SelectionBehaviour(string title, string note, ModTag tag)
            {
                Title = title;
                Note = note;
                Tag = tag;
                IsFile = true;
            }

            public SelectionBehaviour(string title, ConfigEntry<KeyCode> keyname, string note, ModTag tag)
            {
                Title = title;
                KeyName = keyname;
                Note = note + "\nキー押しながらクリックでキー設定を変更\nデフォルトのキー設定\n".SetSize(1.5f) + keyname.DefaultValue.ToString().SetSize(3f);
                Tag = tag;
                IsChild = true;
                IsKeyBind = true;
            }

            public SelectionBehaviour(string title, ConfigEntry<bool> config, string note, ModTag tag, bool ischild = false)
            {
                Title = title;
                Config = config;
                Note = note;
                Tag = tag;
                IsChild = ischild;
            }
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
}
