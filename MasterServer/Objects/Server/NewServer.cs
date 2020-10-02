using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MasterServer
{
    public class NewServer
    {
        public string Name { get; set; }
        [ValidateIPEndPoint]
        public string EndPoint { get; set; }
        public List<Guid> Players { get; set; }
        public int MaxPlayers { get; set; }
        public List<Guid> BannedPlayers { get; set; }
        public bool HasPassword { get; set; }

        public NewServer()
        {
            // TODO: Force to provide all values on creation
            Players = new List<Guid>();
            BannedPlayers = new List<Guid>();
        }
    }
}
