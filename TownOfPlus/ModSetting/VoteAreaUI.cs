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
    class VoteAreaUI
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (main.CrewColorVoteArea.Value)
            {
                if (__instance.playerStates == null) return;
                foreach (PlayerVoteArea pva in __instance.playerStates)
                {
                    try
                    {
                        pva.Background.color = Helpers.playerById(pva.TargetPlayerId).MyRend.material.GetColor("_BodyColor");
                    }
                    catch { }
                }
                CreateFlag.NewFlag("VoteAreaUI");
            }
            else
            {
                CreateFlag.Run(() =>
                {
                    if (__instance.playerStates == null) return;
                    foreach (PlayerVoteArea pva in __instance.playerStates)
                    {
                        try
                        {
                            pva.Background.color = Palette.White;
                        }
                        catch { }
                    }
                }, "VoteAreaUI");
            }
        }
    }
}