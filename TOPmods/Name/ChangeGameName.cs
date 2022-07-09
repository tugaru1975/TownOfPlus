using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ChangeGameName
    {
        public static string name = "";
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer == null) return;
            if (GameState.IsHost)
            {
                if (main.ChangeGameName.Value)
                {
                    if (!GameState.IsGameStart)
                    {
                        Flag.NewFlag("CanChangeGameName");
                        ResetName();
                    }

                    if (GameState.IsGameStart || GameState.IsFreePlay)
                    {
                        Flag.Run(() =>
                        {
                            name = SaveManager.PlayerName;
                            SaveManager.PlayerName = main.SetGameName.Value;
                            PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
                            Flag.NewFlag("ChangedGameName");
                        }, "CanChangeGameName");
                    }
                }
                else ResetName();
            }
        }
        private static void ResetName()
        {
            if (!GameState.IsGameStart)
            {
                Flag.Run(() =>
                {
                    SaveManager.PlayerName = name;
                    PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
                }, "ChangedGameName");
            }
        }
    }
}
