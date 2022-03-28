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
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static class GameSettingMenuPatch
    {
        public static void Prefix(GameSettingMenu __instance)
        {
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
            foreach (var ob in __instance.Children)
            {
                switch(ob.Title)
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
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
    public static class KillCoolDown
    {
        public static void Prefix(GameOptionsMenu __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                if (PlayerControl.GameOptions.KillCooldown == 0 || PlayerControl.GameOptions.KillCooldown == 0.00001f)
                {
                    PlayerControl.GameOptions.KillCooldown = 0.00001f;
                    Helpers.SyncSettings();
                }
                else
                {
                    PlayerControl.GameOptions.KillCooldown = (float)(System.Math.Truncate(PlayerControl.GameOptions.KillCooldown * System.Math.Pow(10, 1)) / System.Math.Pow(10, 1));
                    Helpers.SyncSettings();
                }
            }
        }
    }
}
