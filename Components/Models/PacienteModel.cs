using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

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
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

    }
}

