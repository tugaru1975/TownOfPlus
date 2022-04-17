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
    public static class Outline
    {
        public static void Prefix()
        {
            //キル対象
            if (main.RainbowOutline.Value)
            {
                if (PlayerControl.LocalPlayer == null) return;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p != PlayerControl.LocalPlayer) p.MyRend.material.SetColor("_OutlineColor", Color.HSVToRGB(Time.time % 1, 1, 1));
                }
            }
            if (main.CrewColorOutline.Value)
            {
                if (PlayerControl.LocalPlayer == null) return;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p != PlayerControl.LocalPlayer) p.MyRend.material.SetColor("_OutlineColor", p.MyRend.material.GetColor("_BodyColor"));
                }
            }

            //ベント
            if (main.RainbowVent.Value)
            {
                if (ShipStatus.Instance == null) return;
                foreach (Vent vent in ShipStatus.Instance.AllVents)
                {
                    vent.myRend.material.SetColor("_OutlineColor", Color.HSVToRGB(Time.time % 1, 1, 1));
                }
            }
            if (main.CrewColorVent.Value)
            {
                if (ShipStatus.Instance == null) return;
                foreach (Vent vent in ShipStatus.Instance.AllVents)
                {
                    vent.myRend.material.SetColor("_OutlineColor", PlayerControl.LocalPlayer.MyRend.material.GetColor("_BodyColor"));
                }
            }
        }
    }
}