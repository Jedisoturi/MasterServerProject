using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public static class ExtensionMethods
    {
        public static int TrueCount(this bool[] array)
        {
            return array.Count(t => t);
        }
    }
}
