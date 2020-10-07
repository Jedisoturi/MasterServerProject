using System;
using System.Collections.Generic;
using System.Linq;
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

        public PlayerController(ILogger<PlayerController> logger, DBRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpGet]
        public async Task<Player[]> GetAll(PlayerSort sort = 0)
        {
            return await _repo.GetAllPlayers(sort);
        }

        [HttpGet("allGuids")]
        public async Task<List<Guid>> GetAllGuids()
        {
            return await _repo.GetAllPlayerIDs();
        }

        [HttpGet("{id:Guid}")]
        public async Task<Player> Get(Guid id)
        {
            Player player = await _repo.GetPlayer(id);
            return player;
        }

        [HttpGet("name/{name}")]
        public async Task<Player> Get(string name)
        {
            Player player = await _repo.GetPlayer(name);
            return player;
        }
        
        [AppAuthenticationFilter]
        [HttpPost("create")]
        public async Task<Player> Create(string name = null)
        {
            if (name == null)
                name = "Player" + (await _repo.GetSize() + 1);
            Player player = new Player();
            player.Id = Guid.NewGuid();
            player.Name = name;
            player.CreationTime = DateTime.Now;

            return await _repo.CreatePlayer(player);
        }

        [AppAuthenticationFilter]
        [HttpPost("{id}/rename/{name}")]
        public async Task<Player> Rename(Guid id, string name)
        {
            await ValidatePlayerId(id);
            return await _repo.Rename(id, name);
        }

        [AppAuthenticationFilter]
        [HttpPost("{id}/incScore/{inc}")]
        public async Task<Player> IncPlayerScore(Guid id, int inc)
        {
            await ValidatePlayerId(id);
            return await _repo.IncPlayerScore(id, inc);
        }

        [AppAuthenticationFilter]
        [HttpPost("{id}/incLevel/{inc}")]
        public async Task<Player> IncPlayerLevel(Guid id, int inc)
        {
            await ValidatePlayerId(id);
            return await _repo.IncPlayerLevel(id, inc);
        }

        [AppAuthenticationFilter]
        [HttpPost("{id}/addAchievement/{index}")]
        public async Task<Player> AddAchievement(Guid id, Achievement index)
        {
            await ValidatePlayerId(id);
            return await _repo.AddAchievement(id, index);
        }

        [HttpGet("{id}/achievements")]
        public async Task<bool[]> GetAchievements(Guid id)
        {
            await ValidatePlayerId(id);
            return await _repo.GetAchievements(id);
        }

        [HttpGet("{id}/achievementCount")]
        public async Task<int> GetAchievementCount(Guid id)
        {
            await ValidatePlayerId(id);
            return await _repo.GetAchievementCount(id);
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

        private async Task ValidatePlayerId(Guid id)
        {
            if (!(await _repo.ValidatePlayerId(id))) throw new IdNotFoundException("Could not find player with ID: " + id);
        }
    }
}