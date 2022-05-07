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
using System.Collections;
using System.Linq;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class SendChat
    {
        public static List<byte> SendPlayerList = new List<byte>();
        public static void Prefix()
        {
            if (main.SendJoinPlayer.Value && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player == null || player.Data == null) return;
                    if (player == PlayerControl.LocalPlayer) continue;
                    if (!SendPlayerList.Contains(player.PlayerId))
                    {
                        if (player.name == player.Data.PlayerName && HudManager.Instance.Chat.TimeSinceLastMessage >= 3.0f)
                        {
                            HudManager.Instance.Chat.TimeSinceLastMessage = 0f;
                            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                            {
                                SendPlayerList.Add(p.PlayerId);
                            }
                            
                            PlayerControl.LocalPlayer.RpcSendChat("※TownOfPlusによる自動送信\n" + main.SetSendJoinChat.Value);
                        }
                    }
                }
            }
            else
            {
                SendPlayerList = new List<byte>();
            }
        }
    }
}