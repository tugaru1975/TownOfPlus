using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public class ChangeLobbyCodes
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (!SaveManager.StreamerMode && GameState.IsLocalGame) return;

            if (main.ChangeLobbyCodes.Value)
            {
                __instance.GameRoomNameCode.text = main.SetLobbyCode.Value.SetColor(main.SetCodeColor.Value);
                Flag.NewFlag("ChangeLobbyCodes");
            }
            else
            {
                Flag.Run(() =>
                {
                    __instance.GameRoomNameCode.text = "******";
                }, "ChangeLobbyCodes");
            }
        }
    }
}
