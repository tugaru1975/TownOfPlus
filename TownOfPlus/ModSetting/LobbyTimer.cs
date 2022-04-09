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
    //ロビータイマー
    public class LobbyTimer
    {
        private static float timer = 600f;
        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public class GameStartManagerStartPatch
        {
            public static void Postfix(GameStartManager __instance)
            {
                // Reset lobby countdown timer
                timer = 600f;
            }
        }
        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public class GameStartManagerUpdatePatch
        {

            private static bool update = false;
            private static string currentText = "";
            public static void Prefix(GameStartManager __instance)
            {
                if (!AmongUsClient.Instance.AmHost || !GameData.Instance || AmongUsClient.Instance.GameMode == GameModes.LocalGame) return; // Not host or no instance
                update = GameData.Instance.PlayerCount != __instance.LastPlayerCount;
            }
            public static void Postfix(GameStartManager __instance)
            {
                // Lobby timer
                if (!AmongUsClient.Instance.AmHost || !GameData.Instance || AmongUsClient.Instance.GameMode == GameModes.LocalGame) return;
                if (update) currentText = __instance.PlayerCounter.text;
                if (main.LobbyTimer.Value) __instance.PlayerCounter.text = currentText;
                timer = Mathf.Max(0f, timer -= Time.deltaTime);
                int minutes = (int)timer / 60;
                int seconds = (int)timer % 60;
                string suffix = $" ({minutes:00}:{seconds:00})";
                if (main.LobbyTimer.Value) 
                { 
                    __instance.PlayerCounter.text = currentText + suffix;
                    __instance.PlayerCounter.autoSizeTextContainer = true;
                }

            }
        }
    }
}