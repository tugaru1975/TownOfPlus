using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System;
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
            foreach (var op in ModOptionSetting.AllOptions.Where(w => w.Config?.Value is true || w.IsFile))
            {
                if (op.IsFile)
                {
                    if (ModOptionSetting.AllOptions.Any(w => w.Tag == op.Tag && w.IsChild && w.Config?.Value is true))
                    {
                        log($"=====[{op.Title}Option]=====", null, LogType.Log, false);
                    }
                    continue;
                }
                if (op.IsChild)
                {
                    if (!ModOptionSetting.AllOptions.Any(w => w.Tag == op.Tag && !w.IsChild && w.Config?.Value is true)) continue;
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
    }

}
