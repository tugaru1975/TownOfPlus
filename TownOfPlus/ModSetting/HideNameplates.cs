using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;
using System.Collections;
using System;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace TownOfPlus
{
    [HarmonyPatch]
    class MeetingHudPatch
    {
        private static Sprite blankNameplate = null;
        public static bool nameplatesChanged = true;

        public static void updateNameplate(PlayerVoteArea pva, byte playerId = Byte.MaxValue)
        {
            blankNameplate = blankNameplate ?? HatManager.Instance.GetNamePlateById("nameplate_NoPlate")?.viewData?.viewData?.Image;

            var nameplate = blankNameplate;
            if (!main.HideNameplates.Value)
            {
                var p = Helpers.playerById(playerId != Byte.MaxValue ? playerId : pva.TargetPlayerId);
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
