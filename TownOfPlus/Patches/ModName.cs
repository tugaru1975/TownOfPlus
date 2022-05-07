using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    class ModNamePing
    {
        static void Postfix(PingTracker __instance)
        {
            __instance.text.alignment = TMPro.TextAlignmentOptions.TopRight;
            __instance.text.text += main.ModNameText;
            __instance.text.text += "\n<size=2>https://is.gd/TownOfPlus</size>";
            if (main.OldPingPositon.Value)
            {
                if (HudManager.Instance.Chat.isActiveAndEnabled && MeetingHud.Instance == null)
                {
                    __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(1.75f, __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge.y, __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge.z);
                }
                else
                {
                    __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(1.25f, __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge.y, __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge.z);
                }
                CreateFlag.NewFlag("PingPosition");
            }
            else
            {
                CreateFlag.Run(() =>
                {
                    __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(3.69f, __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge.y, __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge.z);
                }, "PingPosition");
            }
        }
    }
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    class ModNameVersion
    {
        static void Postfix(VersionShower __instance)
        {
            __instance.text.alignment = TMPro.TextAlignmentOptions.TopLeft;
            __instance.text.text += main.ModNameText;
        }
    }
}
