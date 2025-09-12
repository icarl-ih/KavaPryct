using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KavaPryct.Components.Models
{
    public class EstudiosModel
    {
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("Nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("Abrv")]
        public string Abrv { get; set; }
        [JsonPropertyName("Cedula")]
        public string Cedula { get; set; }
        [JsonPropertyName("ClavePosgrado")]
        public bool ClavePosgrado { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        public List<PosgradosModel> Posgrados { get; set; }
    }

    public class PosgradosModel
    {
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }
        [JsonPropertyName("TipoId")]
        public int Id { get; set; }
        [JsonPropertyName("Nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("Abrv")]
        public string Abrv { get; set; }
        [JsonPropertyName("Cedula")]
        public string Cedula { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

}
