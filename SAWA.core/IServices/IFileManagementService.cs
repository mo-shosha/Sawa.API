using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.IServices
{
    public interface IFileManagementService
    {
        Task<List<string>> AddImagesAsync(IFormFileCollection files, string src);
        Task<string> AddImagesAsync(IFormFile file, string src);
        void DeleteImageAsync(string src);
    }
}
