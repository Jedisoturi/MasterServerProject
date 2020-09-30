using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MasterServer.Controllers
{
    [ApiController]
    [Route("servers")]
    public class ServersController : ControllerBase
    {
        private readonly ILogger<ServersController> _logger;
        private readonly DBRepository _repository;

        public ServersController(ILogger<ServersController> logger, DBRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
#if false
#region CoreRoutes

        [HttpGet]
        public async Task<Server[]> GetAll()
        {
            return await _repository.GetAll();
        }

        [HttpGet("{id:Guid}")]
        public async Task<Server> Get(Guid id)
        {
            return await _repository.Get(id);
        }

        [HttpPost]
        public async Task<ServerAndKey> Create([FromBody] NewServer newServer)
        {
            var server = new Server(
                newServer.Name, 
                newServer.EndPoint, 
                newServer.MaxPlayers, 
                newServer.Players,
                newServer.BannedPlayers,
                newServer.HasPassword
            );

            Guid adminKey = Guid.NewGuid();


            return await _repository.Create(server);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<Server> Delete(Guid id, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(id, adminKey))
            {
                return await _repository.Delete(id);
            }
            else
            {
                return null;
            }
        }

        [HttpPatch("{serverId:Guid}/playerConnected/{playerId:Guid}")]
        public async Task<Server> PlayerConnected(Guid serverId, Guid playerId, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(serverId, adminKey))
            {
                return await _repository.PlayerConnected(serverId, playerId);
            }
            else
            {
                return null;
            }
        }

        [HttpPatch("{serverId:Guid}/playerDisconnected/{playerId:Guid}")]
        public async Task<Server> PlayerDisconnected(Guid serverId, Guid playerId, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(serverId, adminKey))
            {
                return await _repository.PlayerDisconnected(serverId, playerId);
            }
            else
            {
                return null;
            }
        }

        [HttpPatch("{serverId:Guid}/banPlayer/{playerId:Guid}")]
        public async Task<Server> BanPlayer(Guid serverId, Guid playerId, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(serverId, adminKey))
            {
                return await _repository.BanPlayer(serverId, playerId);
            }
            else
            {
                return null;
            }
        }

        [HttpPatch("{serverId:Guid}/unbanPlayer/{playerId:Guid}")]
        public async Task<Server> UnbanPlayer(Guid serverId, Guid playerId, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(serverId, adminKey))
            {
                return await _repository.UnbanPlayer(serverId, playerId);
            }
            else
            {
                return null;
            }
        }

#endregion

#region Modify

        [HttpPatch("{id:Guid}")]
        public async Task<Server> Modify(Guid id, [FromBody] ModifiedServer modifiedServer)
        {
            return await _repository.Modify(id, modifiedServer);
        }

        // TODO: Maybe change validation of string from alpha
        [HttpPatch("{serverId:Guid}/modifyName/{name:alpha}")]
        public async Task<Server> ModifyName(Guid serverId, string name, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(serverId, adminKey))
            {
                return await _repository.ModifyName(serverId, name);
            }
            else
            {
                return null;
            }
        }

        [HttpPatch("{serverId:Guid}/modifyEndPoint/{endPoint:IPEndPoint}")]
        public async Task<Server> ModifyEndPoint(Guid serverId, IPEndPoint endPoint, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(serverId, adminKey))
            {
                return await _repository.ModifyName(serverId, endPoint);
            }
            else
            {
                return null;
            }
        }

        [HttpPatch("{serverId:Guid}/modifyMaxPlayers/{maxPlayers:int}")]
        public async Task<Server> ModifyMaxPlayers(Guid serverId, int maxPlayers, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(serverId, adminKey))
            {
                return await _repository.ModifyMaxPlayers(serverId, maxPlayers);
            }
            else
            {
                return null;
            }
        }

        [HttpPatch("{serverId:Guid}/modifyHasPassword/{hasPassword:bool}")]
        public async Task<Server> ModifyHasPassword(Guid serverId, bool hasPassword, Guid? adminKey)
        {
            if (adminKey.HasValue && await _repository.CheckKey(serverId, adminKey))
            {
                return await _repository.HasPassword(serverId, hasPassword);
            }
            else
            {
                return null;
            }
        }

#endregion

#region GetRoutes

        [HttpGet("{id:Guid}/players")]
        public async Task<Player[]> GetPlayers(Guid id)
        {
            return await _repository.GetPlayers(id);
        }

#endregion

#endif
    }
}
