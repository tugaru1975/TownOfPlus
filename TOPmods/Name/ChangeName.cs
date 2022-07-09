using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    //名前変更
    public class ChangeName
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (main.RainbowName.Value || main.TranslucentName.Value)
            {
                var p = PlayerControl.LocalPlayer;
                if (p?.Data is null || p?.Data?.Role is null) return;
                Color color = Helpers.GetPlayerRoleColor(p.Data.Role.Role);
                if (main.NameOutline.Value) color = Palette.Black;
                IsChange.Run(() =>
                {
                    Reset();
                }, main.NameOutline.Value, "NameOutline");
                if (main.RainbowName.Value)
                {
                    color = Color.HSVToRGB(Time.time % 1, 1, 1);
                }
                if (main.TranslucentName.Value)
                {
                    color.a = (100f - main.SetTranslucentName.Value) / 100f;
                }
                if (main.NameOutline.Value) p.cosmetics.nameText.outlineColor = color;
                else p.cosmetics.nameText.color = color;
                Flag.NewFlag("NamePlus");
            }
            else
            {
                Reset();
            }

        }
        public static void Reset()
        {
            Flag.Run(() =>
            {
                var p = PlayerControl.LocalPlayer;
                p.cosmetics.nameText.color = Helpers.GetPlayerRoleColor(p.Data.Role.Role);
                p.cosmetics.nameText.outlineColor = Palette.Black;
            }, "NamePlus");
        }
    }
}