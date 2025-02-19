using System.Text.Json.Serialization;

namespace GUIDE.Models
{
    public class StudentModel
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [JsonPropertyName("photo_url")]
        public string PhotoUrl { get; set; }
    }
}
