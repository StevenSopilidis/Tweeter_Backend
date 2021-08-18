using System;
using System.Text.Json.Serialization;

namespace Domain
{
    public class UserProfileImage
    {
        public Guid Id { get; set; } 
        public string Url { get; set; }
        public string PublicId { get; set; }
        [JsonIgnore]
        public AppUser AppUser { get; set; }
        public Guid AppUserId { get; set; }
    }
}