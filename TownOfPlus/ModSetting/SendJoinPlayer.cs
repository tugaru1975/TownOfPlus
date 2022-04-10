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
        private static StartAction Action = new StartAction();
        private static StartAction StertAction = new StartAction();
        public static List<int> SendPlayerList = new List<int>();
        public static float count = 3.5f;
        public static void Prefix()
        {
            if (main.SendJoinPlayer.Value && AmongUsClient.Instance.AmHost)
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player == PlayerControl.LocalPlayer) continue;
                    var clientId = AmongUsClient.Instance.allClients.ToArray().Where(cd => cd.Character.PlayerId == player.PlayerId).FirstOrDefault().Id;
                    if (!SendPlayerList.Contains(clientId))
                    {
                        Action.Run(() =>
                        {
                            count = HudManager.Instance.Chat.TimeSinceLastMessage < 3.0f ? 3.0f - HudManager.Instance.Chat.TimeSinceLastMessage : 0f;
                        });
                        if (player.name == player.Data.PlayerName)
                        {
                            StertAction.Run(() =>
                            {
                                new Timer(() =>
                                {
                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SendChat, SendOption.Reliable, clientId);
                                    writer.Write("*TownOfPlusによる自動送信\n" + main.SetSendJoinChat.Value);
                                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"「{player.name}」にチャットを送りました");
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    SendPlayerList.Add(clientId);
                                    Action.Reset();
                                    StertAction.Reset();
                                }, count + 0.5f);
                            });
                        }
                        HudManager.Instance.Chat.TimeSinceLastMessage = 0;
                    }
                    else
                    {
                        Action.Reset();
                    }
                }
            }
        }
    }
    
}