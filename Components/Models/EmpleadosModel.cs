using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KavaPryct.Components.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class EmpleadosModel
    {
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }

        [JsonPropertyName("Nombres")]
        public string Nombres { get; set; }

        [JsonPropertyName("A_Paterno")]
        public string A_Paterno { get; set; }

        [JsonPropertyName("A_Materno")]
        public string A_Materno { get; set; }

        [JsonPropertyName("Direccion")]
        public string Direccion { get; set; }

        [JsonPropertyName("Telefono")]
        public long Telefono { get; set; }

        [JsonPropertyName("FechaNac")]
        public FechaModel FechaNac { get; set; }

        // UTC seguro de la fecha de nacimiento
        [JsonIgnore]
        public DateTime FechaNacUtc => FechaNac?.Iso.Kind == DateTimeKind.Utc
            ? FechaNac.Iso
            : DateTime.SpecifyKind(FechaNac?.Iso ?? default, DateTimeKind.Utc);

        // Fecha de nacimiento en hora local de Monterrey
        [JsonIgnore]
        public DateTime NacimientoDate => ConvertUtcToMonterrey(FechaNacUtc);

        // >>> Campo calculado: Edad en años
        [JsonIgnore]
        public int Edad
        {
            get
            {
                if (FechaNacUtc == default) return 0;
                var hoyLocal = GetNowMonterrey().Date;
                var nacimiento = NacimientoDate.Date;

                int edad = hoyLocal.Year - nacimiento.Year;
                if (hoyLocal < nacimiento.AddYears(edad)) edad--; // aún no cumple años este año
                return Math.Max(0, edad);
            }
        }

        [JsonPropertyName("EdoCivilId")]
        public int EdoCivilId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("EstudiosObjectId")]
        public string EstudiosObjectId { get; set; }

        [JsonPropertyName("ContEmergObjectId")]
        public string ContEmergObjectId { get; set; }

        // --- Helpers de zona horaria (internos al modelo) ---
        private static DateTime ConvertUtcToMonterrey(DateTime utc)
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Monterrey");
                return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            }
            catch
            {
                return utc.ToLocalTime(); // fallback
            }
        }

        private static DateTime GetNowMonterrey()
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Monterrey");
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
            }
            catch
            {
                return DateTime.Now; // fallback
            }
        }
    }


    public class FechaModel
    {
        [JsonPropertyName("__type")]
        

        public string Type { get; set; } = "Date";
        [JsonPropertyName("iso")]
        public DateTime Iso {  get; set; }
    }
}
