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
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public class GameStartRandomMap
    {
        public static bool Prefix()
        {
            bool RandomMap = true;
            if (main.RandomMaps.Value && AmongUsClient.Instance.AmHost)
            {
                if (main.AddTheSkeld || main.AddMIRAHQ || main.AddPolus || main.AddAirShip)
                {
                    var rand = new System.Random();
                    System.Collections.Generic.List<byte> RandomMaps = new System.Collections.Generic.List<byte>();
                    /*TheSkeld   = 0
                      MIRAHQ     = 1
                      Polus      = 2
                      Dleks      = 3
                      TheAirShip = 4*/
                    if (main.AddTheSkeld) RandomMaps.Add(0);
                    if (main.AddMIRAHQ) RandomMaps.Add(1);
                    if (main.AddPolus) RandomMaps.Add(2);
                    if (main.AddAirShip) RandomMaps.Add(4);
                    var MapsId = RandomMaps[rand.Next(RandomMaps.Count)];
                    PlayerControl.GameOptions.MapId = MapsId;
                }
            }
            return RandomMap;
        }
    }
}
