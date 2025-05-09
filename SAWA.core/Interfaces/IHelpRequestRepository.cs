using SAWA.core.DTO;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Interfaces
{
    public interface IHelpRequestRepository:IGenericRepository<HelpRequest>
    {
        Task<HelpRequestDto> AddHelpRequestAsync(HelpRequestCreateDto createDto, string UserId);
        Task<HelpRequestDto> UpdateHelpRequestStatusAsync(HelpRequestUpdateStatusDto updateStatusDto);
    }
}
