using System.Globalization;

using System.Globalization;

namespace ExpenseTracker.Service
{
    public static class CommonService
    {
        private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

        /// <summary>
        /// Formatea moneda en es-MX. Por defecto sin decimales (C0).
        /// Ej.: 12345 => $12,345 | -12345 => -$12,345
        /// </summary>
        public static string GetCurrencyVal(decimal val, int decimals = 0)
        {
            // Clonamos para poder ajustar el patrón negativo sin tocar la instancia global
            var culture = (CultureInfo)EsMx.Clone();

            // -$n (1) en lugar de ($n) u otros formatos
            if (val < 0)
                culture.NumberFormat.CurrencyNegativePattern = 1;

            string format = $"C{decimals}"; // C0, C2, etc.
            return val.ToString(format, culture);
        }

        /// <summary>
        /// Formatea número con separadores de miles en es-MX.
        /// Por defecto sin decimales (N0).
        /// </summary>
        public static string GetNumberVal(decimal val, int decimals = 0)
        {
            string format = $"N{decimals}"; // N0, N2, etc.
            return val.ToString(format, EsMx);
        }
    }
}
