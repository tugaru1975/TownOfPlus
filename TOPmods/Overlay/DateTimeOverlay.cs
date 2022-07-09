using System;
using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class DateTimeOverlay
    {
        private static SpriteRenderer DateTimeUnderlay;
        private static TMPro.TextMeshPro DateTimetext;

        public static void Postfix(HudManager __instance)
        {
            if (main.DateTimeSetting.Value)
            {
                if (!initializeOverlays()) return;

                HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
                if (PlayerControl.LocalPlayer == null || hudManager == null)
                    return;

                var parent = hudManager.transform;

                DateTimetext.transform.SetParent(parent);
                DateTimeUnderlay.transform.SetParent(parent);
                DateTimeUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
                DateTimeUnderlay.transform.localScale = new Vector3(2f, 0.25f, 0.5f);
                DateTimeUnderlay.transform.localPosition = new Vector3(main.DateTimepositionX.Value, main.DateTimepositionY.Value, -900f);
                DateTimetext.transform.localPosition = new Vector3(main.DateTimepositionX.Value, main.DateTimepositionY.Value, -900f);

                DateTimetext.text = DateTime.Now.ToString("G");

                if (main.SettingDateTime && GameState.IsChatOpen)
                {
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        main.DateTimepositionX.Value += 0.05f;
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        main.DateTimepositionX.Value -= 0.05f;
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        main.DateTimepositionY.Value -= 0.05f;
                    }
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        main.DateTimepositionY.Value += 0.05f;
                    }
                    if (Input.GetMouseButton(1))
                    {
                        main.DateTimepositionX.Value = Helpers.ScreenToMousePositon.x;
                        main.DateTimepositionY.Value = Helpers.ScreenToMousePositon.y;
                    }
                }
                else
                {
                    main.SettingDateTime = false;
                }
                DateTimetext.enabled = true;
                DateTimeUnderlay.enabled = true;
                Flag.NewFlag("DateTime");
            }
            else
            {
                Flag.Run(() =>
                {
                    DateTimeUnderlay?.gameObject.Destroy();
                    DateTimetext?.gameObject.Destroy();
                }, "DateTime");
            }
        }
        public static bool initializeOverlays()
        {
            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (hudManager == null) return false;
            if (DateTimeUnderlay == null)
            {
                DateTimeUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
                DateTimeUnderlay.transform.localPosition = new Vector3(main.DateTimepositionX.Value, main.DateTimepositionY.Value, -900f);
                DateTimeUnderlay.name = "DateTimeUnderlay";
                DateTimeUnderlay.gameObject.SetActive(true);
                DateTimeUnderlay.enabled = false;
            }
            if (DateTimetext == null)
            {
                DateTimetext = UnityEngine.Object.Instantiate(hudManager.TaskText, hudManager.transform);
                DateTimetext.fontSize = DateTimetext.fontSizeMin = DateTimetext.fontSizeMax = 1.15f;
                DateTimetext.autoSizeTextContainer = false;
                DateTimetext.enableWordWrapping = false;
                DateTimetext.alignment = TMPro.TextAlignmentOptions.Center;
                DateTimetext.transform.localPosition = new Vector3(main.DateTimepositionX.Value, main.DateTimepositionY.Value, -900f);
                DateTimetext.transform.localScale = Vector3.one * 1.5f;
                DateTimetext.color = Palette.White;
                DateTimetext.name = "DateTimetext";
                DateTimetext.text = "0";
                DateTimetext.enabled = false;
            }
            return true;
        }
    }

}