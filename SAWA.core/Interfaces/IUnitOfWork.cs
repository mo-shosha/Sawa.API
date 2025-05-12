using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        public IPostRepository postRepository { get; }
        public ICommentRepository CommentRepository { get; }
        public IBranchesRepository branchesRepository { get; }
        public IHelpRequestRepository helpRequestRepository { get; }
        public IDonationRepository donationRepository { get; }
        public IReportRepository reportRepository { get; }
        public Task SaveAsync();
    }
}
