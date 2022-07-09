using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class StartButton
    {
        public static void Postfix(GameStartManager __instance)
        {
            //常時有効化
            if (main.AlwaysStart.Value) __instance.MinPlayers = 1;

            //開始カウントをチャットUIより上に
            if (main.StartCountText.Value) __instance.GameStartText.gameObject.transform.SetPos(z: -430);

            //即開始/開始キャンセル
            if (GameState.IsHost && GameState.IsCountDown)
            {
                if (Input.GetKeyDown(main.StartCount.Value))
                {
                    __instance.countDownTimer = 0;
                }
                if (Input.GetKeyDown(main.ResetCount.Value))
                {
                    __instance.ResetStartState();
                }
            }
        }
    }
}