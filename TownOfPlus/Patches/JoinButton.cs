using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using Hazel;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnhollowerBaseLib;
using System.Text.RegularExpressions;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(JoinGameButton), nameof(JoinGameButton.OnClick))]
    class JoinButton
    {
        public static void Prefix(JoinGameButton __instance)
        {
            var text = __instance.GameIdText.text;
            if (Regex.IsMatch(GUIUtility.systemCopyBuffer, @"[A-Z]{6}"))
            {
                if (text == "") __instance.GameIdText.SetText(GUIUtility.systemCopyBuffer);
            }
            if (__instance.GameIdText.text.Length == 5)
            {
                StringNames n = DestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
                if (n == StringNames.ServerNA) __instance.GameIdText.SetText(text + "G");
                if (n == StringNames.ServerEU) __instance.GameIdText.SetText(text + "F");
                if (n == StringNames.ServerAS) __instance.GameIdText.SetText(text + "F");
            }
        }
    }
}