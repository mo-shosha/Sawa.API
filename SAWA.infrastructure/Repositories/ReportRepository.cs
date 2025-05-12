using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class ReportRepository :GenericRepository<Report>, IReportRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        public ReportRepository(AppDbContext db, IMapper mapper, IFileManagementService fileManagementService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
        }

        public async Task CreateReportAsync(ReportCreateDto reportDto,string UserId)
        {
            var report = _mapper.Map<Report>(reportDto);
            report.ReporterId = UserId;
            await _db.Reports.AddAsync(report);
            _db.SaveChanges();
        }

        public async Task<IEnumerable<ReportDto>> GetAllReportsAsync()
        {
            var reports = await _db.Reports
                .Include(r => r.Reporter)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReportDto>>(reports);
        }
    }

}
