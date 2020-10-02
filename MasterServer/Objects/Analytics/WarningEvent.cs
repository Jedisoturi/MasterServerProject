using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public class WarningEvent : IEvent
    {
        public WarningEvent(Guid playerId, string message, DateTime creationTime)
        {
            PlayerId = playerId;
            Message = message;
            CreationTime = creationTime;
        }

        public Guid PlayerId { get; private set; }
        public string Message { get; private set; }
        public DateTime CreationTime { get; private set; }
    }
}
