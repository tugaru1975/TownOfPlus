using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ResetDoubleName
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer == null) return;
            if (GameState.IsHost)
            {
                if (main.DoubleName.Value && GameState.IsLobby)
                {
                    PlayerControl.LocalPlayer.RpcSetName($"{main.SetDoubleName.Value}\n{SaveManager.PlayerName.SetColor("FFFFFF50")}\n");
                    Flag.NewFlag("DoubleName");
                }
                else
                {
                    Flag.Run(() =>
                    {
                        PlayerControl.LocalPlayer.RpcSetName(SaveManager.PlayerName);
                    }, "DoubleName");
                }
            }
        }
    }
}
