using Microsoft.AspNetCore.Http;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.DTO
{
    public record PostDto
    {
        public int Id { get; init; }
        public string UserName { get; init; }
        public string CharityName { get; init; }
        public DateTime CreateAt { get; init; }
        public string Content { get;init; }
        public List<PhotoDto> Photos { get; init; }
        public ICollection<CommentDto> Comments { get; set; }
        public string User_PhotoUrl { get; init; }

    }

    

    public record PhotoDto
    {
        public string ImgName { get; set; }
    }

    public record PostCreateDto 
    {
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
        [Required]
        public IFormFileCollection Photos { get; set; }
    }

}
