using HarmonyLib;

namespace TownOfPlus
{
    //スキンがズレてるのを修正
    class FixSkinBag
    {
        [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.Awake))]
        public static class FixSkinBagAwake
        {
            public static void Postfix(PoolablePlayer __instance)
            {
                FixSkin(__instance);
            }
        }

        [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.SetMaskLayer))]
        public static class FixSkinBagSetMaskLayer
        {
            public static void Postfix(PoolablePlayer __instance)
            {
                FixSkin(__instance);
            }
        }

        public static void FixSkin(PoolablePlayer __instance)
        {
            if (!main.FixSkinBag.Value) return;
            try
            {
                __instance.cosmetics.visor.transform.SetPos(y: 0.575f);
                __instance.cosmetics.hat.transform.SetPos(y: 0.575f);
            }
            catch { }
        }
    }
}