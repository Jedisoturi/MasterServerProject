﻿using System;
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
        public async Task<long> GetSize()
        {
            var players = await _playerCollection.Find(new BsonDocument()).CountDocumentsAsync();
            return players;
        }

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
            var sortDef = Builders<Player>.Sort.Descending(p => p.Score);
            var players = await _playerCollection.Find(new BsonDocument()).Sort(sortDef).ToListAsync();

            return players.ToArray();
        }
        public async Task<Player[]> GetAllSortDate()
        {
            var sortDef = Builders<Player>.Sort.Descending(p => p.CreationTime);
            var players = await _playerCollection.Find(new BsonDocument()).Sort(sortDef).ToListAsync();

            return players.ToArray();
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
        public async Task<Player> AddAchievement(Guid id, int index)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, id);
            var addAchievement = Builders<Player>.Update.Set(p => p.Achievements[index], true);
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

        public async Task<Player[]> GetTop3Achievers()
        {
            var sortDef = Builders<Player>.Sort.Descending(p => p.Achievements.TrueCount());
            var players = await _playerCollection.Find(new BsonDocument()).Sort(sortDef).Limit(3).ToListAsync();

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
    }
}
