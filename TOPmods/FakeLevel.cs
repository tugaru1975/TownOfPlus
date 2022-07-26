using System;
using HarmonyLib;

namespace TownOfPlus
{
    //フェイクレベル
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public class FakeLevel
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (PlayerControl.LocalPlayer == null) return;
            
            if (main.FakeLevel.Getbool())
            {
                PlayerControl.LocalPlayer.RpcSetLevel((uint)new Random().Next(0,99));
            }
        }
    }
}
