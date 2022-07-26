using HarmonyLib;
using System;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch]
    class MeetingHudPatch
    {
        private static Sprite blankNameplate = null;
        public static bool nameplatesChanged = true;

        public static void updateNameplate(PlayerVoteArea pva, byte playerId = Byte.MaxValue)
        {
            blankNameplate ??= HatManager.Instance.GetNamePlateById("nameplate_NoPlate")?.viewData?.viewData?.Image;

            var nameplate = blankNameplate;
            if (!main.HideNameplates.Getbool())
            {
                Helpers.TryGetPlayer(playerId != byte.MaxValue ? playerId : pva.TargetPlayerId, out var p);
                var nameplateId = p?.CurrentOutfit?.NamePlateId;
                nameplate = HatManager.Instance.GetNamePlateById(nameplateId).viewData.viewData.Image;
            }
            pva.Background.sprite = nameplate;
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
        class PlayerVoteAreaCosmetics
        {
            static void Postfix(PlayerVoteArea __instance, GameData.PlayerInfo playerInfo)
            {
                updateNameplate(__instance, playerInfo.PlayerId);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch
        {
            static void Postfix(MeetingHud __instance)
            {
                if (nameplatesChanged)
                {
                    foreach (var pva in __instance.playerStates)
                    {
                        updateNameplate(pva);
                    }
                    nameplatesChanged = false;
                }
            }
        }
    }
}
