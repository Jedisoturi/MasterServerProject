using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MasterServer.Controllers
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
        }
    }
}