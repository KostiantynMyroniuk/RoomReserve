using RoomReserve.API.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.API.Models
{
    public class Service
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public decimal Price { get; private set; }

        private Service()
        {
        }

        public static Service Create(
            string name,
            decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Service name must not be empty.");
            }

            if (price < 0)
            {
                throw new DomainException("Service price cannot be negative.");
            }

            return new Service
            {
                Id = Guid.CreateVersion7(),
                Name = name,
                Price = price
            };
        }
    }
}
