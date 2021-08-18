using System;
using Domain;

namespace Application.Dtos
{
    public class RetweetDto
    {
        public Guid Id { get; set; }
        public PostDto Post { get; set; }
    }
}