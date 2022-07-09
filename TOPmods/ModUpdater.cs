using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using Newtonsoft.Json.Linq;
using Twitch;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class ModUpdaterButton 
    {
        private static void Prefix(MainMenuManager __instance) {
            CustomHatLoader.LaunchHatFetcher();
            CustomVisorLoader.LaunchVisorFetcher();
            CustomNamePlateLoader.LaunchNamePlateFetcher();
            ModUpdater.LaunchUpdater();
            if (ModUpdater.hasUpdate)
            {
                var OnlineButton = GameObject.Find("PlayOnlineButton");
                UnityEngine.Object.Destroy(OnlineButton.GetComponent<BoxCollider2D>());

                var OnlineButtonText = OnlineButton.transform.FindChild("Text_TMP");
                var text = UnityEngine.Object.Instantiate(OnlineButtonText.GetComponent<TextMeshPro>(), OnlineButton.transform);
                text.enableWordWrapping = true;
                OnlineButtonText.gameObject.Destroy();
                new LateTask(() =>
                {
                    text.text = "TownOfPlusを最新版にアップデートしてください";
                });
            }
            if (ModUpdater.hasUpdate && !ModUpdater.IsUpdated || main.ShowPopUpVersion.Value != main.Version)
            {
                ModUpdater.ShowAnnounce();
                main.ShowPopUpVersion.Value = main.Version;
            }

            if (!ModUpdater.hasUpdate || ModUpdater.IsUpdated) return;

            TwitchManager man = DestroyableSingleton<TwitchManager>.Instance;
            ModUpdater.InfoPopup = UnityEngine.Object.Instantiate(man.TwitchPopup);
            ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.7f;
            ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = false;

            ModUpdater.ExecuteUpdate();
        }
    }

    public class ModUpdater
    { 
        public static bool running = false;
        public static bool hasUpdate = false;
        public static bool IsUpdated = false;
        public static string updateURI = null;
        public static string lalestversion = null;
        public static string AnnounceText = null;
        private static Task updateTask = null;
        public static GenericPopup InfoPopup;
        public static AnnouncementPopUp LatestVersinPopup;

        public static void LaunchUpdater()
        {
            if (running) return;
            running = true;
            checkForUpdate().GetAwaiter().GetResult();
            clearOldVersions();
        }

        public static void ExecuteUpdate()
        {
            string info = $"TownOfPlus {lalestversion}の\nアップデートをしています...\nしばらくお待ち下さい";
            InfoPopup.Show(info); // Show originally
            if (updateTask == null) {
                if (updateURI != null) {
                    if (!main.DebugMode.Value) updateTask = downloadUpdate();
                    IsUpdated = true;
                } else {
                    info = "手動で最新してください";
                }
            } else {
                info = "最新中です...";
            }
            InfoPopup.StartCoroutine(Effects.Lerp(0.01f, new System.Action<float>((p) => { setPopupText(info); })));
        }
        
        public static void clearOldVersions()
        {
            try {
                DirectoryInfo d = new (Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\plugins");
                string[] files = d.GetFiles("*.old").Select(x => x.FullName).ToArray(); // Getting old versions
                foreach (string f in files)
                    File.Delete(f);
            } catch {
            }
        }

        public static async Task<bool> checkForUpdate()
        {
            try
            {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "TownOfPlus Updater");
                var response = await http.GetAsync(new System.Uri("https://api.github.com/repos/tugaru1975/TownOfPlus/releases/latest"), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    return false;
                }
                string json = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(json);

                lalestversion = data["tag_name"]?.ToString();
                if (lalestversion == null)
                {
                    return false; // Something went wrong
                }

                string changeLog = $"<size=4>TownOfPlus {lalestversion}</size>\n";
                changeLog += "=====[アップデート内容]=====\n";
                changeLog += data["body"]?.ToString();
                changeLog += "\n<color=#7272e3><color=#7272e3><link=https://github.com/tugaru1975/TownOfPlus/releases/latest/>GitHub</color></link></color>より";
                if (changeLog != null) AnnounceText = changeLog;
                // check version
                System.Version ver = System.Version.Parse(lalestversion.TrimAll("v"));
                int diff = main.VersionId.CompareTo(ver);
                if (diff < 0)
                { // Update required
                    hasUpdate = true;
                    JToken assets = data["assets"];
                    if (!assets.HasValues)
                        return false;

                    for (JToken current = assets.First; current != null; current = current.Next)
                    {
                        string browser_download_url = current["browser_download_url"]?.ToString();
                        if (browser_download_url != null && current["content_type"] != null)
                        {
                            if (current["content_type"].ToString().Equals("application/x-msdownload") &&
                                browser_download_url.EndsWith(".dll"))
                            {
                                updateURI = browser_download_url;
                                return true;
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public static async Task<bool> downloadUpdate()
        {
            try {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "TownOfPlus Updater");
                var response = await http.GetAsync(new System.Uri(updateURI), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null) {
                    return false;
                }
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                System.UriBuilder uri = new System.UriBuilder(codeBase);
                string fullname = System.Uri.UnescapeDataString(uri.Path);
                if (File.Exists(fullname + ".old")) // Clear old file in case it wasnt;
                    File.Delete(fullname + ".old");

                File.Move(fullname, fullname + ".old"); // rename current executable to old

                using (var responseStream = await response.Content.ReadAsStreamAsync()) {
                    using (var fileStream = File.Create(fullname)) { // probably want to have proper name here
                        responseStream.CopyTo(fileStream); 
                    }
                }
                showPopup($"TownOfPlus {lalestversion}の\nアップデートが完了しました\nAmongUsを再起動してください");
                return true;
            } catch {
            }
            showPopup("最新に失敗しました");
            return false;
        }

        private static void showPopup(string message) {
            setPopupText(message);
            InfoPopup.gameObject.SetActive(true);
        }

        public static void setPopupText(string message) {
            if (InfoPopup == null)
                return;
            if (InfoPopup.TextAreaTMP != null) {
                InfoPopup.TextAreaTMP.text = message;
            }
        }
        public static void ShowAnnounce()
        {
            if (main.DebugMode.Value) return;
            if (LatestVersinPopup != null) return;
            LatestVersinPopup = UnityEngine.Object.Instantiate(DestroyableSingleton<MainMenuManager>.Instance.Announcement);
            LatestVersinPopup.transform.SetPos(z: -15);
            LatestVersinPopup.AnnounceTextMeshPro.text = AnnounceText;
            LatestVersinPopup.gameObject.SetActive(true);
        }
    }
}