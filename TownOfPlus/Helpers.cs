using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Collections;
using UnhollowerBaseLib;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using Hazel;

namespace TownOfPlus {
    public static class Helpers
    {
        public static void destroyList<T>(Il2CppSystem.Collections.Generic.List<T> items) where T : UnityEngine.Object
        {
            if (items == null) return;
            foreach (T item in items)
            {
                UnityEngine.Object.Destroy(item);
            }
        }

        public static void destroyList<T>(List<T> items) where T : UnityEngine.Object
        {
            if (items == null) return;
            foreach (T item in items)
            {
                UnityEngine.Object.Destroy(item);
            }
        }
        public static Texture2D loadTextureFromResources(string path) {
            try {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var byteTexture = new byte[stream.Length];
                var read = stream.Read(byteTexture, 0, (int) stream.Length);
                LoadImage(texture, byteTexture, false);
                return texture;
            } catch {
            }
            return null;
        }

        public static Texture2D loadTextureFromDisk(string path) {
            try {          
                if (File.Exists(path))     {
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                    byte[] byteTexture = File.ReadAllBytes(path);
                    LoadImage(texture, byteTexture, false);
                    return texture;
                }
            } catch {
            }
            return null;
        }

        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        private static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable) {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2cppArray = (Il2CppStructArray<byte>) data;
            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }

        public static Sprite CreateSprite(string path, bool fromDisk = false)
        {
            Texture2D texture = fromDisk ? Helpers.loadTextureFromDisk(path) : Helpers.loadTextureFromResources(path);
            if (texture == null)
                return null;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.53f, 0.575f), texture.width * 0.375f);
            if (sprite == null)
                return null;
            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

        public static void SyncSettings()
        {
            var optByte = PlayerControl.GameOptions.DeepCopy();
            PlayerControl.LocalPlayer.RpcSyncSettings(optByte);
        }
        public static GameOptionsData DeepCopy(this GameOptionsData opt)
        {
            var optByte = opt.ToBytes(5);
            return GameOptionsData.FromBytes(optByte);
        }

        public static PlayerControl playerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;
            return null;
        }

        public static InnerNet.ClientData playerByClient(PlayerControl player)
        {
            var client = AmongUsClient.Instance.allClients.ToArray().Where(cd => cd.Character.PlayerId == player.PlayerId).FirstOrDefault();
            return client;
        }

        public static PlayerControl GetNamePlayer(string name)
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                if (p.Data.PlayerName.DeleteHTML() == name) return p;
            }
            return null;
        }

        public static Color GetPlayerColor(PlayerControl p)
        {
            var RoleType = p.Data.Role.Role;
            if (RoleType == RoleTypes.Impostor || RoleType == RoleTypes.Shapeshifter)
            {
                return Palette.ImpostorRed;
            }
            else
            {
                if (RoleType == RoleTypes.Engineer || RoleType == RoleTypes.Scientist || RoleType == RoleTypes.GuardianAngel)
                {
                    return Palette.CrewmateBlue;
                }
                else
                {
                    return Palette.White;
                }
            }
        }
        public static string GetColorHEX(InnerNet.ClientData Client)
        {
            try
            {
                return ColorToHex(Palette.PlayerColors[Client.ColorId]);
            }
            catch
            {
                return "";
            }
        }
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
            return hex;
        }
        public static string CutString(this string text, int length)
        {
            if (text == null)
            {
                return string.Empty;
            }
            if (length > text.Length)
            {
                return text;
            }
            return text.Substring(0, length);
        }
        public static int AllCommandNum(string text)
        {
            var List = CommandList.AllCommand();
            int Contains = -1;
            for (var i = 0; i < List.Length; i++)
            {
                foreach (var args in List[i].Command)
                {
                    if (args.ToLower() == text.ToLower())
                    {
                        Contains = i;
                        break;
                    }
                }
            }
            return Contains;
        }

        public static bool StringListContains(this string text, string[] stringlist)
        {
            bool Contains = false;
            foreach (var List in stringlist)
            {
                if (text.ToLower() == List.ToLower())
                {
                    Contains = true;
                    break;
                }
            }
            return Contains;
        }
        public static void DMChat(int clientId, string text)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SendChat, SendOption.Reliable, clientId);
            writer.Write(text);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            HudManager.Instance.Chat.TimeSinceLastMessage = 0;
        }

        public static string DeleteHTML(this string name)
        {
            var PlayerName = name.Replace("\n", "").Replace("\r", "");
            while (PlayerName.Contains("<") || PlayerName.Contains(">"))
            {
                PlayerName = PlayerName.Remove(PlayerName.IndexOf("<"), PlayerName.IndexOf(">") - PlayerName.IndexOf("<") + 1);
            }
            return PlayerName;
        }
    }
    class LateTask
    {
        public float timer;
        public Action action;
        public static List<LateTask> Timers = new List<LateTask>();
        public bool run(float deltaTime)
        {
            timer -= deltaTime;
            if (timer <= 0)
            {
                action();
                return true;
            }
            return false;
        }
        public LateTask(Action action, float time)
        {
            this.action = action;
            this.timer = time;
            Timers.Add(this);
        }
        public static void Update(float deltaTime)
        {
            var TimersToRemove = new List<LateTask>();
            Timers.ForEach((Timer) => {
                if (Timer.run(deltaTime))
                {
                    TimersToRemove.Add(Timer);
                }
            });
            TimersToRemove.ForEach(Timer => Timers.Remove(Timer));
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class LateTaskUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            LateTask.Update(Time.deltaTime);
        }
    }

    public static class CreateFlag
    {
        public static List<string> OneTimeList = new List<string>();
        public static List<string> FirstRunList = new List<string>();
        public static void Run (Action action, string type, bool firstrun = false)
        {
            if ((OneTimeList.Contains(type)) || (firstrun && !FirstRunList.Contains(type)))
            {
                if (!FirstRunList.Contains(type)) FirstRunList.Add(type);
                OneTimeList.Remove(type);
                action();
            }
            
        }
        public static void NewFlag(string type)
        {
            if (!OneTimeList.Contains(type)) OneTimeList.Add(type);
        }
    }
}
