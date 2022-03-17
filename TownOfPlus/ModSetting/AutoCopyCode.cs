using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnhollowerBaseLib;
using Hazel;
using System;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class AutoCopyCode
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (main.AutoCopyCode.Value)
            {
                GUIUtility.systemCopyBuffer = InnerNet.GameCode.IntToGameName(AmongUsClient.Instance.GameId);
            }
        }
    }
    
}
