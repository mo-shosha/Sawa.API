using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.IServices
{
    public interface IGenerateTokenServices
    {
        Task<string> GetAndCreateToken(AppUser user);
    }
}
