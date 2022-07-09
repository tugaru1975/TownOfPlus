using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerCrewColorText
    {
        public static void Postfix()
        {
            if (main.CrewColorText.Value)
            {
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    p.cosmetics.colorBlindText.color = p.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
                }
                Flag.NewFlag("CrewColorText");
            }
            else
            {
                Flag.Run(() =>
                {
                    foreach (var p in PlayerControl.AllPlayerControls)
                    {
                        p.cosmetics.colorBlindText.color = Color.white;
                    }
                }, "CrewColorText");
            }
        }
    }

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetMaskLayer))]
    public static class PlayerVoteAreaCrewColorText
    {
        public static void Postfix(PlayerVoteArea __instance)
        {
            if (main.CrewColorText.Value) __instance.PlayerIcon.cosmetics.colorBlindText.color = __instance.PlayerIcon.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
        }
    }

    [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.SetMaskLayer))]
    public static class PoolableCrewColorText
    {
        public static void Postfix(PoolablePlayer __instance)
        {
            if (main.CrewColorText.Value) __instance.cosmetics.colorBlindText.color = __instance.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
        }
    }
}