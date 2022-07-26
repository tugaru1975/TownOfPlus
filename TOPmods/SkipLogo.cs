using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
    class SkipLogo
    {
        public static void Postfix(SplashManager __instance)
        {
            if (main.SkipLogo.Getbool())
            {
                __instance.sceneChanger.AllowFinishLoadingScene();
                __instance.startedSceneLoad = true;
            }
        }
    }
}