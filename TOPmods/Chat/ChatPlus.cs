using HarmonyLib;
using InnerNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


namespace TownOfPlus
{
    class Chat
    {
        public static PlayerControl SpectatePlayer = null;
        public static readonly string SkinDataPass = main.TOPUrl + @"SkinData\SkinData.csv";

        //チャット送られた時にClassSendChatに送る
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        class ChatCommand
        {
            public static void Prefix(ChatController __instance)
            {
                if (!main.ChatCommand.Getbool()) return;
                SendChat.Addchat(__instance, __instance.TextArea.text);
            }
        }

        //Tabでコマンド補完
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        class TabChatCommand
        {
            public static string ChatText;
            public static int Count = -1;
            public static void Postfix(ChatController __instance)
            {
                if (!main.ChatCommand.Getbool() || !main.ComTab.Getbool()) return;
                if (!CommandList.AllCommand.Any(a => a.Command.Contains(__instance.TextArea.text, StringComparison.OrdinalIgnoreCase)))
                {
                    ChatText = __instance.TextArea.text;
                }
                if (Input.GetKeyDown(KeyCode.Tab) && ChatText.FirstOrDefault() == TextPlus.ComWord)
                {
                    var ChatCommand = new List<string>();
                    var args = ChatText.Split(' ');
                    foreach (var List in CommandList.AllCommand)
                    {
                        if (!List.Terms) continue;
                        if (ChatText.Equals(List.Command[0].TrySubstring(0, ChatText.Length), StringComparison.OrdinalIgnoreCase)
                            || CommandList.AllCommand.Any(a => a.Command.Contains(ChatText)))
                        {
                            ChatCommand.Add(List.Command[0]);
                        }
                        else
                        {
                            if (List.Command.Contains(args[0], StringComparison.OrdinalIgnoreCase))
                            {
                                var Command = args[0] + " ";
                                ChatCommand = List.Type switch
                                {
                                    CommandText.Name => PlayerControl.AllPlayerControls.ToArray().Select(s => Command + s.Data.PlayerName.RemoveHTML()).ToList(),
                                    CommandText.FileName => Helpers.TryGetFileText(SkinDataPass, out var list) ? list.Select(s => Command + s[0]).ToList() : ChatCommand,
                                    CommandText.Reset => new() { Command + "Reset" },
                                    CommandText.All => new() { Command + "All" },
                                    _ => ChatCommand,
                                };
                            }
                        }
                    }
                    if (ChatCommand.Count == 0) return;
                    IsChange.Run(() =>
                    {
                        Count = -1;
                    }, string.Join("\n", ChatCommand), "TabCommand");
                    if (Count >= ChatCommand.Count - 1) Count = -1;
                    Count++;
                    __instance.TextArea.SetText(ChatCommand[Count]);
                }
            }
        }

        //コマンドが含まれているかの判断
        public static class SendChat
        {
            public static ValueTuple<byte, CommandTag> ComTuple = (byte.MaxValue, CommandTag.None);
            public static void Addchat(ChatController __instance, string text, bool CopyAndPasteMode = false)
            {
                var IsChatCommand = false;
                if (text == "") return;
                string[] args = text.Split(' ');
                var AddChat = "";
                bool SetCom = false;
                var IsSecCom = ComTuple.Item2 != CommandTag.None;

                if (text[0] == TextPlus.ComWord)
                {
                    var info = CommandList.AllCommand.FirstOrDefault(f => f.Command.Contains(args[0], StringComparison.OrdinalIgnoreCase) && f.Terms);
                    if (info != null && !IsSecCom || ComTuple.Item2 == info?.Tag)
                    {
                        IsChatCommand = true;
                        switch (info.Tag)
                        {
                            case CommandTag.Help:
                                if (args.Length > 1)
                                {
                                    if (args[1].Equals("All", StringComparison.OrdinalIgnoreCase))
                                    {
                                        AddChat = CommandList.GetHelpText(true);
                                        break;
                                    }
                                }
                                AddChat = CommandList.GetHelpText();
                                break;

                            case CommandTag.LobbyMaxPlayer:
                                if (args.Length > 1)
                                {
                                    if (int.TryParse(args[1], out var LobbyLimit))
                                    {
                                        LobbyLimit = Math.Clamp(LobbyLimit, 4, 15);
                                        if (LobbyLimit != PlayerControl.GameOptions.MaxPlayers)
                                        {
                                            PlayerControl.GameOptions.MaxPlayers = LobbyLimit;
                                            DestroyableSingleton<GameStartManager>.Instance.LastPlayerCount = LobbyLimit;
                                            PlayerControl.GameOptions.SyncSettings();
                                            AddChat = $"最大人数が[{LobbyLimit}]人になりました";
                                        }
                                        else
                                        {
                                            AddChat = "最大人数が同じです";
                                            SetCom = true;
                                        }
                                        break;
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.Kick:
                                if (args.Length > 1)
                                {
                                    if (AmongUsClient.Instance != null)
                                    {
                                        if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                        {
                                            if (!pc.Equals(PlayerControl.LocalPlayer) && AmongUsClient.Instance.CanKick())
                                            {
                                                AmongUsClient.Instance.KickPlayer(AmongUsClient.Instance.GetClient(pc.OwnerId).Id, false);
                                                AddChat = playername + "をKickしました";
                                                break;
                                            }
                                        }
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.Ban:
                                if (args.Length > 1)
                                {
                                    if (AmongUsClient.Instance != null)
                                    {
                                        if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                        {
                                            if (!pc.Equals(PlayerControl.LocalPlayer) && AmongUsClient.Instance.CanBan())
                                            {
                                                AmongUsClient.Instance.KickPlayer(AmongUsClient.Instance.GetClient(pc.OwnerId).Id, true);
                                                AddChat = playername + "をbanしました";
                                                break;
                                            }
                                        }
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.Tp:
                                if (args.Length > 1)
                                {
                                    if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                    {
                                        if (!pc.Equals(PlayerControl.LocalPlayer))
                                        {
                                            PlayerControl.LocalPlayer.transform.position = pc.transform.position;
                                            AddChat = playername + "にテレポートしました";
                                            break;
                                        }
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.SpectatePlayer:
                                if (args.Length > 1)
                                {
                                    if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                    {
                                        if (!pc.Equals(PlayerControl.LocalPlayer))
                                        {
                                            SpectatePlayer = pc;
                                            AddChat = playername + "を観戦します";
                                            break;
                                        }
                                    }
                                }
                                if (SpectatePlayer != null)
                                {
                                    SpectatePlayer = null;
                                    AddChat = ("スペクテイターモードを解除しました");
                                    break;
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.Tpme:
                                if (args.Length > 1)
                                {
                                    if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                    {
                                        pc.transform.position = PlayerControl.LocalPlayer.transform.position;
                                        AddChat = playername + "がテレポートしました";
                                        break;
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.Kill:
                                if (args.Length > 1)
                                {
                                    if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                    {
                                        pc.RpcMurderPlayer(pc);
                                        AddChat = playername + "をキルしました";
                                        break;
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.Exiled:
                                if (args.Length > 1)
                                {
                                    if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                    {
                                        pc.Exiled();
                                        AddChat = playername + "を追放しました";
                                        break;
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.Revive:
                                if (args.Length > 1)
                                {
                                    if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                    {
                                        pc.Revive();
                                        AddChat = playername + "を生き返らせました";
                                        break;
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.CPS:
                                if (args.Length > 1)
                                {
                                    if (args[1].Equals("reset", StringComparison.OrdinalIgnoreCase))
                                    {
                                        main.CPSpositionX.Value = 0f;
                                        main.CPSpositionY.Value = 2.75f;
                                        AddChat = "CPSの位置をリセットしました";
                                        break;
                                    }
                                }
                                if (main.SettingCPS)
                                {
                                    main.SettingCPS = false;
                                    AddChat = "CPSの位置設定を無効化しました";
                                }
                                else
                                {
                                    main.SettingCPS = true;
                                    AddChat = $"CPSの位置設定を有効化しました\n十字キーかマウスの右クリックで動かしてください\n位置リセットは[{info.Command[0]} Reset]。";
                                }
                                break;

                            case CommandTag.DateTime:
                                if (args.Length > 1)
                                {
                                    if (args[1].Equals("reset", StringComparison.OrdinalIgnoreCase))
                                    {
                                        main.DateTimepositionX.Value = 0f;
                                        main.DateTimepositionY.Value = 2.75f;
                                        AddChat = "DateTimeの位置をリセットしました";
                                        break;
                                    }
                                }
                                if (main.SettingDateTime)
                                {
                                    main.SettingDateTime = false;
                                    AddChat = "DateTimeの位置設定を無効化しました";
                                }
                                else
                                {
                                    main.SettingDateTime = true;
                                    AddChat = $"DateTimeの位置設定を有効化しました\n十字キーかマウスの右クリックで動かしてください\n位置リセットは[{info.Command[0]} Reset]。";
                                }
                                break;

                            case CommandTag.SaveSkin:
                                if (args.Length > 1)
                                {
                                    var SkinName = args[1].TrimAll(",");
                                    if (Helpers.TryGetFileText(SkinDataPass, out var list) && list.Any(a => a[0].Equals(SkinName)))
                                    {
                                        AddChat = $"「{SkinName}」はすでに使われている名前です";
                                        SetCom = true;
                                        break;
                                    }

                                    var p = PlayerControl.LocalPlayer.CurrentOutfit;
                                    var data = new string[]
                                    {
                                        SkinName,
                                        p.HatId,
                                        p.VisorId,
                                        p.SkinId,
                                        p.PetId,
                                        p.NamePlateId,
                                    };
                                    using StreamWriter sw = new(SkinDataPass, true);
                                    sw.WriteLine(string.Join(",", data));
                                    AddChat = $"「{SkinName}」を保存しました";
                                    break;
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.LoadSkin:
                                if (args.Length > 1)
                                {
                                    var SkinName = args[1].TrimAll(",");
                                    if (Helpers.TryGetFileText(SkinDataPass, out var list))
                                    {
                                        var index = list.FindIndex(f => f[0].Equals(SkinName));
                                        if (index != -1)
                                        {
                                            var values = list[index];
                                            var player = PlayerControl.LocalPlayer.CurrentOutfit;
                                            if (values[1] != player.HatId)
                                            {
                                                PlayerControl.LocalPlayer.RpcSetHat(values[1]);
                                                SaveManager.LastHat = values[1];
                                            }
                                            if (values[2] != player.VisorId)
                                            {
                                                PlayerControl.LocalPlayer.RpcSetVisor(values[2]);
                                                SaveManager.LastVisor = values[2];
                                            }
                                            if (values[3] != player.SkinId)
                                            {
                                                PlayerControl.LocalPlayer.RpcSetSkin(values[3]);
                                                SaveManager.LastSkin = values[3];
                                            }
                                            if (values[4] != player.PetId)
                                            {
                                                PlayerControl.LocalPlayer.RpcSetPet(values[4]);
                                                SaveManager.LastPet = values[4];
                                            }
                                            if (values[5] != player.NamePlateId)
                                            {
                                                PlayerControl.LocalPlayer.RpcSetNamePlate(values[5]);
                                                SaveManager.LastNamePlate = values[5];
                                            }
                                            AddChat = $"「{SkinName}」にスキンを変更しました";
                                        }
                                        else
                                        {
                                            AddChat = $"「{SkinName}」が見つかりませんでした";
                                            SetCom = true;
                                        }
                                        break;
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.DeleteSkin:
                                if (args.Length > 1)
                                {
                                    var SkinName = args[1].TrimAll(",");
                                    if (Helpers.TryGetFileText(SkinDataPass, out var list))
                                    {
                                        var Data = list.Where(c => !c[0].Equals(SkinName));
                                        if (list.Count > Data.Count())
                                        {
                                            using StreamWriter sw = new(SkinDataPass);
                                            foreach (var line in Data)
                                            {
                                                sw.WriteLine(string.Join(',', line));
                                            }
                                            AddChat = $"「{SkinName}」を削除しました";
                                        }
                                        else
                                        {
                                            AddChat = $"「{SkinName}」が見つかりませんでした";
                                            SetCom = true;
                                        }
                                        break;
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.DirectMessage:
                                if (args.Length > 1)
                                {
                                    if (args[1].Equals("cancel", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (ComTuple.Item2 == CommandTag.DirectMessage)
                                        {
                                            ResetComTuple();
                                            AddChat = "DMをキャンセルしました";
                                            break;
                                        }
                                    }
                                    if (AmongUsClient.Instance != null)
                                    {
                                        if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                        {
                                            ComTuple = (pc.PlayerId, CommandTag.DirectMessage);
                                            AddChat = playername + $"へのメッセージを入力してください\n※キャンセルは[{info.Command[0]} Cancel";
                                            break;
                                        }
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.FPS:
                                if (args.Length > 1)
                                {
                                    if (args[1].Equals("reset", StringComparison.OrdinalIgnoreCase))
                                    {
                                        main.FPSpositionX.Value = 0f;
                                        main.FPSpositionY.Value = 2.75f;
                                        AddChat = "FPSの位置をリセットしました";
                                        break;
                                    }
                                }
                                if (main.SettingFPS)
                                {
                                    main.SettingFPS = false;
                                    AddChat = "FPSの位置設定を無効化しました";
                                }
                                else
                                {
                                    main.SettingFPS = true;
                                    AddChat = $"FPSの位置設定を有効化しました\n十字キーかマウスの右クリックで動かしてください\n位置リセットは[{info.Command[0]} Reset]。";
                                }
                                break;

                            case CommandTag.Platform:
                                var chat = "";
                                foreach (ClientData client in AmongUsClient.Instance.allClients)
                                {
                                    chat += $"{client.PlayerName.RemoveHTML()} {$"{client.PlatformData.Platform}".TrimAll("Standalone")}\n";
                                }
                                AddChat = chat;
                                break;

                            case CommandTag.Color:
                                if (args.Length > 1)
                                {
                                    if (args[1].Equals("cancel", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (ComTuple.Item2 == CommandTag.Color)
                                        {
                                            ResetComTuple();
                                            AddChat = "色の変更をキャンセルしました";
                                            break;
                                        }
                                    }
                                    if (Helpers.TryNamePlayer(text[(args[0].Length + 1)..], out var playername, out var pc))
                                    {
                                        ComTuple = (pc.PlayerId, CommandTag.Color);
                                        var colortext = "";
                                        for (int i = 0; i < Palette.ColorNames.Length; i++)
                                        {
                                            var colorname = Palette.ColorNames[i].GetTranslation();
                                            colortext += colorname + $" [{colorname}({i})]" + "\n";
                                        }
                                        AddChat = playername + $"への変更色を入力してください\n" + colortext + $"※キャンセルは[{info.Command[0]} Cancel]"; ;
                                        break;
                                    }
                                }
                                AddChat = info.Help;
                                SetCom = true;
                                break;

                            case CommandTag.JoinText:
                                TextPlus.RpcSendChat(main.SetSendJoinChat.Getstring());
                                return;

                            default:
                                break;
                        }

                    }

                    if (main.ComCancel.Getbool()) IsChatCommand = true;
                }

                if (!IsChatCommand && IsSecCom)
                {
                    switch (ComTuple.Item2)
                    {
                        case CommandTag.DirectMessage:
                            if (ComTuple.Item1.TryGetClient(out var client))
                            {
                                AddChat = $"「{client.PlayerName.RemoveHTML()}」に\n" + text + "\nを送りました";
                                Helpers.DMChat(client, "※TownOfPlusによる個別送信", text);
                            }
                            break;

                        case CommandTag.Color:
                            if (ComTuple.Item1.TryGetPlayer(out var p))
                            {
                                int colorid = Palette.ColorNames.ToList().FindIndex(f => f.GetTranslation().Equals(text, StringComparison.OrdinalIgnoreCase));
                                if (int.TryParse(text, out var color) && color < Palette.ColorNames.Length)
                                {
                                    colorid = color;
                                }
                                if (colorid != -1)
                                {
                                    p.SetColor(colorid);
                                    AddChat = $"「{text}」に色を変更しました";
                                }
                                else
                                {
                                    AddChat = $"「{text}」の色が見つかりませんでした";
                                }
                            }
                            break;
                    }
                    if (AddChat == "") AddChat = "プレイヤーが見つかりませんでした";
                    ResetComTuple();
                    IsChatCommand = true;
                }

                //コマンドのときは送らない
                if (CopyAndPasteMode && !IsChatCommand)
                {
                    TextPlus.RpcSendChat(text);
                }
                //自分のみ
                if (AddChat != "")
                {
                    __instance.AddComChat(AddChat);
                }
                if (IsChatCommand)
                {
                    __instance.TextArea.Clear();
                    __instance.quickChatMenu.ResetGlyphs();
                }
                if (SetCom)
                {
                    __instance.TimeSinceLastMessage = 2.9f;
                    __instance.TextArea.SetText(args[0] + " ");
                }
            }

            public static void ResetComTuple()
            {
                ComTuple = (byte.MaxValue, CommandTag.None);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerSpectate
        {
            public static void Postfix()
            {
                if (GameState.IsMeeting || !GameState.IsShip) SpectatePlayer = null;
                if (SpectatePlayer != null) PlayerControl.LocalPlayer.transform.localPosition = SpectatePlayer.transform.localPosition;
            }
        }
    }
    class ChatCommandUI
    {
        public static bool IsChatCommand = false;

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        class SetLeft
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (IsChatCommand) __instance.SetLeft();
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        class SetTOPName
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (IsChatCommand) __instance.NameText.text = "TownOfPlus Command";
            }
        }
    }

    //フリープレイ時にチャットを表示する
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class EnableChat
    {
        public static void Postfix(HudManager __instance)
        {
            if (!GameState.IsChatActive && GameState.IsFreePlay && main.AlwaysChat.Getbool())
                __instance.Chat?.SetVisible(true);
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class ChatTextArea
    {
        public static void Postfix(ChatController __instance)
        {
            if (main.ChatLimitPlus.Getbool()) __instance.TextArea.characterLimit = 120;
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
    public static class MapCancel
    {
        public static void Postfix(MapBehaviour __instance)
        {
            if (main.CancelChatMap.Getbool() && GameState.IsMeeting && GameState.IsFocusChatArea) __instance.Close();
        }
    }
}
