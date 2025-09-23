using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KavaPryct.Services
{
    public class UpdaterService
    {
        private readonly HttpClient _http;
        private readonly AppSettings _appSetting = new AppSettings();
        public UpdaterService(HttpClient httpClient) => _http = httpClient;

        public async Task<AppVersionInfo?> GetLatestAsync(string platform)
        {
            var url = $"https://parseapi.back4app.com/classes/AppVersion?where={Uri.EscapeDataString($@"{{""platform"":""{platform}""}}")}&order=-versionCode&limit=1";
            var json = await _http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);
            var arr = doc.RootElement.GetProperty("results");
            if (arr.GetArrayLength() == 0) return null;
            var o = arr[0];
            return new AppVersionInfo
            {
                Platform = platform,
                VersionCode = o.GetProperty("versionCode").GetInt32(),
                Version = o.GetProperty("versionString").GetString(),
                Mandatory = o.GetProperty("mandatory").GetBoolean(),
                FileUrl = o.GetProperty("fileUrl").GetString(),
                Notes = o.TryGetProperty("notes", out var n) ? n.GetString() : ""
            };
        }

    }
    public record AppVersionInfo
    {
        public string Platform { get; init; }
        public int VersionCode { get; init; }
        public string? Version { get; init; }
        public bool Mandatory { get; init; }
        public string? FileUrl { get; init; }
        public string? Notes { get; init; }
    }
}
