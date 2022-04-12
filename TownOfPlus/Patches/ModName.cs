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
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            {
                if (PlayerControl.LocalPlayer.Data.IsDead)
                {
                    __instance.transform.localPosition = new Vector3(3.45f, __instance.transform.localPosition.y, __instance.transform.localPosition.z);
                }
                else
                {
                    __instance.transform.localPosition = new Vector3(4.2f, __instance.transform.localPosition.y, __instance.transform.localPosition.z);
                }
            }
            else
            {
                __instance.transform.localPosition = new Vector3(3.5f, __instance.transform.localPosition.y, __instance.transform.localPosition.z);
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
