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
            if (AmongUsClient.Instance.AmHost && main.OPkick.Value)
            {
                var args = main.SetOPkick.Value.Split(',');
                foreach (InnerNet.ClientData p in AmongUsClient.Instance.allClients)
                {
                    if (p.Id == AmongUsClient.Instance.ClientId) continue;
                    for (int i = 0; i < args.Length - 1; i++)
                    {
                        if (p.PlatformData.Platform == (Platforms)Enum.ToObject(typeof(Platforms), int.Parse(args[i])))
                        {
                            AmongUsClient.Instance.KickPlayer(p.Id, false);
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{(Platforms)Enum.ToObject(typeof(Platforms), int.Parse(args[i]))}");
                            break;
                        }
                    }
                }
            }
        }
    }
}