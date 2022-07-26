using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TownOfPlus
{
    [HarmonyPatch]
    public class CustomHats
    {
        public static Material hatShader;

        public static Dictionary<string, HatExtension> CustomHatRegistry = new();

        public class HatExtension
        {
            public Sprite FlipImage { get; set; }
            public Sprite BackFlipImage { get; set; }
        }

        public class CustomHat
        {
            public string author { get; set; }
            public string name { get; set; }
            public string resource { get; set; }
            public string flipresource { get; set; }
            public string backflipresource { get; set; }
            public string backresource { get; set; }
            public string climbresource { get; set; }
            public bool bounce { get; set; }
            public bool adaptive { get; set; }
            public bool behind { get; set; }
        }

        private static List<CustomHat> createCustomHatDetails(string[] hats)
        {
            Dictionary<string, CustomHat> fronts = new ();
            Dictionary<string, string> backs = new ();
            Dictionary<string, string> flips = new();
            Dictionary<string, string> backflips = new();
            Dictionary<string, string> climbs = new();

            for (int i = 0; i < hats.Length; i++)
            {
                string s = hats[i].Substring(hats[i].LastIndexOf("\\") + 1).Split('.')[0];
                string[] p = s.Split('_');

                HashSet<string> options = new();
                for (int j = 1; j < p.Length; j++)
                    options.Add(p[j]);

                if (options.Contains("back") && options.Contains("flip"))
                    backflips.Add(p[0], hats[i]);
                else if (options.Contains("climb"))
                    climbs.Add(p[0], hats[i]);
                else if (options.Contains("back"))
                    backs.Add(p[0], hats[i]);
                else if (options.Contains("flip"))
                    flips.Add(p[0], hats[i]);
                else
                {
                    CustomHat custom = new() { resource = hats[i] };
                    custom.name = p[0].Replace('-', ' ');
                    custom.bounce = options.Contains("bounce");
                    custom.adaptive = options.Contains("adaptive");
                    custom.behind = options.Contains("behind");

                    fronts.Add(p[0], custom);
                }
            }

            List<CustomHat> customhats = new();

            foreach (string k in fronts.Keys)
            {
                CustomHat hat = fronts[k];
                backs.TryGetValue(k, out string br);
                climbs.TryGetValue(k, out string cr);
                flips.TryGetValue(k, out string fr);
                backflips.TryGetValue(k, out string bfr);
                if (br != null)
                    hat.backresource = br;
                if (cr != null)
                    hat.climbresource = cr;
                if (fr != null)
                    hat.flipresource = fr;
                if (bfr != null)
                    hat.backflipresource = bfr;
                if (hat.backresource != null)
                    hat.behind = true;

                customhats.Add(hat);
            }

            return customhats;
        }

        private static HatData CreateHatData(CustomHat ch)
        {
            if (hatShader == null)
            {
                Material tmpShader = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;
                hatShader = tmpShader;
            }

            HatData hat = ScriptableObject.CreateInstance<HatData>();
            hat.hatViewData.viewData = ScriptableObject.CreateInstance<HatViewData>();
            hat.hatViewData.viewData.MainImage = Helpers.CreateSkinSprite(ch.resource);
            if (ch.backresource != null)
            {
                hat.hatViewData.viewData.BackImage = Helpers.CreateSkinSprite(ch.backresource);
                ch.behind = true; // Required to view backresource
            }
            if (ch.climbresource != null)
                hat.hatViewData.viewData.ClimbImage = Helpers.CreateSkinSprite(ch.climbresource);
            hat.name = ch.name + "\nby " + ch.author;
            hat.displayOrder = 99;
            hat.ProductId = "hat_TOP_" + ch.name.Replace(' ', '_') + ch.author;
            hat.InFront = !ch.behind;
            hat.NoBounce = !ch.bounce;
            hat.ChipOffset = new Vector2(0f, 0.2f);
            hat.Free = true;

            if (ch.adaptive && hatShader != null)
                hat.hatViewData.viewData.AltShader = hatShader;

            HatExtension extend = new();

            if (ch.flipresource != null)
                extend.FlipImage = Helpers.CreateSkinSprite(ch.flipresource);
            if (ch.backflipresource != null)
                extend.BackFlipImage = Helpers.CreateSkinSprite(ch.backflipresource);

            CustomHatRegistry.Add(hat.name, extend);

            return hat;
        }

        private static HatData CreateHatBehaviour(CustomHat chd)
        {
            string filePath = main.TOPUrl + @"TOPHats\";
            chd.resource = filePath + chd.resource;
            if (chd.backresource != null)
                chd.backresource = filePath + chd.backresource;
            if (chd.climbresource != null)
                chd.climbresource = filePath + chd.climbresource;
            if (chd.flipresource != null)
                chd.flipresource = filePath + chd.flipresource;
            if (chd.backflipresource != null)
                chd.backflipresource = filePath + chd.backflipresource;
            return CreateHatData(chd);
        }

        private static bool RUNNING = false;
        private static bool isadded = false;
        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
        private static class HatManagerPatch
        {
            static void Prefix(HatManager __instance)
            {
                if (RUNNING || isadded) return;
                RUNNING = true; 
                try
                {
                    try
                    {
                        string filePath = main.TOPUrl + @"SkinTest\HatTest";
                        DirectoryInfo d = new(filePath);
                        string[] filePaths = d.GetFiles("*.png").Select(x => x.FullName).ToArray(); // Getting Text files
                        List<CustomHat> hats = createCustomHatDetails(filePaths);
                        if (hats.Count > 0)
                        {
                            foreach (var hat in hats)
                            {
                                __instance.allHats.Add(CreateHatData(hat));
                            }
                        }
                    }
                    catch { }

                    
                    while (CustomHatLoader.hatdetails.Count > 0)
                    {
                        isadded = true;
                        __instance.allHats.Add(CreateHatBehaviour(CustomHatLoader.hatdetails[0]));
                        CustomHatLoader.hatdetails.RemoveAt(0);
                    }
                }
                catch { }
            }
            static void Postfix(HatManager __instance)
            {
                RUNNING = false;
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
        private static class PlayerPhysicsHandleAnimationPatch
        {
            private static void Postfix(PlayerPhysics __instance)
            {
                AnimationClip currentAnimation = __instance.Animator.GetCurrentAnimation();
                if (currentAnimation == __instance.CurrentAnimationGroup.ClimbAnim || currentAnimation == __instance.CurrentAnimationGroup.ClimbDownAnim) return;
                HatParent hp = __instance.myPlayer.cosmetics.hat;
                if (hp.Hat == null) return;
                HatExtension extend = hp.Hat.getHatExtension();
                if (extend == null) return;
                if (extend.FlipImage != null)
                {
                    if (__instance.FlipX)
                    {
                        hp.FrontLayer.sprite = extend.FlipImage;
                    }
                    else
                    {
                        hp.FrontLayer.sprite = hp.hatView.MainImage;
                    }
                }
                if (extend.BackFlipImage != null)
                {
                    if (__instance.FlipX)
                    {
                        hp.BackLayer.sprite = extend.BackFlipImage;
                    }
                    else
                    {
                        hp.BackLayer.sprite = hp.hatView.BackImage;
                    }
                }
            }
        }
    }

    public class CustomHatLoader
    {
        public static bool running = false;
        public static string[] REPO = main.NewHatURL.Split(',');

        public static List<CustomHats.CustomHat> hatdetails = new();
        public static void LaunchHatFetcher()
        {
            if (running)
                return;
            running = true;
            _ = LaunchHatFetcherAsync();
        }

        private static async Task LaunchHatFetcherAsync()
        {
            foreach (var repo in REPO)
            {
                try
                {
                    HttpStatusCode status = await FetchHats(repo.log("チェック開始"));
                    if (status != HttpStatusCode.OK) repo.log("URLが見つかりませんでした", LogType.Error);
                    else repo.log("チェック終了").gamelog("ハットダウンロード終了");
                }
                catch { }
            }
            running = false;
        }

        private static string sanitizeResourcePath(string res)
        {
            if (res == null || !res.EndsWith(".png"))
                return null;
            res = res.TrimAll("\\", "/", "*", "..");
            return res;
        }

        public static async Task<HttpStatusCode> FetchHats(string repo)
        {
            HttpClient http = new();
            http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            var response = await http.GetAsync(new System.Uri($"{repo}/CustomHats.json"), HttpCompletionOption.ResponseContentRead);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK) return response.StatusCode;
                if (response.Content == null)
                {
                    System.Console.WriteLine("Server returned no data: " + response.StatusCode.ToString());
                    return HttpStatusCode.ExpectationFailed;
                }
                string json = await response.Content.ReadAsStringAsync();
                JToken jobj = JObject.Parse(json)["hats"];
                if (!jobj.HasValues) return HttpStatusCode.ExpectationFailed;

                List<CustomHats.CustomHat> hatdatas = new();

                for (JToken current = jobj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        CustomHats.CustomHat info = new();

                        info.name = current["name"]?.ToString();
                        info.resource = sanitizeResourcePath(current["resource"]?.ToString());
                        if (info.resource == null || info.name == null) // required
                            continue;
                        info.backresource = sanitizeResourcePath(current["backresource"]?.ToString());
                        info.climbresource = sanitizeResourcePath(current["climbresource"]?.ToString());
                        info.flipresource = sanitizeResourcePath(current["flipresource"]?.ToString());
                        info.backflipresource = sanitizeResourcePath(current["backflipresource"]?.ToString());

                        info.author = current["author"]?.ToString();
                        info.bounce = current["bounce"] != null;
                        info.adaptive = current["adaptive"] != null;
                        info.behind = current["behind"] != null;
                        hatdatas.Add(info);
                    }
                }

                List<string> markedfordownload = new();

                string filePath = main.TOPUrl + @"TOPHats\";
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                foreach (CustomHats.CustomHat data in hatdatas)
                {
                    if (doesResourceRequireDownload(filePath + data.resource))
                        markedfordownload.Add(data.resource);
                    if (data.backresource != null && doesResourceRequireDownload(filePath + data.backresource))
                        markedfordownload.Add(data.backresource);
                    if (data.climbresource != null && doesResourceRequireDownload(filePath + data.climbresource))
                        markedfordownload.Add(data.climbresource);
                    if (data.flipresource != null && doesResourceRequireDownload(filePath + data.flipresource))
                        markedfordownload.Add(data.flipresource);
                    if (data.backflipresource != null && doesResourceRequireDownload(filePath + data.backflipresource))
                        markedfordownload.Add(data.backflipresource);
                }

                foreach (var file in markedfordownload)
                {

                    var hatFileResponse = await http.GetAsync($"{repo}/hats/{file}", HttpCompletionOption.ResponseContentRead);
                    if (hatFileResponse.StatusCode != HttpStatusCode.OK)
                    {
                        file.log($"[{repo}]ダウンロード失敗", LogType.Error);
                        continue;
                    }
                    else file.log($"[{repo}]ダウンロード成功");
                    using (var responseStream = await hatFileResponse.Content.ReadAsStreamAsync())
                    {
                        using (var fileStream = File.Create($"{filePath}\\{file}"))
                        {
                            responseStream.CopyTo(fileStream);
                        }
                    }
                }

                hatdetails = hatdatas;
            }
            catch { }
            return HttpStatusCode.OK;
        }

        private static bool doesResourceRequireDownload(string respath)
        {
            return !File.Exists(respath);
        }
    }
    public static class CustomHatExtensions
    {
        public static CustomHats.HatExtension getHatExtension(this HatData hat)
        {
            CustomHats.CustomHatRegistry.TryGetValue(hat.name, out var ret);
            return ret;
        }
    }
}
