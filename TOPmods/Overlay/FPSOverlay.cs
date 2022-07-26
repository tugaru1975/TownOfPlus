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
            if (main.FPS.Getbool())
            {
                if (DestroyableSingleton<HudManager>.Instance == null) return;
                if (FPSUnderlay == null) FPSUnderlay = Overlay.CreateUnderlay(main.FPSpositionX, main.FPSpositionY, "FPS");
                if (FPStext == null) FPStext = Overlay.CreateText(main.FPSpositionX, main.FPSpositionY, "FPS");
                FPSUnderlay.transform.localScale = new Vector3(1f, 0.25f, 0.5f);

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
                    FPStext.transform.localPosition = FPSUnderlay.transform.localPosition = Overlay.SettingPos(main.FPSpositionX, main.FPSpositionY);
                }
                else
                {
                    main.SettingFPS = false;
                }
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
    }
}