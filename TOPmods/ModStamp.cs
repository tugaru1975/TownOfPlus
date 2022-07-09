using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    class AwakePatch
    {
        public static void Prefix(ModManager __instance)
        {
            __instance.ShowModStamp();
        }
    }
}
