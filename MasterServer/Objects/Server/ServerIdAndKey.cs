using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public class ServerIdAndKey
    {
        public Guid Id { get; set; }
        public Guid AdminKey { get; set; }

        public ServerIdAndKey(Guid serverId, Guid adminKey)
        {
            Id = serverId;
            AdminKey = adminKey;
        }
    }
}
