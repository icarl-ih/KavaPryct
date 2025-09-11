using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KavaPryct.Components.Models
{
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
        public int Telefono { get; set; }
        [JsonPropertyName("FechaNac")]

        [JsonPropertyName("EdoCivilId")]
        [JsonPropertyName("createdAt")]
        [JsonPropertyName("updatedAt")]
        [JsonPropertyName("EstudiosObjectId")]
        [JsonPropertyName("ContEmergObjectId")]
    }

    public class NacDateTime
    {
        [JsonPropertyName("__type")]
        

        public string Type { get; set; } = "Date";
        [JsonPropertyName("iso")]
        public DateTime Iso {  get; set; }
    }
}
