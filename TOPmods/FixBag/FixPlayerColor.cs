using HarmonyLib;

namespace TownOfPlus
{
    //プレイヤーの色が違うバグを修正
    [HarmonyPatch(typeof(PlayerCustomizationMenu), nameof(PlayerCustomizationMenu.Start))]
    public static class FixPlayerColor
    {
        public static void Prefix()
        {
            if (main.FixPlayerColor.Value)
            {
                if (PlayerControl.LocalPlayer?.Data?.DefaultOutfit?.ColorId is int colorid) SaveManager.BodyColor = (byte)colorid;
            }
        }
    }
}