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
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class DateTimeSetting
    {
        private static SpriteRenderer DateTimeUnderlay;
        private static TMPro.TextMeshPro DateTimetext;

        public static void Postfix(HudManager __instance)
        {
            if (!initializeOverlays()) return;

            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (PlayerControl.LocalPlayer == null || hudManager == null)
                return;

            Transform parent;
            parent = hudManager.transform;

            DateTimetext.transform.parent = parent;
            DateTimeUnderlay.transform.parent = parent;
            DateTimeUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
            DateTimeUnderlay.transform.localScale = new Vector3(2f, 0.25f, 0.5f);
            DateTimeUnderlay.transform.localPosition = new Vector3(main.DateTimepositionX.Value, main.DateTimepositionY.Value, -900f);

            DateTimetext.transform.localPosition = new Vector3(main.DateTimepositionX.Value, main.DateTimepositionY.Value, -900f);
            DateTimetext.text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            if (main.DateTimeSetting.Value)
            {
                if (main.SettingDateTime)
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
                }
                DateTimetext.enabled = true;
                DateTimeUnderlay.enabled = true;

            }
            else
            {
                DateTimeUnderlay.enabled = false;
                DateTimetext.enabled = false;
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
                DateTimetext.enabled = false;
            }
            return true;
        }
    }

}