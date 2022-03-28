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
using System.Reactive.Linq;

namespace TownOfPlus
{
    class JoinChat
    {

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        class SendChat
        {
            public static List<int> JoinPlayerList = new List<int>();
            public static List<int> SendPlayerList = new List<int>();
            public static int Count = 50;
            public static bool flag = false;
            public static void Prefix()
            {
                if (main.SendJoinPlayer.Value && AmongUsClient.Instance.AmHost)
                {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        var clientId = AmongUsClient.Instance.allClients.ToArray().Where(cd => cd.Character.PlayerId == player.PlayerId).FirstOrDefault().Id;
                        if (!SendPlayerList.Contains(clientId))
                        {
                            if (!JoinPlayerList.Contains(clientId))
                            {
                                flag = true;
                                Count = 50;
                                JoinPlayerList.Add(clientId);
                            }
                            if (player != PlayerControl.LocalPlayer && Count == 0 && 3.5f <= HudManager.Instance.Chat.TimeSinceLastMessage)
                            {
                                flag = false;
                                HudManager.Instance.Chat.TimeSinceLastMessage = 0;
                                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SendChat, SendOption.Reliable, clientId);
                                writer.Write(main.SetSendJoinChat.Value);
                                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"「{player.name}」にチャットを送りました");
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                SendPlayerList.Add(clientId);
                            }
                        }
                    }
                    if (flag == true)
                    {
                        if (Count != 0)
                            Count -= 1;
                    }

                }
            }
        }
    }
}