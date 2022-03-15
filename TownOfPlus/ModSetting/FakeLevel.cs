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
    //フェイクレベル
    public class FakeLevel
    {
        public static uint Level = SaveManager.PlayerLevel;
        public static void Postfix(HudManager __instance)
        {
            if (main.FakeLevel.Value)
            {
                var rand = new System.Random();
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined)
                {
                    //設定したレベルの数 + 1 される
                    var count = main.SetLevel - 1;
                    if (count == 100)
                    {
                        Level = (uint)rand.Next(20, 70);
                    }
                    else
                    {
                        Level = (uint)count;
                    }
                }
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    PlayerControl.LocalPlayer.RpcSetLevel(Level);
                }
            }
            else
            {
                Level = SaveManager.PlayerLevel;
            }
        }
    }
}
