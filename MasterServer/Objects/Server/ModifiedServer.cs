using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MasterServer
{
    public class ModifiedServer
    {
        public string Name { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public List<Guid> Players { get; set; }
        public int MaxPlayers { get; set; }
        public List<Guid> BannedPlayers { get; set; }
        public bool HasPassword { get; set; }

        public ModifiedServer()
        {
            Players = new List<Guid>();
            BannedPlayers = new List<Guid>();
        }
    }
}
