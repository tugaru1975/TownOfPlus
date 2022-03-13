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
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;
using Il2CppSystem;
using System.Threading;
using System.Threading.Tasks;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    //コード隠し
    public class HideCode
    {
        private static bool flag = false;
        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public class GameStartManagerUpdatePatch
        {
            public static void Postfix(GameStartManager __instance)
            {
                if (AmongUsClient.Instance.GameMode == GameModes.LocalGame) return;
                // Lobby code
                if (main.HideLobbyCodes.Value)
                {
                    flag = false;
                    __instance.GameRoomName.text = main.LobbyCode;
                }
                else
                {
                    if (flag == false)
                    {
                        flag = true;
                        __instance.GameRoomName.text = $"{DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RoomCode, new Il2CppReferenceArray<Il2CppSystem.Object>(0)) + "\r\n" + InnerNet.GameCode.IntToGameName(AmongUsClient.Instance.GameId)}";
                    }
                }
            }
        }
        [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
        public static class HiddenTextPatch
        {
            private static void Postfix(TextBoxTMP __instance)
            {
                if (__instance.name == "GameIdText" && main.HideLobbyCodes.Value) __instance.outputText.text = new string('*', __instance.text.Length);
            }
        }
    }
}
