using SAWA.core.DTO;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Interfaces
{
    public interface ICommentRepository:IGenericRepository<Comment>
    {
        Task<string> CreateCommentAsync(CommentCreateDto model);
    }
}
