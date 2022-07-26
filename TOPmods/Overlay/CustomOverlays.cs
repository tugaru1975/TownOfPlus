using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace TownOfPlus
{
    [Harmony]
    public class CustomOverlays
    {
        private static SpriteRenderer infoUnderlay;
        private static TMPro.TextMeshPro infoOverlayRules;
        private static TMPro.TextMeshPro infoOverlayPlayer;

        public static bool overlayShown = false;
        public static void resetOverlays()
        {
            hideInfoOverlay();
            infoUnderlay?.gameObject.Destroy();
            infoOverlayRules?.gameObject.Destroy();
            infoOverlayPlayer?.gameObject.Destroy();
            infoOverlayRules = infoOverlayPlayer = null;
            infoUnderlay = null;
            overlayShown = false;
        }

        public static bool initializeOverlays()
        {
            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (hudManager == null) return false;

            if (infoUnderlay == null)
            {
                infoUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
                infoUnderlay.transform.localPosition = new Vector3(0f, 0f, -900f);
                infoUnderlay.gameObject.SetActive(true);
                infoUnderlay.name = "infoUnderlay";
                infoUnderlay.enabled = false;
            }

            if (infoOverlayRules == null)
            {
                infoOverlayRules = UnityEngine.Object.Instantiate(hudManager.TaskText, hudManager.transform);
                infoOverlayRules.fontSize = infoOverlayRules.fontSizeMin = infoOverlayRules.fontSizeMax = 0.75f;
                infoOverlayRules.autoSizeTextContainer = false;
                infoOverlayRules.enableWordWrapping = false;
                infoOverlayRules.alignment = TMPro.TextAlignmentOptions.TopLeft;
                infoOverlayRules.transform.position = Vector3.zero;
                infoOverlayRules.transform.localPosition = new Vector3(-1.5f, 0.9f, -910f);
                infoOverlayRules.transform.localScale = Vector3.one * 1.25f;
                infoOverlayRules.color = Palette.White;
                infoOverlayRules.name = "infoOverlayRules";
                infoOverlayRules.enabled = false;
            }

            if (infoOverlayPlayer == null)
            {
                infoOverlayPlayer = UnityEngine.Object.Instantiate(infoOverlayRules, hudManager.transform);
                infoOverlayPlayer.fontSize = infoOverlayPlayer.fontSizeMin = infoOverlayPlayer.fontSizeMax = 1.10f;
                infoOverlayPlayer.outlineWidth += 0.02f;
                infoOverlayPlayer.autoSizeTextContainer = false;
                infoOverlayPlayer.enableWordWrapping = false;
                infoOverlayPlayer.alignment = TMPro.TextAlignmentOptions.TopLeft;
                infoOverlayPlayer.transform.position = Vector3.zero;
                infoOverlayPlayer.transform.localPosition = infoOverlayRules.transform.localPosition + new Vector3(2.75f, 0.1f, 0.0f);
                infoOverlayPlayer.transform.localScale = Vector3.one * 1.25f;
                infoOverlayPlayer.color = Palette.White;
                infoOverlayPlayer.name = "infoOverlayPlayer";
                infoOverlayPlayer.enabled = false;
            }

            return true;
        }

        public static void showInfoOverlay()
        {
            if (overlayShown) return;

            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (PlayerControl.LocalPlayer == null || hudManager == null)
                return;

            if (!initializeOverlays()) return;

            overlayShown = true;

            var parent = hudManager.transform;

            infoUnderlay.transform.SetParent(parent);
            infoOverlayRules.transform.SetParent(parent);
            infoOverlayPlayer.transform.SetParent(parent);

            infoUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
            infoUnderlay.transform.localScale = new Vector3(6f, 5f, 1f);
            infoUnderlay.enabled = true;
            infoOverlayRules.enabled = true;
            infoOverlayPlayer.enabled = true;

            var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
            var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);
            HudManager.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
            {
                infoUnderlay.color = Color.Lerp(underlayTransparent, underlayOpaque, t);
                infoOverlayRules.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
                infoOverlayPlayer.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
            })));
        }

        public static void hideInfoOverlay()
        {
            if (!overlayShown) return;
            overlayShown = false;
            var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
            var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);

            HudManager.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
            {
                if (infoUnderlay != null)
                {
                    infoUnderlay.color = Color.Lerp(underlayOpaque, underlayTransparent, t);
                    if (t >= 1.0f) infoUnderlay.enabled = false;
                }

                if (infoOverlayRules != null)
                {
                    infoOverlayRules.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                    if (t >= 1.0f) infoOverlayRules.enabled = false;
                }

                if (infoOverlayPlayer != null)
                {
                    infoOverlayPlayer.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                    if (t >= 1.0f) infoOverlayPlayer.enabled = false;
                }
            })));
        }

        public static void toggleInfoOverlay()
        {
            if (overlayShown)
                hideInfoOverlay();
            else
                showInfoOverlay();
        }

        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static class CustomOverlayKeybinds
        {
            public static void Postfix(KeyboardJoystick __instance)
            {
                if (!main.CustomOverlay.Getbool()) return;
                if (Input.GetKeyDown(main.CustomOverlayKeyBind.Getkeycode()))
                {
                    toggleInfoOverlay();
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class CustomOverlayUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                if (!main.CustomOverlay.Getbool())
                {
                    resetOverlays();
                    return;
                }
                if (!initializeOverlays()) return;
                if (!overlayShown) return;
                HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
                if (PlayerControl.LocalPlayer == null || hudManager == null)
                    return;

                GameOptionsData o = PlayerControl.GameOptions;
                var op = o.ToString();
                if (main.ShowRolesSetting.Getbool())
                {
                    GetRoleInfo(ref op, StringNames.ScientistRole, RoleTypes.Scientist);
                    GetRoleInfo(ref op, StringNames.GuardianAngelRole, RoleTypes.GuardianAngel);
                    GetRoleInfo(ref op, StringNames.EngineerRole, RoleTypes.Engineer);
                    GetRoleInfo(ref op, StringNames.ShapeshifterRole, RoleTypes.Shapeshifter);
                }
                infoOverlayRules.text = op;

                string PlayerText = "===プレイヤー一覧===".SetSize(1.25f);
                foreach (InnerNet.ClientData Client in AmongUsClient.Instance.allClients.ToArray())
                {
                    try
                    {
                        if (Client?.Character == null) continue;
                        var Platform = $"{Client.PlatformData.Platform}";

                        var FriendCodeText = "";
                        var LevelText = "";
                        if (main.ShowFriendText.Getbool())
                        {
                            var FriendCode = Client.FriendCode.Trim();
                            if (FriendCode != "") FriendCodeText = $" FriendCode : {FriendCode}";
                        }
                        if (main.ShowLevelText.Getbool())
                        {
                            var Level = Client.PlayerLevel;
                            LevelText = $" Lv.{Level + 1}";
                        }
                        var InfoText = LevelText + FriendCodeText;
                        if (InfoText != "") TextPlus.SetSize(ref InfoText, 0.75f);

                        var HEXcolor = Helpers.GetClientColor(Client) ?? "FF000000";
                        
                        var ColorText = Helpers.GetColorName(Client);
                        if (!main.ShowColorName.Getbool() || ColorText == "") ColorText = "■";
                        if (main.FirstColorName.Getbool()) ColorText = ColorText.FirstOrDefault().ToString();

                        var PlayerName = Client.PlayerName.RemoveHTML();
                        if (main.ShowBlockedPlayer.Getbool() && DestroyableSingleton<FriendsListManager>.Instance.IsPlayerBlockedUsername(Client.FriendCode)) TextPlus.SetColor(ref PlayerName, "FF0000");
                        if (main.ShowHostColor.Getbool() && Client.Id == AmongUsClient.Instance.GetHost().Id) TextPlus.SetColor(ref PlayerName, "00FFFF");
                        TextPlus.AddLine(ref PlayerText, $"{ColorText.SetColor(HEXcolor)}{PlayerName} : {Platform.TrimAll("Standalone")}");
                        if (InfoText != "") TextPlus.AddLine(ref PlayerText, InfoText);
                    }
                    catch { }
                }

                infoOverlayPlayer.text = PlayerText;
            }

        }

        public static void GetRoleInfo(ref string op, StringNames rolename, RoleTypes roletype)
        {

            var r = PlayerControl.GameOptions?.RoleOptions;
            if (r.roleRates.ContainsKey(roletype) && r.roleRates[roletype].Chance > 0)
            {
                List<string> RoleList = roletype switch
                {
                    RoleTypes.Scientist => new()
                    {
                        GetRoleText(StringNames.ScientistCooldown, r.ScientistCooldown),
                        GetRoleText(StringNames.ScientistBatteryCharge, r.ScientistBatteryCharge),
                    },
                    RoleTypes.GuardianAngel => new()
                    {
                        GetRoleText(StringNames.GuardianAngelCooldown, r.GuardianAngelCooldown),
                        GetRoleText(StringNames.GuardianAngelDuration, r.ProtectionDurationSeconds),
                        GetRoleText(StringNames.GuardianAngelImpostorSeeProtect, r.ImpostorsCanSeeProtect),
                    },
                    RoleTypes.Engineer => new()
                    {
                        GetRoleText(StringNames.EngineerCooldown, r.EngineerCooldown),
                        GetRoleText(StringNames.EngineerInVentCooldown, r.EngineerInVentMaxTime),
                    },
                    RoleTypes.Shapeshifter => new()
                    {
                        GetRoleText(StringNames.ShapeshifterCooldown, r.ShapeshifterCooldown),
                        GetRoleText(StringNames.ShapeshifterDuration, r.ShapeshifterDuration),
                        GetRoleText(StringNames.ShapeshifterLeaveSkin, r.ShapeshifterLeaveSkin),
                    },
                    _ => new(),
                };
                var roletext = TextPlus.GetTranslation(rolename) + ": " + string.Format(TextPlus.GetTranslation(StringNames.RoleChanceAndQuantity), r.GetNumPerGame(roletype), r.GetChancePerGame(roletype));
                op = op.Replace(roletext, roletext + "\n   " + string.Join("\n   ", RoleList) + "\n");
            }
        }

        public static string GetRoleText(StringNames stringname, float f)
        {
            return TextPlus.GetTranslation(stringname) + ": " + string.Format(TextPlus.GetTranslation(StringNames.SecondsAbbv), f.ToString());
        }

        public static string GetRoleText(StringNames stringname, bool b)
        {
            return TextPlus.GetTranslation(stringname) + ": " + (b ? TextPlus.GetTranslation(StringNames.SettingsOn) : TextPlus.GetTranslation(StringNames.SettingsOff));
        }
    }
}