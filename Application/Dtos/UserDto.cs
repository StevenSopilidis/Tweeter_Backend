using System.Collections.Generic;
using Domain;

namespace Application.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Bio { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}