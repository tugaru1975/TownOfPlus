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
    public static class FPS
    {
        private static SpriteRenderer FPSUnderlay;
        private static TMPro.TextMeshPro FPStext;
        public static void Postfix(HudManager __instance)
        {
            if (!initializeOverlays()) return;

            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (PlayerControl.LocalPlayer == null || hudManager == null)
                return;
            

            Transform parent;
            parent = hudManager.transform;

            if (main.FPS.Value)
            {
                FPStext.transform.parent = parent;
                FPSUnderlay.transform.parent = parent;
                FPSUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
                FPSUnderlay.transform.localScale = new Vector3(1f, 0.25f, 0.5f);
                FPSUnderlay.transform.localPosition = new Vector3(main.FPSpositionX.Value, main.FPSpositionY.Value, -900f);
                FPStext.transform.localPosition = new Vector3(main.FPSpositionX.Value, main.FPSpositionY.Value, -900f);

                CreateFlag.Run(() =>
                {
                    new LateTask(() =>
                    {
                        FPStext.text = (Math.Floor((1 / Time.deltaTime) * Math.Pow(10, 1)) / Math.Pow(10, 1)).ToString() + " FPS";
                        CreateFlag.NewFlag("FPS");
                    }, 1f);
                }, "FPS", true);
                if (main.SettingFPS && __instance.Chat.IsOpen)
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
                        var MousePositon = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.localPosition;
                        main.FPSpositionX.Value = MousePositon.x;
                        main.FPSpositionY.Value = MousePositon.y;
                    }  
                }
                else
                {
                    main.SettingFPS = false;
                }
                FPStext.enabled = true;
                FPSUnderlay.enabled = true;

            }
            else
            {
                FPSUnderlay.enabled = false;
                FPStext.enabled = false;
            }
        }
        public static bool initializeOverlays()
        {
            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (hudManager == null) return false;
            if (FPSUnderlay == null)
            {
                FPSUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
                FPSUnderlay.transform.localPosition = new Vector3(main.FPSpositionX.Value, main.FPSpositionY.Value, -900f);
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
                FPStext.enabled = false;
            }
            return true;
        }
    }

}