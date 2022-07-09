using System;
using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class FPSOverlay
    {
        private static SpriteRenderer FPSUnderlay;
        private static TMPro.TextMeshPro FPStext;
        private static string text = "0";
        public static void Postfix(HudManager __instance)
        {
            if (main.FPS.Value)
            {
                if (!InitializeOverlays()) return;

                HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
                if (PlayerControl.LocalPlayer == null || hudManager == null)
                    return;

                var parent = hudManager.transform;

                FPStext.transform.SetParent(parent);
                FPSUnderlay.transform.SetParent(parent);
                FPSUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
                FPSUnderlay.transform.localScale = new Vector3(1f, 0.25f, 0.5f);
                FPSUnderlay.transform.localPosition = new Vector3(main.FPSpositionX.Value, main.FPSpositionY.Value, -900f);
                FPStext.transform.localPosition = new Vector3(main.FPSpositionX.Value, main.FPSpositionY.Value, -900f);
                FPStext.text = text;

                Flag.Run(() =>
                {
                    new LateTask(() =>
                    {
                        text = (Math.Floor(1 / Time.deltaTime * 10) / 10).ToString("F1") + " FPS";
                        Flag.NewFlag("FPSText");
                    }, 1f);
                }, "FPSText", true);

                if (main.SettingFPS && GameState.IsChatOpen)
                {
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        main.FPSpositionX.Value += 0.05f;
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        main.FPSpositionX.Value -= 0.05f;
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        main.FPSpositionY.Value -= 0.05f;
                    }
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        main.FPSpositionY.Value += 0.05f;
                    }
                    if (Input.GetMouseButton(1))
                    {
                        main.FPSpositionX.Value = Helpers.ScreenToMousePositon.x;
                        main.FPSpositionY.Value = Helpers.ScreenToMousePositon.y;
                    }
                }
                else
                {
                    main.SettingFPS = false;
                }
                FPStext.enabled = true;
                FPSUnderlay.enabled = true;
                Flag.NewFlag("FPS");
            }
            else
            {
                Flag.Run(() =>
                {
                    FPSUnderlay?.gameObject.Destroy();
                    FPStext?.gameObject.Destroy();
                }, "FPS");
            }
        }
        public static bool InitializeOverlays()
        {
            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (hudManager == null) return false;
            if (FPSUnderlay == null)
            {
                FPSUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
                FPSUnderlay.transform.localPosition = new Vector3(main.FPSpositionX.Value, main.FPSpositionY.Value, -900f);
                FPSUnderlay.name = "FPSUnderlay";
                FPSUnderlay.gameObject.SetActive(true);
                FPSUnderlay.enabled = false;
            }
            if (FPStext == null)
            {
                FPStext = UnityEngine.Object.Instantiate(hudManager.TaskText, hudManager.transform);
                FPStext.fontSize = FPStext.fontSizeMin = FPStext.fontSizeMax = 1.15f;
                FPStext.autoSizeTextContainer = false;
                FPStext.enableWordWrapping = false;
                FPStext.alignment = TMPro.TextAlignmentOptions.Center;
                FPStext.transform.localPosition = new Vector3(main.FPSpositionX.Value, main.FPSpositionY.Value, -900f);
                FPStext.transform.localScale = Vector3.one * 1.5f;
                FPStext.color = Palette.White;
                FPStext.name = "FPStext";
                FPStext.text = "0";
                FPStext.enabled = false;
            }
            return true;
        }
    }

}