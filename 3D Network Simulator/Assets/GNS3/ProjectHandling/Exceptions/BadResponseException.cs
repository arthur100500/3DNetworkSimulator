using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GNSHandling
{
    public class BadResponseException : Exception
    {
        public BadResponseException() : base()
        {
        }

        public BadResponseException(string message) : base(message)
        {
        }

        public BadResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}