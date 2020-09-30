using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

public class DBRepository
{
    private readonly IMongoCollection<Player> _playerCollection;
    private readonly IMongoCollection<Server> _serverCollection;
    private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection;
    private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection2;

    public MongoDbRepository()
    {
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var database = mongoClient.GetDatabase("game");
        _playerCollection = database.GetCollection<Player>("players");
        _serverCollection = database.GetCollection<Player>("servers");

        _bsonDocumentCollection = database.GetCollection<BsonDocument>("players");
        _bsonDocumentCollection2 = database.GetCollection<BsonDocument>("servers");
    }

    #region Player Database

    public async Task<Player> CreatePlayer(Player player)
    {
        await _playerCollection.InsertOneAsync(player);
        return player;
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


    public async Task<Player[]> GetAllTag(string tag)
    {
        var filter = Builders<Player>.Filter.AnyEq(p => p.Tags, tag);

        var players = await _playerCollection.Find(filter).ToListAsync();
        return players.ToArray();
    }

    public async Task<Player[]> GetAllItemType(ItemType type)
    {
        var playersWithWeapons =
            Builders<Player>.Filter.ElemMatch<Item>(
                p => p.Items,
                Builders<Item>.Filter.Eq(
                    i => i.Type,
                    type
                )
            );

        var players = await _playerCollection.Find(playersWithWeapons).ToListAsync();
        return players.ToArray();
    }

    public async Task<Player[]> GetAllItemCount(int count)
    {
        var filter = Builders<Player>.Filter.Size(p => p.Items, count);

        var players = await _playerCollection.Find(filter).ToListAsync();
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

    public async Task<Player> IncrementPlayerScore(Guid playerId, int increment)
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



    public async Task<Player> Rename(Guid playerId, string name)
    {
        FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
        var options = new FindOneAndUpdateOptions<Player>()
        {
            ReturnDocument = ReturnDocument.After
        };
        return await _playerCollection.FindOneAndUpdateAsync(filter, Builders<Player>.Update.Set("Name", name), options);
    }

    public async Task<Player> DeletePlayer(Guid playerId)
    {
        FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
        return await _playerCollection.FindOneAndDeleteAsync(filter);
    }

    public async Task<Player[]> GetTop10SortedByScoreDescending()
    {
        var sortDef = Builders<Player>.Sort.Descending(p => p.Score);
        var players = await _playerCollection.Find(new BsonDocument()).Limit(10).Sort(sortDef).ToListAsync();

        return players.ToArray();
    }

    public async Task<int> GetMostCommonLevel()
    {
        var query = _playerCollection.AsQueryable()
            .GroupBy(p => p.Level)
            .Select(p => new LevelCount { Id = p.Key, Count = p.Count() })
            .OrderByDescending(p => p.Count)
            .First();
        return query.Count;
    }
    public async Task<float> GetAvgScoreBetweenDates(DateTime a, DateTime b)
    {
        var query = _playerCollection.AsQueryable()
            .Where(p => a <= p.CreationTime && p.CreationTime <= b)
            .Select(p => p.Score)
            .Average(p => (float)p);
        return query;
    }
    #endregion
}

