using Osmuim.GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Osmuim.GUI.Services
{
    internal class ChromeDataService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<ChromeForTesting> FetchChromeDataAsync(string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChromeForTesting>(jsonResponse);
        }

        public List<VersionInfo> GetLatestVersions(ChromeForTesting root, int count = 10)
        {
            return root.Versions
                .OrderByDescending(v => Version.Parse(v.Version))
                .Take(count)
                .ToList();
        }

        public DownloadInfo GetDownloadInfo(VersionInfo version, string downloadType, string platform)
        {
            if (downloadType == "chrome")
            {
                return version.Downloads.Chrome.FirstOrDefault(d => d.Platform == platform);
            }
            else if (downloadType == "chromedriver")
            {
                return version.Downloads.Chromedriver.FirstOrDefault(d => d.Platform == platform);
            }
            else if (downloadType == "chrome-headless-shell")
            {
                return version.Downloads.ChromeHeadlessShell.FirstOrDefault(d => d.Platform == platform);
            }
            return null;
        }
    }
}
