using System.Text.Json.Serialization;

namespace Partico_Delivery.Models
{
    public class Delivery
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        // Add more properties as needed based on the API
    }
}
