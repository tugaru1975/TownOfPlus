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
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class EndGame
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            if (ShipStatus.Instance == null) return;
            if (Input.GetKeyDown(KeyCode.Return) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.LeftShift) && AmongUsClient.Instance.AmHost)
            {
                 ShipStatus.Instance.enabled = false;
                 ShipStatus.RpcEndGame(GameOverReason.ImpostorByKill, false);
            }
        }
    }
}
