using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public static class AutoBanBlockedPlayer
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] InnerNet.ClientData client)
        {
            if (main.AutoBanBlockedPlayer.Getbool() && GameState.IsHost && GameState.IsLobby)
            {
                if (DestroyableSingleton<FriendsListManager>.Instance.IsPlayerBlockedUsername(client.FriendCode))
                {
                    __instance.KickPlayer(client.Id, true);
                }
            }
        }
    }
}