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
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //名前変更
    public class ChangeName
    {
        private static int R = 99;
        private static int G = 00;
        private static int B = 00;
        private static bool flag = false;
        public static void Postfix(HudManager __instance)
        {
            string name = SaveManager.PlayerName;
            if (main.RainbowName.Value)
            {
                string TranslucentName = "";
                if (main.TranslucentName.Value) TranslucentName = "75";
                if (R != 99 && G == 00 && B == 99) R += 3;
                if (R != 00 && G == 99 && B == 00) R -= 3;
                if (R == 99 && G != 99 && B == 00) G += 3;
                if (R == 00 && G != 00 && B == 99) G -= 3;
                if (R == 00 && G == 99 && B != 99) B += 3;
                if (R == 99 && G == 00 && B != 00) B -= 3;
                string Rcount = R.ToString("00");
                string Gcount = G.ToString("00");
                string Bcount = B.ToString("00");
                name = $"<color=#{Rcount}{Gcount}{Bcount}{TranslucentName}>" + name + "</color>";
            }
            else
            {
                if (main.TranslucentName.Value)
                {
                    if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                    {
                        name = $"<color=#FF000075>" + name + "</color>";
                    }
                    else
                    {
                        if (PlayerControl.LocalPlayer.Data.Role.IsSimpleRole)
                        {
                            name = $"<color=#FFFFFF75>" + name + "</color>";
                        }
                        else
                        {
                            name = $"<color=#00FFFF75>" + name + "</color>";
                        }


                    }

                }
            }
            if (main.RainbowName.Value || main.TranslucentName.Value)
            {
                if (name != PlayerControl.LocalPlayer.name && PlayerControl.LocalPlayer.CurrentOutfitType == PlayerOutfitType.Default) PlayerControl.LocalPlayer.SetName(name);
                flag = true;
            }
            else 
            {
                if (flag) 
                {
                    PlayerControl.LocalPlayer.SetName(SaveManager.PlayerName);
                    flag = false;
                }
            } 
        }
    }
}
