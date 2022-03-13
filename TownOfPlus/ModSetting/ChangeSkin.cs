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
    //スキン
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //ニット帽
    public class ChangeBeanie
    {
        private static int BeanieColorCount = 1;
        private static int BeanieCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
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
            if (!(Beanie.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (BeanieCount == 0) BeanieCount = 25;
            else BeanieCount -= 1;
            if (BeanieCount == 1)
            {
                if (BeanieColorCount == Beanie.Count - 2) BeanieColorCount = -1;
                BeanieColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Beanie[BeanieColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //頭巾
    public class Changehood
    {
        private static int HoodColorCount = 1;
        private static int HoodCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Hood = new System.Collections.Generic.List<string>();
            Hood.Add("hat_pk02_HeroCap");
            Hood.Add("hat_Herohood_Black");
            Hood.Add("hat_Herohood_Blue");
            Hood.Add("hat_Herohood_Pink");
            Hood.Add("hat_Herohood_Purple");
            Hood.Add("hat_Herohood_Red");
            Hood.Add("hat_Herohood_White");
            Hood.Add("hat_Herohood_Yellow");
            if (!(Hood.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (HoodCount == 0) HoodCount = 25;
            else HoodCount -= 1;
            if (HoodCount == 1)
            {
                if (HoodColorCount == Hood.Count - 1) HoodColorCount = 0;
                else HoodColorCount += 1;
            }
            
            PlayerControl.LocalPlayer.RpcSetHat(Hood[HoodColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //野球帽子
    public class ChangeBaseball
    {
        private static int BaseballColorCount = 1;
        private static int BaseballCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
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
            if (!(Baseball.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (BaseballCount == 0) BaseballCount = 25;
            else BaseballCount -= 1;
            if (BaseballCount == 1)
            {
                if (BaseballColorCount == Baseball.Count - 1) BaseballColorCount = 0;
                else BaseballColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Baseball[BaseballColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //折り紙
    public class Paperhat
    {
        private static int PaperhatColorCount = 1;
        private static int PaperhatCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Paperhat = new System.Collections.Generic.List<string>();
            Paperhat.Add("hat_paperhat");
            Paperhat.Add("hat_Paperhat_Black");
            Paperhat.Add("hat_Paperhat_Blue");
            Paperhat.Add("hat_Paperhat_Cyan");
            Paperhat.Add("hat_Paperhat_Lightblue");
            Paperhat.Add("hat_Paperhat_Pink");
            Paperhat.Add("hat_Paperhat_Yellow");
            if (!(Paperhat.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (PaperhatCount == 0) PaperhatCount = 25;
            else PaperhatCount -= 1;
            if (PaperhatCount == 1)
            {
                if (PaperhatColorCount == Paperhat.Count - 1) PaperhatColorCount = 0;
                else PaperhatColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Paperhat[PaperhatColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //1つ目生物
    public class Slung
    {
        private static int SlungColorCount = 1;
        private static int SlungCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Slung = new System.Collections.Generic.List<string>();
            Slung.Add("hat_brainslug");
            Slung.Add("hat_headslug_Purple");
            Slung.Add("hat_headslug_Red");
            Slung.Add("hat_headslug_White");
            Slung.Add("hat_headslug_Yellow");
            if (!(Slung.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (SlungCount == 0) SlungCount = 25;
            else SlungCount -= 1;
            if (SlungCount == 1)
            {
                if (SlungColorCount == Slung.Count - 1) SlungColorCount = 0;
                else SlungColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Slung[SlungColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //バンダナ
    public class Bandana
    {
        private static int BandanaColorCount = 1;
        private static int BandanaCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Bandana = new System.Collections.Generic.List<string>();
            Bandana.Add("hat_pk04_Bandana");
            Bandana.Add("hat_Bandana_Blue");
            Bandana.Add("hat_Bandana_Green");
            Bandana.Add("hat_Bandana_Pink");
            Bandana.Add("hat_Bandana_Red");
            Bandana.Add("hat_Bandana_White");
            Bandana.Add("hat_Bandana_Yellow");
            if (!(Bandana.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (BandanaCount == 0) BandanaCount = 25;
            else BandanaCount -= 1;
            if (BandanaCount == 1)
            {
                if (BandanaColorCount == Bandana.Count - 1) BandanaColorCount = 0;
                else BandanaColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Bandana[BandanaColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //DVD
    public class Doctor
    {
        private static int DocColorCount = 1;
        private static int DocCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Doctor = new System.Collections.Generic.List<string>();
            Doctor.Add("hat_stethescope");
            Doctor.Add("hat_Doc_black");
            Doctor.Add("hat_Doc_Orange");
            Doctor.Add("hat_Doc_Purple");
            Doctor.Add("hat_Doc_Red");
            Doctor.Add("hat_Doc_White");
            if (!(Doctor.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (DocCount == 0) DocCount = 25;
            else DocCount -= 1;
            if (DocCount == 1)
            {
                if (DocColorCount == Doctor.Count - 1) DocColorCount = 0;
                else DocColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Doctor[DocColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //デュラグ
    public class Dorag
    {
        private static int DoragColorCount = 1;
        private static int DoragCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Dorag = new System.Collections.Generic.List<string>();
            Dorag.Add("hat_pk04_Dorag");
            Dorag.Add("hat_Dorag_Black");
            Dorag.Add("hat_Dorag_Desert");
            Dorag.Add("hat_Dorag_Jungle");
            Dorag.Add("hat_Dorag_Purple");
            Dorag.Add("hat_Dorag_Sky");
            Dorag.Add("hat_Dorag_Snow");
            Dorag.Add("hat_Dorag_Yellow");
            if (!(Dorag.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (DoragCount == 0) DoragCount = 25;
            else DoragCount -= 1;
            if (DoragCount == 1)
            {
                if (DoragColorCount == Dorag.Count - 1) DoragColorCount = 0;
                else DoragColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Dorag[DoragColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //ヘッドホン
    public class Headphone
    {
        private static int HeadphoneColorCount = 1;
        private static int HeadphoneCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Headphone = new System.Collections.Generic.List<string>();
            Headphone.Add("hat_pk03_Headphones");
            Headphone.Add("hat_GovtHeadset");
            Headphone.Add("hat_mira_headset_blue");
            Headphone.Add("hat_mira_headset_pink");
            Headphone.Add("hat_mira_headset_yellow");
            if (!(Headphone.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (HeadphoneCount == 0) HeadphoneCount = 25;
            else HeadphoneCount -= 1;
            if (HeadphoneCount == 1)
            {
                if (HeadphoneColorCount == Headphone.Count - 1) HeadphoneColorCount = 0;
                else HeadphoneColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Headphone[HeadphoneColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //ヘルメット
    public class Hardhat
    {
        private static int HardhatColorCount = 1;
        private static int HardhatCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
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
            if (!(Hardhat.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (HardhatCount == 0) HardhatCount = 25;
            else HardhatCount -= 1;
            if (HardhatCount == 1)
            {
                if (HardhatColorCount == Hardhat.Count - 1) HardhatColorCount = 0;
                else HardhatColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Hardhat[HardhatColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //ライト
    public class ChangeLight
    {
        private static int LightColorCount = 1;
        private static int LightCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Light = new System.Collections.Generic.List<string>();
            Light.Add("hat_pk06_Lights");
            Light.Add("hat_w21_lights_white");
            if (!(Light.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (LightCount == 0) LightCount = 50;
            else LightCount -= 1;
            if (LightCount == 1)
            {
                if (LightColorCount == Light.Count - 1) LightColorCount = 0;
                else LightColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Light[LightColorCount]);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //雪だるま
    public class ChangeSnowman
    {
        private static int SnowmanColorCount = 1;
        private static int SnowmanCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowSkin.Value) return;
            System.Collections.Generic.List<string> Snowman = new System.Collections.Generic.List<string>();
            Snowman.Add("hat_w21_snowman_greenred");
            Snowman.Add("hat_w21_snowman_redgreen");
            Snowman.Add("hat_pk06_Snowman");
            if (!(Snowman.Contains(PlayerControl.LocalPlayer.CurrentOutfit.HatId))) return;
            if (SnowmanCount == 0) SnowmanCount = 50;
            else SnowmanCount -= 1;
            if (SnowmanCount == 1)
            {
                if (SnowmanColorCount == Snowman.Count - 2) SnowmanColorCount = 0;
                else SnowmanColorCount += 1;
            }

            PlayerControl.LocalPlayer.RpcSetHat(Snowman[SnowmanColorCount]);
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
        private static int StickynoteCount = 1;
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
            if (StickynoteCount == 0) StickynoteCount = 25;
            else StickynoteCount -= 1;
            if (StickynoteCount == 1)
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
        private static int LolliCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowVisor.Value) return;
            System.Collections.Generic.List<string> Lolli = new System.Collections.Generic.List<string>();
            Lolli.Add("visor_LolliBlue");
            Lolli.Add("visor_LolliBrown");
            Lolli.Add("visor_LolliOrange");
            Lolli.Add("visor_LolliRed");
            if (!(Lolli.Contains(PlayerControl.LocalPlayer.CurrentOutfit.VisorId))) return;
            if (LolliCount == 0) LolliCount = 25;
            else LolliCount -= 1;
            if (LolliCount == 1)
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
        private static int MasqueCount = 1;
        public static void Postfix(HudManager __instance)
        {
            if (!main.RainbowVisor.Value) return;
            System.Collections.Generic.List<string> Masque = new System.Collections.Generic.List<string>();
            Masque.Add("visor_masque_blue");
            Masque.Add("visor_masque_green");
            Masque.Add("visor_masque_red");
            Masque.Add("visor_masque_white");
            if (!(Masque.Contains(PlayerControl.LocalPlayer.CurrentOutfit.VisorId))) return;
            if (MasqueCount == 0) MasqueCount = 25;
            else MasqueCount -= 1;
            if (MasqueCount == 1)
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