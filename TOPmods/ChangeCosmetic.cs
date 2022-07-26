using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class ChangeCosmetic
    {
        public static void Postfix()
        {
            if (main.RainbowHat.Getbool())
            {
                try
                { 
                    PlayerControl.LocalPlayer.cosmetics.hat.SpriteColor = Color.HSVToRGB(Time.time % 1, 1, 1);
                    Flag.NewFlag("RainbowHat");
                }
                catch { }
            }
            else
            {
                Flag.Run(() =>
                {
                    try
                    {
                        PlayerControl.LocalPlayer.cosmetics.hat.SpriteColor = Color.white;
                    }
                    catch { }
                }, "RainbowHat");
            }

            if (main.RainbowVisor.Getbool())
            {
                try
                {
                    PlayerControl.LocalPlayer.cosmetics.visor.Image.color = Color.HSVToRGB(Time.time % 1, 1, 1);
                    Flag.NewFlag("RainbowVisor");
                }
                catch { }
            }
            else
            {
                Flag.Run(() =>
                {
                    try
                    {
                        PlayerControl.LocalPlayer.cosmetics.visor.Image.color = Color.white;
                    }
                    catch { }
                }, "RainbowVisor");
            }

            if (main.RainbowSkin.Getbool())
            {
                try
                {
                    PlayerControl.LocalPlayer.cosmetics.skin.layer.color = Color.HSVToRGB(Time.time % 1, 1, 1);
                    Flag.NewFlag("RainbowSkin");
                }
                catch { }
            }
            else
            {
                Flag.Run(() =>
                {
                    try
                    {
                        PlayerControl.LocalPlayer.cosmetics.skin.layer.color = Color.white;
                    }
                    catch { }
            }, "RainbowSkin");
            }

            if (main.RainbowPet.Getbool())
            {
                try
                {
                    PlayerControl.LocalPlayer.cosmetics.currentPet.rend.color = Color.HSVToRGB(Time.time % 1, 1, 1);
                    Flag.NewFlag("RainbowPet");
                }
                catch { }
            }
            else
            {
                Flag.Run(() =>
                {
                    try
                    {
                        PlayerControl.LocalPlayer.cosmetics.currentPet.rend.color = Color.white;
                    }
                    catch { }
                }, "RainbowPet");
            }
        }
    }

}