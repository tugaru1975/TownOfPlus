using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public static class SendJoinPlayer
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (main.SendJoinPlayer.Getbool() && GameState.IsHost && GameState.IsLobby)
            {
                new LateTask(() =>
                {
                    if (__instance.PlayerId.TryGetClient(out var client))
                    {
                        Helpers.DMChat(client.log(), "※TownOfPlusによる自動送信", main.SetSendJoinChat.Getstring());
                        HudManager.Instance?.Chat?.AddComChat(client.PlayerName.RemoveHTML() + "に参加メッセージを送りました。");
                    }
                }, 1f);
            }
        }
    }
}