using BepInEx.IL2CPP;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnhollowerBaseLib;
using UnityEngine;


namespace TownOfPlus
{
    public static class Helpers
    {
        public static Texture2D LoadTextureFromDisk(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                    byte[] byteTexture = File.ReadAllBytes(path);
                    LoadImage(texture, byteTexture, false);
                    return texture;
                }
            }
            catch { }
            return null;
        }

        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        private static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2cppArray = (Il2CppStructArray<byte>)data;
            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }

        public static Sprite CreateSkinSprite(string path)
        {
            Texture2D texture = LoadTextureFromDisk(path);
            if (texture == null)
                return null;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.53f, 0.575f), texture.width * 0.375f);
            if (sprite == null)
                return null;
            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

        public static Sprite CreateNamePlateSprite(string path)
        {
            Texture2D texture = LoadTextureFromDisk(path);
            if (texture == null)
                return null;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.505f, 0.4825f), texture.width * 0.375f);
            if (sprite == null)
                return null;
            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

        public static void SyncSettings(this GameOptionsData data) =>
            PlayerControl.LocalPlayer.RpcSyncSettings(data);

        public static bool TryGetPlayer(this byte id, out PlayerControl p)
        {
            p = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(f => f?.PlayerId == id);
            if (p == null) return false;
            return true;
        }

        public static bool TryGetClient(this byte id, out ClientData client)
        {
            client = AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(f => f.Character?.PlayerId == id);
            if (client == null) return false;
            return true;
        }
        

        public static bool TryNamePlayer(string name, out string playername, out PlayerControl pc)
        {
            playername = name;
            pc = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(f => f.Data.PlayerName.RemoveHTML() == name);
            if (pc == null) return false;
            return true;
        }

        public static Color GetPlayerRoleColor(RoleTypes RoleType) => RoleType switch
        {
            RoleTypes.Impostor or RoleTypes.Shapeshifter => Palette.ImpostorRed,
            RoleTypes.Engineer or RoleTypes.Scientist or RoleTypes.GuardianAngel => Palette.CrewmateBlue,
            _ => Palette.White,
        };

        public static bool TryGetPlayerColor(int colorid, out Color32 color)
        {
            color = new Color32();
            if (Palette.PlayerColors.Length > colorid)
            {
                color = Palette.PlayerColors[colorid];
                return true;
            }
            return false;
        }

        public static bool TryGetPlayerName(int colorid, out string color)
        {
            color = "";
            if (Palette.ColorNames.Length > colorid)
            {
                color = Palette.ColorNames[colorid].GetTranslation();
                return true;
            }
            return false;
        }

        public static string GetClientColor(ClientData client) =>
            TryGetPlayerColor(client.ColorId, out var color) ? ColorToHex(color) : null;
        

        public static string ColorToHex(Color32 color) =>
            color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");

        public static string GetColorName(ClientData client) =>
            TryGetPlayerName(client.ColorId, out var color) ? color : "";

        public static void DMChat(ClientData client, string title, string text)
        {
            StartRpc($"{title}\n{text}", PlayerControl.LocalPlayer.NetId, RpcCalls.SendChat, client.Id);
        }

        public static void StartRpc(string value, uint targetid, RpcCalls rpccalls, int targetClientid = -1)
        {
            var msg = AmongUsClient.Instance.StartRpcImmediately(targetid, (byte)rpccalls, SendOption.None, targetClientid);
            msg.Write(value);
            AmongUsClient.Instance.FinishRpcImmediately(msg);
        }

        public static Vector3 ScreenToMousePositon =>
            Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.localPosition;

        public static void SetPos(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(x ?? pos.x, y ?? pos.y, z ?? pos.z);
        }

        public static void SetPos(this AspectPosition aspectposition, float? x = null, float? y = null, float? z = null)
        {
            var pos = aspectposition.DistanceFromEdge;
            aspectposition.DistanceFromEdge = new Vector3(x ?? pos.x, y ?? pos.y, z ?? pos.z);
        }

        public static void SetSc(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            var pos = transform.localScale;
            transform.localScale = new Vector3(x ?? pos.x, y ?? pos.y, z ?? pos.z);
        }

        public static bool TryGetFileText(string FileUrl, out List<string[]> list)
        {
            list = new();
            if (File.Exists(FileUrl))
            {
                try
                {
                    using StreamReader sr = new(FileUrl);
                    while (!sr.EndOfStream)
                    {
                        list.Add(sr.ReadLine().Split(','));
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        public static KeyCode GetKey()
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code))
                {
                    return code;
                }
            }
            return KeyCode.None;
        }

        public static void Destroy(this GameObject ob)
        {
            UnityEngine.Object.Destroy(ob);
        }

        public static bool Contains(this IEnumerable<string> list, string value, StringComparison stringcomparison)
        {
            return list.Any(a => a.Equals(value, stringcomparison));
        }

        public static bool ModContains(string mod)
        {
            foreach (var p in IL2CPPChainloader.Instance.Plugins)
            {
                if (p.Value.Metadata.Name == mod)
                {
                    return true;
                }
            }
            return false;
        }
    }

    class LateTask
    {
        public float timer;
        public Action action;
        public static List<LateTask> Timers = new();
        public bool Run(float deltaTime)
        {
            timer -= deltaTime;
            if (timer <= 0)
            {
                action();
                return true;
            }
            return false;
        }

        public LateTask(Action action, float timer = 0f)
        {
            this.action = action;
            this.timer = timer;
            Timers.Add(this);
        }

        public static void Update(float deltaTime)
        {
            List<LateTask> TimersToRemove = new();
            Timers.ForEach((Timer) =>
            {
                if (Timer.Run(deltaTime))
                {
                    TimersToRemove.Add(Timer);
                }
            });
            TimersToRemove.ForEach(Timer => Timers.Remove(Timer));
        }
    }
    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    class LateTaskUpdate
    {
        public static void Postfix()
        {
            LateTask.Update(Time.deltaTime);
        }
    }

    public static class Flag
    {
        private static List<string> OneTimeList = new();
        private static List<string> FirstRunList = new();
        public static void Run(Action action, string type, bool firstrun = false)
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

        public static void DeleteFlag(string type)
        {
            if (OneTimeList.Contains(type)) OneTimeList.Remove(type);
        }
    }

    public static class IsChange
    {
        private static Dictionary<string, object> Dic = new();
        private static List<string> SkipList = new();
        public static void Run(Action action, object obj, string type, bool firstrun = false)
        {
            if (!Dic.ContainsKey(type))
            {
                Dic.Add(type, obj);
                if (SkipList.Contains(type)) SkipList.Remove(type);
                else if (firstrun) action();
                return;
            }
            if (!Dic[type].Equals(obj))
            {
                Dic[type] = obj;
                if (SkipList.Contains(type)) SkipList.Remove(type);
                else action();
            }
        }

        public static void SkipRun(string type)
        {
            if (!SkipList.Contains(type)) SkipList.Add(type);
        }
    }
    public static class TextPlus
    {
        public static string AddLine(ref string text, string addtext) => text += "\n" + addtext;

        public static string AddLine(this string text) => "\n" + text;

        public static string SetColor(ref string text, string color) => text = text.SetColor(color);

        public static string SetColor(this string text, string color) => $"<color=#{color}>" + text + "</color>";

        public static string SetSize(ref string text, float size) => text = text.SetSize(size);

        public static string SetSize(this string text, float size) => $"<size={size}>" + text + "</size>";

        public static string RemoveHTML(this string text) => Regex.Replace(text.TrimAll("\n", "\r"), "<[^>]*?>", "");

        public static string TrySubstring(this string text, int startindex, int? length = null)
        {
            if (text.Length > startindex)
            {
                if (length == null)
                    return text.Substring(startindex);
                if (text.Length >= startindex + length)
                    return text.Substring(startindex, (int)length);
            }
            return text;
        }

        public static string Clipboard(string text = null)
        {
            if (text != null) GUIUtility.systemCopyBuffer = text;
            return GUIUtility.systemCopyBuffer;
        }

        public static string TrimAll(this string text, params string[] oldValue)
        {
            oldValue.ToList().ForEach(f => text = text.Replace(f, ""));
            return text;
        }

        public static void AddComChat(this ChatController __instance, string text)
        {
            ChatCommandUI.IsChatCommand = true;
            __instance.AddChat(PlayerControl.LocalPlayer, text);
            ChatCommandUI.IsChatCommand = false;
        }

        public static void RpcSendChat(string text)
        {
            if (GameState.IsCanSendChat)
            {
                HudManager.Instance.Chat.TimeSinceLastMessage = 0f;
                PlayerControl.LocalPlayer.RpcSendChat(text);
            }
        }

        public static string GetTranslation(this StringNames name) => DestroyableSingleton<TranslationController>.Instance.GetString(name, new Il2CppReferenceArray<Il2CppSystem.Object>(0));

        public static char ComWord => main.ChangeComWord.Value ? '.' : '/';
    }
    public static class GameState
    {
        public static bool IsLobby => AmongUsClient.Instance?.GameState is InnerNetClient.GameStates.Joined && !IsFreePlay;
        public static bool IsGameStart => AmongUsClient.Instance?.IsGameStarted is true;
        public static bool IsLocalGame => AmongUsClient.Instance?.GameMode is GameModes.LocalGame;
        public static bool IsHost => AmongUsClient.Instance?.AmHost is true;
        public static bool IsFreePlay => AmongUsClient.Instance?.GameMode is GameModes.FreePlay;
        public static bool IsMeeting => MeetingHud.Instance != null;
        public static bool IsShip => ShipStatus.Instance != null;
        public static bool IsChatOpen => HudManager.Instance?.Chat?.IsOpen is true;
        public static bool IsChatActive => HudManager.Instance?.Chat?.isActiveAndEnabled is true;
        public static bool IsCanSendChat => HudManager.Instance?.Chat?.TimeSinceLastMessage >= 3f;
        public static bool IsFocusChatArea => HudManager.Instance?.Chat?.TextArea?.hasFocus is true;
        public static bool IsCanKeyCommand => IsFocusChatArea && main.KeyCommand.Value;
        public static bool IsCanMove => PlayerControl.LocalPlayer?.CanMove is true;
        public static bool IsCountDown => GameStartManager.Instance?.startState is GameStartManager.StartingStates.Countdown;
        public static bool IsDead => PlayerControl.LocalPlayer?.Data?.IsDead is true;
    }
}
