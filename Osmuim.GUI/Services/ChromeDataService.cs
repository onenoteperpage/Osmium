using Osmuim.GUI.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Osmuim.GUI.Services
{
    public class ChromeDataService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        // Public constructor for DI
        public ChromeDataService() { }

        public async Task<ChromeForTesting> FetchChromeDataAsync(string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChromeForTesting>(jsonResponse);
        }

        public List<VersionInfo> GetLatestVersions(ChromeForTesting chromeData, int count = 10)
        {
            return chromeData.Versions
                .OrderByDescending(v => Version.Parse(v.Version))
                .Take(count)
                .ToList();
        }

        public async Task<List<VersionInfo>> GetLatestVersionsAsync(ChromeForTesting chromeData, int count = 10)
        {
            return await Task.Run(() =>
            {
                return chromeData.Versions
                    .OrderByDescending(v => Version.Parse(v.Version))
                    .Take(count)
                    .ToList();
            });
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
            else
            {
                return null;
            }
        }
    }
}
