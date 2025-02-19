using Newtonsoft.Json;

namespace GUIDE.Models
{
    public class StudentModel
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("photo_url")]
        public string PhotoUrl { get; set; }
        public string PhotoBase64 { get; set; }
    }
}
