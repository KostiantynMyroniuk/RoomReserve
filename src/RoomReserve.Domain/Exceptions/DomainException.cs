using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Domain.Exceptions
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
