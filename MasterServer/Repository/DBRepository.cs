using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MasterServer
{
    public class DBRepository
    {
        #region Field and init
        private readonly IMongoCollection<Player> _playerCollection;
        private readonly IMongoCollection<Server> _serverCollection;
        private readonly IMongoCollection<ServerIdAndKey> _serverAdminKeyCollection;
        private readonly IMongoCollection<AnalyticEvent> _analyticsCollection;
        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection;
        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection2;
        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection3;
        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection4;

        public DBRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("masterserver");
            _playerCollection = database.GetCollection<Player>("players");
            _serverCollection = database.GetCollection<Server>("servers");
            _serverAdminKeyCollection = database.GetCollection<ServerIdAndKey>("serverAdminKeys");
            _analyticsCollection = database.GetCollection<AnalyticEvent>("analytics");

            _bsonDocumentCollection = database.GetCollection<BsonDocument>("players");
            _bsonDocumentCollection2 = database.GetCollection<BsonDocument>("servers");
            _bsonDocumentCollection3 = database.GetCollection<BsonDocument>("serverAdminKeys");
            _bsonDocumentCollection4 = database.GetCollection<BsonDocument>("analytics");
        }

        #endregion

        #region Player Database
        public async Task<long> GetSize()
        {
            var players = await _playerCollection.Find(new BsonDocument()).CountDocumentsAsync();
            return players;
        }

        public async Task<Player[]> GetAllPlayers(PlayerSort sort = 0)
        {
            var options = new FindOptions() { Collation = new Collation(locale: "en_US", numericOrdering: true) };
            Console.WriteLine(sort);
            SortDefinition<Player> sortDef;
            switch (sort)
            {
                case PlayerSort.TimeAsc:
                    sortDef = Builders<Player>.Sort.Ascending(p => p.CreationTime);
                    break;
                case PlayerSort.TimeDesc:
                    sortDef = Builders<Player>.Sort.Descending(p => p.CreationTime);
                    break;
                case PlayerSort.NameAsc:
                    sortDef = Builders<Player>.Sort.Ascending(p => p.Name);
                    break;
                case PlayerSort.NameDesc:
                    sortDef = Builders<Player>.Sort.Descending(p => p.Name);
                    break;
                case PlayerSort.ScoreAsc:
                    sortDef = Builders<Player>.Sort.Ascending(p => p.Score);
                    break;
                case PlayerSort.ScoreDesc:
                    sortDef = Builders<Player>.Sort.Descending(p => p.Score);
                    break;
                case PlayerSort.LevelAsc:
                    sortDef = Builders<Player>.Sort.Ascending(p => p.Level);
                    break;
                case PlayerSort.LevelDesc:
                    sortDef = Builders<Player>.Sort.Descending(p => p.Level);
                    break;
                default:
                    sortDef = Builders<Player>.Sort.Ascending(p => p.CreationTime);
                    break;
            }
            
            var players = await _playerCollection.Find(new BsonDocument(), options).Sort(sortDef).ToListAsync();
            return players.ToArray();
        }

        public async Task<bool> ValidatePlayerId(Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(player => player.Id, id);
            var result = await _playerCollection.Find(filter).FirstOrDefaultAsync();
            return result != null;
        }

        public async Task<List<Guid>> GetAllPlayerIDs()
        {
            var players = await _playerCollection.Find(new BsonDocument()).Project(d => d.Id).ToListAsync();
            return players;
        }

        public async Task<Player[]> GetAllMinScore(int minScore)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Gte(p => p.Score, minScore);
            var players = await _playerCollection.Find(filter).ToListAsync();
            return players.ToArray();
        }

        public Task<Player> GetPlayer(Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(player => player.Id, id);
            return _playerCollection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<Player> GetPlayer(string name)
        {
            var filter = Builders<Player>.Filter.Eq(player => player.Name, name);
            return _playerCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Player> CreatePlayer(Player player)
        {
            await _playerCollection.InsertOneAsync(player);
            return player;
        }

        public async Task<Player> Rename(Guid playerId, string name)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            var options = new FindOneAndUpdateOptions<Player>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _playerCollection.FindOneAndUpdateAsync(filter, Builders<Player>.Update.Set(p => p.Name, name), options);
        }

        public async Task<Player> IncPlayerScore(Guid playerId, int increment)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            var incrementScoreUpdate = Builders<Player>.Update.Inc(p => p.Score, increment);
            var options = new FindOneAndUpdateOptions<Player>()
            {
                ReturnDocument = ReturnDocument.After
            };
            Player player = await _playerCollection.FindOneAndUpdateAsync(filter, incrementScoreUpdate, options);
            return player;
        }

        public async Task<Player> IncPlayerLevel(Guid playerId, int increment)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            var incrementLevelUpdate = Builders<Player>.Update.Inc(p => p.Level, increment);
            var options = new FindOneAndUpdateOptions<Player>()
            {
                ReturnDocument = ReturnDocument.After
            };
            Player player = await _playerCollection.FindOneAndUpdateAsync(filter, incrementLevelUpdate, options);
            return player;
        }
        public async Task<Player> AddAchievement(Guid id, Achievement index)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, id);
            var addAchievement = Builders<Player>.Update.Set(p => p.Achievements[(int)index], true);
            var options = new FindOneAndUpdateOptions<Player>()
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _playerCollection.FindOneAndUpdateAsync(filter, addAchievement, options);
        }

        public async Task<bool[]> GetAchievements(Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Id, id);
            Player player = await _playerCollection.Find(filter).FirstAsync();
            Console.WriteLine(player.Achievements.TrueCount());
            return player.Achievements.ToArray();
        }

        public async Task<int> GetAchievementCount(Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Id, id);
            Player player = await _playerCollection.Find(filter).FirstAsync();
            return player.Achievements.TrueCount();
        }

        public async Task<Player[]> GetAllWithAchievement(int index)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Achievements[index], true);

            var players = await _playerCollection.Find(filter).ToListAsync();
            return players.ToArray();
        }

        public async Task<LevelCount[]> GetAvgPlayersPerLevel()
        {
            var levelCounts =
                await _playerCollection.Aggregate()
                    .Project(p => new LevelContainer { Level = p.Level })
                    .Group(levelContainer => levelContainer.Level, grouping => new LevelCount { Id = grouping.Key, Count = grouping.Select(levelContainer => levelContainer.Level).Count() })
                    .SortBy(l => l.Id)
                    .ToListAsync();
            return levelCounts.ToArray();
        }
        public async Task<Player[]> GetTop10()
        {
            var sortDef = Builders<Player>.Sort.Descending(p => p.Score);
            var players = await _playerCollection.Find(new BsonDocument()).Sort(sortDef).Limit(10).ToListAsync();

            return players.ToArray();
        }
        #endregion

        #region Server Database

        // TODO: Better error reporting if wrong id is supplied

        public async Task<Server[]> GetAllServers()
        {
            return (await (await _serverCollection.FindAsync(new BsonDocument())).ToListAsync()).ToArray();
        }

        public async Task<Server> GetServer(Guid id)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, id);
            return await (await _serverCollection.FindAsync(filter)).FirstAsync();
        }

        public async Task<Server> CreateServer(Server server)
        {
            await _serverCollection.InsertOneAsync(server);
            return server;
        }

        public async Task<Server> DeleteServer(Guid id)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, id);
            return await _serverCollection.FindOneAndDeleteAsync(filter);
        }

        public async Task<Server> PlayerConnected(Guid serverId, Guid playerId)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, serverId);
            var update = Builders<Server>.Update.Push(s => s.Players, playerId);
            var options = new FindOneAndUpdateOptions<Server>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _serverCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Server> PlayerDisconnected(Guid serverId, Guid playerId)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, serverId);
            var update = Builders<Server>.Update.Pull(s => s.Players, playerId);
            var options = new FindOneAndUpdateOptions<Server>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _serverCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Server> BanPlayer(Guid serverId, Guid playerId)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, serverId);
            var update = Builders<Server>.Update.Push(s => s.BannedPlayers, playerId);
            var options = new FindOneAndUpdateOptions<Server>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _serverCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Server> UnbanPlayer(Guid serverId, Guid playerId)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, serverId);
            var update = Builders<Server>.Update.Pull(s => s.BannedPlayers, playerId);
            var options = new FindOneAndUpdateOptions<Server>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _serverCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Server> ModifyServerName(Guid serverId, string name)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, serverId);
            var update = Builders<Server>.Update.Set(s => s.Name, name);
            var options = new FindOneAndUpdateOptions<Server>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _serverCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Server> ModifyServerEndPoint(Guid serverId, string endPoint)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, serverId);
            var update = Builders<Server>.Update.Set(s => s.EndPoint, endPoint);
            var options = new FindOneAndUpdateOptions<Server>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _serverCollection.FindOneAndUpdateAsync(filter, update, options);
        }
        public async Task<Server> ModifyServerMaxPlayers(Guid serverId, int maxPlayers)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, serverId);
            var update = Builders<Server>.Update.Set(s => s.MaxPlayers, maxPlayers);
            var options = new FindOneAndUpdateOptions<Server>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _serverCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Server> ModifyServerHasPassword(Guid serverId, bool hasPassword)
        {
            var filter = Builders<Server>.Filter.Eq(s => s.Id, serverId);
            var update = Builders<Server>.Update.Set(s => s.HasPassword, hasPassword);
            var options = new FindOneAndUpdateOptions<Server>()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _serverCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Player[]> GetServerPlayerFromDB(Guid id)
        {
            var serverFilter = Builders<Server>.Filter.Eq(s => s.Id, id);
            var playerIds = (await (await _serverCollection.FindAsync(serverFilter)).FirstAsync()).Players;

            var players = new List<Player>();
            foreach (Guid playerId in playerIds)
            {
                var playerFilter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
                players.Add(await (await _playerCollection.FindAsync(playerFilter)).FirstAsync());
            }
            return players.ToArray();
        }

        #endregion

        #region ServerAdminKey Database

        public async Task<ServerIdAndKey> CreateServerAdminKey(ServerIdAndKey serverIdAndKey)
        {
            await _serverAdminKeyCollection.InsertOneAsync(serverIdAndKey);
            return serverIdAndKey;
        }

        public async Task<ServerIdAndKey> DeleteServerAdminKey(Guid id)
        {
            var filter = Builders<ServerIdAndKey>.Filter.Eq(s => s.Id, id);
            return await _serverAdminKeyCollection.FindOneAndDeleteAsync(filter);
        }

        public async Task<Guid> GetAdminKey(Guid id)
        {
            var filter = Builders<ServerIdAndKey>.Filter.Eq(s => s.Id, id);
            return (await (await _serverAdminKeyCollection.FindAsync(filter)).FirstAsync()).AdminKey;
        }

        #endregion

        #region Analytics Database
        public async Task<AnalyticEvent[]> GetEvents(EventType? type, Guid? playerId, Search search)
        {
            //Sorting by creation ascending/descending
            SortDefinition<AnalyticEvent> sortDef;
            if (search.sortAscending)
                sortDef = Builders<AnalyticEvent>.Sort.Ascending(e => e.CreationTime);
            else
                sortDef = Builders<AnalyticEvent>.Sort.Descending(e => e.CreationTime);

            //Filtering by type or not
            FilterDefinition<AnalyticEvent> typeFilter; 
            if(type.HasValue)
                typeFilter = Builders<AnalyticEvent>.Filter.Eq(a => a.Type, type.Value);
            else
                typeFilter = Builders<AnalyticEvent>.Filter.Exists(a => a.Type);

            //Filtering by player or not
            FilterDefinition<AnalyticEvent> playerFilter;
            if (playerId.HasValue)
                playerFilter = Builders<AnalyticEvent>.Filter.Eq(a => a.PlayerId, playerId.Value);
            else
                playerFilter = Builders<AnalyticEvent>.Filter.Exists(a => a.PlayerId);

            //Filtering by creation date start/min or not
            FilterDefinition<AnalyticEvent> creationFilter = Builders<AnalyticEvent>.Filter.Gte(e => e.CreationTime, search.startTime) & Builders<AnalyticEvent>.Filter.Lte(e => e.CreationTime, search.endTime);

            //Combine filters
            FilterDefinition<AnalyticEvent> fullFilter = typeFilter & playerFilter & creationFilter;

            var events = await _analyticsCollection.Find(fullFilter).Sort(sortDef).Limit(search.limit).ToListAsync();

            return events.ToArray();
        }

        public async Task<List<Guid>> GetAllPlayerIDsWithAnalytics(EventType ? type)
        {
            //Filtering by type or not
            FilterDefinition<AnalyticEvent> typeFilter;
            if (type.HasValue)
                typeFilter = Builders<AnalyticEvent>.Filter.Eq(a => a.Type, type.Value);
            else
                typeFilter = Builders<AnalyticEvent>.Filter.Exists(a => a.Type);


            var players = await _analyticsCollection.Find(typeFilter).Project(p => p.PlayerId).ToListAsync();
            List<Guid> playerList = new List<Guid>();
            foreach (Guid p in players)
            {
                if (!playerList.Contains(p))
                    playerList.Add(p);
            }
            return playerList;
        }

        public void aa(Guid player)
        {

        }
        
        public async Task<AnalyticEvent> NewEvent(AnalyticEvent newEvent)
        {
            await _analyticsCollection.InsertOneAsync(newEvent);
            return newEvent;
        }

        #endregion
    }
}
