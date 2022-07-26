using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class CPSOverlay
    {
        private static SpriteRenderer CPSUnderlay;
        private static TMPro.TextMeshPro CPStext;
        private static int count = 0;
        public static void Postfix(HudManager __instance)
        {
            if (main.CPS.Getbool())
            {
                if (DestroyableSingleton<HudManager>.Instance == null) return;
                if (CPSUnderlay == null) CPSUnderlay = Overlay.CreateUnderlay(main.CPSpositionX, main.CPSpositionY, "CPS");
                if (CPStext == null) CPStext = Overlay.CreateText(main.CPSpositionX, main.CPSpositionY, "CPS");
                CPSUnderlay.transform.localScale = new Vector3(1f, 0.25f, 0.5f);

                CPStext.text = count.ToString("D3") + " CPS";
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    count++;
                    new LateTask(() => 
                    {
                        if (count != 0) count--;
                    },1f);
                }

                if (main.SettingCPS && GameState.IsChatOpen)
                {
                    CPStext.transform.localPosition = CPSUnderlay.transform.localPosition = Overlay.SettingPos(main.CPSpositionX, main.CPSpositionY);
                }
                else
                {
                    main.SettingCPS = false;
                }
                Flag.NewFlag("CPS");
            }
            else
            {
                Flag.Run(() =>
                {
                    count = 0;
                    CPSUnderlay?.gameObject.Destroy();
                    CPStext?.gameObject.Destroy();
                }, "CPS");
            }
        }
    }
}