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
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    //ニット帽
    public class ChangeSkin
    {
        private static int timer = 1;
        private static int BeanieColorCount = 1;
        private static int HoodColorCount = 1;
        private static int BaseballColorCount = 1;
        private static int PaperhatColorCount = 1;
        private static int SlungColorCount = 1;
        private static int BandanaColorCount = 1;
        private static int DoctorColorCount = 1;
        private static int DoragColorCount = 1;
        private static int HeadphoneColorCount = 1;
        private static int HardhatColorCount = 1;
        private static int LightColorCount = 1;
        private static int SnowmanColorCount = 1;
        private static int StickynoteColorCount = 1;
        private static int LolliColorCount = 1;
        private static int MasqueColorCount = 1;
        public static void Postfix(HudManager __instance)
        {
            timer += 1;
            if (main.RainbowHat.Value)
            {
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
                if (CheckTime(true))
                {
                    BeanieColorCount = SkinCount(Beanie, BeanieColorCount, true);
                    RawSetSkin(Beanie, BeanieColorCount, false);
                }
                System.Collections.Generic.List<string> Hood = new System.Collections.Generic.List<string>();
                Hood.Add("hat_pk02_HeroCap");
                Hood.Add("hat_Herohood_Black");
                Hood.Add("hat_Herohood_Blue");
                Hood.Add("hat_Herohood_Pink");
                Hood.Add("hat_Herohood_Purple");
                Hood.Add("hat_Herohood_Red");
                Hood.Add("hat_Herohood_White");
                Hood.Add("hat_Herohood_Yellow");
                if (CheckTime(true))
                {
                    HoodColorCount = SkinCount(Hood, HoodColorCount, false);
                    RawSetSkin(Hood, HoodColorCount, false);
                }
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
                if (CheckTime(true))
                {
                    BaseballColorCount = SkinCount(Baseball, BaseballColorCount, false);
                    RawSetSkin(Baseball, BaseballColorCount, false);
                }
                System.Collections.Generic.List<string> Paperhat = new System.Collections.Generic.List<string>();
                Paperhat.Add("hat_paperhat");
                Paperhat.Add("hat_Paperhat_Black");
                Paperhat.Add("hat_Paperhat_Blue");
                Paperhat.Add("hat_Paperhat_Cyan");
                Paperhat.Add("hat_Paperhat_Lightblue");
                Paperhat.Add("hat_Paperhat_Pink");
                Paperhat.Add("hat_Paperhat_Yellow");
                if (CheckTime(true))
                {
                    PaperhatColorCount = SkinCount(Paperhat, PaperhatColorCount, false);
                    RawSetSkin(Paperhat, PaperhatColorCount, false);
                }
                System.Collections.Generic.List<string> Slung = new System.Collections.Generic.List<string>();
                Slung.Add("hat_brainslug");
                Slung.Add("hat_headslug_Purple");
                Slung.Add("hat_headslug_Red");
                Slung.Add("hat_headslug_White");
                Slung.Add("hat_headslug_Yellow");
                if (CheckTime(true))
                {
                    SlungColorCount = SkinCount(Slung, SlungColorCount, false);
                    RawSetSkin(Slung, SlungColorCount, false);
                }
                System.Collections.Generic.List<string> Bandana = new System.Collections.Generic.List<string>();
                Bandana.Add("hat_pk04_Bandana");
                Bandana.Add("hat_Bandana_Blue");
                Bandana.Add("hat_Bandana_Green");
                Bandana.Add("hat_Bandana_Pink");
                Bandana.Add("hat_Bandana_Red");
                Bandana.Add("hat_Bandana_White");
                Bandana.Add("hat_Bandana_Yellow");
                if (CheckTime(true))
                {
                    BandanaColorCount = SkinCount(Bandana, BandanaColorCount, false);
                    RawSetSkin(Bandana, BandanaColorCount, false);
                }
                System.Collections.Generic.List<string> Doctor = new System.Collections.Generic.List<string>();
                Doctor.Add("hat_stethescope");
                Doctor.Add("hat_Doc_black");
                Doctor.Add("hat_Doc_Orange");
                Doctor.Add("hat_Doc_Purple");
                Doctor.Add("hat_Doc_Red");
                Doctor.Add("hat_Doc_White");
                if (CheckTime(true))
                {
                    DoctorColorCount = SkinCount(Doctor, DoctorColorCount, false);
                    RawSetSkin(Doctor, DoctorColorCount, false);
                }
                System.Collections.Generic.List<string> Dorag = new System.Collections.Generic.List<string>();
                Dorag.Add("hat_pk04_Dorag");
                Dorag.Add("hat_Dorag_Black");
                Dorag.Add("hat_Dorag_Desert");
                Dorag.Add("hat_Dorag_Jungle");
                Dorag.Add("hat_Dorag_Purple");
                Dorag.Add("hat_Dorag_Sky");
                Dorag.Add("hat_Dorag_Snow");
                Dorag.Add("hat_Dorag_Yellow");
                if (CheckTime(true))
                {
                    DoragColorCount = SkinCount(Dorag, DoragColorCount, false);
                    RawSetSkin(Dorag, DoragColorCount, false);
                }
                System.Collections.Generic.List<string> Headphone = new System.Collections.Generic.List<string>();
                Headphone.Add("hat_pk03_Headphones");
                Headphone.Add("hat_GovtHeadset");
                Headphone.Add("hat_mira_headset_blue");
                Headphone.Add("hat_mira_headset_pink");
                Headphone.Add("hat_mira_headset_yellow");
                if (CheckTime(true))
                {
                    HeadphoneColorCount = SkinCount(Headphone, HeadphoneColorCount, false);
                    RawSetSkin(Headphone, HeadphoneColorCount, false);
                }
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
                if (CheckTime(true))
                {
                    HardhatColorCount = SkinCount(Hardhat, HardhatColorCount, false);
                    RawSetSkin(Hardhat, HardhatColorCount, false);
                }
                System.Collections.Generic.List<string> Light = new System.Collections.Generic.List<string>();
                Light.Add("hat_pk06_Lights");
                Light.Add("hat_w21_lights_white");
                if (CheckTime(false))
                {
                    LightColorCount = SkinCount(Light, LightColorCount, false);
                    RawSetSkin(Light, LightColorCount, false);
                }
                System.Collections.Generic.List<string> Snowman = new System.Collections.Generic.List<string>();
                Snowman.Add("hat_w21_snowman_greenred");
                Snowman.Add("hat_w21_snowman_redgreen");
                Snowman.Add("hat_pk06_Snowman");
                if (CheckTime(false))
                {
                    SnowmanColorCount = SkinCount(Snowman, SnowmanColorCount, true);
                    RawSetSkin(Snowman, SnowmanColorCount, false);
                }
            }
            if (main.RainbowVisor.Value)
            {
                System.Collections.Generic.List<string> Stickynote = new System.Collections.Generic.List<string>();
                Stickynote.Add("visor_pk01_DumStickerVisor");
                Stickynote.Add("visor_Stickynote_Cyan");
                Stickynote.Add("visor_Stickynote_Green");
                Stickynote.Add("visor_Stickynote_Orange");
                Stickynote.Add("visor_Stickynote_Pink");
                Stickynote.Add("visor_Stickynote_Purple");
                if (CheckTime(true))
                {
                    StickynoteColorCount = SkinCount(Stickynote, StickynoteColorCount, false);
                    RawSetSkin(Stickynote, StickynoteColorCount, true);
                }
                System.Collections.Generic.List<string> Lolli = new System.Collections.Generic.List<string>();
                Lolli.Add("visor_LolliBlue");
                Lolli.Add("visor_LolliBrown");
                Lolli.Add("visor_LolliOrange");
                Lolli.Add("visor_LolliRed");
                if (CheckTime(true))
                {
                    LolliColorCount = SkinCount(Lolli, LolliColorCount, false);
                    RawSetSkin(Lolli, LolliColorCount, true);
                }
                System.Collections.Generic.List<string> Masque = new System.Collections.Generic.List<string>();
                Masque.Add("visor_masque_blue");
                Masque.Add("visor_masque_green");
                Masque.Add("visor_masque_red");
                Masque.Add("visor_masque_white");
                if (CheckTime(true))
                {
                    MasqueColorCount = SkinCount(Masque, MasqueColorCount, false);
                    RawSetSkin(Masque, MasqueColorCount, true);
                }
            }
            if (timer == 50) timer = 0;
        }
        public static int SkinCount(System.Collections.Generic.List<string> List, int ColorCount, bool flag)
        {
            var Count = ColorCount;
            var ListCount = 1;
            if (flag) ListCount = 2;
            if (ColorCount == List.Count - ListCount) Count = 0;
            else Count += 1;
            return Count;
        }
        public static void RawSetSkin(System.Collections.Generic.List<string> SkinList, int ColorCount, bool visor)
        {
            var p = PlayerControl.LocalPlayer;
            if (visor)
            {
                if (SkinList.Contains(p.CurrentOutfit.VisorId))
                {
                    p.RawSetVisor(SkinList[ColorCount]);
                }
            }
            else
            {
                if (SkinList.Contains(p.CurrentOutfit.HatId))
                {
                    p.RawSetHat(SkinList[ColorCount], p.CurrentOutfit.ColorId);
                }
            }
        }
        public static bool CheckTime(bool Rainbow)
        {
            if (Rainbow)
            {
                if (timer == 25 || timer == 50)
                {
                    return true;
                }
            }
            else
            {
                if (timer == 50)
                {
                    return true;
                }
            }
            return false;
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