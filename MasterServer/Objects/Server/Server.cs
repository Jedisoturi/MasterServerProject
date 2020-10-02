using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MasterServer
{
    public class Server
    {
        // TODO: Add attributes
        public Guid Id { get; set; }
        public string Name { get; set; }
        [ValidateIPEndPoint]
        public string EndPoint { get; set; }
        public List<Guid> Players { get; set; }
        public int MaxPlayers { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Guid> BannedPlayers { get; set; }
        public bool HasPassword { get; set; }
        // TODO: Add tags
        // TODO: Add map

        public Server(string name, string endPoint, int maxPlayers, List<Guid> players, 
            List<Guid> bannedPlayers, bool hasPassword)
        {
            Id = Guid.NewGuid();
            Name = name;
            EndPoint = endPoint;
            Players = players;
            MaxPlayers = maxPlayers;
            CreationDate = DateTime.Now;
            BannedPlayers = bannedPlayers;
            HasPassword = hasPassword;
        }
    }
}
