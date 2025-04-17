using System.Text.Json.Serialization;

namespace ALRS.DTO
{
    public class CrimeDateDto
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }
    }
}
