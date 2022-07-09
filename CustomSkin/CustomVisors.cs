using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TownOfPlus
{
    public static class CustomVisorLoader
    {
        public static bool running = false;
        public static string[] REPO = main.NewVisorURL.Split(',');
        public static List<CustomVisors.CustomVisor> Visordetails = new();
        public static void LaunchVisorFetcher()
        {
            if (running)
                return;
            running = true;
            _ = LaunchVisorFetcherAsync();
        }

        private static async Task LaunchVisorFetcherAsync()
        {
            foreach (var repo in REPO)
            {
                try
                {
                    HttpStatusCode status = await FetchVisors(repo.log("チェック開始"));
                    if (status != HttpStatusCode.OK) repo.log("URLが見つかりませんでした", LogType.Error);
                    else repo.log("チェック終了");
                }
                catch { }
            }
            running = false;
        }

        private static string sanitizeResourcePath(string res)
        {
            if (res == null || !res.EndsWith(".png"))
                return null;

            return res.TrimAll("\\", "/", "*", "..");
        }

        private static bool doesResourceRequireDownload(string respath)
        {
            return !File.Exists(respath);
        }
        public static async Task<HttpStatusCode> FetchVisors(string repo)
        {
            HttpClient http = new();
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

                List<CustomVisors.CustomVisor> Visordatas = new();

                for (JToken current = jobj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        CustomVisors.CustomVisor info = new();

                        info.name = current["name"]?.ToString();
                        info.resource = sanitizeResourcePath(current["resource"]?.ToString());
                        if (info.resource == null || info.name == null) // required
                            continue;
                        info.flipresource = sanitizeResourcePath(current["flipresource"]?.ToString());
                        info.author = current["author"]?.ToString();
                        Visordatas.Add(info);
                    }
                }

                List<string> markedfordownload = new();

                string filePath = main.TOPUrl + @"TOPVisors\";
                foreach (CustomVisors.CustomVisor data in Visordatas)
                {
                    if (doesResourceRequireDownload(filePath + data.resource))
                        markedfordownload.Add(data.resource);
                    if (data.flipresource != null && doesResourceRequireDownload(filePath + data.flipresource))
                        markedfordownload.Add(data.flipresource);
                }

                foreach (var file in markedfordownload)
                {

                    var visorFileResponse = await http.GetAsync($"{repo}/Visors/{file}", HttpCompletionOption.ResponseContentRead);
                    if (visorFileResponse.StatusCode != HttpStatusCode.OK)
                    {
                        file.log($"[{repo}]ダウンロード失敗", LogType.Error);
                        continue;
                    }
                    else file.log($"[{repo}]ダウンロード成功");

                    using (var responseStream = await visorFileResponse.Content.ReadAsStreamAsync())
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

    [HarmonyPatch]
    public class CustomVisors
    {
        public class CustomVisor
        {
            public string author { get; set; }
            public string name { get; set; }
            public string resource { get; set; }
            public string flipresource { get; set; }
        }

        public static bool isAdded = false;
        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetVisorById))]
        class UnlockedVisorPatch
        {
            public static void Prefix(HatManager __instance)
            {
                if (isAdded) return;
                isAdded = true;
                try
                {
                    try
                    {
                        string filePath = main.TOPUrl + @"SkinTest\VisorTest";
                        DirectoryInfo d = new(filePath);
                        string[] filePaths = d.GetFiles("*.png").Select(x => x.FullName).ToArray(); // Getting Text files
                        List<CustomVisor> visors = createCustomVisorDetails(filePaths);
                        if (visors.Count > 0)
                        {
                            foreach (var visor in visors)
                            {
                                __instance.allVisors.Add(CreateVisorData(visor));
                            }
                        }
                    }
                    catch { }

                    while (CustomVisorLoader.Visordetails.Count > 0)
                    {
                        __instance.allVisors.Add(CreateVisorBehaviour(CustomVisorLoader.Visordetails[0]));
                        CustomVisorLoader.Visordetails.RemoveAt(0);
                    }
                }
                catch { }
            }
        }

        private static VisorData CreateVisorBehaviour(CustomVisor cvd)
        {
            string filePath = main.TOPUrl + @"TOPVisors\";
            cvd.resource = filePath + cvd.resource;
            if (cvd.flipresource != null)
                cvd.flipresource = filePath + cvd.flipresource;
            return CreateVisorData(cvd);
        }

        private static VisorData CreateVisorData(CustomVisor cv)
        {
            var Visor = ScriptableObject.CreateInstance<VisorData>();
            Visor.name = cv.name + "\nby " + cv.author;
            Visor.BundleId = Visor.ProductId = "Visor_TOP_" + cv.name.Replace(' ', '_') + cv.author;
            Visor.viewData.viewData = ScriptableObject.CreateInstance<VisorViewData>(); ;
            Visor.viewData.viewData.IdleFrame = Helpers.CreateSkinSprite(cv.resource);
            if (cv.flipresource != null)
                Visor.viewData.viewData.LeftIdleFrame = Helpers.CreateSkinSprite(cv.flipresource);
            Visor.displayOrder = 99;
            Visor.ChipOffset = new Vector2(0f, 0.2f);
            Visor.Free = true;
            return Visor;
        }

        private static List<CustomVisor> createCustomVisorDetails(string[] visors)
        {
            Dictionary<string, CustomVisor> fronts = new();
            Dictionary<string, string> flips = new();

            for (int i = 0; i < visors.Length; i++)
            {
                string s = visors[i].Substring(visors[i].LastIndexOf("\\") + 1).Split('.')[0];
                string[] p = s.Split('_');

                HashSet<string> options = new();
                for (int j = 1; j < p.Length; j++)
                    options.Add(p[j]);

                if (options.Contains("flip"))
                    flips.Add(p[0], visors[i]);
                else
                {
                    CustomVisor custom = new() { resource = visors[i] };
                    custom.name = p[0].Replace('-', ' ');
                    custom.author = "LocalFile";

                    fronts.Add(p[0], custom);
                }
            }

            List<CustomVisor> customvisors = new();

            foreach (string k in fronts.Keys)
            {
                CustomVisor visor = fronts[k];
                flips.TryGetValue(k, out string fr);
                if (fr != null)
                    visor.flipresource = fr;

                customvisors.Add(visor);
            }

            return customvisors;
        }
    }
}