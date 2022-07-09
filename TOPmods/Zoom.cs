using HarmonyLib;
using UnityEngine;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Zoom
    {
        public static void Postfix()
        {
            if (main.Zoom.Value && GameState.IsShip && !GameState.IsMeeting && GameState.IsCanMove)
            {
                if (Input.mouseScrollDelta.y > 0)
                {
                    if (Camera.main.orthographicSize > 3.0f)
                    {
                        SetZoomSize(times: false);
                    }

                }
                if (Input.mouseScrollDelta.y < 0)
                {
                    if (GameState.IsDead || GameState.IsFreePlay)
                    {
                        if (Camera.main.orthographicSize < 18.0f)
                        {
                            SetZoomSize(times: true);
                        }
                    }
                }
                Flag.NewFlag("Zoom");
            }
            else
            {
                Flag.Run(() =>
                {
                    SetZoomSize(reset: true);
                }, "Zoom");
            }
        }

        static void SetZoomSize(bool times = false, bool reset = false)
        {
            var size = 1.5f;
            if (!times) size = 1 / size;
            if (reset)
            {
                Camera.main.orthographicSize = 3.0f;
                HudManager.Instance.UICamera.orthographicSize = 3.0f;
                HudManager.Instance.Chat.transform.localScale = Vector3.one;
                if (GameState.IsMeeting) MeetingHud.Instance.transform.localScale = Vector3.one;
            }
            else
            {
                Camera.main.orthographicSize *= size;
                HudManager.Instance.UICamera.orthographicSize *= size;
            }
            ResolutionManager.ResolutionChanged.Invoke(((float)Screen.width / Screen.height));
        }
    }
}
