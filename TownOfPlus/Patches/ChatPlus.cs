using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TownOfPlus
{
    class Chat
    {
        public static PlayerControl PlayerSpectatePlayer = null;

        //Shift + Backspace で全消し
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        public static class Delete
        {
            public static void Prefix(ChatController __instance)
            {
                if (!HudManager.Instance.Chat.IsOpen) return;
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Backspace))
                {
                    __instance.TextArea.SetText("");
                }
            }
        }
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

                        if (Shift)
                        {
                            //文字数判定
                            if (CopyWordCount <= 120)
                            {
                                //コマンドを対応させる
                                text = (CopyWord);
                                CopyAndPaste = true;
                                SendChat.Addchat(__instance);
                            }
                            else
                            {
                                //文字数多いときに送る
                                __instance.AddChat(PlayerControl.LocalPlayer, "ERROR:文字数は120文字までです");
                            }
                        }
                        else
                        {
                            if (CopyWordCount + __instance.TextArea.text.Length >= 100)
                            {
                                CopyWord = CopyWord.Substring(0, 100 - __instance.TextArea.text.Length);
                            }
                            __instance.TextArea.SetText(__instance.TextArea.text + CopyWord);
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
                SendChat.Addchat(__instance);
                return !chat;
            }
        }
        //コマンドが含まれているかの判断
        class SendChat
        {
            public static int LobbyLimit = 15;
            public static int LevelLimit = 100;
            public static int TranslucentName = 100;
            public static bool Addchat(ChatController __instance)
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
                var Command0 = args[0];
                var Command1 = args[1];

                //小文字化
                Command0 = Command0.ToLower();
                Command1 = Command1.ToLower();
                switch (Command0)
                {
                    case "/help":
                        AddChat = "===コマンド一覧===";
                        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay)
                        {
                            AddChat += ("\n/LobbyMaxPlayer(LMP) [人数(4~15)] : 部屋の最大人数の変更");
                            AddChat += ("\n/Kick [名前] : Kickできます");
                            AddChat += ("\n/Ban [名前] : banできます");
                        }
                        if (PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                        {
                            AddChat += ("\n/Tp [名前] : 特定の人にテレポートします");
                            AddChat += ("\n/SpectatePlayer [名前] : 特定の人を観戦します");
                        }
                        if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                        {
                            AddChat += ("\n/Tpme [名前] : 特定の人を自分にテレポートします");
                            AddChat += ("\n/Kill [名前] : 指定した人をキルします");
                            AddChat += ("\n/Exiled [名前] : 指定した人を追放します");
                            AddChat += ("\n/Revive [名前] : 指定した人を生きかえらせます");
                        }
                        if (main.ChangeGameName.Value &&
                            AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                            && (AmongUsClient.Instance.AmHost || AmongUsClient.Instance.GameMode == GameModes.FreePlay))
                        {
                            AddChat += ("\n/ChangeGameName(CGN) [変更したい名前(10文字)] : ゲーム中の名前を変えられます");
                        }
                        if (main.RandomMaps.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost)
                        {
                            AddChat += ("\n/RandomMap(RM) [マップ名] : 選択したものからランダムにマップを選ぶ" +
                                        "\n===マップコマンド一覧===" +
                                        "\nマップ名 [コマンド]" +
                                        "\nTheSkeld [Skeld(S)]" +
                                        "\nMIRA HQ [MIRA(MH)]" +
                                        "\nPolus [Polus(P)]" +
                                        "\nAirShip [AirShip(AS)]");
                        }
                        if (main.HideLobbyCodes.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.GameMode != GameModes.FreePlay)
                        {
                            AddChat += ("\n/ChangeLobbyCode(CLC) : ロビーコードの名前を変更できます");
                        }
                        if (main.FakeLevel.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.GameMode != GameModes.FreePlay)
                        {
                            AddChat += ("\n/FakeLevel(FL) [数(1~100)] : レベルを指定出来ます");
                        }
                        if (main.SendJoinPlayer.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay)
                        {
                            AddChat += ("\n/SendChat(SC) [送るチャット] : 指定したチャットを参加者に送ります");
                        }
                        if (main.DoubleName.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay)
                        {
                            AddChat += ("\n/DoubleName(DN) [変えたい名前] : 名前に二段目を追加します");
                        }
                        if (main.TranslucentName.Value)
                        {
                            AddChat += ("\n/TranslucentName(TN) [数値] : 名前の透明度を変更します");
                        }
                        if (AddChat == "===コマンド一覧===") AddChat += ("\n実行可能なコマンドはありません");
                        break;

                    case "/lmp":
                    case "/lobbymaxplayer":
                        if (!(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                            && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan() && AmongUsClient.Instance.GameMode != GameModes.FreePlay)) break;
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
                        if (!(main.RandomMaps.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay)) break;
                        if (args.Length < 3)
                        {
                            RandomMap(__instance);
                            break;
                        }
                        switch (Command1)
                        {
                            case "s":
                            case "skeld":
                                main.AddTheSkeld.Value = !main.AddTheSkeld.Value;
                                RandomMap(__instance);
                                break;

                            case "mh":
                            case "mira":
                                main.AddMIRAHQ.Value = !main.AddMIRAHQ.Value;
                                RandomMap(__instance);
                                break;

                            case "o":
                            case "polus":
                                main.AddPolus.Value = !main.AddPolus.Value;
                                RandomMap(__instance);
                                break;

                            case "as":
                            case "airship":
                                main.AddAirShip.Value = !main.AddAirShip.Value;
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
                        if (!(main.HideLobbyCodes.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.GameMode != GameModes.FreePlay)) break;
                        if (args.Length > 2)
                        {
                            main.SetLobbyCode.Value = args[1];

                        }
                        else
                        {
                            main.SetLobbyCode.Value = main.Name;
                        }
                        AddChat = ($"コードが[{main.SetLobbyCode.Value}]になりました");
                        break;

                    case "/fl":
                    case "/fakelevel":
                        if (!(main.FakeLevel.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.GameMode != GameModes.FreePlay)) break;
                        if (args.Length < 3)
                        {
                            AddChat = ($"偽のレベルは[{main.SetLevel.Value}]です"); ;
                            break;
                        }
                        if (Int32.TryParse(args[1], out LevelLimit))
                        {
                            LevelLimit = Math.Clamp(LevelLimit, 1, 100);
                            main.SetLevel.Value = LevelLimit;
                            AddChat = ($"レベルが[{main.SetLevel.Value}]になりました");
                            break;
                        }
                        main.SetLevel.Value = 101;
                        AddChat = ("レベルがランダムに変更されました\n" +
                                   "/FakeLevel(FL) [数(1~100)] でレベルを変更します");
                        break;

                    case "/kick":
                        if (!(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay && (args.Length > 2))) break;
                        string playerkickName = text.Substring(args[0].Length + 1);
                        PlayerControl kicktarget = Helpers.GetNamePlayer(playerkickName);
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
                        if (!(AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.GameMode != GameModes.FreePlay && (args.Length > 2))) break;
                        string playerbanName = text.Substring(args[0].Length + 1);
                        PlayerControl bantarget = Helpers.GetNamePlayer(playerbanName);
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
                        string TpplayerName = text.Substring(args[0].Length + 1);
                        PlayerControl Tptarget = Helpers.GetNamePlayer(TpplayerName);
                        if (Tptarget != null)
                        {
                            PlayerControl.LocalPlayer.transform.position = Tptarget.transform.position;
                            AddChat = TpplayerName + "にテレポートしました";
                        }
                        break;

                    case "/sp":
                    case "/spectateplayer":
                        if (!(PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameMode == GameModes.FreePlay)) break;
                        if (args.Length < 3)
                        {
                            PlayerSpectatePlayer = null;
                            AddChat = ("スペクテイターモードを解除しました");
                            break;
                        }
                        string psplayerName = text.Substring(args[0].Length + 1);
                        PlayerControl pstarget = Helpers.GetNamePlayer(psplayerName);
                        if (pstarget != null)
                        {
                            PlayerSpectatePlayer = pstarget;
                            AddChat = psplayerName + "を観戦します";
                            break;
                        }
                        SetText = args[0];
                        break;

                    case "/tpme":
                        if (!(PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameMode == GameModes.FreePlay && (args.Length > 2))) break;
                        string TpmeplayerName = text.Substring(args[0].Length + 1);
                        PlayerControl Tpmetarget = Helpers.GetNamePlayer(TpmeplayerName);
                        if (Tpmetarget != null)
                        {
                            Tpmetarget.transform.position = PlayerControl.LocalPlayer.transform.position;
                            AddChat = TpmeplayerName + "がテレポートしました";
                        }
                        break;

                    case "/cgn":
                    case "/changegamename":
                        if (!(main.ChangeGameName.Value &&
                            AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started
                            && AmongUsClient.Instance.AmHost)) break;
                        if (10 < args[1].Length)
                        {
                            AddChat = ("/ChangeGameName(CGN) [変更したい名前(10文字)] : ゲーム中の名前を変えられます");
                            SetText = args[0];
                            break;
                        }
                        main.SetGameName.Value = text.Substring(args[0].Length + 1);
                        AddChat = ($"ゲーム中の名前が[{main.SetGameName.Value}]になりました");
                        break;

                    case "/kill":
                        if (!(AmongUsClient.Instance.GameMode == GameModes.FreePlay && (args.Length > 2))) break;
                        string killplayerName = text.Substring(args[0].Length + 1);
                        PlayerControl killtarget = Helpers.GetNamePlayer(killplayerName);
                        if (killtarget != null)
                        {
                            killtarget.RpcMurderPlayer(killtarget);
                            AddChat = killplayerName + "をキルしました";
                        }
                        break;

                    case "/exiled":
                        if (!(AmongUsClient.Instance.GameMode == GameModes.FreePlay && (args.Length > 2))) break;
                        string ExiledplayerName = text.Substring(args[0].Length + 1);
                        PlayerControl Exiledtarget = Helpers.GetNamePlayer(ExiledplayerName);
                        if (Exiledtarget != null)
                        {
                            Exiledtarget.Exiled();
                            AddChat = ExiledplayerName + "を追放しました";
                        }
                        break;

                    case "/revive":
                        if (!(AmongUsClient.Instance.GameMode == GameModes.FreePlay && (args.Length > 2))) break;
                        string reviveplayerName = text.Substring(args[0].Length + 1);
                        PlayerControl revivetarget = Helpers.GetNamePlayer(reviveplayerName);
                        if (revivetarget != null)
                        {
                            revivetarget.Revive();
                            AddChat = reviveplayerName + "を生き返らせました";
                        }
                        break;

                    case "/sc":
                    case "/sendchat":
                        if (!(main.SendJoinPlayer.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay)) break;
                        if (args.Length < 3)
                        {
                            AddChat = $"送るチャットは\n{main.SetSendJoinChat.Value}\nです";
                            break;
                        }
                        main.SetSendJoinChat.Value = args[1];
                        AddChat = $"送るチャットが\n{main.SetSendJoinChat.Value}\nになりました";
                        break;

                    case "/dn":
                    case "/doublename":
                        if (!(main.DoubleName.Value && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode != GameModes.FreePlay)) break;
                        if (args.Length < 3)
                        {
                            AddChat = ("/DoubleName(DN) [変えたい名前] : 名前に二段目を追加します");
                            SetText = args[0];
                            break;
                        }
                        main.SetDoubleName.Value = args[1];
                        AddChat = $"二段目の名前が\n{main.SetDoubleName.Value}\nになりました";
                        break;

                    case "/tn":
                    case "/translucentname":
                        if (!main.TranslucentName.Value) break;
                        if (Int32.TryParse(args[1], out TranslucentName) && args.Length > 2)
                        {
                            TranslucentName = Math.Clamp(TranslucentName, 1, 100);
                            main.SetTranslucentName.Value = TranslucentName;
                            AddChat = ($"名前の透明度が[{main.SetTranslucentName.Value}%]になりました");
                            break;
                        }
                        AddChat = ("\n/TranslucentName(TN) [数値%] : 名前の透明度を変更します");
                        SetText = args[0];
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
                 $"TheSkeld : {(main.AddTheSkeld.Value ? "ON" : "OFF")}\n" +
                 $"MIRAHQ : {(main.AddMIRAHQ.Value ? "ON" : "OFF")}\n" +
                 $"Polus : {(main.AddPolus.Value ? "ON" : "OFF")}\n" +
                 $"AirShip : {(main.AddAirShip.Value ? "ON" : "OFF")}"));
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class PlayerSpectate
        {
            public static bool flag = false;
            public static void Postfix(HudManager __instance)
            {
                if (PlayerSpectatePlayer != null)
                {
                    PlayerControl.LocalPlayer.transform.position = PlayerSpectatePlayer.transform.position;
                    if (!flag)
                    {
                        flag = true;
                        PlayerControl.LocalPlayer.gameObject.SetActive(false);
                    }
                    if (MeetingHud.Instance != null) PlayerSpectatePlayer = null;
                }
                else
                {
                    if (flag)
                    {
                        flag = false;
                        PlayerControl.LocalPlayer.gameObject.SetActive(true);
                    }
                }
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
        }
    }
}
