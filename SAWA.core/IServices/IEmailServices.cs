using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.IServices
{
    public interface IEmailServices
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);
    }
}

