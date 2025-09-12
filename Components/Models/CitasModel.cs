using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace KavaPryct.Components.Models
{
    public class CitasModel
    {
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }

        [JsonPropertyName("PacienteNombre")]
        public string PacienteNombre { get; set; }

        [JsonPropertyName("PacienteEdad")]
        public int PacienteEdad { get; set; }

        [JsonPropertyName("PacienteCelular")]
        public long PacienteCelular { get; set; }

        [JsonPropertyName("PacienteTypeId")]
        public int PacienteTypeId { get; set; }

        [JsonPropertyName("MotivoConsulta")]
        public string MotivoConsulta { get; set; }

        [JsonPropertyName("StatusCitaId")]
        public int StatusCitaId { get; set; }

        [JsonPropertyName("Amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("IsPaid")]
        public bool IsPaid { get; set; }

        [JsonPropertyName("PayMethodId")]
        public int PayMethodId { get; set; }

        [JsonPropertyName("FechaIni")]
        public FechaModel FechaIni { get; set; }

        [JsonPropertyName("FechaFin")]
        public FechaModel FechaFin { get; set; }

        [JsonPropertyName("PsicoObjectId")]
        public string PsicoObjectId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        // ----------------- Helpers de zona horaria -----------------
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

        private static DateTime ToLocalFromUtc(DateTime utc)
        {
            if (utc == default) return default;
            var tz = GetTz();
            var u = utc.Kind == DateTimeKind.Utc ? utc : DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(u, tz);
        }

        private static DateTime ToUtcFromLocal(DateTime local)
        {
            if (local == default) return default;
            var tz = GetTz();
            // El picker entrega "local" sin info de zona → úsalo como Unspecified
            var unspecified = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, tz);
        }

        // ----------------- Propiedades calculadas para UI -----------------
        [JsonIgnore]
        public DateTime StartUtc =>
            FechaIni?.Iso == default ? default :
            (FechaIni.Iso.Kind == DateTimeKind.Utc ? FechaIni.Iso : DateTime.SpecifyKind(FechaIni.Iso, DateTimeKind.Utc));

        [JsonIgnore]
        public DateTime EndUtc =>
            FechaFin?.Iso == default ? default :
            (FechaFin.Iso.Kind == DateTimeKind.Utc ? FechaFin.Iso : DateTime.SpecifyKind(FechaFin.Iso, DateTimeKind.Utc));

        [JsonIgnore]
        public DateTime StartLocal
        {
            get => ToLocalFromUtc(StartUtc);
            set
            {
                var utc = ToUtcFromLocal(value);
                (FechaIni ??= new FechaModel()).Iso = utc;

                // Si no hay fin o quedó antes/igual, fijar +1 hora
                if (FechaFin == null || FechaFin.Iso <= utc)
                    (FechaFin ??= new FechaModel()).Iso = utc.AddHours(1);
            }
        }

        [JsonIgnore]
        public DateTime EndLocal
        {
            get => ToLocalFromUtc(EndUtc);
            set
            {
                var utc = ToUtcFromLocal(value);
                (FechaFin ??= new FechaModel()).Iso = utc;
            }
        }
    }
}

