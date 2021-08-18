using System;
using System.Text.Json.Serialization;

namespace Domain
{
    //image that might be uploaded in a post
    public class PostImage
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        
        [JsonIgnore]
        public Post ImagesPost { get; set;}
        public Guid PostId { get; set; }
    }
}