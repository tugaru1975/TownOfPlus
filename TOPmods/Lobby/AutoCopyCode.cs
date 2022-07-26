using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class AutoCopyCode
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (main.AutoCopyCode.Getbool()) __instance.CopyGameCode();
        }
    }
}
