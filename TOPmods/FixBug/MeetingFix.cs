using HarmonyLib;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
	class MeetingFix
	{
		static void Postfix(MeetingHud __instance)
		{
			if (!main.FixLeftPlayer.Getbool()) return;
			try
			{
				foreach (var area in __instance.playerStates)
				{
					if (area?.Overlay is null) continue;
					if (!area.TargetPlayerId.TryGetPlayer(out var p) || p.Data?.Disconnected is true || p.Data?.IsDead is true)
					{
						area.Overlay.gameObject.SetActive(true);
					}
				}
			}
			catch { }
		}
	}
}