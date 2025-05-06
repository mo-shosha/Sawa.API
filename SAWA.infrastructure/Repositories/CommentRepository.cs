using AutoMapper;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using SAWA.core.IServices;
using SAWA.core.Models;
using SAWA.infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        public CommentRepository(AppDbContext db, IMapper mapper, IFileManagementService fileManagementService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
        }

        public async Task<string> CreateCommentAsync(CommentCreateDto model)
        {
            try
            {
                var comment = _mapper.Map<Comment>(model);
                comment.CreatedAt = DateTime.UtcNow;
                await _db.Comments.AddAsync(comment);
                await _db.SaveChangesAsync();

                return "Success";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        

    }
}
