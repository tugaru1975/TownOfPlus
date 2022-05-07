using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using UnityEngine;
using UnhollowerBaseLib;
using Hazel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TownOfPlus
{

    [HarmonyPatch]
    public class CustomVisors
    {

        public class CustomVisor
        {
            public string author { get; set; }
            public string name { get; set; }
            public string resource { get; set; }
            public string reshasha { get; set; }
        }
    }
    public static class CustomVisorLoader
    {
        public static bool IsEndDownload = false;
        public static string[] VisorRepos = main.NewVisorURL.Split(',');
        public static List<CustomVisors.CustomVisor> Visordetails = new List<CustomVisors.CustomVisor>();

        public static void LaunchVisorFetcher()
        {
            IsEndDownload = false;
            foreach (string repo in VisorRepos)
            {
                try
                {
                    _ = FetchVisors(repo);
                }
                catch { }
            }
            IsEndDownload = true;
        }
        private static string sanitizeResourcePath(string res)
        {
            if (res == null || !res.EndsWith(".png"))
                return null;

            res = res.Replace("\\", "")
                     .Replace("/", "")
                     .Replace("*", "")
                     .Replace("..", "");
            return res;
        }
        private static bool doesResourceRequireDownload(string respath, string reshash, MD5 md5)
        {
            if (reshash == null || !File.Exists(respath))
                return true;

            using (var stream = File.OpenRead(respath))
            {
                var hash = System.BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                return !reshash.Equals(hash);
            }
        }
        public static async Task<HttpStatusCode> FetchVisors(string repo)
        {
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            var response = await http.GetAsync(new System.Uri($"{repo}/CustomVisors.json"), HttpCompletionOption.ResponseContentRead);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK) return response.StatusCode;
                if (response.Content == null)
                {
                    return HttpStatusCode.ExpectationFailed;
                }
                string json = await response.Content.ReadAsStringAsync();
                JToken jobj = JObject.Parse(json)["Visors"];
                if (!jobj.HasValues) return HttpStatusCode.ExpectationFailed;

                List<CustomVisors.CustomVisor> Visordatas = new List<CustomVisors.CustomVisor>();

                for (JToken current = jobj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        CustomVisors.CustomVisor info = new CustomVisors.CustomVisor();

                        info.name = current["name"]?.ToString();
                        info.resource = sanitizeResourcePath(current["resource"]?.ToString());
                        if (info.resource == null || info.name == null) // required
                            continue;
                        info.author = current["author"]?.ToString();
                        info.reshasha = current["name"]?.ToString() + "topvisor";
                        Visordatas.Add(info);
                    }
                }

                List<string> markedfordownload = new List<string>();

                string filePath = Path.GetDirectoryName(Application.dataPath) + @"\TownOfPlus\TOPVisors\";
                MD5 md5 = MD5.Create();
                foreach (CustomVisors.CustomVisor data in Visordatas)
                {
                    if (doesResourceRequireDownload(filePath + data.resource, data.reshasha, md5))
                        markedfordownload.Add(data.resource);
                }

                foreach (var file in markedfordownload)
                {

                    var hatFileResponse = await http.GetAsync($"{repo}/Visors/{file}", HttpCompletionOption.ResponseContentRead);
                    if (hatFileResponse.StatusCode != HttpStatusCode.OK) continue;
                    using (var responseStream = await hatFileResponse.Content.ReadAsStreamAsync())
                    {
                        using (var fileStream = File.Create($"{filePath}\\{file}"))
                        {
                            responseStream.CopyTo(fileStream);
                        }
                    }
                }

                Visordetails.AddRange(Visordatas);
            }
            catch { }
            return HttpStatusCode.OK;
        }
    }
    
    public class CustomVisor
    {
        public static bool isAdded = false;
        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetVisorById))]
        class UnlockedVisorPatch
        {
            public static void Postfix(HatManager __instance)
            {
                if (isAdded || !CustomVisorLoader.IsEndDownload) return;
                isAdded = true;
                var path = @"TownOfPlus\TOPVisors\";
                var plateDir = new DirectoryInfo(path);
                if (!plateDir.Exists) plateDir.Create();
                var Files = plateDir.GetFiles("*.png").ToList();
                foreach (var file in Files)
                {
                    try
                    {
                        __instance.allVisors.Add(CreateVisorData(file, path));
                    }
                    catch { }
                }
            }
        }

        private static VisorData CreateVisorData(FileInfo file, string path)
        {
            var Visor = ScriptableObject.CreateInstance<VisorData>();
            var FileName = file.Name.Replace(".png", "");
            var Data = CustomVisorLoader.Visordetails.FirstOrDefault(data => data.resource.Replace(".png", "") == FileName);
            Visor.name = Data.name + "\nby " + Data.author;
            Visor.ProductId = "Visor_TOP_" + Data.resource.Replace(".png", "");
            Visor.BundleId = "Visor_TOP_" + Data.resource.Replace(".png", "");
            Visor.viewData.viewData = ScriptableObject.CreateInstance<VisorViewData>(); ;
            Visor.viewData.viewData.IdleFrame = Helpers.CreateSprite(path + file.Name, true);
            Visor.displayOrder = 99;
            Visor.ChipOffset = new Vector2(0f, 0.2f);
            Visor.Free = true;
            return Visor;
        }
    }
}