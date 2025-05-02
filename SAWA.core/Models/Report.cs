using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Models
{
    public enum ReportType
    {
        Post,
        Comment
    }

    public class Report : BaseEntity<int>
    {
        public string ReporterId { get; set; }
        public AppUser Reporter { get; set; } // The user who reported
        public int TargetId { get; set; } // The post or comment ID being reported
        public ReportType Type { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
