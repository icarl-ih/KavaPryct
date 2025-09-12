using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        // --------- Propiedades calculadas para UI (no se serializan) ---------

        [JsonIgnore]
        public DateTime StartUtc => FechaIni?.Iso.Kind == DateTimeKind.Utc
            ? FechaIni.Iso
            : DateTime.SpecifyKind(FechaIni?.Iso ?? default, DateTimeKind.Utc);

        [JsonIgnore]
        public DateTime EndUtc => FechaFin?.Iso.Kind == DateTimeKind.Utc
            ? FechaFin.Iso
            : DateTime.SpecifyKind(FechaFin?.Iso ?? default, DateTimeKind.Utc);

        [JsonIgnore]
        public DateTime StartLocal => ConvertUtcToMonterrey(StartUtc);

        [JsonIgnore]
        public DateTime EndLocal => ConvertUtcToMonterrey(EndUtc);

        public static DateTime ConvertUtcToMonterrey(DateTime utc)
        {
            try
            {
                // IANA en Android/iOS/Linux; en Windows podrías mapear con TZConvert si falla
                var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Monterrey");
                return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            }
            catch
            {
                // Fallback razonable
                return utc.ToLocalTime();
            }
        }
    }
}
