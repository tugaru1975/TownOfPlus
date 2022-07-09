using HarmonyLib;
using System.Collections.Generic;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public class OtherPlatformPkick
    {
        public static List<int> KickedList = new();
        public static void Postfix(GameStartManager __instance)
        {
            if (GameState.IsHost && main.OPkick.Value)
            {
                List<Platforms> KickList = new();
                if (main.AddUnknown.Value)
                    KickList.Add(Platforms.Unknown);
                if (main.AddEpicPC.Value)
                    KickList.Add(Platforms.StandaloneEpicPC);
                if (main.AddSteamPC.Value)
                    KickList.Add(Platforms.StandaloneSteamPC);
                if (main.AddMac.Value)
                    KickList.Add(Platforms.StandaloneMac);
                if (main.AddWin10.Value)
                    KickList.Add(Platforms.StandaloneWin10);
                if (main.AddItch.Value)
                    KickList.Add(Platforms.StandaloneItch);
                if (main.AddIPhone.Value)
                    KickList.Add(Platforms.IPhone);
                if (main.AddAndroid.Value)
                    KickList.Add(Platforms.Android);
                if (main.AddSwitch.Value)
                    KickList.Add(Platforms.Switch);
                if (main.AddXbox.Value)
                    KickList.Add(Platforms.Xbox);
                if (main.AddPlaystation.Value)
                    KickList.Add(Platforms.Playstation);

                if (KickList.Count != 0)
                {
                    foreach (InnerNet.ClientData p in AmongUsClient.Instance.allClients)
                    {
                        if (p?.Id == AmongUsClient.Instance.ClientId) continue;
                        if (KickList.Contains(p.PlatformData.Platform) && !KickedList.Contains(p.Id))
                        {
                            KickedList.Add(p.Id);
                            new LateTask(() => KickedList.Remove(p.Id), 1f);
                            AmongUsClient.Instance.KickPlayer(p.Id, false);
                            var poptext = DestroyableSingleton<HudManager>.Instance;
                            if (poptext != null) poptext.Notifier.AddItem(string.Format(StringNames.PlayerWasKickedBy.GetTranslation(), p.PlayerName.ToString(), "TownOfPlus") + " : " + p.PlatformData.Platform.ToString().TrimAll("Standalone"));
                        }
                    }
                }
            }
        }
    }
}