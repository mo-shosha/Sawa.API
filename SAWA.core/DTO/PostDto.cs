using Microsoft.AspNetCore.Http;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.DTO
{
    public record PostDto
    {
        public int Id { get; init; }
        public string CharityName { get; init; }
        public DateTime CreateAt { get; init; }
        public string Content { get;init; }
        public List<PhotoDto> Photos { get; init; }
        public ICollection<CommentDto> Comments { get; set; }

    }

    public record CommentDto
    {
        public string Content { get; init; }
        public int PostId { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record PhotoDto
    {
        public string ImgName { get; set; }
        public int PostId { get; set; }
    }

    public record PostCreateDto 
    {
        public string Content { get; set; }
        public IFormFileCollection Photos { get; set; }
    }

}
