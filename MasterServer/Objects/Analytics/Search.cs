using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public struct Search
    {
        public Search(string message, int limit, bool sortAscending, DateTime startTime, DateTime endTime)
        { 
            this.message = message;
            this.limit = limit;
            this.sortAscending = sortAscending;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public string message;
        public int limit;
        public bool sortAscending;
        public DateTime startTime;
        public DateTime endTime;
    }
}
