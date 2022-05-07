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
    public static class ResetDoubleName
    {
        public static void Prefix(GameStartManager __instance)
        {
            if (PlayerControl.LocalPlayer == null) return;
            if (AmongUsClient.Instance.AmHost)
            {
                if (main.DoubleName.Value && (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.GameMode != GameModes.FreePlay))
                {
                    PlayerControl.LocalPlayer.RpcSetName($"{main.SetDoubleName.Value}\n<color=#FFFFFF50>{SaveManager.PlayerName}</color>\n");
                    CreateFlag.NewFlag("DoubleName");
                }
                else
                {
                    ResetName();
                }
            }
        }
        private static void ResetName()
        {
            CreateFlag.Run(() =>
            {
                PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
            }, "DoubleName");
        }
    }
}
