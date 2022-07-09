using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Outline
    {
        public static void Postfix()
        {
            //キル対象
            if (main.RainbowOutline.Value)
            {
                if (PlayerControl.LocalPlayer == null) return;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p != PlayerControl.LocalPlayer) p.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", Color.HSVToRGB(Time.time % 1, 1, 1));
                }
            }
            if (main.CrewColorOutline.Value)
            {
                if (PlayerControl.LocalPlayer == null) return;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p != PlayerControl.LocalPlayer) p.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", p.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor"));
                }
            }

            //ベント
            if (main.RainbowVent.Value)
            {
                if (!GameState.IsShip) return;
                foreach (Vent vent in ShipStatus.Instance.AllVents)
                {
                    vent.myRend.material.SetColor("_OutlineColor", Color.HSVToRGB(Time.time % 1, 1, 1));
                }
            }
            if (main.CrewColorVent.Value)
            {
                if (!GameState.IsShip) return;
                foreach (Vent vent in ShipStatus.Instance.AllVents)
                {
                    vent.myRend.material.SetColor("_OutlineColor", PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor"));
                }
            }
        }
    }
}