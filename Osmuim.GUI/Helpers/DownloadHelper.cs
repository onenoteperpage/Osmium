using Osmium.GUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Osmium.GUI.Helpers
{
    public static class DownloadHelper
    {

        public static async Task<List<string>> GetDownloadUrlsByPlatformAsync(VersionInfo versionInfo, string platform)
        {
            var urls = new List<string>();

            // Simulate asynchronous processing with Task.Run (optional, can be removed if unnecessary)
            await Task.Run(() =>
            {
                // Get URLs for the specified platform from each download type
                var chromeUrl = versionInfo.Downloads.Chrome.FirstOrDefault(d => d.Platform == platform)?.Url;
                if (chromeUrl != null) urls.Add(chromeUrl);

                var chromedriverUrl = versionInfo.Downloads.Chromedriver.FirstOrDefault(d => d.Platform == platform)?.Url;
                if (chromedriverUrl != null) urls.Add(chromedriverUrl);

                var chromeHeadlessShellUrl = versionInfo.Downloads.ChromeHeadlessShell.FirstOrDefault(d => d.Platform == platform)?.Url;
                if (chromeHeadlessShellUrl != null) urls.Add(chromeHeadlessShellUrl);
            });

            return urls;
        }

        public static List<string> GetDownloadUrlsByPlatform(VersionInfo versionInfo, string platform)
        {
            var urls = new List<string>();

            // Get URLs for the specified platform from each download type
            var chromeUrl = versionInfo.Downloads.Chrome.FirstOrDefault(d => d.Platform == platform)?.Url;
            if (chromeUrl != null) urls.Add(chromeUrl);

            var chromedriverUrl = versionInfo.Downloads.Chromedriver.FirstOrDefault(d => d.Platform == platform)?.Url;
            if (chromedriverUrl != null) urls.Add(chromedriverUrl);

            var chromeHeadlessShellUrl = versionInfo.Downloads.ChromeHeadlessShell.FirstOrDefault(d => d.Platform == platform)?.Url;
            if (chromeHeadlessShellUrl != null) urls.Add(chromeHeadlessShellUrl);

            return urls;
        }

        public static async Task DownloadFilesAsync(List<string> urls, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);

            using (var httpClient = new HttpClient())
            {
                foreach (var url in urls)
                {
                    string fileName = Path.GetFileName(new Uri(url).LocalPath);
                    string filePath = Path.Combine(destinationFolder, fileName);

                    byte[] fileData = await httpClient.GetByteArrayAsync(url);

                    // Use Task.Run to write the bytes to file in an async manner compatible with .NET Framework
                    await Task.Run(() => File.WriteAllBytes(filePath, fileData));
                }
            }
        }
    }
}

