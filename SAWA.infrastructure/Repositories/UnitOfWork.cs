using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SAWA.core.Interfaces;
using SAWA.core.IServices;
using SAWA.infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        public IPostRepository postRepository { get; }

        public UnitOfWork(AppDbContext db, IMapper mapper,IFileManagementService fileManagementService)
        {
            _db = db;
            _mapper = mapper;
            _fileManagementService = fileManagementService;

            postRepository = new PostRepository(_db,_mapper,_fileManagementService);
        }


        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
