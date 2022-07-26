using HarmonyLib;
using System.Text.RegularExpressions;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(JoinGameButton), nameof(JoinGameButton.OnClick))]
    class JoinButton
    {
        public static void Prefix(JoinGameButton __instance)
        {
            if (!main.PasteCodeText.Getbool() && !main.CodeTextPlus.Getbool()) return;
            try
            {
                var text = __instance.GameIdText.text;
                var cbt = TextPlus.Clipboard();
                if (main.PasteCodeText.Getbool() && Regex.IsMatch(cbt, "[A-Z]{6}"))
                {
                    if (text == "") __instance.GameIdText.SetText(cbt);
                }
                if (main.CodeTextPlus.Getbool() && __instance.GameIdText.text.Length == 5)
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