using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Osmuim.GUI.Helpers
{
    public static class BrowserFolderChecker
    {
        private static readonly string exePath = AppDomain.CurrentDomain.BaseDirectory;

        public static async Task<Dictionary<string, List<string>>> GetBrowserVersionFoldersAsync()
        {
            var result = new Dictionary<string, List<string>>();
            var browserFolders = new[] { "Chrome", "Firefox", "Edge" };
            var versionPattern = new Regex(@"^\d+(\.\d+){1,3}$"); // Matches x.y, x.y.z, x.y.z.o

            foreach (var browser in browserFolders)
            {
                var browserPath = Path.Combine(exePath, browser);
                if (Directory.Exists(browserPath))
                {
                    var versionFolders = new List<string>();
                    var subDirectories = await Task.Run(() => Directory.GetDirectories(browserPath));

                    foreach (var subDir in subDirectories)
                    {
                        var folderName = Path.GetFileName(subDir);
                        if (versionPattern.IsMatch(folderName))
                        {
                            versionFolders.Add(folderName);
                        }
                    }

                    if (versionFolders.Count > 0)
                    {
                        result[browser] = versionFolders;
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, List<string>> GetBrowserVersionFolders()
        {
            var result = new Dictionary<string, List<string>>();
            var browserFolders = new[] { "Chrome", "Firefox", "Edge" };
            var versionPattern = new Regex(@"^\d+(\.\d+){1,3}$"); // Matches x.y, x.y.z, x.y.z.o

            foreach (var browser in browserFolders)
            {
                var browserPath = Path.Combine(exePath, browser);
                if (Directory.Exists(browserPath))
                {
                    var versionFolders = new List<string>();
                    foreach (var subDir in Directory.GetDirectories(browserPath))
                    {
                        var folderName = Path.GetFileName(subDir);
                        if (versionPattern.IsMatch(folderName))
                        {
                            versionFolders.Add(folderName);
                        }
                    }
                    if (versionFolders.Count > 0)
                    {
                        result[browser] = versionFolders;
                    }
                }
            }
            return result;
        }

        public static List<string> GetBrowserVersions(Dictionary<string, List<string>> result, string browserName)
        {
            // Define the allowed browser names
            var allowedBrowsers = new HashSet<string> { "Chrome", "Edge", "Firefox" };

            // Check if the provided browser name is valid
            if (!allowedBrowsers.Contains(browserName))
            {
                throw new ArgumentException($"Invalid browser name: {browserName}. Allowed values are 'Chrome', 'Edge', and 'Firefox'.");
            }

            // Return the versions if found, or an empty list if not
            return result.TryGetValue(browserName, out var versions) ? versions : new List<string>();
        }

    }
}
