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
            if (main.DateTimeSetting.Getbool())
            {
                if (DestroyableSingleton<HudManager>.Instance == null) return;
                if (DateTimeUnderlay == null) DateTimeUnderlay = Overlay.CreateUnderlay(main.DateTimepositionX, main.DateTimepositionY, "DateTime");
                if (DateTimetext == null) DateTimetext = Overlay.CreateText(main.DateTimepositionX, main.DateTimepositionY, "DateTime");
                DateTimeUnderlay.transform.localScale = new Vector3(1.75f, 0.25f, 0.5f);

                DateTimetext.text = DateTime.Now.ToString("G");

                if (main.SettingDateTime && GameState.IsChatOpen)
                {
                    DateTimetext.transform.localPosition = DateTimeUnderlay.transform.localPosition = Overlay.SettingPos(main.DateTimepositionX, main.DateTimepositionY);
                }
                else
                {
                    main.SettingDateTime = false;
                }
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
    }
}