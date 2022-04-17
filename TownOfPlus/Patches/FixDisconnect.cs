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
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    class FixDisconnect
    {
        static void Postfix(MeetingHud __instance)
        {
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (pva == null) continue;
                PlayerControl player = Helpers.playerById(pva.TargetPlayerId);
                if(player == null || player.Data == null || player.Data.Disconnected)
                {
                    pva.Overlay.gameObject.SetActive(true);
                    if (player.Data.IsDead) pva.XMark.gameObject.SetActive(true);
                }
            }
        }
    }
}