using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using SAWA.core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Services
{
    public class FileManagementService : IFileManagementService
    {
        private readonly IFileProvider fileProvider;
        public FileManagementService(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        public async Task<List<string>> AddImagesAsync(IFormFileCollection files, string src)
        {
            List<string> SaveImageSrc = new List<string>();

            var ImageDirctory = Path.Combine("wwwroot", "Images", src);

            if (!Directory.Exists(ImageDirctory))
            {
                Directory.CreateDirectory(ImageDirctory);
            }

            foreach (var item in files)
            {
                if (item.Length > 0)
                {
                    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);  
                    var imageSrc = $"/Images/{src}/{imageName}";
                    var root = Path.Combine(ImageDirctory, imageName);

                    using (FileStream stream = new FileStream(root, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                    SaveImageSrc.Add(imageSrc);
                }

            }

            return SaveImageSrc;
        }

        public async Task<string> AddImagesAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            var directory = Path.Combine("wwwroot", "Images", folder);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);  
            var filePath = Path.Combine(directory, fileName);
            var fileUrl = $"/Images/{folder}/{fileName}";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileUrl;
        }


        public void DeleteImageAsync(string src)
        {
            var info = fileProvider.GetFileInfo(src);

            var root = info.PhysicalPath;
            File.Delete(root);
        }
    }
}
