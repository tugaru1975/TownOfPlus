using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetMaskLayer))]
    class ChatUI
    {
        public static void Postfix(ChatBubble __instance)
        {
            if (main.CrewColorChat.Getbool() || main.TranslucentChat.Getbool())
            {
                if (!__instance.playerInfo.PlayerId.TryGetClient(out var client)) return;
                if (client.Character?.Data?.IsDead is true) return;
                var chatcolor = __instance.Background.color;
                if (main.CrewColorChat.Getbool() && Helpers.TryGetPlayerColor(client.ColorId, out var color)) chatcolor = color;
                if (main.TranslucentChat.Getbool()) chatcolor.a = (100f - main.SetTranslucentChat.Getint()) / 100f;
                __instance.Background.color = chatcolor;
            }
        }
    }
}