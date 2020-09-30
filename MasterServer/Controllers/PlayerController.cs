using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MasterServer
{
    [ApiController]
    [Route("api/players")]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly DBRepository _repo;
        //Random rand;
        public PlayerController(ILogger<PlayerController> logger, DBRepository repo)
        {
            _logger = logger;
            _repo = repo;
            //rand = new Random();
        }

        [HttpGet]
        public async Task<Player[]> GetAll(int? minScore)
        {
            if (minScore.HasValue)
                return await _repo.GetAllMinScore(minScore.Value);
            else
                return await _repo.GetAllPlayers();
        }

        [HttpGet("sortScore")]
        public async Task<Player[]> GetAllSortScore()
        {
            return await _repo.GetAllSortScore();
        }

        [HttpGet("sortDate")]
        public async Task<Player[]> GetAllSortDate()
        {
            return await _repo.GetAllSortDate();
        }

        [HttpGet("{id:Guid}")]
        public async Task<Player> Get(Guid id)
        {
            Player player = await _repo.GetPlayer(id);
            return player;
        }

        [HttpGet("{name:alpha}")]
        public async Task<Player> Get(string name)
        {
            Player player = await _repo.GetPlayer(name);
            return player;
        }

        [HttpPost("create")]
        public async Task<Player> Create(string name = null)
        {
            if (name == null)
                name = "Player" + (await _repo.GetSize() + 1); //rand.Next(1, Int32.MaxValue);
            Player player = new Player();
            player.Id = Guid.NewGuid();
            player.Name = name;
            player.CreationTime = DateTime.Now;
            return await _repo.CreatePlayer(player);
        }

        [HttpPost("{id}/rename/({name}")]
        public async Task<Player> Delete(Guid id, string name)
        {
            return await _repo.Rename(id, name);
        }

        [HttpPost("{id}/incScore/{inc}")]
        public async Task<Player> IncPlayerScore(Guid id, int inc)
        {
            return await _repo.IncPlayerScore(id, inc);
        }

        [HttpPost("{id}/incLevel/{inc}")]
        public async Task<Player> IncPlayerLevel(Guid id, int inc)
        {
            return await _repo.IncPlayerLevel(id, inc);
        }

        [HttpPost("{id}/addAchievement/{index}")]
        public async Task<Player> AddAchievement(Guid id, int index)
        {
            return await _repo.IncPlayerLevel(id, index);
        }

        [HttpGet("{id}/achievements")]
        public async Task<bool[]> GetAchievements(Guid id)
        {
            return await _repo.GetAchievements(id);
        }

        [HttpGet("AllWithAchievement/{index}")]
        public async Task<Player[]> GetAllWithAchievement(int index)
        {
            return await _repo.GetAllWithAchievement(index);
        }

        [HttpGet("AvgPlayersPerLevel")]
        public async Task<LevelCount[]> GetAvgPlayersPerLevel()
        {
            return await _repo.GetAvgPlayersPerLevel();
        }

        [HttpGet("top10")]
        public async Task<Player[]> GetTop10(int? minScore)
        {
            return await _repo.GetTop10();
        }
    }
}