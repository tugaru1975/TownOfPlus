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
    //フェイクレベル
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public class FakeLevel
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (PlayerControl.LocalPlayer == null) return;
            
            if (main.FakeLevel.Value)
            {
                PlayerControl.LocalPlayer.RpcSetLevel((uint)new System.Random().Next(0,99));
            }
        }
    }
}
