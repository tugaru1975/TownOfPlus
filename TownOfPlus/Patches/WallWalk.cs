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
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class WallWalk
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            if (PlayerControl.LocalPlayer == null) return;
            //壁抜け
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if ((AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Joined ||
                AmongUsClient.Instance.GameMode == GameModes.FreePlay) && PlayerControl.LocalPlayer.CanMove)
                {
                    PlayerControl.LocalPlayer.Collider.offset = new Vector2(0f, 127f);
                }
            }
            //壁抜け解除
            if (PlayerControl.LocalPlayer.Collider.offset.y == 127f)
            {
                if (!Input.GetKey(KeyCode.LeftControl) || AmongUsClient.Instance.IsGameStarted)
                {
                    PlayerControl.LocalPlayer.Collider.offset = new Vector2(0f, -0.3636f);
                }
            }
        }
    }

}