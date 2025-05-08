using SAWA.core.DTO;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Interfaces
{
    public interface IBranchesRepository:IGenericRepository<Branch>
    {
        Task CreateBranchAsync(BranchCreateDto model, string CharityId);
        Task<IEnumerable<BranchDto>> GetBranchesWithPhotosAsync();
        Task UpdateBranchAsync(int branchId, BranchUpdateDto model);
        Task<bool> DeleteBrachAsync(int id);

        
    }
}
