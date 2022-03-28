using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using System;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnhollowerBaseLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;
using Il2CppSystem;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace TownOfPlus
{
    class Hat
    {
        //カウントダウン
        private static int CountRainbow = 1;
        private static int Countblink = 1;
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class Count
        {
            public static void Postfix()
            {
                if (CountRainbow == 0) CountRainbow = 25;
                else CountRainbow -= 1;

                if (Countblink == 0) Countblink = 50;
                else Countblink -= 1;
            }
        }
        //帽子
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //ニット帽
        public class ChangeBeanie
        {
            private static int BeanieColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Beanie = new System.Collections.Generic.List<string>();
                Beanie.Add("hat_Beanie_Black");
                Beanie.Add("hat_Beanie_Blue");
                Beanie.Add("hat_Beanie_Green");
                Beanie.Add("hat_Beanie_Lightblue");
                Beanie.Add("hat_Beanie_LightGreen");
                Beanie.Add("hat_Beanie_LightPurple");
                Beanie.Add("hat_Beanie_Pink");
                Beanie.Add("hat_Beanie_Purple");
                Beanie.Add("hat_Beanie_White");
                Beanie.Add("hat_Beanie_Yellow");
                Beanie.Add("hat_pk04_Beanie");
                if (CountRainbow == 1)
                {
                    if (BeanieColorCount == Beanie.Count - 2) BeanieColorCount = -1;
                    BeanieColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Beanie.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Beanie[BeanieColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //頭巾
        public class Changehood
        {
            private static int HoodColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Hood = new System.Collections.Generic.List<string>();
                Hood.Add("hat_pk02_HeroCap");
                Hood.Add("hat_Herohood_Black");
                Hood.Add("hat_Herohood_Blue");
                Hood.Add("hat_Herohood_Pink");
                Hood.Add("hat_Herohood_Purple");
                Hood.Add("hat_Herohood_Red");
                Hood.Add("hat_Herohood_White");
                Hood.Add("hat_Herohood_Yellow");
                if (CountRainbow == 1)
                {
                    if (HoodColorCount == Hood.Count - 1) HoodColorCount = 0;
                    else HoodColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Hood.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Hood[HoodColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //野球帽子
        public class ChangeBaseball
        {
            private static int BaseballColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Baseball = new System.Collections.Generic.List<string>();
                Baseball.Add("hat_pk01_BaseballCap");
                Baseball.Add("hat_baseball_Black");
                Baseball.Add("hat_baseball_Green");
                Baseball.Add("hat_baseball_Lightblue");
                Baseball.Add("hat_baseball_LightGreen");
                Baseball.Add("hat_baseball_Lilac");
                Baseball.Add("hat_baseball_Orange");
                Baseball.Add("hat_baseball_Pink");
                Baseball.Add("hat_baseball_Purple");
                Baseball.Add("hat_baseball_Red");
                Baseball.Add("hat_baseball_White");
                Baseball.Add("hat_baseball_Yellow");
                if (CountRainbow == 1)
                {
                    if (BaseballColorCount == Baseball.Count - 1) BaseballColorCount = 0;
                    else BaseballColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Baseball.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Baseball[BaseballColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //折り紙
        public class Paperhat
        {
            private static int PaperhatColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Paperhat = new System.Collections.Generic.List<string>();
                Paperhat.Add("hat_paperhat");
                Paperhat.Add("hat_Paperhat_Black");
                Paperhat.Add("hat_Paperhat_Blue");
                Paperhat.Add("hat_Paperhat_Cyan");
                Paperhat.Add("hat_Paperhat_Lightblue");
                Paperhat.Add("hat_Paperhat_Pink");
                Paperhat.Add("hat_Paperhat_Yellow");
                if (CountRainbow == 1)
                {
                    if (PaperhatColorCount == Paperhat.Count - 1) PaperhatColorCount = 0;
                    else PaperhatColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Paperhat.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Paperhat[PaperhatColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //1つ目生物
        public class Slung
        {
            private static int SlungColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Slung = new System.Collections.Generic.List<string>();
                Slung.Add("hat_brainslug");
                Slung.Add("hat_headslug_Purple");
                Slung.Add("hat_headslug_Red");
                Slung.Add("hat_headslug_White");
                Slung.Add("hat_headslug_Yellow");
                if (CountRainbow == 1)
                {
                    if (SlungColorCount == Slung.Count - 1) SlungColorCount = 0;
                    else SlungColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Slung.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Slung[SlungColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //バンダナ
        public class Bandana
        {
            private static int BandanaColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Bandana = new System.Collections.Generic.List<string>();
                Bandana.Add("hat_pk04_Bandana");
                Bandana.Add("hat_Bandana_Blue");
                Bandana.Add("hat_Bandana_Green");
                Bandana.Add("hat_Bandana_Pink");
                Bandana.Add("hat_Bandana_Red");
                Bandana.Add("hat_Bandana_White");
                Bandana.Add("hat_Bandana_Yellow");
                if (CountRainbow == 1)
                {
                    if (BandanaColorCount == Bandana.Count - 1) BandanaColorCount = 0;
                    else BandanaColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Bandana.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Bandana[BandanaColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //DVD
        public class Doctor
        {
            private static int DocColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Doctor = new System.Collections.Generic.List<string>();
                Doctor.Add("hat_stethescope");
                Doctor.Add("hat_Doc_black");
                Doctor.Add("hat_Doc_Orange");
                Doctor.Add("hat_Doc_Purple");
                Doctor.Add("hat_Doc_Red");
                Doctor.Add("hat_Doc_White");
                if (CountRainbow == 1)
                {
                    if (DocColorCount == Doctor.Count - 1) DocColorCount = 0;
                    else DocColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Doctor.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Doctor[DocColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //デュラグ
        public class Dorag
        {
            private static int DoragColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Dorag = new System.Collections.Generic.List<string>();
                Dorag.Add("hat_pk04_Dorag");
                Dorag.Add("hat_Dorag_Black");
                Dorag.Add("hat_Dorag_Desert");
                Dorag.Add("hat_Dorag_Jungle");
                Dorag.Add("hat_Dorag_Purple");
                Dorag.Add("hat_Dorag_Sky");
                Dorag.Add("hat_Dorag_Snow");
                Dorag.Add("hat_Dorag_Yellow");
                if (CountRainbow == 1)
                {
                    if (DoragColorCount == Dorag.Count - 1) DoragColorCount = 0;
                    else DoragColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Dorag.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Dorag[DoragColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //ヘッドホン
        public class Headphone
        {
            private static int HeadphoneColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Headphone = new System.Collections.Generic.List<string>();
                Headphone.Add("hat_pk03_Headphones");
                Headphone.Add("hat_GovtHeadset");
                Headphone.Add("hat_mira_headset_blue");
                Headphone.Add("hat_mira_headset_pink");
                Headphone.Add("hat_mira_headset_yellow");
                if (CountRainbow == 1)
                {
                    if (HeadphoneColorCount == Headphone.Count - 1) HeadphoneColorCount = 0;
                    else HeadphoneColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Headphone.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Headphone[HeadphoneColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //ヘルメット
        public class Hardhat
        {
            private static int HardhatColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Hardhat = new System.Collections.Generic.List<string>();
                Hardhat.Add("hat_hardhat");
                Hardhat.Add("hat_Hardhat_black");
                Hardhat.Add("hat_Hardhat_Blue");
                Hardhat.Add("hat_Hardhat_Green");
                Hardhat.Add("hat_Hardhat_Orange");
                Hardhat.Add("hat_Hardhat_Pink");
                Hardhat.Add("hat_Hardhat_Purple");
                Hardhat.Add("hat_Hardhat_Red");
                Hardhat.Add("hat_Hardhat_White");
                if (CountRainbow == 1)
                {
                    if (HardhatColorCount == Hardhat.Count - 1) HardhatColorCount = 0;
                    else HardhatColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Hardhat.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Hardhat[HardhatColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //ライト
        public class ChangeLight
        {
            private static int LightColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Light = new System.Collections.Generic.List<string>();
                Light.Add("hat_pk06_Lights");
                Light.Add("hat_w21_lights_white");
                if (Countblink == 1)
                {
                    if (LightColorCount == Light.Count - 1) LightColorCount = 0;
                    else LightColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Light.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Light[LightColorCount]);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //雪だるま
        public class ChangeSnowman
        {
            private static int SnowmanColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowHat.Value) return;
                System.Collections.Generic.List<string> Snowman = new System.Collections.Generic.List<string>();
                Snowman.Add("hat_w21_snowman_greenred");
                Snowman.Add("hat_w21_snowman_redgreen");
                Snowman.Add("hat_pk06_Snowman");
                if (Countblink == 1)
                {
                    if (SnowmanColorCount == Snowman.Count - 2) SnowmanColorCount = 0;
                    else SnowmanColorCount += 1;
                }
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if ((Snowman.Contains(p.CurrentOutfit.HatId) && (AmongUsClient.Instance.AmHost || (p == PlayerControl.LocalPlayer))))
                    {
                        p.RpcSetHat(Snowman[SnowmanColorCount]);
                    }
                }
            }
        }

        //[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        //public class ShowSkinName
        //{
        //    public static string Text = "";
        //    public static void Postfix(ChatController __instance)
        //    {
        //        if (!(PlayerControl.LocalPlayer.CurrentOutfit.HatId == Text))
        //        {
        //            __instance.AddChat(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.CurrentOutfit.HatId);
        //            Text = PlayerControl.LocalPlayer.CurrentOutfit.HatId;
        //        }
        //    }
        //}

        //バイザー
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //紙
        public class Stickynote
        {
            private static int StickynoteColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowVisor.Value) return;
                System.Collections.Generic.List<string> Stickynote = new System.Collections.Generic.List<string>();
                Stickynote.Add("visor_pk01_DumStickerVisor");
                Stickynote.Add("visor_Stickynote_Cyan");
                Stickynote.Add("visor_Stickynote_Green");
                Stickynote.Add("visor_Stickynote_Orange");
                Stickynote.Add("visor_Stickynote_Pink");
                Stickynote.Add("visor_Stickynote_Purple");
                if (!(Stickynote.Contains(PlayerControl.LocalPlayer.CurrentOutfit.VisorId))) return;
                if (CountRainbow == 1)
                {
                    if (StickynoteColorCount == Stickynote.Count - 1) StickynoteColorCount = 0;
                    else StickynoteColorCount += 1;
                }

                PlayerControl.LocalPlayer.RpcSetVisor(Stickynote[StickynoteColorCount]);
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //キャンディ
        public class Lolli
        {
            private static int LolliColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowVisor.Value) return;
                System.Collections.Generic.List<string> Lolli = new System.Collections.Generic.List<string>();
                Lolli.Add("visor_LolliBlue");
                Lolli.Add("visor_LolliBrown");
                Lolli.Add("visor_LolliOrange");
                Lolli.Add("visor_LolliRed");
                if (!(Lolli.Contains(PlayerControl.LocalPlayer.CurrentOutfit.VisorId))) return;
                if (CountRainbow == 1)
                {
                    if (LolliColorCount == Lolli.Count - 1) LolliColorCount = 0;
                    else LolliColorCount += 1;
                }

                PlayerControl.LocalPlayer.RpcSetVisor(Lolli[LolliColorCount]);
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        //仮面
        public class Masque
        {
            private static int MasqueColorCount = 1;
            public static void Postfix(HudManager __instance)
            {
                if (!main.RainbowVisor.Value) return;
                System.Collections.Generic.List<string> Masque = new System.Collections.Generic.List<string>();
                Masque.Add("visor_masque_blue");
                Masque.Add("visor_masque_green");
                Masque.Add("visor_masque_red");
                Masque.Add("visor_masque_white");
                if (!(Masque.Contains(PlayerControl.LocalPlayer.CurrentOutfit.VisorId))) return;
                if (CountRainbow == 1)
                {
                    if (MasqueColorCount == Masque.Count - 1) MasqueColorCount = 0;
                    else MasqueColorCount += 1;
                }

                PlayerControl.LocalPlayer.RpcSetVisor(Masque[MasqueColorCount]);
            }
        }

        //[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        //public class ShowVisorName
        //{
        //    public static string Text = "";
        //    public static void Postfix(ChatController __instance)
        //    {
        //        if (!(PlayerControl.LocalPlayer.CurrentOutfit.VisorId == Text))
        //        {
        //            __instance.AddChat(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.CurrentOutfit.VisorId);
        //            Text = PlayerControl.LocalPlayer.CurrentOutfit.VisorId;
        //        }
        //    }
        //}
    }
}