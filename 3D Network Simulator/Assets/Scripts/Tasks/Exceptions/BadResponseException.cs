using System;

namespace GNS3.ProjectHandling.Exceptions
{
    public class BadResponseException : Exception
    {
        public BadResponseException(string message) : base(message)
        {
        }
    }
}