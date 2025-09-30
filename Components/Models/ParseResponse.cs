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

        // Opcional, útil cuando llamas con &count=1
        [JsonPropertyName("count")]
        public int? Count { get; set; }
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
        public string Reason { get; init; } = string.Empty;
        public string? ObjectId { get; init; }
        public string? ErrorBody { get; init; }
    }
}
