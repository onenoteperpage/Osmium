using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Osmuim.GUI.Models
{
    public class ChromeForTesting
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("versions")]
        public List<VersionInfo> Versions { get; set; }
    }

    public class VersionInfo
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("revision")]
        public string Revision { get; set; }

        [JsonPropertyName("downloads")]
        public Downloads Downloads { get; set; }
    }

    public class Downloads
    {
        [JsonPropertyName("chrome")]
        public List<DownloadInfo> Chrome { get; set; } = new List<DownloadInfo>();

        [JsonPropertyName("chromedriver")]
        public List<DownloadInfo> Chromedriver { get; set; } = new List<DownloadInfo>();

        [JsonPropertyName("chrome-headless-shell")]
        public List<DownloadInfo> ChromeHeadlessShell { get; set; } = new List<DownloadInfo>();
    }

    public class DownloadInfo
    {
        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
