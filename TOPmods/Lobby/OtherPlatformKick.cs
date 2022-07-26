using HarmonyLib;
using System.Collections.Generic;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public class OtherPlatformPkick
    {
        public static List<int> KickedList = new();
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] InnerNet.ClientData client)
        {
            if (GameState.IsHost && main.OPkick.Getbool())
            {
                List<Platforms> KickList = new();
                if (main.AddUnknown.Getbool())
                    KickList.Add(Platforms.Unknown);
                if (main.AddEpicPC.Getbool())
                    KickList.Add(Platforms.StandaloneEpicPC);
                if (main.AddSteamPC.Getbool())
                    KickList.Add(Platforms.StandaloneSteamPC);
                if (main.AddMac.Getbool())
                    KickList.Add(Platforms.StandaloneMac);
                if (main.AddWin10.Getbool())
                    KickList.Add(Platforms.StandaloneWin10);
                if (main.AddItch.Getbool())
                    KickList.Add(Platforms.StandaloneItch);
                if (main.AddIPhone.Getbool())
                    KickList.Add(Platforms.IPhone);
                if (main.AddAndroid.Getbool())
                    KickList.Add(Platforms.Android);
                if (main.AddSwitch.Getbool())
                    KickList.Add(Platforms.Switch);
                if (main.AddXbox.Getbool())
                    KickList.Add(Platforms.Xbox);
                if (main.AddPlaystation.Getbool())
                    KickList.Add(Platforms.Playstation);

                if (KickList.Count != 0)
                {
                    __instance.KickPlayer(client.Id, false);
                    var poptext = DestroyableSingleton<HudManager>.Instance;
                    if (poptext != null) poptext.Notifier.AddItem(string.Format(StringNames.PlayerWasKickedBy.GetTranslation(), client.PlayerName.ToString(), "TownOfPlus") + " : " + client.PlatformData.Platform.ToString().TrimAll("Standalone"));
                }
            }
        }
    }
}