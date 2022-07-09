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
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Networking;


namespace TownOfPlus
{
    //Shift + Backspace で全消し
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class Delete
    {
        public static void Postfix(ChatController __instance)
        {
            if (!GameState.IsCanKeyCommand || !main.KeyDelete.Value) return;
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
        public static List<string> list = new();
        public static int count = 1;
        public static void Postfix(ChatController __instance)
        {
            if (!GameState.IsCanKeyCommand || !main.KeyUndoAndRedo.Value) return;
            IsChange.Run(() =>
            {
                Flag.Run(() =>
                {
                    var i = count + 1;
                    list.RemoveRange(i, list.Count - i);
                }, "UndoRedo");
                list.Add(__instance.TextArea.text);
                count = list.Count - 1;
            }, __instance.TextArea.text, "UndoAndRedo", true);
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            {
                if (count != 0)
                {
                    Flag.NewFlag("UndoRedo");
                    count--;
                    __instance.TextArea.SetText(list[count]);
                    IsChange.SkipRun("UndoAndRedo");
                }
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
            {
                if (count != list.Count - 1)
                {
                    Flag.NewFlag("UndoRedo");
                    count++;
                    __instance.TextArea.SetText(list[count]);
                    IsChange.SkipRun("UndoAndRedo");
                }
            }
        }
    }
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ResetUndoAndRedo
    {
        public static void Prefix()
        {
            UndoAndRedo.list.Clear();
            UndoAndRedo.count = 1;
            Flag.DeleteFlag("UndeoRedo");
        }
    }

    //Control+Cでコピー　Control+Xでカット 
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class CopyAndCut
    {
        public static void Postfix(ChatController __instance)
        {
            if (!GameState.IsCanKeyCommand) return;
            if (main.KeyCut.Value && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X))
            {
                TextPlus.Clipboard(__instance.TextArea.text);
                __instance.TextArea.Clear();
                __instance.quickChatMenu.ResetGlyphs();
            }
            if (main.KeyCopy.Value && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
            {
                TextPlus.Clipboard(__instance.TextArea.text);
            }
        }
    }

    //Control+Vでペースト
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class Paste
    {
        public static void Postfix(ChatController __instance)
        {
            if (!GameState.IsCanKeyCommand || !main.KeyPaste.Value) return;
            if (Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftControl))
            {
                //コピーしてあるのを調べる
                var CopyWord = TextPlus.Clipboard();
                //Banされる文字の削除
                CopyWord = CopyWord.TrimAll("<", ">", "\r");
                if (CopyWord == "") return;
                //コピーしてある文字数
                var CopyWordCount = CopyWord.Length;
                //チャット文字数
                var TextAreaLimit = __instance.TextArea.characterLimit;

                if (GameState.IsCanSendChat)
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        if (CopyWordCount > TextAreaLimit)
                        {
                            //文字数多いときに送る
                            __instance.AddComChat($"ERROR:最大文字数は{TextAreaLimit}文字までです");
                            return;
                        }
                        //コマンドを対応させる
                        Chat.SendChat.Addchat(__instance, CopyWord, true);
                        return;
                    }
                }
                __instance.TextArea.SetText(__instance.TextArea.text + CopyWord.TrimAll("\n"));
            }
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class WallWalk
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer == null) return;
            if (!main.WallWalk.Value) return;
            //壁抜け
            if (Input.GetKeyDown(main.WallWalkKeyBind.Value))
            {
                if ((GameState.IsLobby || GameState.IsFreePlay) && GameState.IsCanMove)
                {
                    PlayerControl.LocalPlayer.Collider.offset = new Vector2(0f, 127f);
                }
            }
            //壁抜け解除
            if (PlayerControl.LocalPlayer.Collider.offset.y == 127f)
            {
                if (!Input.GetKey(main.WallWalkKeyBind.Value) || GameState.IsGameStart)
                {
                    PlayerControl.LocalPlayer.Collider.offset = new Vector2(0f, -0.3636f);
                }
            }
        }
    }

    //廃村コマンド
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
    public static class EndGame
    {
        public static void Postfix(ShipStatus __instance)
        {
            if (!GameState.IsShip || !main.EndGame.Value || !GameState.IsHost || !__instance.enabled) return;
            if ((main.EndGameKeyBindFirst.Value == KeyCode.None || Input.GetKey(main.EndGameKeyBindFirst.Value)) &&
                (main.EndGameKeyBindSecond.Value == KeyCode.None || Input.GetKey(main.EndGameKeyBindSecond.Value)) &&
                (main.EndGameKeyBindThird.Value == KeyCode.None || Input.GetKey(main.EndGameKeyBindThird.Value)))
            {
                __instance.enabled = false;
                ShipStatus.RpcEndGame(GameOverReason.HumansDisconnect, false);
            }
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class DebugKeyCommand
    {
        public static void Postfix()
        {
            if (!main.DebugMode.Value) return;
            if (Input.GetKeyDown(KeyCode.F1) && Input.GetKey(KeyCode.LeftControl))
            {
                var p = PlayerControl.LocalPlayer.CurrentOutfit;
                var list = new string[]
                {
                    p.PlayerName,
                    p.HatId,
                    p.VisorId,
                    p.SkinId,
                    p.PetId,
                    p.NamePlateId,
                };
                Log.log(string.Join('\n', list));
            }
            if (Input.GetKeyDown(KeyCode.F2) && Input.GetKey(KeyCode.LeftControl))
            {
                var list = TextPlus.Clipboard().Split(',');
                foreach (var op in ModOptionSetting.AllOptions)
                {
                    try
                    {
                        op.Config.Value = list.Contains(op.Title);
                    }
                    catch { }
                }

                ModOptionSetting.UpdateColor();
            }
        }
    }
}
