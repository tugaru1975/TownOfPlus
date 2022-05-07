using HarmonyLib;
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
        public static bool isChatCommand = false;
        public static PlayerControl SpectatePlayer = null;
        //Shift + Backspace で全消し
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        public static class Delete
        {
            public static void Prefix(ChatController __instance)
            {
                if (!HudManager.Instance.Chat.IsOpen) return;
                if (SaveManager.chatModeType != 1) return;
                if (!main.KeyCommand.Value) return;
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Backspace))
                {
                    __instance.TextArea.Clear();
                    __instance.quickChatMenu.ResetGlyphs();
                }
            }
        }
        //Control+Zで一個戻す Control+Yで一個取り消し
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        public static class UndoAndRedo
        {
            public static List<string> List = new List<string>();
            public static int count = 1;
            public static bool UndoRedo = false;
            public static string Text = "text";
            public static void Prefix(ChatController __instance)
            {
                if (!HudManager.Instance.Chat.IsOpen) return;
                if (SaveManager.chatModeType != 1) return;
                if (!main.KeyCommand.Value) return;
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
                        __instance.quickChatMenu.ResetGlyphs();
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
                        __instance.quickChatMenu.ResetGlyphs();
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
                if (SaveManager.chatModeType != 1) return;
                if (!main.KeyCommand.Value) return;
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X))
                {
                    GUIUtility.systemCopyBuffer = __instance.TextArea.text;
                    __instance.TextArea.SetText("");
                    __instance.quickChatMenu.ResetGlyphs();
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
                if (SaveManager.chatModeType != 1) return;
                if (!main.KeyCommand.Value) return;
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

                    if (Shift)
                    {
                        if (num <= 0.0f)
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
                    }
                    else
                    {
                        if (CopyWordCount + __instance.TextArea.text.Length >= 100)
                        {
                            CopyWord = CopyWord.Substring(0, 100 - __instance.TextArea.text.Length);
                        }
                        __instance.TextArea.SetText(__instance.TextArea.text + CopyWord);
                        __instance.quickChatMenu.ResetGlyphs();
                    }
                }
            }
        }
        //チャット送られた時にClassSendChatに送る
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        class ChatCommand
        {
            public static void Prefix(ChatController __instance)
            {
                if (!main.ChatCommand.Value) return;
                if (SaveManager.chatModeType != 1) return;
                text = __instance.TextArea.text;
                SendChat.Addchat(__instance);
            }
        }
        ///Tabでコマンド補完
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        class TabChatCommand
        {
            public static string ChatText;
            public static int Count = -1;
            public static List<string> ChatArea = new List<string>();
            public static void Prefix(ChatController __instance)
            {
                if (!HudManager.Instance.Chat.IsOpen) return;
                if (!main.ChatCommand.Value) return;
                var Check = true;
                foreach (var info in CommandList.AllCommand())
                {
                    if (__instance.TextArea.text.StringListContains(info.Command))
                    {
                        Check = false;
                        break;
                    }
                }
                if (Check) ChatText = __instance.TextArea.text;
                if (Input.GetKeyDown(KeyCode.Tab) && ChatText.CutString(1) == "/")
                {
                    var ChatCommand = new List<string>();
                    var args = ChatText.Split(' ');
                    foreach (var List in CommandList.AllCommand())
                    {
                        if (!List.Terms) continue;
                        if (ChatText.ToLower() == List.Command[0].CutString(ChatText.Length).ToLower() || Helpers.AllCommandNum(ChatText) >= 0)
                        {
                            ChatCommand.Add(List.Command[0]);
                        }
                        else
                        {
                            var ComFlag = false;
                            foreach (var Command in List.Command)
                            {
                                if (args[0].ToLower() == Command.ToLower())
                                {
                                    ComFlag = true;
                                    break;
                                }
                            }

                            if (ComFlag)
                            {
                                var Command = args[0] + " ";
                                switch (List.Type)
                                {
                                    case (int)CommandList.CommandText.Name:
                                        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                                        {
                                            if (p == PlayerControl.LocalPlayer) ChatCommand.Add(Command + SaveManager.PlayerName);
                                            else ChatCommand.Add(Command + p.Data.PlayerName.DeleteHTML());
                                        }
                                        break;

                                    case (int)CommandList.CommandText.MapName:
                                        ChatCommand.Add(Command + "Skeld");
                                        ChatCommand.Add(Command + "MIRA");
                                        ChatCommand.Add(Command + "Polus");
                                        ChatCommand.Add(Command + "AirShip");
                                        break;

                                    case (int)CommandList.CommandText.FileName:
                                        try
                                        {
                                            using StreamReader sr = new StreamReader(Path.GetDirectoryName(Application.dataPath) + @"\TownOfPlus\SkinData\SkinData.csv");
                                            {
                                                while (!sr.EndOfStream)
                                                {
                                                    var line = sr.ReadLine();
                                                    string[] values = line.Split(',');
                                                    ChatCommand.Add(Command + values[0]);
                                                }
                                            }
                                        }
                                        catch { }
                                        break;


                                    case (int)CommandList.CommandText.Platforms:
                                        for (int i = 1; i <= Enum.GetNames(typeof(Platforms)).Length - 1; i++)
                                        {
                                            ChatCommand.Add(Command + $"{Enum.ToObject(typeof(Platforms), i)}".Replace("Standalone", ""));
                                        }
                                        break;

                                    case (int)CommandList.CommandText.Reset:
                                        ChatCommand.Add(Command + "Reset");
                                        break;
                                }
                            }
                        }
                    }
                    if (!(ChatArea.All(y => ChatCommand.Any(l => l == y)) && ChatCommand.All(y => ChatArea.Any(l => l == y))))
                    {
                        Count = -1;
                        ChatArea = new List<string>(ChatCommand);
                    }
                    if (Count >= ChatCommand.Count - 1) Count = -1;
                    Count += 1;
                    __instance.TextArea.SetText(ChatCommand[Count]);
                }
            }
        }
        //コマンドが含まれているかの判断
        class SendChat
        {
            public static PlayerControl DMPlayer = null;
            public static bool Addchat(ChatController __instance)
            {
                var canceled = false;
                var argsText = text;
                argsText += " ";
                //コマンドかどうか
                if (text.Substring(0, 1) == "/")
                {
                    canceled = true;
                    isChatCommand = true;
                }
                string[] args = argsText.Split(' ');
                var AddChat = "";
                var RpcSendChat = "";
                var SetText = "";
                foreach (var info in CommandList.AllCommand())
                {
                    if (!args[0].StringListContains(info.Command)) continue;
                    switch (args[0].ToLower())
                    {
                        case "/help":
                            AddChat = "===コマンド一覧===";
                            foreach (var List in CommandList.AllCommand())
                            {
                                if (!List.Terms) continue;
                                if (List.Help == "") continue;
                                AddChat += "\n" + List.Help;
                            }
                            if (AddChat == "===コマンド一覧===") AddChat += ("\n実行可能なコマンドはありません");
                            break;

                        case "/lmp":
                        case "/lobbymaxplayer":
                            if (!info.Terms) break;
                            var LobbyLimit = 15;
                            if (!Int32.TryParse(args[1], out LobbyLimit))
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
                            else
                            {
                                LobbyLimit = Math.Clamp(LobbyLimit, 4, 15);
                                if (LobbyLimit != PlayerControl.GameOptions.MaxPlayers)
                                {
                                    PlayerControl.GameOptions.MaxPlayers = LobbyLimit;
                                    DestroyableSingleton<GameStartManager>.Instance.LastPlayerCount = LobbyLimit;
                                    Helpers.SyncSettings();
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
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                RandomMap(__instance);
                                break;
                            }
                            switch (args[1].ToLower())
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
                                    AddChat = info.Help;
                                    SetText = args[0];
                                    break;
                            }
                            break;

                        case "/clc":
                        case "/changelobbycode":
                            if (!info.Terms) break;
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

                        case "/ccc":
                        case "/changecodecolor":
                            if (!info.Terms) break;
                            if (args.Length > 2)
                            {
                                if (!(args[1].Length == 6 | args[1].Length == 8)) break;
                                if (!Regex.IsMatch(args[1], @"[0-Z]")) break;
                                main.SetCodeColor.Value = args[1];
                            }
                            else
                            {
                                main.SetCodeColor.Value = "FFFFFF";
                            }
                            AddChat = ($"コードカラーが[#{main.SetCodeColor.Value}]になりました");
                            break;

                        case "/kick":
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
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
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
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
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
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
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                SpectatePlayer = null;
                                AddChat = ("スペクテイターモードを解除しました");
                                break;
                            }
                            string psplayerName = text.Substring(args[0].Length + 1);
                            PlayerControl pstarget = Helpers.GetNamePlayer(psplayerName);
                            if (pstarget != null && pstarget != PlayerControl.LocalPlayer)
                            {
                                SpectatePlayer = pstarget;
                                AddChat = psplayerName + "を観戦します";
                                break;
                            }
                            SetText = args[0];
                            break;

                        case "/tpme":
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
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
                            if (!info.Terms) break;
                            if (10 < args[1].Length)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                                break;
                            }
                            main.SetGameName.Value = text.Substring(args[0].Length + 1);
                            AddChat = ($"ゲーム中の名前が[{main.SetGameName.Value}]になりました");
                            break;

                        case "/kill":
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
                            string killplayerName = text.Substring(args[0].Length + 1);
                            PlayerControl killtarget = Helpers.GetNamePlayer(killplayerName);
                            if (killtarget != null)
                            {
                                killtarget.RpcMurderPlayer(killtarget);
                                AddChat = killplayerName + "をキルしました";
                            }
                            break;

                        case "/exiled":
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
                            string ExiledplayerName = text.Substring(args[0].Length + 1);
                            PlayerControl Exiledtarget = Helpers.GetNamePlayer(ExiledplayerName);
                            if (Exiledtarget != null)
                            {
                                Exiledtarget.Exiled();
                                AddChat = ExiledplayerName + "を追放しました";
                            }
                            break;

                        case "/revive":
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
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
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = $"送るチャットは\n{main.SetSendJoinChat.Value}\nです";
                                break;
                            }
                            main.SetSendJoinChat.Value = args[1].Length > 100 ? args[1].Remove(100) : args[1];
                            AddChat = $"送るチャットが\n{main.SetSendJoinChat.Value}\nになりました";
                            break;

                        case "/dn":
                        case "/doublename":
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                                break;
                            }
                            main.SetDoubleName.Value = args[1];
                            AddChat = $"二段目の名前が\n{main.SetDoubleName.Value}\nになりました";
                            break;

                        case "/tn":
                        case "/translucentname":
                            if(!info.Terms) break;
                            var TranslucentName = 100;
                            if (Int32.TryParse(args[1], out TranslucentName) && args.Length > 2)
                            {
                                TranslucentName = Math.Clamp(TranslucentName, 1, 100);
                                main.SetTranslucentName.Value = TranslucentName;
                                AddChat = ($"名前の透明度が[{main.SetTranslucentName.Value}%]になりました");
                                break;
                            }
                            AddChat = info.Help;
                            SetText = args[0];
                            break;

                        case "/opkick":
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                SendOPkick(__instance);
                                break;
                            }
                            var Contain = false;
                            for (int i = 1; i <= Enum.GetNames(typeof(Platforms)).Length - 1; i++)
                            {
                                if (args[1].ToLower() == Enum.ToObject(typeof(Platforms), i).ToString().Replace("Standalone", "").ToLower() || args[1].ToLower() == i.ToString().ToLower())
                                {
                                    if (main.SetOPkick.Value.Contains($"{i},"))
                                    {
                                        main.SetOPkick.Value = main.SetOPkick.Value.Replace($"{i},", "");
                                    }
                                    else
                                    {
                                        main.SetOPkick.Value += $"{i},";
                                    }
                                    SendOPkick(__instance);
                                    Contain = true;
                                    break;
                                }
                            }
                            if (Contain) break;
                            var platforms = "";
                            for (int i = 1; i <= Enum.GetNames(typeof(Platforms)).Length - 1; i++)
                            {
                                platforms += Enum.ToObject(typeof(Platforms), i).ToString().Replace("Standalone", "") + $" [{Enum.ToObject(typeof(Platforms), i).ToString().Replace("Standalone", "")}({i})]\n";
                            }
                            AddChat = args[1] + "\n" + "=== 機種 [コマンド名(数値)] ===\n" + platforms;
                            SetText = args[0];
                            break;

                        case "/cps":
                            if (!info.Terms) break;
                            if (args[1].ToLower() == "reset")
                            {
                                main.CPSpositionX.Value = 0f;
                                main.CPSpositionY.Value = 2.75f;
                                AddChat = "CPSの位置をリセットしました";
                                break;
                            }
                            if (main.SettingCPS)
                            {
                                main.SettingCPS = false;
                                AddChat = "CPSの位置設定を無効化しました";
                            }
                            else
                            {
                                main.SettingCPS = true;
                                AddChat = "CPSの位置設定を有効化しました\n十字キーかマウスの右クリックで動かしてください\n位置リセットは[/CPS Reset]です。";
                            }
                            break;

                        case "/dt":
                        case "/datetime":
                            if (!info.Terms) break;
                            if (args[1].ToLower() == "reset")
                            {
                                main.DateTimepositionX.Value = 0f;
                                main.DateTimepositionY.Value = 2.75f;
                                AddChat = "DateTimeの位置をリセットしました";
                                break;
                            }
                            if (main.SettingDateTime)
                            {
                                main.SettingDateTime = false;
                                AddChat = "DateTimeの位置設定を無効化しました";
                            }
                            else
                            {
                                main.SettingDateTime = true;
                                AddChat = "DateTimeの位置設定を有効化しました\n十字キーかマウスの右クリックで動かしてください\n位置リセットは[/DateTime Reset]です。";
                            }
                            break;

                        case "/ss":
                        case "/saveskin":
                            if (!info.Terms) break;
                            if (args[1] == "" || args[1].Contains(","))
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                                break;
                            }
                            try
                            {
                                var savechack = false;
                                using StreamReader read = new StreamReader(Path.GetDirectoryName(Application.dataPath) + @"\TownOfPlus\SkinData\SkinData.csv");
                                {
                                    while (!read.EndOfStream)
                                    {
                                        string[] values = read.ReadLine().Split(',');
                                        if (values[0] == args[1])
                                        {
                                            savechack = true;
                                            break;
                                        }
                                    }
                                }
                                if (savechack)
                                {
                                    AddChat = $"「{args[1]}」はすでに使われている名前です";
                                    SetText = args[0];
                                    break;
                                }
                            }
                            catch { }

                            var p = PlayerControl.LocalPlayer.CurrentOutfit;
                            var data = new string[]
                            {
                                args[1],
                                p.HatId,
                                p.VisorId,
                                p.SkinId,
                                p.PetId,
                                p.NamePlateId,
                            };
                            try
                            {
                                using StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Application.dataPath) + @"\TownOfPlus\SkinData\SkinData.csv", true);
                                sw.WriteLine(string.Join(",", data));
                                AddChat = $"「{args[1]}」を保存しました";
                            }
                            catch { }
                            break;

                        case "/ls":
                        case "/loadskin":
                            if (!info.Terms) break;
                            if (args[1].ToLower() == "")
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                                break;
                            }
                            var loadchack = false;
                            try
                            {
                                using StreamReader sr = new StreamReader(Path.GetDirectoryName(Application.dataPath) + @"\TownOfPlus\SkinData\SkinData.csv");
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();
                                        string[] values = line.Split(',');
                                        if (values[0] != args[1]) continue;
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
                                        loadchack = true;
                                        AddChat = $"「{args[1]}」にスキンを変更しました";
                                    }
                                }
                            }
                            catch { }

                            if (!loadchack)
                            {
                                AddChat = $"「{args[1]}」が見つかりませんでした";
                            }
                            break;

                        case "/ds":
                        case "/deleteskin":
                            if (!info.Terms) break;
                            if (args[1].ToLower() == "")
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                                break;
                            }
                            var SkinData = new List<string>();
                            var DaleteChack = false;
                            try
                            {

                                using StreamReader StreamReader = new StreamReader(Path.GetDirectoryName(Application.dataPath) + @"\TownOfPlus\SkinData\SkinData.csv");
                                {
                                    while (!StreamReader.EndOfStream)
                                    {
                                        var line = StreamReader.ReadLine();
                                        string[] values = line.Split(',');
                                        if (values[0] == args[1])
                                        {
                                            DaleteChack = true;
                                        }
                                        else
                                        {
                                            SkinData.Add(line);
                                        }

                                    }
                                }
                            }
                            catch { }
                            try
                            {
                                if (DaleteChack)
                                {
                                    using StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Application.dataPath) + @"\TownOfPlus\SkinData\SkinData.csv");
                                    foreach(var line in SkinData)
                                    {
                                        sw.WriteLine(line);
                                    }
                                    AddChat = $"「{args[1]}」を削除しました";
                                }
                                else
                                {
                                    AddChat = $"「{args[1]}」が見つかりませんでした";
                                }
                            }
                            catch { }
                            break;

                        case "/tc":
                        case "/translucentchat":
                            if (!info.Terms) break;
                            var TranslucentChat = 100;
                            if (Int32.TryParse(args[1], out TranslucentChat) && args.Length > 2)
                            {
                                TranslucentChat = Math.Clamp(TranslucentChat, 1, 100);
                                main.SetTranslucentChat.Value = TranslucentChat;
                                AddChat = ($"チャットの透明度が[{main.SetTranslucentChat.Value}%]になりました");
                                break;
                            }
                            AddChat = info.Help;
                            SetText = args[0];
                            break;

                        case "/dm":
                        case "/directmessage":
                            if (!info.Terms) break;
                            if (args.Length < 3)
                            {
                                AddChat = info.Help;
                                SetText = args[0];
                            }
                            if (args[1].ToLower() == "cancel")
                            {
                                SendChat.DMPlayer = null;
                                AddChat = "DMをキャンセルしました";
                                break;
                            }
                            string DMPlayer = text.Substring(args[0].Length + 1);
                            PlayerControl DMtarget = Helpers.GetNamePlayer(DMPlayer);
                            if (DMtarget != null)
                            {
                                SendChat.DMPlayer = DMtarget;
                                AddChat = DMPlayer + $"へのメッセージを入力してください\n※キャンセルは[{info.Command[0]}({info.Command[1].Replace("/","")}) Cancel]";
                            }
                            break;

                        case "/fps":
                            if (!info.Terms) break;
                            if (args[1].ToLower() == "reset")
                            {
                                main.FPSpositionX.Value = 0f;
                                main.FPSpositionY.Value = 2.75f;
                                AddChat = "FPSの位置をリセットしました";
                                break;
                            }
                            if (main.SettingFPS)
                            {
                                main.SettingFPS = false;
                                AddChat = "FPSの位置設定を無効化しました";
                            }
                            else
                            {
                                main.SettingFPS = true;
                                AddChat = "FPSの位置設定を有効化しました\n十字キーかマウスの右クリックで動かしてください\n位置リセットは[/FPS Reset]です。";
                            }
                            break;

                        default:
                            break;
                    }

                }

                //DM処理
                if (DMPlayer != null && !canceled)
                {
                    if (text.Length > 100)
                    {
                        AddChat = "ERROR:文字数は100文字までです";
                    }
                    else
                    {
                        var clientId = Helpers.playerByClient(DMPlayer).Id;
                        AddChat = DMPlayer.Data.PlayerName + "に\n" + text +"\nを送りました";
                        isChatCommand = true;
                        Helpers.DMChat(clientId, "※TownOfPlusによる個別送信\n" + text);
                        DMPlayer = null;
                    }
                    canceled = true;
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
                ($"===有効マップ一覧===\n" +
                 $"マップ名 : 設定\n" +
                 $"TheSkeld : {(main.AddTheSkeld.Value ? "ON" : "OFF")}\n" +
                 $"MIRAHQ : {(main.AddMIRAHQ.Value ? "ON" : "OFF")}\n" +
                 $"Polus : {(main.AddPolus.Value ? "ON" : "OFF")}\n" +
                 $"AirShip : {(main.AddAirShip.Value ? "ON" : "OFF")}"));
            }
            public static void SendOPkick(ChatController __instance)
            {
                var text = ($"===Kickする機種一覧===\n");
                for (int i = 1; i <= Enum.GetNames(typeof(Platforms)).Length - 1; i++)
                {
                    text += $"{Enum.ToObject(typeof(Platforms), i)}".Replace("Standalone", "") + $" : {(main.SetOPkick.Value.Contains($"{i},") ? "ON" : "OFF")}\n";
                }
                __instance.AddChat(PlayerControl.LocalPlayer, text);
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class PlayerSpectate
        {
            public static PlayerControl player;
            public static void Postfix(HudManager __instance)
            {
                if (MeetingHud.Instance != null) SpectatePlayer = null;
                if (SpectatePlayer != null)
                {
                    if (SpectatePlayer != player)
                    {
                        player = SpectatePlayer;
                        Reset();
                        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                        {
                            if (p.Data.IsDead && p != SpectatePlayer) p.gameObject.SetActive(false);
                        }
                    }
                    PlayerControl.LocalPlayer.transform.position = SpectatePlayer.transform.position;
                    CreateFlag.NewFlag("SpectatePlayer");
                }
                else
                {
                    CreateFlag.Run(() =>
                    {
                        Reset();
                    }, "SpectatePlayer");
                }
            }
            public static void Reset()
            {
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    p.gameObject.SetActive(true);
                }
            }
        }
        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        class ChatCommandUI
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (isChatCommand)
                {
                    __instance.SetLeft();
                    __instance.NameText.text = "TownOfPlus Command";
                    isChatCommand = false;
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
            SaveManager.chatModeType = 1;
            if (__instance.Chat.isActiveAndEnabled == false && (AmongUsClient.Instance?.GameMode == GameModes.FreePlay))
                __instance.Chat.SetVisible(true);
        }
    }
}
