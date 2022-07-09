using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
    class SkipLogo
    {
        public static void Prefix(SplashManager __instance)
        {
            if (main.SkipLogo.Value)
            {
                __instance.sceneChanger.AllowFinishLoadingScene();
                __instance.startedSceneLoad = true;
                __instance.minimumSecondsBeforeSceneChange = 0;
            }
        }
    }
}