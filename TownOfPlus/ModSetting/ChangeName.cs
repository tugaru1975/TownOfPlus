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
    //名前変更
    public class ChangeName
    {
        private static string Outline = "false";
        private static StartAction Action = new StartAction();
        public static void Postfix(HudManager __instance)
        {
            var p = PlayerControl.LocalPlayer;
            Color color = Helpers.GetPlayerColor(p);
            if (main.NameOutline.Value) color = Palette.Black;
            if (main.RainbowName.Value || main.TranslucentName.Value)
            {
                Action.Reset();
                if (Outline != main.NameOutline.Value.ToString())
                {
                    Outline = main.NameOutline.Value.ToString();
                    Reset(p);
                }
                if (main.RainbowName.Value)
                {
                    color = Color.HSVToRGB(Time.time % 1, 1, 1);
                }
                if (main.TranslucentName.Value)
                {
                    color.a = (100f - main.SetTranslucentName.Value) / 100f;
                }
                if (main.NameOutline.Value) p.nameText.outlineColor = color;
                else p.nameText.color = color;
            }
            else
            {
                Reset(p);
            }

        }
        public static void Reset(PlayerControl p)
        {
            Action.Run(() =>
            {
                p.nameText.color = Helpers.GetPlayerColor(p);
                p.nameText.outlineColor = Palette.Black;
            });
        }
    }
}