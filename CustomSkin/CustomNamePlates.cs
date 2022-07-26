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
    public static class CustomNamePlateLoader
    {
        public static bool running = false;
        public static string[] REPO = main.NewNamePlateURL.Split(',');
        public static List<CustomNamePlates.CustomNamePlate> NamePlatedetails = new();
        public static void LaunchNamePlateFetcher()
        {
            if (running)
                return;
            running = true;
            _ = LaunchNamePlateFetcherAsync();
        }

        private static async Task LaunchNamePlateFetcherAsync()
        {
            foreach (var repo in REPO)
            {
                try
                {
                    HttpStatusCode status = await FetchNamePlates(repo.log("チェック開始"));
                    if (status != HttpStatusCode.OK) repo.log("URLが見つかりませんでした", LogType.Error);
                    else repo.log("チェック終了").gamelog("ネームプレートダウンロード終了");
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
        public static async Task<HttpStatusCode> FetchNamePlates(string repo)
        {
            HttpClient http = new();
            http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            var response = await http.GetAsync(new System.Uri($"{repo}/CustomNamePlates.json"), HttpCompletionOption.ResponseContentRead);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK) return response.StatusCode;
                if (response.Content == null)
                {
                    return HttpStatusCode.ExpectationFailed;
                }
                string json = await response.Content.ReadAsStringAsync();
                JToken jobj = JObject.Parse(json)["nameplates"];
                if (!jobj.HasValues) return HttpStatusCode.ExpectationFailed;

                List<CustomNamePlates.CustomNamePlate> NamePlatedatas = new();

                for (JToken current = jobj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        CustomNamePlates.CustomNamePlate info = new();

                        info.name = current["name"]?.ToString();
                        info.resource = sanitizeResourcePath(current["resource"]?.ToString());
                        if (info.resource == null || info.name == null) // required
                            continue;
                        info.author = current["author"]?.ToString();
                        NamePlatedatas.Add(info);
                    }
                }

                List<string> markedfordownload = new();

                string filePath = main.TOPUrl + @"TOPNamePlates\";
                foreach (CustomNamePlates.CustomNamePlate data in NamePlatedatas)
                {
                    if (doesResourceRequireDownload(filePath + data.resource))
                        markedfordownload.Add(data.resource);
                }

                foreach (var file in markedfordownload)
                {

                    var nameplateFileResponse = await http.GetAsync($"{repo}/NamePlates/{file}", HttpCompletionOption.ResponseContentRead);
                    if (nameplateFileResponse.StatusCode != HttpStatusCode.OK)
                    {
                        file.log($"[{repo}]ダウンロード失敗", LogType.Error);
                        continue;
                    }
                    else file.log($"[{repo}]ダウンロード成功");

                    using (var responseStream = await nameplateFileResponse.Content.ReadAsStreamAsync())
                    {
                        using (var fileStream = File.Create($"{filePath}\\{file}"))
                        {
                            responseStream.CopyTo(fileStream);
                        }
                    }
                }

                NamePlatedetails.AddRange(NamePlatedatas);
            }
            catch { }
            return HttpStatusCode.OK;
        }
    }

    [HarmonyPatch]
    public class CustomNamePlates
    {
        public class CustomNamePlate
        {
            public string author { get; set; }
            public string name { get; set; }
            public string resource { get; set; }
        }

        private static bool RUNNING = false;
        private static bool isadded = false;
        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
        class UnlockedNamePlatePatch
        {
            public static void Prefix(HatManager __instance)
            {
                if (RUNNING || isadded) return;
                RUNNING = true;
                try
                {
                    try
                    {
                        string filePath = main.TOPUrl + @"SkinTest\NamePlateTest";
                        DirectoryInfo d = new(filePath);
                        string[] filePaths = d.GetFiles("*.png").Select(x => x.FullName).ToArray(); // Getting Text files
                        List<CustomNamePlate> hats = createCustomNamePlateDetails(filePaths);
                        if (hats.Count > 0)
                        {
                            foreach (var hat in hats)
                            {
                                __instance.allNamePlates.Add(CreateNamePlateData(hat));
                            }
                        }
                    }
                    catch { }

                    while (CustomNamePlateLoader.NamePlatedetails.Count > 0)
                    {
                        isadded = true;
                        __instance.allNamePlates.Add(CreateNamePlateBehaviour(CustomNamePlateLoader.NamePlatedetails[0]));
                        CustomNamePlateLoader.NamePlatedetails.RemoveAt(0);
                    }
                }
                catch { }
            }
            static void Postfix(HatManager __instance)
            {
                RUNNING = false;
            }
        }

        private static NamePlateData CreateNamePlateBehaviour(CustomNamePlate cvd)
        {
            string filePath = main.TOPUrl + @"TOPNamePlates\";
            cvd.resource = filePath + cvd.resource;
            return CreateNamePlateData(cvd);
        }

        private static NamePlateData CreateNamePlateData(CustomNamePlate cv)
        {
            var NamePlate = ScriptableObject.CreateInstance<NamePlateData>();
            NamePlate.name = cv.name + "\nby " + cv.author;
            NamePlate.BundleId = NamePlate.ProductId = "NamePlate_TOP_" + cv.name.Replace(' ', '_') + cv.author;
            NamePlate.viewData.viewData = ScriptableObject.CreateInstance<NamePlateViewData>(); ;
            NamePlate.viewData.viewData.Image = Helpers.CreateNamePlateSprite(cv.resource);
            NamePlate.ChipOffset = new Vector2(0f, 0.2f);
            NamePlate.displayOrder = 99;
            NamePlate.Free = true;
            return NamePlate;
        }

        private static List<CustomNamePlate> createCustomNamePlateDetails(string[] nameplates)
        {
            Dictionary<string, CustomNamePlate> fronts = new();

            for (int i = 0; i < nameplates.Length; i++)
            {
                string s = nameplates[i].Substring(nameplates[i].LastIndexOf("\\") + 1).Split('.')[0];

                CustomNamePlate custom = new() { resource = nameplates[i] };
                custom.name = s.Replace('-', ' ');
                custom.author = "LocalFile";

                fronts.Add(s, custom);
            }

            List<CustomNamePlate> customhats = new();

            foreach (string k in fronts.Keys)
            {
                CustomNamePlate hat = fronts[k];
                customhats.Add(hat);
            }

            return customhats;
        }
    }
}