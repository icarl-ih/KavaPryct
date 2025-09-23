using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KavaPryct.Components.Models
{
    public class ParseResponse<T>
    {
        [JsonPropertyName("results")]
        public List<T> Results { get; set; } = new();
    }
    public sealed class ParseCreateResponse
    {
        [JsonPropertyName("objectId")]
        public string? ObjectId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }
    }

    public sealed class ParseResult
    {
        public bool Ok { get; init; }
        public string Reason { get; init; } = string.Empty; // "Created", "Bad Request", etc.
        public string? ObjectId { get; init; }
        public string? ErrorBody { get; init; }            // cuerpo de error devuelto por Parse
    }
}
