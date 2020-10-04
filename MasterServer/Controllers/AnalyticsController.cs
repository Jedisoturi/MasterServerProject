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
            /*
            [HttpGet("all")]
            public async Task<IEvent[]> GetAll()
            {
                return await _repo.GetAllEvents();
            }*/
            
            
            [HttpGet]
            public async Task<AnalyticEvent[]> GetAll(EventType? type, Guid? playerId, int? limit, bool? sortAscending, DateTime? startTime, DateTime? endTime)
            {
                int searchLimit = limit.HasValue ? limit.Value : Int32.MaxValue;
                bool searchSort = sortAscending.HasValue ? sortAscending.Value : false;
                DateTime searchStartTime = startTime.HasValue ? startTime.Value : DateTime.MinValue;
                DateTime searchEndTime = endTime.HasValue ? endTime.Value : DateTime.MaxValue;
                Search search = new Search(searchLimit, searchSort, searchStartTime, searchEndTime);

                return await _repo.GetEvents(type, playerId, search);
            }
            

            [HttpPost("new/{type:int}")]
            public async Task<AnalyticEvent> NewEvent(EventType type, [FromBody] NewEvent inEvent)
            {
                //if (type < 0 || type >= Enum.GetNames(typeof(EventType)).Length)
                //    throw new Exception("Bad event type, valid types are: 0 to " + ((Enum.GetNames(typeof(EventType)).Length) - 1));

                if (inEvent.Message == null || inEvent.Message.Length == 0)
                    throw new Exception("Empty message");

                AnalyticEvent outEvent = new AnalyticEvent((EventType)type, inEvent.PlayerId, inEvent.Message, DateTime.Now);
                return await _repo.NewEvent(outEvent);
            }
        }
    }
}