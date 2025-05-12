using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.DTO
{
    public record ReportDto
    {
        public int Id { get; set; }
        public string ReporterName { get; set; }
        public int TargetId { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }  
        public DateTime CreatedAt { get; set; }
    }

    public record ReportCreateDto
    {
        public int TargetId { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; } 
    }

}
