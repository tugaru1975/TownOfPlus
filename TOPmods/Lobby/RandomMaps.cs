using HarmonyLib;
using System.Collections.Generic;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public class GameStartRandomMap
    {
        public static void Prefix()
        {
            if (main.RandomMaps.Value && GameState.IsHost)
            {
                var rand = new System.Random();
                List<byte> RandomMaps = new();
                /*TheSkeld   = 0
                  MIRAHQ     = 1
                  Polus      = 2
                  Dleks      = 3
                  TheAirShip = 4*/
                if (main.AddTheSkeld.Value) RandomMaps.Add(0);
                if (main.AddMIRAHQ.Value) RandomMaps.Add(1);
                if (main.AddPolus.Value) RandomMaps.Add(2);
                if (main.AddAirShip.Value) RandomMaps.Add(4);
                if (RandomMaps.Count != 0)
                {
                    var MapsId = RandomMaps[rand.Next(RandomMaps.Count)];
                    PlayerControl.GameOptions.MapId = MapsId;
                    PlayerControl.GameOptions.SyncSettings();
                }
            }
        }
    }
}
