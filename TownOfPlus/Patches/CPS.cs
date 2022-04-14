using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using System;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnhollowerBaseLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;
using Il2CppSystem;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class CPS
    {
        private static SpriteRenderer CPSUnderlay;
        private static TMPro.TextMeshPro CPStext;
        private static int count = 0;

        public static void Postfix(HudManager __instance)
        {
            if (!initializeOverlays()) return;

            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (PlayerControl.LocalPlayer == null || hudManager == null)
                return;

            Transform parent;
            parent = hudManager.transform;

            CPStext.transform.parent = parent;
            CPSUnderlay.transform.parent = parent;
            CPSUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
            CPSUnderlay.transform.localScale = new Vector3(1f, 0.25f, 0.5f);
            CPSUnderlay.transform.localPosition = new Vector3(main.CPSpositionX.Value, main.CPSpositionY.Value, -900f);

            CPStext.transform.localPosition = new Vector3(main.CPSpositionX.Value, main.CPSpositionY.Value, -900f);
            CPStext.text = count.ToString() + " CPS";
            if (main.CPS.Value)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    count += 1;
                    new Timer(() => 
                    {
                        count -= 1;
                    },1f);
                }
                if (main.SettingCPS)
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
                }
                CPStext.enabled = true;
                CPSUnderlay.enabled = true;

            }
            else
            {
                CPSUnderlay.enabled = false;
                CPStext.enabled = false;
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
                CPStext.enabled = false;
            }
            return true;
        }
    }

}