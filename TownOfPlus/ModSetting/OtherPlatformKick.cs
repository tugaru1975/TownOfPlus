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
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public class GameStartManagerUpdatePatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                if (!main.OPkick.Value) return;
                foreach (InnerNet.ClientData p in AmongUsClient.Instance.allClients)
                {
                    if (p.PlatformData.Platform != Platforms.StandaloneEpicPC && p.PlatformData.Platform != Platforms.StandaloneSteamPC)
                    {
                        AmongUsClient.Instance.KickPlayer(p.Id, false);
                    }
                }
            }
        }
    }
}