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
        public static bool flag = false;
        private static StartAction StartAction = new StartAction();
        public static string name = "";
        public static void Prefix(GameStartManager __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                if (main.ChangeGameName.Value)
                {
                    if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                    {
                        StartAction.Reset();
                        ResetName();
                    }

                    if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started || AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                    {
                        StartAction.Run(() =>
                        {
                            name = SaveManager.PlayerName;
                            SaveManager.PlayerName = main.SetGameName.Value;
                            PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
                            flag = true;
                        });
                    }
                }
                else ResetName();
            }
        }
        private static void ResetName()
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
            {
                if (flag)
                {
                    SaveManager.PlayerName = name;
                    PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
                }
            }
        }
    }
}
