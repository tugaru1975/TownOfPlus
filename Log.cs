using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace TownOfPlus
{
    public static class Log
    {
        [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
        public class SceneLogger
        {
            public static void Postfix()
            {
                IsChange.Run(() =>
                {
                    $"======[{SceneManager.GetActiveScene().name}]======".log(null, LogType.Log, false);
                }, SceneManager.GetActiveScene().name, "ActiveScene");
            }
        }

        public static void DumpLog(bool iscopy = false)
        {
            log("=====[AllOptions]=====", null, LogType.Log, false);
            string modname = "";
            foreach (var mod in IL2CPPChainloader.Instance.Plugins.Values)
            {
                if (!mod.Metadata.Name.Equals(main.Name)) modname += $"[{mod.Metadata.Name} v{mod.Metadata.Version.Clean()}],";
            }
            if (modname != "") modname = "[併用mod]:\n" + modname.TrimEnd(',') + "\n";
            var text = $"[TOP v{main.Version}] [BepInEx v{Paths.BepInExVersion}]\n" +
                modname +
                "[発生したバグ]:\n\n" +
                "[バグが発生した時の状況]:\n\n" +
                "[有効にしている設定]:\n";
            foreach (var op in ModOption.AllOptions.Where(w => w.Getbool() || w.ModType == ModType.File))
            {
                if (op.ModType == ModType.File)
                {
                    if (ModOption.AllOptions.Any(a => a.Tag == op.Tag && a.IsChild && a.Getbool()))
                    {
                        log($"=====[{op.Title}Option]=====", null, LogType.Log, false);
                    }
                    continue;
                }
                if (op.IsChild)
                {
                    if (!ModOption.AllOptions.Any(a => a.Tag == op.Tag && !a.IsChild && a.Getbool())) continue;
                }
                else log($"=====[{op.Title}Option]=====", null, LogType.Log, false);
                text += $"{op.Title},";
                log($"[{op.Title}]" + $" tag:{op.Tag}", null, LogType.Log, false);
            }
            if (iscopy) TextPlus.Clipboard(text);
            else
            {
                var logfile = main.TOPUrl + @$"Log\TownOfPlus-v{main.Version}-{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.log";
                FileInfo file = new(@$"{Environment.CurrentDirectory}\BepInEx\LogOutput.log");
                file.CopyTo(logfile);
                Process.Start("explorer.exe", @$"/select,{logfile}");
                if (PlayerControl.LocalPlayer != null) HudManager.Instance?.Chat?.AddComChat($"{logfile}\nログを保存しました。");
            }
        }

        public static T log<T>(this T log, string text = "", LogType type = LogType.Log, bool line = true, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
        {
            var mes = text is "" or null ? log.ToString() : text + ":" + log.ToString();
            StackFrame stack = new(1);
            mes = DateTime.Now.ToString("[HH:mm:ss] ") + (line && main.DebugMode.Value ? $" \"{stack.GetMethod().ReflectedType.Name}.{stack.GetMethod().Name}\" Called in \"{Path.GetFileName(fileName)}({lineNumber})\"" : "") + (line ? "\n" : "") + mes;
            switch (type)
            {
                case LogType.Log:
                    main.Logger.LogInfo(mes);
                    break;

                case LogType.Error:
                    main.Logger.LogError(mes);
                    break;

                case LogType.Warning:
                    main.Logger.LogWarning(mes);
                    break;
            }
            return log;
        }

        public static T gamelog<T>(this T log, string note = "")
        {
            if (main.DebugMode.Value)
            {
                if (!note.IsNullOrWhiteSpace()) note += " : ";
                GameLog.LogList.Add(note + log.ToString());
            }
            return log;
        }
    }

    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    class GameLog
    {
        public static List<string> LogList = new();
        public static List<TMPro.TextMeshPro> LogsList = new();
        public static TMPro.TextMeshPro text;
        public static void Postfix(ModManager __instance)
        {
            while (LogList.Count > 0)
            {
                var ob = UnityEngine.Object.Instantiate(text, null);
                ob.gameObject.SetActive(true);
                ob.gameObject.layer = 5;
                ob.text = LogList[0];
                ob.gameObject.DontDestroyOnLoad();
                var show = new Color(1f, 1f, 0f, 0f);
                var hide = new Color(1f, 1f, 0f, 1f);
                ModManager.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t => ob.color = Color.Lerp(show, hide, t))));
                new LateTask(() =>
                {
                    ModManager.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
                    {
                        ob.color = Color.Lerp(hide, show, t);
                        if (t >= 1.0f) ob.gameObject.Destroy();
                    })));
                }, 5);
                LogList.RemoveAt(0);
                LogsList.Insert(0, ob);
            }
            for (int i = 0; i < LogsList.Count; i++)
            {
                var ob = LogsList[i];
                if (ob != null)
                {
                    ob.transform.position = AspectPosition.ComputeWorldPosition(__instance.localCamera, AspectPosition.EdgeAlignments.Center, new Vector3(0, 2.75f - (0.25f * i), __instance.localCamera.nearClipPlane + 0.1f));
                }
                else LogsList.RemoveAt(i);
            }
        }

        public static void SetText(MainMenuManager __instance)
        {
            if (text != null) return;
            text = UnityEngine.Object.Instantiate(__instance.Announcement.AnnounceTextMeshPro, null);
            text.alignment = TMPro.TextAlignmentOptions.Center;
            text.enableWordWrapping = false;
            text.fontSizeMax = text.fontSizeMin = text.fontSize = 2f;
            text.gameObject.SetActive(false);
            text.gameObject.DontDestroyOnLoad();
        }
    }
}
