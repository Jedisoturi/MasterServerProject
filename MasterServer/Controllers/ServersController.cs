using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MasterServer.Controllers
{
    [ApiController]
    [Route("api/servers")]
    public class ServersController : ControllerBase
    {
        private readonly ILogger<ServersController> _logger;
        private readonly DBRepository _repository;

        public ServersController(ILogger<ServersController> logger, DBRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        #region CoreRoutes

        [HttpGet]
        public async Task<Server[]> GetAll()
        {
            return await _repository.GetAllServers();
        }

        [HttpGet("{id:Guid}")]
        public async Task<Server> Get(Guid id)
        {
            return await _repository.GetServer(id);
        }

        [AppAuthenticationFilter]
        [HttpPost("create")]
        public async Task<ServerAndKey> Create([FromBody] NewServer newServer, string signature = null)
        {
            var server = new Server(
                newServer.Name, 
                newServer.EndPoint, 
                newServer.MaxPlayers, 
                newServer.Players,
                newServer.BannedPlayers,
                newServer.HasPassword
            );

            var adminKey = Guid.NewGuid();

            await _repository.CreateServer(server);
            await _repository.CreateServerAdminKey(new ServerIdAndKey(server.Id, adminKey));

            return new ServerAndKey(server, adminKey);
        }

        [AppAuthenticationFilter]
        [HttpDelete("{id:Guid}")]
        public async Task<Server> Delete(Guid id, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(id, adminKey);
            await _repository.DeleteServerAdminKey(id);
            return await _repository.DeleteServer(id);
        }

        [AppAuthenticationFilter]
        [HttpPost("{serverId:Guid}/playerConnected/{playerId:Guid}")]
        public async Task<Server> PlayerConnected(Guid serverId, Guid playerId, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(serverId, adminKey);
            return await _repository.PlayerConnected(serverId, playerId);
        }

        [AppAuthenticationFilter]
        [HttpPost("{serverId:Guid}/playerDisconnected/{playerId:Guid}")]
        public async Task<Server> PlayerDisconnected(Guid serverId, Guid playerId, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(serverId, adminKey);
            return await _repository.PlayerDisconnected(serverId, playerId);
        }

        [AppAuthenticationFilter]
        [HttpPost("{serverId:Guid}/banPlayer/{playerId:Guid}")]
        public async Task<Server> BanPlayer(Guid serverId, Guid playerId, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(serverId, adminKey);
            return await _repository.BanPlayer(serverId, playerId);
        }

        [AppAuthenticationFilter]
        [HttpPost("{serverId:Guid}/unbanPlayer/{playerId:Guid}")]
        public async Task<Server> UnbanPlayer(Guid serverId, Guid playerId, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(serverId, adminKey);
            return await _repository.UnbanPlayer(serverId, playerId);
        }

        #endregion

        #region Modify

        [HttpPost("{id:Guid}")]
        public async Task<Server> Modify(Guid id, [FromBody] ModifiedServer modifiedServer)
        {
            // TODO: Implement server modify
            //return await _repository.Modify(id, modifiedServer);
            return null;
        }

        [AppAuthenticationFilter]
        [HttpPost("{serverId:Guid}/modifyName/{name}")]
        public async Task<Server> ModifyName(Guid serverId, string name, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(serverId, adminKey);
            return await _repository.ModifyServerName(serverId, name);
        }

        [AppAuthenticationFilter]
        [HttpPost("{serverId:Guid}/modifyEndPoint/{endPoint}")]
        public async Task<Server> ModifyEndPoint(Guid serverId, [ValidateIPEndPoint] string endPoint, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(serverId, adminKey);
            return await _repository.ModifyServerEndPoint(serverId, endPoint);
        }

        [AppAuthenticationFilter]
        [HttpPost("{serverId:Guid}/modifyMaxPlayers/{maxPlayers:int}")]
        public async Task<Server> ModifyMaxPlayers(Guid serverId, int maxPlayers, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(serverId, adminKey);
            return await _repository.ModifyServerMaxPlayers(serverId, maxPlayers);
        }

        [AppAuthenticationFilter]
        [HttpPost("{serverId:Guid}/modifyHasPassword/{hasPassword:bool}")]
        public async Task<Server> ModifyHasPassword(Guid serverId, bool hasPassword, Guid? adminKey, string signature = null)
        {
            await ValidateAdminKey(serverId, adminKey);
            return await _repository.ModifyServerHasPassword(serverId, hasPassword);
        }

        #endregion

        #region GetRoutes

        [HttpGet("{id:Guid}/players")]
        public async Task<Player[]> GetPlayers(Guid id)
        {
            return await _repository.GetServerPlayerFromDB(id);
        }

        #endregion

        #region Tools

        private async Task ValidateAdminKey(Guid id, Guid? adminKey)
        {
            if (!adminKey.HasValue) throw new Exception($"You must provide an adminKey");
            if (await _repository.GetAdminKey(id) != adminKey.Value) throw new Exception($"AdminKey doesn't match server");
        }

        #endregion
    }
}
