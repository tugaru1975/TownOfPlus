using BepInEx;
using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    class ModNamePing
    {
        static void Postfix(PingTracker __instance)
        {
            __instance.text.alignment = TMPro.TextAlignmentOptions.TopRight;
            __instance.text.text += main.ModNameText.AddLine();
            __instance.text.text += "https://is.gd/TownOfPlus".SetSize(2).AddLine();
            if (main.DebugMode.Value) __instance.text.text += "デバッグモード".SetColor("FF0000").SetSize(2).AddLine();
            if (main.FakePing.Getbool()) __instance.text.text = __instance.text.text.Replace($"Ping: {AmongUsClient.Instance?.Ping}", $"Ping: {main.SetFakePing.Getint()}");
            var pingpos = __instance.gameObject.GetComponent<AspectPosition>();
            if (main.OldPingPositon.Getbool())
            {
                if (GameState.IsChatActive && !GameState.IsMeeting) pingpos.SetPos(1.75f, null);
                else pingpos.SetPos(1.25f, null);
                Flag.NewFlag("PingPosition");
            }
            else
            {
                Flag.Run(() =>
                {
                    pingpos.SetPos(3.69f, null);
                }, "PingPosition");
            }
        }
    }
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    class ModNameVersion
    {
        static void Postfix(VersionShower __instance)
        {
            __instance.text.alignment = TMPro.TextAlignmentOptions.TopLeft;
            __instance.text.text += main.ModNameText.AddLine();
            __instance.text.text += $"BepInEx v{Paths.BepInExVersion}".AddLine();
        }
    }
}
