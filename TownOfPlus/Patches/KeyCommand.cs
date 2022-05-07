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
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class KeyCommand
    {
        private static readonly System.Random random = new System.Random((int)System.DateTime.Now.Ticks);
        private static List<PlayerControl> bots = new List<PlayerControl>();
        public static void Postfix(ChatController __instance)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (!__instance.isActiveAndEnabled) return;
                __instance.SetVisible(false);
                new LateTask(() =>
                {
                    __instance.SetVisible(true);
                }, 0f);
            }
            //チャットバグ
            if (__instance.IsOpen)
            {
                if (__instance.animating)
                {
                    __instance.BanButton.MenuButton.enabled = false;
                }
                else
                {
                    __instance.BanButton.MenuButton.enabled = true;
                }
            }
        }
        //[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
        //class CheckGameEndPatch
        //{
        //    public static bool Prefix(ShipStatus __instance)
        //    {
        //        return false;
        //    }
        //}
    }
}