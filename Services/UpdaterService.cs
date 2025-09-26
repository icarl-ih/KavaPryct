using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static bool TryParseVersionLoose(string? input, out Version result)
        {
            result = new Version(0, 0, 0, 0);
            if (string.IsNullOrWhiteSpace(input)) return false;

            var s = input.Trim();
            var cut = s.IndexOfAny(new[] { '-', '+' });
            if (cut >= 0) s = s[..cut];

            var parts = s.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            int[] nums = new int[4];
            for (int i = 0; i < Math.Min(parts.Length, 4); i++)
                nums[i] = int.TryParse(parts[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? Math.Max(n, 0) : 0;

            try
            {
                result = parts.Length switch
                {
                    1 => new Version(nums[0], 0),
                    2 => new Version(nums[0], nums[1]),
                    3 => new Version(nums[0], nums[1], nums[2]),
                    _ => new Version(nums[0], nums[1], nums[2], nums[3]),
                };
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// -1 si current < latest, 0 si equivalentes/no comparables, +1 si current > latest.
        /// Empata por VersionCode si las versiones de texto son iguales.
        /// </summary>
        public static int CompareAppVersions(string? currentVersionStr, string? latestVersionStr, int currentBuildCode, int latestVersionCode)
        {
            var okCurrent = TryParseVersionLoose(currentVersionStr, out var vCurr);
            var okLatest = TryParseVersionLoose(latestVersionStr, out var vLat);

            if (okCurrent && okLatest)
            {
                var cmp = vCurr.CompareTo(vLat);
                if (cmp != 0) return cmp;
                if (currentBuildCode > 0 && latestVersionCode > 0)
                    return currentBuildCode.CompareTo(latestVersionCode);
                return 0;
            }

            if (latestVersionCode > 0)
                return (currentBuildCode > 0 ? currentBuildCode : 0).CompareTo(latestVersionCode);

            return 0;
        }

        // ========= API de alto nivel para reutilizar en UI =========

        public async Task<UpdateCheckResult> CheckAsync(string platform)
        {
            var latest = await GetLatestAsync(platform);

            var currentVersionStr = AppInfo.Current?.VersionString ?? VersionTracking.CurrentVersion;
            var currentBuildStr = AppInfo.Current?.BuildString ?? VersionTracking.CurrentBuild;
            var currentBuildCode = int.TryParse(currentBuildStr, out var cb) ? cb : 0;

            var cmp = CompareAppVersions(
                currentVersionStr,
                latest?.Version,
                currentBuildCode,
                latest?.VersionCode ?? 0
            );

            var status = UpdateStatus.UpToDate;
            if (latest is not null && cmp < 0)
                status = latest.Mandatory ? UpdateStatus.Mandatory : UpdateStatus.UpdateAvailable;

            return new UpdateCheckResult
            {
                Status = status,
                CurrentVersion = currentVersionStr ?? "—",
                CurrentBuild = currentBuildCode,
                Latest = latest
            };
        }
    }

    public class UpdateCheckResult
    {
        public UpdateStatus Status { get; set; }
        public string CurrentVersion { get; set; } = "—";
        public int CurrentBuild { get; set; }
        public AppVersionInfo? Latest { get; set; }

        public bool IsUpdateAvailable => Status != UpdateStatus.UpToDate;
    }

    public enum UpdateStatus { UpToDate, UpdateAvailable, Mandatory }

    //// Ajusta esto a tu modelo real
    //public class AppVersionInfo
    //{
    //    public string? Version { get; set; }
    //    public int VersionCode { get; set; }
    //    public bool Mandatory { get; set; }
    //    public string? Notes { get; set; }
    //    public string? FileUrl { get; set; }
    //}
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


