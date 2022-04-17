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
    public class ChangeSkin
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.LocalPlayer == null) return;
            if (main.RainbowHat.Value)
            {
                PlayerControl.LocalPlayer.HatRenderer.color = Color.HSVToRGB(Time.time % 1, 1, 1);
            }
            if (main.RainbowVisor.Value)
            {
                PlayerControl.LocalPlayer.VisorSlot.color = Color.HSVToRGB(Time.time % 1, 1, 1);
            }
        }
    }

}