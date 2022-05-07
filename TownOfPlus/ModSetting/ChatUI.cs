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
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetMaskLayer))]
    class ChatUI
    {
        public static void Postfix(ChatBubble __instance)
        {
            if (main.CrewColorChat.Value || main.TranslucentChat.Value)
            {
                try
                {
                    if (Helpers.playerById(__instance.playerInfo.PlayerId).Data.IsDead) return;
                    var color = Palette.White;
                    if (main.CrewColorChat.Value) color = Helpers.playerById(__instance.playerInfo.PlayerId).MyRend.material.GetColor("_BodyColor");
                    if (main.TranslucentChat.Value) color.a = (100f - main.SetTranslucentChat.Value) / 100f;
                    __instance.Background.color = color;
                }
                catch { }
            }
        }
    }
}