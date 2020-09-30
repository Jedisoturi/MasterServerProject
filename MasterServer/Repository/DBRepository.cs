using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MasterServer
{
    public class DBRepository
    {
        private readonly IMongoCollection<Player> _playerCollection;
        private readonly IMongoCollection<Server> _serverCollection;
        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection;
        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection2;

        public DBRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("masterserver");
            _playerCollection = database.GetCollection<Player>("players");
            _serverCollection = database.GetCollection<Server>("servers");

            _bsonDocumentCollection = database.GetCollection<BsonDocument>("players");
            _bsonDocumentCollection2 = database.GetCollection<BsonDocument>("servers");
        }

        #region Player Database

        public async Task<Player[]> GetAllPlayers()
        {
            var players = await _playerCollection.Find(new BsonDocument()).ToListAsync();
            return players.ToArray();
        }
        public async Task<Player[]> GetAllMinScore(int minScore)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Gte(p => p.Score, minScore);
            var players = await _playerCollection.Find(filter).ToListAsync();
            return players.ToArray();
        }

        public async Task<Player[]> GetAllSortScore( )
        {
            return null;
        }
        public async Task<Player[]> GetAllSortDate()
        {
            return null;
        }
        public Task<Player> GetPlayer(Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(player => player.Id, id);
            return _playerCollection.Find(filter).FirstAsync();
        }

        public Task<Player> GetPlayer(string name)
        {
            var filter = Builders<Player>.Filter.Eq(player => player.Name, name);
            return _playerCollection.Find(filter).FirstAsync();
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
            return await _playerCollection.FindOneAndUpdateAsync(filter, Builders<Player>.Update.Set("Name", name), options);
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
        public async Task<Player[]> AddAchievement(Guid id, int index)
        {
            return null;
        }

        public async Task<bool[]> GetAchievements(Guid id)
        {
            return null;
        }

        public async Task<Player[]> GetAllWithAchievement(int index)
        {
            return null;
        }

        public async Task<Player[]> GetAvgPlayersPerLevel()
        {
            return null;
        }
        public async Task<Player[]> GetTop10Descending()
        {
            var sortDef = Builders<Player>.Sort.Descending(p => p.Score);
            var players = await _playerCollection.Find(new BsonDocument()).Limit(10).Sort(sortDef).ToListAsync();

            return players.ToArray();
        }

        #endregion
    }
}
