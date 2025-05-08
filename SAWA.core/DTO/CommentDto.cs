using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.DTO
{
    public record CommentDto
    {
        public int Id { get; init; }
        public string Content { get; init; }
        public int PostId { get; init; }
        public DateTime CreatedAt { get; init; }
        public string User_FullName { get; init; }
        public string User_PhotoUrl { get; init; }
    }
    public class CommentCreateDto
    {
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
    }
    public record CommentUpdateDto
    {
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
    }


}
