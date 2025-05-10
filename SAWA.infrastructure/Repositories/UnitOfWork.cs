using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        private readonly UserManager<AppUser> _userManager;
        public IPostRepository postRepository { get; }

        public ICommentRepository CommentRepository { get; }

        public IBranchesRepository branchesRepository { get; }

        public IHelpRequestRepository helpRequestRepository{ get; }

        public IDonationRepository donationRepository { get; }
        public UnitOfWork(AppDbContext db, IMapper mapper,IFileManagementService fileManagementService, UserManager<AppUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
            _userManager = userManager;

            postRepository = new PostRepository(_db,_mapper,_fileManagementService);
            CommentRepository = new CommentRepository(_db, _mapper, _fileManagementService);
            branchesRepository = new BranchesRepository(_db, _mapper, _fileManagementService);
            helpRequestRepository=new HelpRequestRepository(_db, _mapper, _fileManagementService,_userManager);
            donationRepository=new DonationRepository(_db, _mapper, _fileManagementService, _userManager);
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
