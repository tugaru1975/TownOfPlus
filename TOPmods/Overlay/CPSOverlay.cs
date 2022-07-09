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
            if (main.CPS.Value)
            {
                if (!initializeOverlays()) return;

                HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
                if (PlayerControl.LocalPlayer == null || hudManager == null)
                    return;

                var parent = hudManager.transform;

                CPStext.transform.SetParent(parent);
                CPSUnderlay.transform.SetParent(parent);
                CPSUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
                CPSUnderlay.transform.localScale = new Vector3(1f, 0.25f, 0.5f);
                CPSUnderlay.transform.localPosition = new Vector3(main.CPSpositionX.Value, main.CPSpositionY.Value, -900f);
                CPStext.transform.localPosition = new Vector3(main.CPSpositionX.Value, main.CPSpositionY.Value, -900f);
                
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
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        main.CPSpositionX.Value += 0.05f;
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        main.CPSpositionX.Value -= 0.05f;
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        main.CPSpositionY.Value -= 0.05f;
                    }
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        main.CPSpositionY.Value += 0.05f;
                    }
                    if (Input.GetMouseButton(1))
                    {
                        main.CPSpositionX.Value = Helpers.ScreenToMousePositon.x;
                        main.CPSpositionY.Value = Helpers.ScreenToMousePositon.y;
                    }
                }
                else
                {
                    main.SettingCPS = false;
                }
                CPStext.enabled = true;
                CPSUnderlay.enabled = true;
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
        public static bool initializeOverlays()
        {
            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (hudManager == null) return false;
            if (CPSUnderlay == null)
            {
                CPSUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
                CPSUnderlay.transform.localPosition = new Vector3(main.CPSpositionX.Value, main.CPSpositionY.Value, -900f);
                CPSUnderlay.name = "CPSUnderlay";
                CPSUnderlay.gameObject.SetActive(true);
                CPSUnderlay.enabled = false;
            }
            if (CPStext == null)
            {
                CPStext = UnityEngine.Object.Instantiate(hudManager.TaskText, hudManager.transform);
                CPStext.fontSize = CPStext.fontSizeMin = CPStext.fontSizeMax = 1.15f;
                CPStext.autoSizeTextContainer = false;
                CPStext.enableWordWrapping = false;
                CPStext.alignment = TMPro.TextAlignmentOptions.Center;
                CPStext.transform.localPosition = new Vector3(main.CPSpositionX.Value, main.CPSpositionY.Value, -900f);
                CPStext.transform.localScale = Vector3.one * 1.5f;
                CPStext.color = Palette.White;
                CPStext.name = "CPStext";
                CPStext.text = "0";
                CPStext.enabled = false;
            }
            return true;
        }
    }

}