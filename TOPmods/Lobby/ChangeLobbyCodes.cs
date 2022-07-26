using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public class ChangeLobbyCodes
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (GameState.IsLocalGame) return;

            if (SaveManager.StreamerMode && main.ChangeLobbyCodes.Getbool())
            {
                __instance.GameRoomNameCode.text = main.SetLobbyCode.Getstring().SetColor(main.SetCodeColor.Getstring());
                Flag.NewFlag("ChangeLobbyCodes");
            }
            else
            {
                Flag.Run(() =>
                {
                    if (!SaveManager.StreamerMode)__instance.GameRoomNameCode.text = "******";
                }, "ChangeLobbyCodes");
            }
        }
    }
}
