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
        [JsonIgnore]
        public string PrimerNombre => ObtenerPrimerToken(Nombres);

        // (opcional) útil si quieres “Nombre + Apellido”
        [JsonIgnore]
        public string NombreCorto => string.IsNullOrWhiteSpace(PrimerNombre) ? string.Empty : $"{PrimerNombre} {A_Paterno}".Trim();
        [JsonIgnore]
        public string NombreCompleto => string.IsNullOrWhiteSpace(PrimerNombre) ? string.Empty : $"{Nombres} {A_Paterno} {A_Materno}".Trim();

        private static string ObtenerPrimerToken(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            // Quita espacios repetidos y toma el primer “token” por espacio
            var span = texto.AsSpan().Trim();
            int idx = span.IndexOf(' ');
            var first = (idx >= 0 ? span[..idx] : span).ToString();

            return first;
        }

        [JsonPropertyName("A_Paterno")]
        public string A_Paterno { get; set; }

        [JsonPropertyName("A_Materno")]
        public string A_Materno { get; set; }

        [JsonPropertyName("Direccion")]
        public string Direccion { get; set; }

        [JsonPropertyName("Telefono")]
        public string Telefono { get; set; }
        // -------- FECHA NACIMIENTO --------
        // Campo solo para el UI (picker)
        [JsonIgnore]
        public DateTime? FechaNacLocal { get; set; }

        // Campo real que va/ viene de Back4App
        [JsonPropertyName("FechaNac")]
        public FechaModel? FechaNac { get; set; }

        // UTC seguro (cuando exista)
        [JsonIgnore]
        public DateTime? FechaNacUtc =>
            FechaNac?.Iso.Kind == DateTimeKind.Utc
                ? FechaNac.Iso
                : FechaNac?.Iso == default ? null
                : DateTime.SpecifyKind(FechaNac.Iso, DateTimeKind.Utc);

        [JsonIgnore]
        public DateTime? NacimientoLocal =>
            FechaNacUtc is null ? null : ConvertUtcToMonterrey(FechaNacUtc.Value);

        // Edad (0 si no hay fecha)
        [JsonIgnore]
        public int Edad
        {
            get
            {
                if (NacimientoLocal is null) return 0;
                var hoy = GetNowMonterrey().Date;
                var nac = NacimientoLocal.Value.Date;
                int edad = hoy.Year - nac.Year;
                if (hoy < nac.AddYears(edad)) edad--;
                return Math.Max(0, edad);
                
            }
        }
        [JsonPropertyName("EdoCivilId")]
        public int EdoCivilId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("EstudiosId")]
        public int EstudiosLast { get; set; }
        [JsonPropertyName("RolId")]
        public int RolEmpleo { get; set; }
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
        public void SyncFechaLocalDesdeServidor()
        {
            // Llamar después de deserializar desde Back4App
            FechaNacLocal = FechaNac?.Iso == default ? null : NacimientoLocal;
        }

        public void MapearFechaNacParaServidor()
        {
            // Llamar antes de serializar/guardar a Back4App
            if (FechaNacLocal is null)
            {
                FechaNac = null;
            }
            else
            {
                var utc = DateTime.SpecifyKind(FechaNacLocal.Value, DateTimeKind.Local).ToUniversalTime();
                FechaNac = new FechaModel
                {
                    Iso = utc  // se serializa como "yyyy-MM-ddTHH:mm:ss.fffZ"
                };
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

    public class EmergContact
    {
        
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }
        [JsonPropertyName("Nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("Telefono")]
        public string Celular { get; set; }
        [JsonPropertyName("Parentezco")]
        public string Parentezco { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
