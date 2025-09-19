using KavaPryct.Components.Models;
using System;
using System.Text.Json.Serialization;

namespace ExpenseTracker.Models
{
    public class ExpenseData
    {
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }           // id en Back4App (opcional, al guardar)
        [JsonPropertyName("TransactionTypeId")]
        public int TransactionTypeId { get; set; }    // "Income" | "Expense"
        [JsonPropertyName("Amount")]
        public decimal Amount { get; set; }            // <- decimal
        [JsonPropertyName("DatFecha")]
        public FechaModel DatFecha { get; set; }
        
        [JsonIgnore]
        public DateTime fechautc =>
            DatFecha?.Iso == default ? default :
            (DatFecha.Iso.Kind == DateTimeKind.Utc? DatFecha.Iso : DateTime.SpecifyKind(DatFecha.Iso, DateTimeKind.Utc));
        private static TimeZoneInfo GetTz()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("America/Chihuahua"); } // IANA
            catch
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time (Mexico)"); } // Windows
                catch
                {
                    try { return TimeZoneInfo.FindSystemTimeZoneById("America/Monterrey"); } // IANA alterna
                    catch { return TimeZoneInfo.Local; }
                }
            }
        }
        private static DateTime ToUtcFromLocal(DateTime local)
        {
            if (local == default) return default;
            var tz = GetTz();
            // El picker entrega "local" sin info de zona → úsalo como Unspecified
            var unspecified = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, tz);
        }
        private static DateTime ToLocalFromUtc(DateTime utc)
        {
            if (utc == default) return default;
            var tz = GetTz();
            var u = utc.Kind == DateTimeKind.Utc ? utc : DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(u, tz);
        }

        
        [JsonIgnore]
        public DateTime dateTime
        {
            get => ToLocalFromUtc(fechautc);
            set
            {
                var utc = ToUtcFromLocal(value);
                (DatFecha ??= new FechaModel()).Iso = utc;
            }
        }

        // Categoría/Descripción/Concepto:
        [JsonPropertyName("Category")]
        public string Category { get; set; }           // p.ej. "Consultas", "Gastos"
        [JsonPropertyName("Description")]
        public string Description { get; set; }        // texto libre



        // Pago:
        [JsonPropertyName("PayMethodId")]
        public int PayMethodId { get; set; }           // enum entero
        [JsonPropertyName("PaymentMode")]
        public string PaymentMode { get; set; }        // nombre del enum (display)


        // Empleado (para filtrar y calcular salario):
        [JsonPropertyName("EmployeeObjectId")]
        public string EmployeeObjectId { get; set; }
        [JsonPropertyName("citaObjectId")]
        public string citaObjectId { get; set; }
        [JsonPropertyName("EmployeeName")]
        public string EmployeeName { get; set; }

        // (si mantienes estos, se llenan al guardar)
        [JsonPropertyName("MonthShort")]
        public string MonthShort { get; set; }
        [JsonPropertyName("MonthFull")]
        public string MonthFull { get; set; }
        [JsonPropertyName("FormattedDate")]
        public string FormattedDate { get; set; }
    }
}
