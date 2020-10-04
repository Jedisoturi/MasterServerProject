using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public class NewEvent
    {
        public Guid PlayerId { get; set; }
        public string Message { get; set; }
    }
}
