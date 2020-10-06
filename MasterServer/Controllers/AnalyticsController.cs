using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MasterServer
{
    namespace MasterServer
    {
        [ApiController]
        [Route("api/analytics")]
        public class AnalyticsController : ControllerBase
        {
            private readonly ILogger<AnalyticsController> _logger;
            private readonly DBRepository _repo;

            public AnalyticsController(ILogger<AnalyticsController> logger, DBRepository repo)
            {
                _logger = logger;
                _repo = repo;
                //rand = new Random();
            }
                     
            [HttpGet]
            public async Task<AnalyticEvent[]> GetAll(EventType? type, Guid? playerId, int? limit, bool? sortAscending, DateTime? startTime, DateTime? endTime, string message = null)
            {
                int searchLimit = limit.HasValue ? limit.Value : Int32.MaxValue;
                bool searchSort = sortAscending.HasValue ? sortAscending.Value : false;
                DateTime searchStartTime = startTime.HasValue ? startTime.Value : DateTime.MinValue;
                DateTime searchEndTime = endTime.HasValue ? endTime.Value : DateTime.MaxValue;
                Search search = new Search(message, searchLimit, searchSort, searchStartTime, searchEndTime);

                return await _repo.GetEvents(type, playerId, search);
            }

            [HttpGet("allGuids")]
            public async Task<List<Guid>> GetAllGuids(EventType ? type)
            {
                return await _repo.GetAllPlayerIDsWithAnalytics(type);
            }

            [HttpPost("new/{type:int}")]
            public async Task<AnalyticEvent> NewEvent(EventType type, [FromBody] NewEvent inEvent)
            {
                Player player = await _repo.GetPlayer(inEvent.PlayerId);
                if (player == null || player.Id != inEvent.PlayerId) throw new IdNotFoundException();

                if (inEvent.Message == null || inEvent.Message.Length == 0)
                    throw new Exception("Empty message");

                AnalyticEvent outEvent = new AnalyticEvent((EventType)type, inEvent.PlayerId, inEvent.Message, DateTime.Now);
                return await _repo.NewEvent(outEvent);
            }
        }
    }
}