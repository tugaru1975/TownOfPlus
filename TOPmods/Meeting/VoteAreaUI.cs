using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    class VoteAreaUI
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (main.CrewColorVoteArea.Value)
            {
                if (__instance.playerStates is null) return;
                foreach (PlayerVoteArea pva in __instance.playerStates)
                {
                    if (pva is null) continue;
                    if (pva.TargetPlayerId.TryGetClient(out var client) && Helpers.TryGetPlayerColor(client.ColorId, out var color)) pva.Background.color = color;
                  
                }
                Flag.NewFlag("VoteAreaUI");
            }
            else
            {
                Flag.Run(() =>
                {
                    if (__instance.playerStates is null) return;
                    foreach (PlayerVoteArea pva in __instance.playerStates)
                    {
                        if (pva is null) continue;
                        pva.Background.color = Palette.White;
                    }
                }, "VoteAreaUI");
            }
        }
    }
}