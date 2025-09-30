using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KavaPryct.Components.Models
{
    public class PacienteModel
    {
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }
       

        [JsonPropertyName("PacienteNombre")]
        public string PacienteNombre { get; set; }

        [JsonPropertyName("PacienteEdad")]
        public int PacienteEdad { get; set; }

        [JsonPropertyName("PacienteCelular")]
        public string PacienteCelular { get; set; }

        [JsonPropertyName("PacienteTypeId")]
        public int PacienteTypeId { get; set; }

        [JsonPropertyName("MotivoConsulta")]
        public string MotivoConsulta { get; set; }


        [JsonPropertyName("PsicoObjectId")]
        public string PsicoObjectId { get; set; }
        [JsonPropertyName("expObjectId")]
        public string ExpedienteObjectId { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        

    }

    public class FormularioExp
    {
        // ----- propiedades de Back4app -----
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        
        public DateTime UpdatedAt { get; set; }

        // ----- Resto de la clase -----
        [JsonPropertyName("nombre")]
        public string nombre { get; set; }
        [JsonPropertyName("sexoId")]
        public int SexoID { get; set; }
        [JsonPropertyName("NacimientoDt")]
        public FechaModel Nacimiento { get; set; }
        // UTC seguro (cuando exista)
        [JsonIgnore]
        public DateTime? FechaNacUtc =>
            Nacimiento?.Iso.Kind == DateTimeKind.Utc
                ? Nacimiento.Iso
                : Nacimiento?.Iso == default ? null
                : DateTime.SpecifyKind(Nacimiento.Iso, DateTimeKind.Utc);

        [JsonIgnore]
        public DateTime? NacimientoLocal =>
            FechaNacUtc is null ? null : ConvertUtcToMonterrey(FechaNacUtc.Value);

        [JsonPropertyName("edad")]
        public int Edad {
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
        [JsonPropertyName("lastEstudiosId")]
        public int LastEstudiosId { get; set;}
        [JsonPropertyName("ocupacion")]
        public string Ocupacion { get; set; }
        [JsonPropertyName("domicilio")]
        public string domicilio { get; set; }
        [JsonPropertyName("celular")]
        public string celular { get; set; }
        [JsonPropertyName("prevTerapia")]
        public bool prevTerapia { get; set; }
        [JsonPropertyName("hLive")]
        public int hLive { get; set; }
        [JsonPropertyName("mLive")]
        public int mLive {  get; set; }
        [JsonPropertyName("edoCivilId")]
        public int edoCivilId {  get; set; }
        [JsonPropertyName("familia")]
        [JsonConverter(typeof(EmptyStringToListConverter<Familiar>))]
        public List<Familiar> Familia { get; set; } = new();   // ← Nunca null
        [JsonPropertyName("mConsulta")]
        public string mConsulta { get; set; }

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

        public sealed class Familiar
        {
            public string? Nombre { get; set; }
            public int? Edad { get; set; }
            public string? Parentesco { get; set; }
            public string? Ocupacion { get; set; }
        }
        public class EmptyStringToListConverter<T> : JsonConverter<List<T>>
        {
            public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // null => []
                if (reader.TokenType == JsonTokenType.Null)
                    return new List<T>();

                // "" => []
                if (reader.TokenType == JsonTokenType.String)
                {
                    reader.GetString(); // consume el valor
                    return new List<T>();
                }

                // [] => deserializa normal
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    var list = new List<T>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var item = JsonSerializer.Deserialize<T>(ref reader, options);
                        if (item is not null) list.Add(item);
                    }
                    return list;
                }

                // Cualquier otra cosa: intenta deserializar (o lanza)
                throw new JsonException($"Token inesperado para List<{typeof(T).Name}>: {reader.TokenType}");
            }

            public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
            {
                // Siempre escribe como []
                writer.WriteStartArray();
                if (value != null)
                    foreach (var item in value)
                        JsonSerializer.Serialize(writer, item, options);
                writer.WriteEndArray();
            }
        }
    }
}

