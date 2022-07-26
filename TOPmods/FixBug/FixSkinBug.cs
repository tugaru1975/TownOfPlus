using HarmonyLib;

namespace TownOfPlus
{
    //スキンがズレてるのを修正
    class FixSkinBug
    {
        [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.Awake))]
        public static class FixSkinBugAwake
        {
            public static void Postfix(PoolablePlayer __instance)
            {
                FixSkin(__instance);
            }
        }

        [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.SetMaskLayer))]
        public static class FixSkinBugSetMaskLayer
        {
            public static void Postfix(PoolablePlayer __instance)
            {
                FixSkin(__instance);
            }
        }

        public static void FixSkin(PoolablePlayer __instance)
        {
            if (!main.FixSkinBug.Getbool()) return;
            try
            {
                __instance.cosmetics.visor.transform.SetPos(y: 0.575f);
                __instance.cosmetics.hat.transform.SetPos(y: 0.575f);
            }
            catch { }
        }
    }
}