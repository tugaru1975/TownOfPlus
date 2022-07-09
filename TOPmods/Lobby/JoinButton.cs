using HarmonyLib;
using System.Text.RegularExpressions;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(JoinGameButton), nameof(JoinGameButton.OnClick))]
    class JoinButton
    {
        public static void Prefix(JoinGameButton __instance)
        {
            if (!main.PasteCodeText.Value && !main.CodeTextPlus.Value) return;
            try
            {
                var text = __instance.GameIdText.text;
                var cbt = TextPlus.Clipboard();
                if (main.PasteCodeText.Value && Regex.IsMatch(cbt, "[A-Z]{6}"))
                {
                    if (text == "") __instance.GameIdText.SetText(cbt);
                }
                if (main.CodeTextPlus.Value && __instance.GameIdText.text.Length == 5)
                {
                    __instance.GameIdText.SetText(text + DestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName switch
                    {
                        StringNames.ServerNA => "G",
                        StringNames.ServerEU => "F",
                        StringNames.ServerAS => "F",
                        _ => "",
                    });
                }
            }
            catch { }
        }
    }
}