using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    public class Constants
    {
        public static readonly byte[] secret = Encoding.UTF8.GetBytes("8EB6F8D11A6D3F42813B2C43DD6C8".ToLower());
    }
}
