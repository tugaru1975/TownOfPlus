using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace TownOfPlus
{
    [HarmonyPatch]
    public static class ModOptionSetting
    {
        public static SelectionBehaviour[] AllOptions()
        {
            var Options = new SelectionBehaviour[]
            {
                new SelectionBehaviour("コード隠し", () => main.HideLobbyCodes.Value = !main.HideLobbyCodes.Value, main.HideLobbyCodes.Value,
                    "部屋のコードを隠すことができます。" + ChatComHelp(new string[] { "/ChangeLobbyCode", "/ChangeCodeColor" })),

                new SelectionBehaviour("ロビータイマー", () => main.LobbyTimer.Value = !main.LobbyTimer.Value, main.LobbyTimer.Value,
                    "ロビーが落ちるまでの目安となるタイマーを表示します。",true),

                new SelectionBehaviour("ランダムマップ", () => main.RandomMaps.Value = !main.RandomMaps.Value, main.RandomMaps.Value,
                    "毎試合マップをランダムに変更します。" + ChatComHelp(new string[] { "/RandomMap" }), true),

                new SelectionBehaviour("虹色の帽子", () => main.RainbowHat.Value = !main.RainbowHat.Value, main.RainbowHat.Value,
                    "帽子を虹色に変更します。"),

                new SelectionBehaviour("虹色のバイザー", () => main.RainbowVisor.Value = !main.RainbowVisor.Value, main.RainbowVisor.Value,
                    "バイザーを虹色に変更します。"),

                new SelectionBehaviour("虹色の名前", () => main.RainbowName.Value = !main.RainbowName.Value, main.RainbowName.Value,
                    "名前を虹色に変更します。"),

                new SelectionBehaviour("半透明の名前", () => main.TranslucentName.Value = !main.TranslucentName.Value, main.TranslucentName.Value,
                    "名前の透明度を変更します。" + ChatComHelp(new string[] { "/TranslucentName" })),

                new SelectionBehaviour("名前のアウトライン", () => main.NameOutline.Value = !main.NameOutline.Value, main.NameOutline.Value,
                    "名前の虹色、半透明になる部分を名前の縁に変更します。" +
                    "\n虹色の名前か半透明の名前が有効でないと機能しません。"),

                new SelectionBehaviour("虹色のアウトライン", () => main.RainbowOutline.Value = !main.RainbowOutline.Value, main.RainbowOutline.Value,
                    "キルターゲットの縁取りの色を虹色にします。"),

                new SelectionBehaviour("クルー色のアウトライン", () => main.CrewColorOutline.Value = !main.CrewColorOutline.Value, main.CrewColorOutline.Value,
                    "キルターゲットの縁取りの色を相手のクルーの色に変更します。"),

                new SelectionBehaviour("虹色のベント", () => main.RainbowVent.Value = !main.RainbowVent.Value, main.RainbowVent.Value,
                    "ベントの縁取りを虹色に変更します。"),

                new SelectionBehaviour("クルー色のベント", () => main.CrewColorVent.Value = !main.CrewColorVent.Value, main.CrewColorVent.Value,
                    "ベントの縁取りを自身の色に変更します。"),

                new SelectionBehaviour("偽のレベル", () => main.FakeLevel.Value = !main.FakeLevel.Value, main.FakeLevel.Value,
                    "自身のレベルを1にします。"),

                new SelectionBehaviour("ネームプレート非表示", () => main.HideNameplates.Value = !main.HideNameplates.Value, main.HideNameplates.Value,
                    "全員のネームプレートを真っ白にします。"),

                new SelectionBehaviour("拡大縮小機能", () => main.Zoom.Value = !main.Zoom.Value, main.Zoom.Value,
                    "死亡時にマップ全体を見渡せるようにします。" +
                    "\nフリープレイ時は拡大ができます。"),

                new SelectionBehaviour("特定の機種を追い出す", () => main.OPkick.Value = !main.OPkick.Value, main.OPkick.Value,
                    "特定の機種を追い出すことができます。" + ChatComHelp(new string[] { "/OPkick" }), true),

                new SelectionBehaviour("参加者にチャットを送る", () => main.SendJoinPlayer.Value = !main.SendJoinPlayer.Value, main.SendJoinPlayer.Value,
                    "入ってきたプレイヤーに任意のチャットを送ることができます。" + ChatComHelp(new string[] { "/SendChat" }), true),

                new SelectionBehaviour("二段の名前", () => main.DoubleName.Value = !main.DoubleName.Value, main.DoubleName.Value,
                    "名前を二段に変更できます。" + ChatComHelp(new string[] { "/DoubleName" }), true),

                new SelectionBehaviour("ゲーム中の名前", () => main.ChangeGameName.Value = !main.ChangeGameName.Value, main.ChangeGameName.Value,
                    "ゲーム中の名前を任意の名前に変更できます。" + ChatComHelp(new string[] { "/ChangeGameName" }), true),

                new SelectionBehaviour("自動コードコピー", () => main.AutoCopyCode.Value = !main.AutoCopyCode.Value, main.AutoCopyCode.Value,
                    "部屋コードを自動でコピーします。"),

                new SelectionBehaviour("チャットコマンド", () => main.ChatCommand.Value = !main.ChatCommand.Value, main.ChatCommand.Value,
                    "チャットコマンド[/コマンド名]を有効、無効にできます。"),

                new SelectionBehaviour("キーコマンド", () => main.KeyCommand.Value = !main.KeyCommand.Value, main.KeyCommand.Value,
                    "キーコマンド[Ctrl+C(コピー) Ctrl+V(ペースト) Ctrl+X(カット) Ctrl+Z(戻す) Ctrl+Y(取り消し) Shift+BackSpace(全消し)]を有効、無効にできます。"),

                new SelectionBehaviour("部屋設定の拡張", () => main.RoomOption.Value = !main.RoomOption.Value, main.RoomOption.Value,
                    "オンライン状態でのマップ切り替え、設定のキルクールやタスク量の上限を変更します。", true),

                new SelectionBehaviour("0.00001秒のキルクール", () => main.NokillCool.Value = !main.NokillCool.Value, main.NokillCool.Value,
                    "キルクールを0.00001秒に変更します。", true),

                new SelectionBehaviour("CPS", () => main.CPS.Value = !main.CPS.Value, main.CPS.Value,
                    "一秒間のクリック数を表示します。" + ChatComHelp(new string[] { "/CPS" })),

                new SelectionBehaviour("DateTime", () => main.DateTimeSetting.Value = !main.DateTimeSetting.Value, main.DateTimeSetting.Value,
                    "今日の日付、時間を表示します。" + ChatComHelp(new string[] { "/DateTime" })),

                new SelectionBehaviour("フレンドコードの非表示", () => main.HideFriendCode.Value = !main.HideFriendCode.Value, main.HideFriendCode.Value,
                    "プレイヤー一覧のフレンドコードを非表示にします。"),

                new SelectionBehaviour("旧Ping位置", () => main.OldPingPositon.Value = !main.OldPingPositon.Value, main.OldPingPositon.Value,
                    "Pingの表示位置を旧バージョンの表示位置に変更します。"),

                new SelectionBehaviour("クルー色のチャット", () => main.CrewColorChat.Value = !main.CrewColorChat.Value, main.CrewColorChat.Value,
                    "チャットの色をクルー色にします。"),

                new SelectionBehaviour("半透明のチャット", () => main.TranslucentChat.Value = !main.TranslucentChat.Value, main.TranslucentChat.Value,
                    "チャットを半透明にします。" + ChatComHelp(new string[] { "/TranslucentChat" })),

                new SelectionBehaviour("クルー色のネームプレート", () => main.CrewColorVoteArea.Value = !main.CrewColorVoteArea.Value, main.CrewColorVoteArea.Value,
                    "ネームプレートの色をクルー色にします。"),

                new SelectionBehaviour("FPS", () => main.FPS.Value = !main.FPS.Value, main.FPS.Value,
                    "一秒間のフレームレートを表示します。" + ChatComHelp(new string[] { "/FPS" })),

            };
            return Options;
        }
        private static string ChatComHelp(string[] com)
        {
            var help = "\n\n===チャットコマンド===";
            foreach (var args in com)
            {
                help += "\n<size=1.5>" + CommandList.AllCommand()[Helpers.AllCommandNum(args)].Help + "</size>";
            }
            return help;
        }

        private static GameObject popUp;
        private static TextMeshPro titleText;

        private static TextMeshPro NoteText;
        private static List<ToggleButtonBehaviour> modButtonsList;

        private static ToggleButtonBehaviour buttonPrefab;
        private static Vector3? _origin;
        private static int page = 1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static void MainMenuManager_StartPostfix(MainMenuManager __instance)
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
        public static void OptionsMenuBehaviour_StartPostfix(OptionsMenuBehaviour __instance)
        {
            if (!__instance.CensorChatButton) return;

            if (!popUp)
            {
                CreateCustom(__instance);
            }

            if (!buttonPrefab)
            {
                buttonPrefab = Object.Instantiate(__instance.CensorChatButton);
                Object.DontDestroyOnLoad(buttonPrefab);
                buttonPrefab.name = "CensorChatPrefab";
                buttonPrefab.gameObject.SetActive(false);
            }

            SetUpOptions();
            InitializeMoreButton(__instance);
        }

        private static void CreateCustom(OptionsMenuBehaviour prefab)
        {
            popUp = Object.Instantiate(prefab.gameObject);
            Object.DontDestroyOnLoad(popUp);
            var transform = popUp.transform;
            var pos = transform.localPosition;
            pos.z = -810f;
            transform.localPosition = pos;

            Object.Destroy(popUp.GetComponent<OptionsMenuBehaviour>());
            foreach (var gObj in popUp.gameObject.GetAllChilds())
            {
                if (gObj.name == "Background")
                {
                    gObj.transform.localScale = new Vector3(1.55f, gObj.transform.localScale.y);
                }
                else
                {
                    if (gObj.name == "CloseButton")
                    {
                        gObj.transform.localPosition = new Vector3(-4.5f, gObj.transform.localPosition.y);
                    }
                    else
                    {
                        Object.Destroy(gObj);
                    }
                }
            }

            popUp.SetActive(false);
        }

        private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
        {
            var moreOptions = Object.Instantiate(buttonPrefab, __instance.MouseAndKeyboardOptions.transform.parent);
            var transform = __instance.MouseAndKeyboardOptions.transform;
            _origin ??= transform.localPosition;
            moreOptions.transform.localPosition = _origin.Value + Vector3.up * 0.1f;

            moreOptions.gameObject.SetActive(true);
            moreOptions.Text.text = "TownOfPlusOptions";
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
                RefreshOpen(__instance);
            }));
        }

        private static void RefreshOpen(OptionsMenuBehaviour __instance)
        {
            popUp.gameObject.SetActive(false);
            popUp.gameObject.SetActive(true);
            SetUpOptions();
        }

        private static void SetNoteText()
        {
            NoteText = Object.Instantiate(titleText, popUp.transform);
            NoteText.GetComponent<RectTransform>().localPosition = new Vector3(2.25f, 1.75f , -1f);
            NoteText.GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f);
            NoteText.gameObject.SetActive(true);
            NoteText.alignment = TMPro.TextAlignmentOptions.Top;
            NoteText.fontSizeMin = NoteText.fontSize = 2f;
            NoteText.text = "";
            NoteText.name = "noteText";
        }

        private static void SetUpOptions()
        {
            if (popUp.transform.GetComponentInChildren<ToggleButtonBehaviour>()) return;

            RightOption();
            LeftOption();
            GithubButton();
            BagButton();
            SetNoteText();

            modButtonsList = new List<ToggleButtonBehaviour>();

            for (var i = 0; i < AllOptions().Length; i++)
            {
                var info = AllOptions()[i];
                var button = Object.Instantiate(buttonPrefab, popUp.transform);
                int count = i + 1;
                int Y = ((i - ((int)Math.Floor((decimal)i / 12)) * 12));

                var pos = new Vector3(i % 2 == 0 ? -2.92f : -0.58f, 1.75f - Y / 2 * 0.8f, -.5f);

                var transform = button.transform;
                transform.localPosition = pos;

                button.onState = info.DefaultValue;
                button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                
                button.Text.text = info.Title;
                button.Text.fontSizeMin = button.Text.fontSizeMax = 2.2f;
                button.Text.font = Object.Instantiate(titleText.font);
                button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

                button.name = info.Title.Replace(" ", "") + "Toggle";

                var passiveButton = button.GetComponent<PassiveButton>();
                var colliderButton = button.GetComponent<BoxCollider2D>();

                button.gameObject.SetActive(true);

                colliderButton.size = new Vector2(2.2f, .7f);

                passiveButton.OnClick = new ButtonClickedEvent();
                passiveButton.OnMouseOver = new UnityEvent();
                passiveButton.OnMouseOut = new UnityEvent();

                passiveButton.OnClick.AddListener((Action)(() =>
                {
                    button.onState = info.OnClick();
                    button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                    
                }));

                passiveButton.OnMouseOver.AddListener((Action)(() =>
                {
                    NoteText.text = $"<size=3>{info.Title}</size>\n{(info.Host ? "<size=2.5><color=#00bfff>部屋主限定</color></size>" : "")}\n{info.Note}";
                    button.Background.color = new Color32(34, 139, 34, byte.MaxValue);
                }));

                passiveButton.OnMouseOut.AddListener((Action)(() =>
                {
                    NoteText.text = "";
                    button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                }));

                foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    spr.size = new Vector2(2.2f, .7f);
                modButtonsList.Add(button);
            }
            ModButtonsActive();
        }

        private static void RightOption()
        {
            var button = Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(-0.58f, 2.4f);
            var transform = button.transform;
            transform.localPosition = pos;
            button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

            button.Text.text = "右ページ";
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.5f;
            button.Text.font = Object.Instantiate(titleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            button.name = "右ページ";
            button.gameObject.SetActive(true);

            var passiveButton = button.GetComponent<PassiveButton>();
            var colliderButton = button.GetComponent<BoxCollider2D>();

            colliderButton.size = new Vector2(2.2f, .4f);

            passiveButton.OnClick = new ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEvent();
            passiveButton.OnMouseOver = new UnityEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                if (page != (int)Math.Ceiling((decimal)AllOptions().Length / 12))
                {
                    page += 1;
                }
                else
                {
                    page = 1;
                }
                ModButtonsActive();
            }));
            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));
            foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                spr.size = new Vector2(2.2f, .4f);

        }
        private static void LeftOption()
        {
            var button = Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(-2.92f, 2.4f);
            var transform = button.transform;
            transform.localPosition = pos;
            button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

            button.Text.text = "左ページ";
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.5f;
            button.Text.font = Object.Instantiate(titleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            button.name = "左ページ";
            button.gameObject.SetActive(true);

            var passiveButton = button.GetComponent<PassiveButton>();
            var colliderButton = button.GetComponent<BoxCollider2D>();

            colliderButton.size = new Vector2(2.2f, .4f);

            passiveButton.OnClick = new ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEvent();
            passiveButton.OnMouseOver = new UnityEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                if (page != 1)
                {
                    page -= 1;
                }
                else
                {
                    page = (int)Math.Ceiling((decimal)AllOptions().Length / 12);
                }
                ModButtonsActive();
            }));
            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));
            foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                spr.size = new Vector2(2.2f, .4f);
        }

        private static void GithubButton()
        {
            var button = Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(3.25f, 2.4f);
            var transform = button.transform;
            transform.localPosition = pos;
            button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

            button.Text.text = "Github";
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.5f;
            button.Text.font = Object.Instantiate(titleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            button.name = "Github";
            button.gameObject.SetActive(true);

            var passiveButton = button.GetComponent<PassiveButton>();
            var colliderButton = button.GetComponent<BoxCollider2D>();

            colliderButton.size = new Vector2(.5f, .5f);

            passiveButton.OnClick = new ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEvent();
            passiveButton.OnMouseOver = new UnityEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                System.Diagnostics.Process.Start("https://github.com/tugaru1975/TownOfPlus");
            }));
            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));
            foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                spr.size = new Vector2(.5f, .5f);
        }

        private static void BagButton()
        {
            var button = Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(3.75f, 2.4f);
            var transform = button.transform;
            transform.localPosition = pos;
            button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;

            button.Text.text = "Bag";
            button.Text.fontSizeMin = button.Text.fontSizeMax = 2.5f;
            button.Text.font = Object.Instantiate(titleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            button.name = "Bag";
            button.gameObject.SetActive(true);

            var passiveButton = button.GetComponent<PassiveButton>();
            var colliderButton = button.GetComponent<BoxCollider2D>();

            colliderButton.size = new Vector2(.5f, .5f);

            passiveButton.OnClick = new ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEvent();
            passiveButton.OnMouseOver = new UnityEvent();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                System.Diagnostics.Process.Start("https://marshmallow-qa.com/tugaruyukkuri");
            }));
            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));
            foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                spr.size = new Vector2(.5f, .5f);
        }

        public static void ModButtonsActive()
        {
            for (var i = 0; i < AllOptions().Length; i++)
            {
                var count = i + 1;
                if ((page - 1) * 12 < count && count <= page * 12)
                {
                    (modButtonsList[i]).gameObject.SetActive(true);
                }
                else
                {
                    (modButtonsList[i]).gameObject.SetActive(false);
                }
            }
        }
        private static IEnumerable<GameObject> GetAllChilds(this GameObject Go)
        {
            for (var i = 0; i < Go.transform.childCount; i++)
            {
                yield return Go.transform.GetChild(i).gameObject;
            }
        }

        public class SelectionBehaviour
        {
            public string Title;
            public Func<bool> OnClick;
            public bool DefaultValue;
            public string Note;
            public bool Host;

            public SelectionBehaviour(string title, Func<bool> onClick, bool defaultValue, string note, bool host = false)
            {
                Title = title;
                OnClick = onClick;
                DefaultValue = defaultValue;
                Note = note;
                Host = host;
            }
        }
    }
}
