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
using System.Threading;
using System.Threading.Tasks;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.InitializeOptions))]
    public static class GameSettingMenuPatch
    {
        public static void Prefix(GameSettingMenu __instance)
        {
            if (!main.RoomOption.Value) return;
            // オンラインモードで部屋を立て直さなくてもマップを変更できるように変更
            __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    [HarmonyPriority(Priority.First)]
    public static class GameOptionsMenuPatch
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            if (!main.RoomOption.Value) return;
            foreach (var ob in __instance.Children)
            {
                switch (ob.Title)
                {
                    case StringNames.GameShortTasks:
                    case StringNames.GameLongTasks:
                    case StringNames.GameCommonTasks:
                        ob.Cast<NumberOption>().ValidRange = new FloatRange(0, 99);
                        break;
                    case StringNames.GameKillCooldown:
                        ob.Cast<NumberOption>().ValidRange = new FloatRange(0, 180);
                        break;
                    case StringNames.GameRecommendedSettings:
                        ob.enabled = false;
                        ob.gameObject.SetActive(false);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class KillCoolDown
    {
        public static void Prefix(GameStartManager __instance)
        {
            if (PlayerControl.LocalPlayer == null) return;
            if (AmongUsClient.Instance.AmHost)
            {
                if (main.NokillCool.Value)
                {
                    PlayerControl.GameOptions.KillCooldown = 0.00001f;
                    Helpers.SyncSettings();
                    CreateFlag.NewFlag("KillCool0");
                }
                else
                {
                    CreateFlag.Run(() =>
                    {
                        PlayerControl.GameOptions.KillCooldown = 0f;
                        Helpers.SyncSettings();
                    }, "KillCool0");
                }
            }
        }
    }
}
