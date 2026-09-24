using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReserve.Domain.Models
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
            return new Service
            {
                Id = Guid.CreateVersion7(),
                Name = name,
                Price = price
            };
        }
    }
}
