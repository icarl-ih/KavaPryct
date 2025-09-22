using System.Globalization;
using System.Net;
using Microsoft.Maui.Networking; // IConnectivity
using System.Net.Http;
using System.Threading;

using System.Globalization;

namespace ExpenseTracker.Service
{
    public static class CommonService
    {
        // ---------- Formato / Cultura ----------
        private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

        /// <summary>
        /// Formatea moneda en es-MX. Por defecto sin decimales (C0).
        /// Ej.: 12345 => $12,345 | -12345 => -$12,345
        /// </summary>
        public static string GetCurrencyVal(decimal val, int decimals = 0)
        {
            var culture = (CultureInfo)EsMx.Clone();

            // -$n (1) en lugar de ($n)
            if (val < 0)
                culture.NumberFormat.CurrencyNegativePattern = 1;

            string format = $"C{decimals}"; // C0, C2, etc.
            return val.ToString(format, culture);
        }

        /// <summary>
        /// Formatea número con separadores de miles en es-MX (N0 por defecto).
        /// </summary>
        public static string GetNumberVal(decimal val, int decimals = 0)
        {
            string format = $"N{decimals}";
            return val.ToString(format, EsMx);
        }

        // ---------- Conectividad ----------
        private static IConnectivity? _connectivity;
        private static HttpClient? _http;
        private static CancellationTokenSource? _probeCts;

        /// <summary>
        /// Evento que notifica cambio de estado online (true = con Internet real).
        /// </summary>
        public static event EventHandler<bool>? OnlineStatusChanged;

        /// <summary>
        /// Llamar una sola vez al arrancar la app (en MauiProgram) para habilitar conectividad.
        /// </summary>
        public static void InitConnectivity(IConnectivity connectivity, IHttpClientFactory? httpFactory = null)
        {
            // Idempotente: si ya estaba inicializado, nos desuscribimos y reiniciamos
            if (_connectivity is not null)
                _connectivity.ConnectivityChanged -= OnConnectivityChanged;

            _connectivity = connectivity;

            _http = httpFactory?.CreateClient(nameof(CommonService))
                    ?? new HttpClient();
            _http.Timeout = TimeSpan.FromSeconds(3);

            _connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        /// <summary>
        /// Limpia suscripciones/recursos del módulo de conectividad (opcional).
        /// </summary>
        public static void DisposeConnectivity()
        {
            if (_connectivity is not null)
                _connectivity.ConnectivityChanged -= OnConnectivityChanged;

            _probeCts?.Cancel();
            _probeCts?.Dispose();
            _probeCts = null;
            _http = null;
            _connectivity = null;
        }

        /// <summary>
        /// Indica si el sistema reporta acceso a Internet (puede dar “falso positivo” con portales cautivos).
        /// </summary>
        public static bool IsInternet()
            => _connectivity?.NetworkAccess == NetworkAccess.Internet;

        /// <summary>
        /// Verifica “alcance real” con un HEAD rápido a un endpoint 204.
        /// Usa junto con IsInternet() para mayor precisión.
        /// </summary>
        public static async Task<bool> IsInternetReachableAsync(CancellationToken ct = default)
        {
            if (!IsInternet() || _http is null) return false;

            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Head, "https://clients3.google.com/generate_204");
                using var resp = await _http.SendAsync(req, ct);
                return (int)resp.StatusCode == 204;
            }
            catch
            {
                return false;
            }
        }

        private static async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            // Confirmamos con sonda para evitar falsos positivos
            _probeCts?.Cancel();
            _probeCts = new CancellationTokenSource();

            var online = e.NetworkAccess == NetworkAccess.Internet
                && await IsInternetReachableAsync(_probeCts.Token);

            OnlineStatusChanged?.Invoke(null, online);
        }
    }
}
