using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer.ErrorHandling
{
    public class AppAuthenticationException : Exception
    {
        public AppAuthenticationException()
        {
        }

        public AppAuthenticationException(string message)
            : base(message)
        {
        }

        public AppAuthenticationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
