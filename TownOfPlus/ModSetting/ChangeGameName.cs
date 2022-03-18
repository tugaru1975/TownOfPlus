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
    public static class ChangeGameName
    {
        public static bool resetflag = false;
        public static bool flag = false;
        public static string name = "";
        public static void Prefix(GameStartManager __instance)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (main.ChangeGameName.Value)
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                { 
                    flag = true;
                    ResetName();
                }
                if (flag == false) return;
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    name = SaveManager.PlayerName;
                    SaveManager.PlayerName = main.SetGameName.Value;
                    PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
                    flag = false;
                    resetflag = true;
                }
            }
            else ResetName();
        }
        private static void ResetName()
        {
            if (resetflag == true && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
            {
                SaveManager.PlayerName = name;
                PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
                resetflag = false;
            }
        }
    }
}
