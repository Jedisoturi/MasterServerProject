using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public class AnalyticEvent
    {
        public AnalyticEvent(EventType type, Guid playerId, string message, DateTime creationTime)
        {
            Type = type;
            Id = Guid.NewGuid();
            PlayerId = playerId;
            Message = message;
            CreationTime = creationTime;
        }
        public EventType Type { get; private set; }
        public Guid Id { get; private set; }
        public Guid PlayerId { get; private set; }
        public string Message { get; private set; }
        public DateTime CreationTime { get; private set; }
    }
}
