using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public interface IEvent
    {
        public Guid PlayerId { get; }
        public string Message { get; }
        public DateTime CreationTime { get; }
    }
}
