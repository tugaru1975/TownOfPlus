using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public static class SendJoinPlayer
    {
        public static void Prefix(PlayerPhysics __instance)
        {
            if (main.SendJoinPlayer.Value && GameState.IsHost && GameState.IsLobby)
            {
                if (AmongUsClient.Instance != null && __instance.myPlayer != PlayerControl.LocalPlayer)
                {
                    var client = AmongUsClient.Instance.GetClient(__instance.OwnerId);
                    Helpers.DMChat(client, "※TownOfPlusによる自動送信", main.SetSendJoinChat.Value);
                    HudManager.Instance?.Chat?.AddComChat(client.PlayerName.RemoveHTML() + "に参加メッセージを送りました。");
                }
            }
        }
    }
}