using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.API.Models.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }

    public class ServiceNotFoundException : DomainException
    {
        public ServiceNotFoundException(string message) : base(message)
        {
        }
    }
}
