using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using Hazel;
using System;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnhollowerBaseLib;
using System.Threading.Tasks;
using System.Threading;

namespace TownOfPlus
{
    class Chat
    {
        //Control+Zで一個戻す Control+Yで一個取り消し
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        public static class UndoAndRedo
        {
            public static List<string> List = new List<string>(); 
            public static string Text = "text";
            public static int count = 1;
            public static bool UndoRedo = false;
            public static void Prefix(ChatController __instance)
            {
                if (!HudManager.Instance.Chat.IsOpen) return;
                if (__instance.TextArea.text != Text)
                {
                    Text = __instance.TextArea.text;
                    if (UndoRedo)
                    {
                        List.RemoveRange(count + 1, List.Count - (count + 1));
                        UndoRedo = false;
                    }
                    List.Add(Text);
                    count = List.Count - 1;
                }
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
                {
                    if (count != 0)
                    {
                        UndoRedo = true;
                        count -= 1;
                        __instance.TextArea.SetText(List[count]);
                        Text = __instance.TextArea.text;
                    }
                }
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
                {
                    if (count != List.Count - 1)
                    {
                        UndoRedo = true;
                        count += 1;
                        __instance.TextArea.SetText(List[count]);
                        Text = __instance.TextArea.text;
                    }
                }
            }
        }
        //Control+Cでコピー　Control+Xでカット
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        public static class CopyAndCut
        {
            public static void Prefix(ChatController __instance)
            {
                if (!HudManager.Instance.Chat.IsOpen) return;
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X))
                {
                    GUIUtility.systemCopyBuffer = __instance.TextArea.text;
                    __instance.TextArea.SetText("");
                }
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
                {
                    GUIUtility.systemCopyBuffer = __instance.TextArea.text;
                }
            }
        }
        private static string text = "";
        private static bool CopyAndPaste = false;
        //Control+Vでペースト
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        public static class Paste
        {
            public static void Prefix(ChatController __instance)
            {
                if (!HudManager.Instance.Chat.IsOpen) return;
                if (Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftControl))
                {
                    bool Shift = false;
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) Shift = true;
                    //コピーしてあるのを調べる
                    var CopyWord = GUIUtility.systemCopyBuffer;
                    //Banされる文字の削除
                    CopyWord = CopyWord.Replace("<", "").Replace(">", "").Replace("\r", "");
                    //改行の削除
                    if (!Shift) CopyWord = CopyWord.Replace("\n", "");
                    //コピーしてある文字数
                    int CopyWordCount = CopyWord.Length;
                    //クールダウン確認
                    float num = 3f - __instance.TimeSinceLastMessage;

                    if (num <= 0.0f)
                    {
                        //文字数判定
                        if (CopyWordCount <= 120)
                        {
                            if (Shift)
                            { 
                                //コマンドを対応させる
                                text = (CopyWord);
                                CopyAndPaste = true;
                                SendChat.Addedchat(__instance);
                            }
                            else
                            {
                                __instance.TextArea.SetText(__instance.TextArea.text + CopyWord);
                            }

                        }
                        else
                        {
                            //文字数多いときに送る
                            __instance.AddChat(PlayerControl.LocalPlayer, "ERROR:文字数は120文字までです");
                        }
                    }
                }
            }
        }
        //チャット送られた時にClassSendChatに送る
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        class ChatCommand
        {
            public static bool Prefix(ChatController __instance)
            {
                bool chat = false;
                text = __instance.TextArea.text;
                SendChat.Addedchat(__instance);
                return !chat;
            }
        }
        //コマンドが含まれているかの判断
        class SendChat
        {
            public static int LobbyLimit = 15;
            public static bool Addedchat(ChatController __instance)
            {
                var canceled = false;
                var argsText = "";
                argsText = text;
                argsText += " ";
                //コマンドかどうか
                if (text.Substring(0, 1) == "/")
                {
                    canceled = true;
                }
                string[] args = argsText.Split(' ');
                var AddChat = "";
                var RpcSendChat = "";
                var SetText = "";
                var Command0 = "";
                var Command1 = args[1];
                Command0 = args[0];
                //小文字化
                Command0 = Command0.ToLower();
                Command1 = Command1.ToLower();
                switch (Command0)
                {
                    case "/help":
                        AddChat =
                            ("/LobbySetting(LS) : この部屋の設定を表示" +
                            "\n/ShowPlatform(SP) : 参加者の機種の表示");

                        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost)
                        {
                            AddChat += ("\n/LobbyMaxPlayer(LMP) [人数(4~15)] : 部屋の最大人数の変更");
                        }
                        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost)
                        {
                            AddChat += ("\n/Kick [名前] : Kickできます");
                        }
                        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost)
                        {
                            AddChat += ("\n/Ban [名前] : banできます");
                        }
                        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                            && (AmongUsClient.Instance.AmHost || AmongUsClient.Instance.GameMode == GameModes.FreePlay))
                        {
                            AddChat += ("\n/Name [変更したい名前] : 名前を変えられます\n※部屋名は立て直してから変わります");
                        }
                        if (PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                        {
                            AddChat += ("\n/Tp [名前] : 特定の人にテレポートします");
                        }
                        if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                        {
                            AddChat += ("\n/Tpme [名前] : 特定の人を自分にテレポートします");
                        }
                        if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                        {
                            AddChat += ("\n/Kill [名前] : 指定した人をキルします");
                        }
                        if (main.RandomMaps.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                        {
                            AddChat += ("\n/RandomMap(RM) [マップ名] : 選択したものからランダムにマップを選ぶ" +
                                        "\n===マップコマンド一覧===" +
                                        "\nマップ名 [コマンド]" +
                                        "\nTheSkeld [Skeld(S)]" +
                                        "\nMIRA HQ [MIRA(MH)]" +
                                        "\nPolus [Polus(P)]" +
                                        "\nAirShip [AirShip(AS)]");
                        }
                        if (main.HideLobbyCodes.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                        {
                            AddChat += ("\n/ChangeLobbyCode(CLC) : ロビーコードの名前を変更できます");
                        }
                        if (main.FakeLevel.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                        {
                            AddChat += ("\n/FakeLevel(FL) [数(1~100)] : レベルを指定出来ます");
                        }
                        break;

                    case "/ls":
                    case "/lobbysetting":
                        var MapName = "";
                        var ConfirmImpostor = "";
                        var AnonymousVotes = "";
                        var KillDistance = "";
                        var TaskBarMode = "";
                        if (PlayerControl.GameOptions.MapId == 0) MapName = "The Skeld";
                        if (PlayerControl.GameOptions.MapId == 1) MapName = "MIRA HQ";
                        if (PlayerControl.GameOptions.MapId == 2) MapName = "Polus";
                        if (PlayerControl.GameOptions.MapId == 3) MapName = "Dleks";
                        if (PlayerControl.GameOptions.MapId == 4) MapName = "AirShip";
                        if (PlayerControl.GameOptions.ConfirmImpostor) ConfirmImpostor = "オン";
                        else ConfirmImpostor = "オフ";
                        if (PlayerControl.GameOptions.AnonymousVotes) AnonymousVotes = "オン";
                        else AnonymousVotes = "オフ";
                        if (PlayerControl.GameOptions.KillDistance == 0) KillDistance = "ショート";
                        if (PlayerControl.GameOptions.KillDistance == 1) KillDistance = "ミドル";
                        if (PlayerControl.GameOptions.KillDistance == 2) KillDistance = "ロング";
                        if (((int)PlayerControl.GameOptions.TaskBarMode) == 0) TaskBarMode = "常時";
                        if (((int)PlayerControl.GameOptions.TaskBarMode) == 1) TaskBarMode = "会議";
                        if (((int)PlayerControl.GameOptions.TaskBarMode) == 2) TaskBarMode = "行わない";

                        AddChat =
                            ($"この部屋の設定\n" +
                            $"マップ名:{MapName}\n" +
                            $"インポスターの数:{PlayerControl.GameOptions.NumImpostors}\n" +
                            $"追放を確認:{ConfirmImpostor}\n" +
                            $"緊急会議:{PlayerControl.GameOptions.NumEmergencyMeetings}\n" +
                            $"匿名投票:{AnonymousVotes}\n" +
                            $"緊急ボタンクールダウン:{PlayerControl.GameOptions.EmergencyCooldown}秒\n" +
                            $"議論タイム:{PlayerControl.GameOptions.DiscussionTime}秒\n" +
                            $"投票タイム:{PlayerControl.GameOptions.VotingTime}秒\n" +
                            $"プレイヤーの速度:{PlayerControl.GameOptions.PlayerSpeedMod}x\n" +
                            $"クルービジョン:{PlayerControl.GameOptions.CrewLightMod}x\n" +
                            $"インポスタービジョン:{PlayerControl.GameOptions.ImpostorLightMod}x\n" +
                            $"キルのクールダウン:{PlayerControl.GameOptions.KillCooldown}秒\n" +
                            $"キル可能距離:{KillDistance}\n" +
                            $"タスクバーアップデート:{TaskBarMode}\n" +
                            $"通常タスク:{PlayerControl.GameOptions.NumCommonTasks}\n" +
                            $"ロングタスク:{PlayerControl.GameOptions.NumLongTasks}\n" +
                            $"ショートタスク:{PlayerControl.GameOptions.NumShortTasks}\n");
                        break;

                    case "/lmp":
                    case "/lobbymaxplayer":
                        if (!(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                            && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan())) break;
                        if (!Int32.TryParse(args[1], out LobbyLimit))
                        {
                            AddChat = ("/LobbyMaxPlayer(LMP) [人数(4~15)]");
                            SetText = args[0];
                        }
                        else
                        {
                            LobbyLimit = Math.Clamp(LobbyLimit, 4, 15);
                            if (LobbyLimit != PlayerControl.GameOptions.MaxPlayers)
                            {
                                PlayerControl.GameOptions.MaxPlayers = LobbyLimit;
                                DestroyableSingleton<GameStartManager>.Instance.LastPlayerCount = LobbyLimit;
                                PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);
                                AddChat = ($"最大人数が[{LobbyLimit}]人になりました");
                            }
                            else
                            {
                                AddChat = ("最大人数が同じです");
                                SetText = args[0];
                            }
                        }
                        break;

                    case "/rm":
                    case "/randommap":
                        if (!(main.RandomMaps.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)) break;
                        if (args.Length < 3)
                        {
                            RandomMap(__instance);
                            break;
                        }
                        switch (Command1)
                        {
                            case "s":
                            case "skeld":
                                main.AddTheSkeld = !main.AddTheSkeld;
                                RandomMap(__instance);
                                break;

                            case "mh":
                            case "mira":
                                main.AddMIRAHQ = !main.AddMIRAHQ;
                                RandomMap(__instance);
                                break;

                            case "o":
                            case "polus":
                                main.AddPolus = !main.AddPolus;
                                RandomMap(__instance);
                                break;

                            case "as":
                            case "airship":
                                main.AddAirShip = !main.AddAirShip;
                                RandomMap(__instance);
                                break;
                            default:
                                AddChat = ("/RandomMap(RM) [マップ名]\n" +
                                            "===マップコマンド一覧===\n" +
                                            "マップ名 [コマンド]\n" +
                                            "TheSkeld [Skeld(S)]\n" +
                                            "MIRA HQ [MIRA(MH)]\n" +
                                            "Polus [Polus(P)]\n" +
                                            "AirShip [AirShip(AS)]");
                                SetText = args[0];
                                break;
                        }
                        break;

                    case "/clc":
                    case "/changelobbycode":
                        if (!(main.HideLobbyCodes.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)) break;
                        if (args.Length > 2)
                        {
                            main.LobbyCode = args[1];                        

                        }
                        else
                        {
                            main.LobbyCode = main.Name;
                        }
                        AddChat = ($"コードが[{main.LobbyCode}]になりました");
                        break;

                    case "/fl":
                    case "/fakelevel":
                        if (!(main.FakeLevel.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)) break;
                        if (Int32.TryParse(args[1], out main.SetLevel))
                        {
                            LobbyLimit = Math.Clamp(main.SetLevel, 1, 100);
                            AddChat = ($"レベルが[{main.SetLevel}]になりました");
                            break;
                        }
                        main.SetLevel = 101;
                        AddChat = ("レベルがランダムに変更されました\n" +
                                   "/FakeLevel(FL) [数(1~100)] でレベルを変更します");
                        break;
                    case "/kick":
                        if (!(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost && (args.Length > 2))) break;
                        string playerkickName = text.Substring(args[0].Length + 1);
                        PlayerControl kicktarget = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(playerkickName));
                        if (kicktarget != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                        {
                            var Kickclient = AmongUsClient.Instance.GetClient(kicktarget.OwnerId);
                            if (Kickclient != null)
                            {
                                AmongUsClient.Instance.KickPlayer(Kickclient.Id, false);
                                AddChat = playerkickName + "をKickしました";
                            }
                        }
                        break;

                    case "/ban":
                        if (!(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && (args.Length > 2))) break;
                        string playerbanName = text.Substring(args[0].Length + 1);
                        PlayerControl bantarget = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(playerbanName));
                        if (bantarget != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                        {
                            var Banclient = AmongUsClient.Instance.GetClient(bantarget.OwnerId);
                            if (Banclient != null)
                            {
                                AmongUsClient.Instance.KickPlayer(Banclient.Id, true);
                                AddChat = playerbanName + "をbanしました";
                            }
                        }
                        break;

                    case "/tp":
                        if (!(PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameMode == GameModes.FreePlay && (args.Length > 2))) break;
                        string TpplayerName = text.Substring(args[0].Length + 1).ToLower();
                        PlayerControl Tptarget = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.ToLower().Equals(TpplayerName));
                        if (Tptarget != null)
                        {
                            PlayerControl.LocalPlayer.transform.position = Tptarget.transform.position;
                            AddChat = TpplayerName + "にテレポートしました";
                        }
                        break;
                    case "/tpme":
                        if (!(PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameMode == GameModes.FreePlay && (args.Length > 2))) break;
                        string TpmeplayerName = text.Substring(args[0].Length + 1).ToLower();
                        PlayerControl Tpmetarget = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.ToLower().Equals(TpmeplayerName));
                        if (Tpmetarget != null)
                        {
                            Tpmetarget.transform.position = PlayerControl.LocalPlayer.transform.position;
                            AddChat = TpmeplayerName + "がテレポートしました";
                        }
                        break;

                    case "/name":
                        if (!(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                            && (AmongUsClient.Instance.AmHost || AmongUsClient.Instance.GameMode == GameModes.FreePlay))) break;
                        SaveManager.PlayerName = text.Substring(args[0].Length + 1);
                        AddChat = ($"名前が[{SaveManager.PlayerName}]になりました");
                        PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
                        break;

                    case "/kill":
                        if (!(AmongUsClient.Instance.GameMode == GameModes.FreePlay && (args.Length > 2))) break;
                        string killplayerName = text.Substring(args[0].Length + 1).ToLower();
                        PlayerControl killtarget = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.ToLower().Equals(killplayerName));
                        if (killtarget != null)
                        {
                            killtarget.RpcMurderPlayer(killtarget);
                            AddChat = killplayerName + "をキルしました";
                        }
                        break;

                    case "/sp":
                    case "/showplatform":
                        foreach (InnerNet.ClientData p in AmongUsClient.Instance.allClients)
                        {
                            var Platform = $"{p.PlatformData.Platform}";
                            AddChat += $"{p.PlayerName} : {Platform.Replace("Standalone", "")}\n";
                        }
                        break;
                    default:
                        break;
                }

                //コマンドのときは送らない
                if (CopyAndPaste && !canceled)
                {
                    RpcSendChat = (text);
                    CopyAndPaste = false;
                }
                //自分のみ
                if (AddChat != "")
                {
                    __instance.AddChat(PlayerControl.LocalPlayer, AddChat);
                }
                //全員見える
                if (RpcSendChat != "")
                {
                    __instance.TimeSinceLastMessage = 0.0f;
                    PlayerControl.LocalPlayer.RpcSendChat(RpcSendChat);
                }
                if (canceled)
                {
                    __instance.TextArea.Clear();
                    __instance.quickChatMenu.ResetGlyphs();
                }
                if (SetText != "")
                {
                    __instance.TimeSinceLastMessage = 2.9f;
                    __instance.TextArea.SetText(SetText + " ");
                }
                text = "";
                return !canceled;
            }
            public static void RandomMap(ChatController __instance)
            {
                __instance.AddChat(PlayerControl.LocalPlayer,
                ($"マップ名 : 設定\n" +
                 $"TheSkeld : {(main.AddTheSkeld ? "ON" : "OFF")}\n" +
                 $"MIRAHQ : {(main.AddMIRAHQ ? "ON" : "OFF")}\n" +
                 $"Polus : {(main.AddPolus ? "ON" : "OFF")}\n" +
                 $"AirShip : {(main.AddAirShip ? "ON" : "OFF")}"));
            }
        }
    }
    //フリープレイ時にチャットを表示する
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class EnableChat
    {
        public static void Postfix(HudManager __instance)
        {
            if (__instance?.Chat?.isActiveAndEnabled == false && (AmongUsClient.Instance?.GameMode == GameModes.FreePlay))
                __instance?.Chat?.SetVisible(true);
        }
    }
    //フリーチャットの有効化
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class FreeChat
    {
        public static void Postfix(HudManager __instance)
        {
            SaveManager.chatModeType = 1;
            SaveManager.isGuest = false;
            SaveManager.ChatModeType = InnerNet.QuickChatModes.FreeChatOrQuickChat;
        }
    }
}
