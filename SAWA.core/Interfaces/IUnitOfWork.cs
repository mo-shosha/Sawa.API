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
        public Task SaveAsync();
    }
}
