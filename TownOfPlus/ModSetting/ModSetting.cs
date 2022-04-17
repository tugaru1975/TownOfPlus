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
        private static SelectionBehaviour[] AllOptions = {
            new SelectionBehaviour("コード隠し", () => main.HideLobbyCodes.Value = !main.HideLobbyCodes.Value, main.HideLobbyCodes.Value),
            new SelectionBehaviour("<color=#00bfff>ロビータイマー</color>", () => main.LobbyTimer.Value = !main.LobbyTimer.Value, main.LobbyTimer.Value),
            new SelectionBehaviour("<color=#00bfff>ランダムマップ</color>", () => main.RandomMaps.Value = !main.RandomMaps.Value, main.RandomMaps.Value),
            new SelectionBehaviour("虹色の帽子", () => main.RainbowHat.Value = !main.RainbowHat.Value, main.RainbowHat.Value),
            new SelectionBehaviour("虹色のバイザー", () => main.RainbowVisor.Value = !main.RainbowVisor.Value, main.RainbowVisor.Value),
            new SelectionBehaviour("虹色の名前", () => main.RainbowName.Value = !main.RainbowName.Value, main.RainbowName.Value),
            new SelectionBehaviour("半透明の名前", () => main.TranslucentName.Value = !main.TranslucentName.Value, main.TranslucentName.Value),
            new SelectionBehaviour("名前のアウトライン", () => main.NameOutline.Value = !main.NameOutline.Value, main.NameOutline.Value),
            new SelectionBehaviour("虹色のアウトライン", () => main.RainbowOutline.Value = !main.RainbowOutline.Value, main.RainbowOutline.Value),
            new SelectionBehaviour("クルー色のアウトライン", () => main.CrewColorOutline.Value = !main.CrewColorOutline.Value, main.CrewColorOutline.Value),
            new SelectionBehaviour("虹色のベント", () => main.RainbowVent.Value = !main.RainbowVent.Value, main.RainbowVent.Value),
            new SelectionBehaviour("クルー色のベント", () => main.CrewColorVent.Value = !main.CrewColorVent.Value, main.CrewColorVent.Value),
            new SelectionBehaviour("<color=#FF0000>偽のレベル</color>", () => main.FakeLevel.Value = !main.FakeLevel.Value, main.FakeLevel.Value),
            new SelectionBehaviour("ネームプレート非表示", () => main.HideNameplates.Value = !main.HideNameplates.Value, main.HideNameplates.Value),
            new SelectionBehaviour("拡大縮小機能", () => main.Zoom.Value = !main.Zoom.Value, main.Zoom.Value),
            new SelectionBehaviour("<color=#00bfff>特定の機種を追い出す</color>", () => main.OPkick.Value = !main.OPkick.Value, main.OPkick.Value),
            new SelectionBehaviour("<color=#00bfff>参加者にチャットを送る</color>", () => main.SendJoinPlayer.Value = !main.SendJoinPlayer.Value, main.SendJoinPlayer.Value),
            new SelectionBehaviour("<color=#00bfff>二段の名前</color>", () => main.DoubleName.Value = !main.DoubleName.Value, main.DoubleName.Value),
            new SelectionBehaviour("<color=#00bfff>ゲーム中の名前</color>", () => main.ChangeGameName.Value = !main.ChangeGameName.Value, main.ChangeGameName.Value),
            new SelectionBehaviour("自動コードコピー", () => main.AutoCopyCode.Value = !main.AutoCopyCode.Value, main.AutoCopyCode.Value),
            new SelectionBehaviour("チャットコマンド", () => main.ChatCommand.Value = !main.ChatCommand.Value, main.ChatCommand.Value),
            new SelectionBehaviour("キーコマンド", () => main.KeyCommand.Value = !main.KeyCommand.Value, main.KeyCommand.Value),
            new SelectionBehaviour("部屋設定の拡張", () => main.RoomOption.Value = !main.RoomOption.Value, main.RoomOption.Value),
            new SelectionBehaviour("0.00001秒のキルクール", () => main.NokillCool.Value = !main.NokillCool.Value, main.NokillCool.Value),
            new SelectionBehaviour("CPS", () => main.CPS.Value = !main.CPS.Value, main.CPS.Value),
            new SelectionBehaviour("DateTime", () => main.DateTimeSetting.Value = !main.DateTimeSetting.Value, main.DateTimeSetting.Value),

        };

        private static GameObject popUp;
        private static TextMeshPro titleText;
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
                if (gObj.name != "Background" && gObj.name != "CloseButton")
                    Object.Destroy(gObj);
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

        private static void SetUpOptions()
        {
            if (popUp.transform.GetComponentInChildren<ToggleButtonBehaviour>()) return;

            RightOption();
            LeftOption();

            modButtonsList = new List<ToggleButtonBehaviour>();

            for (var i = 0; i < AllOptions.Length; i++)
            {
                var info = AllOptions[i];
                var button = Object.Instantiate(buttonPrefab, popUp.transform);
                int count = i + 1;
                int Y = ((i - ((int)Math.Floor((decimal)i / 12)) * 12));

                var pos = new Vector3(i % 2 == 0 ? -1.17f : 1.17f, 1.75f - Y / 2 * 0.8f, -.5f);

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
                passiveButton.OnMouseOut = new UnityEvent();
                passiveButton.OnMouseOver = new UnityEvent();
                passiveButton.OnClick.AddListener((Action)(() =>
                {
                    button.onState = info.OnClick();
                    button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
                }));

                foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                    spr.size = new Vector2(2.2f, .7f);
                modButtonsList.Add(button);
            }
            for (int i = 0; i < AllOptions.Length; i++)
            {
                if (i >= modButtonsList.Count) break;
                (modButtonsList[i]).Background.color = AllOptions[i].OnClick() ? Color.green : Palette.ImpostorRed;
                (modButtonsList[i]).Background.color = AllOptions[i].OnClick() ? Color.green : Palette.ImpostorRed;
            }
            ModButtonsActive();
        }
        private static void RightOption()
        {
            var button = Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(1.17f, 2.4f);
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
                if (page != (int)Math.Ceiling((decimal)AllOptions.Length / 12))
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
            var pos = new Vector3(-1.17f, 2.4f);
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
                    page = (int)Math.Ceiling((decimal)AllOptions.Length / 12);
                }
                ModButtonsActive();
            }));
            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, byte.MaxValue)));
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));
            foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
                spr.size = new Vector2(2.2f, .4f);
        }
        public static void ModButtonsActive()
        {
            for (var i = 0; i < AllOptions.Length; i++)
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

        private class SelectionBehaviour
        {
            public string Title;
            public Func<bool> OnClick;
            public bool DefaultValue;

            public SelectionBehaviour(string title, Func<bool> onClick, bool defaultValue)
            {
                Title = title;
                OnClick = onClick;
                DefaultValue = defaultValue;
            }
        }
    }
}
