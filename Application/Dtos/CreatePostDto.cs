using Microsoft.AspNetCore.Http;

namespace Application.Dtos
{
    //request data for creating a post
    public class CreatePostDto
    {
        public string Content { get; set; }
        public bool Public { get; set; }
        public IFormFile Photo{ get; set; }
    }
}