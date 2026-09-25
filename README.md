# RoomReserve

REST API for conference room booking. Features include availability search by date, time, and capacity, booking with automatic price calculation based on time zones and selected extra services, and business revenue analytics.

## About the Project

The service allows:
- Managing a room catalog with capacity, base price per hour, and extra services.
- Searching for available rooms for a specific date and time with capacity filtering.
- Booking a room with automatic final price calculation.
- Generating business revenue analytics.

## Tech Stack

| Category | Technology |
|---|---|
| Language | C# |
| Platform | .NET 10 |
| Framework | ASP.NET Core Minimal APIs |
| Architecture | Vertical Slice Architecture, CQRS |
| Libraries | MediatR, FluentValidation |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Documentation | Swagger, OpenAPI |
| Containerization | Docker, Docker Compose |

## Architecture

The project is built using Vertical Slice Architecture with DDD elements. Each business operation is independent and easily scalable.

```text
Features/
 ├─ Rooms/          Commands: Create, Update, Delete | Queries: GetAvailableRooms
 ├─ Bookings/       Commands: BookRoom
 ├─ Services/       Queries: GetAllServices
 └─ Analytics/      Queries: GetRevenueSummary, GetRoomServiceRevenue
```

Key technical decisions:
- **Rich Domain Model** — entities have private setters and are created only via factory methods. These methods check business invariants and throw DomainException on violation.
- **Result Pattern** — expected business errors return as Result objects rather than exceptions. Exceptions are reserved for critical failures.
- **Pipeline validation** — ValidationBehavior in MediatR automatically validates every Command and Query via FluentValidation before handler execution.
- **Centralized error handling** — GlobalExceptionHandler and ValidationExceptionHandler convert any exceptions into a unified ProblemDetails response without exposing implementation details.
- **Optimistic concurrency** — uses RowVersion and handles DbUpdateConcurrencyException during booking to prevent double booking.
- **Pagination** — GetAvailableRooms returns a PaginatedList suitable for large catalogs.

## Business Logic

### PricingCalculator

### Seed Data

| Room | Capacity | Base Price/Hour |
|---|---|---|
| Room A | 50 | 2000 UAH |
| Room B | 100 | 3500 UAH |
| Room C | 30 | 1500 UAH |

| Service | Price |
|---|---|
| Projector | 500 UAH |
| Wi-Fi | 300 UAH |
| Sound | 700 UAH |

Data is seeded automatically on startup in the Development environment.

## Running the Project

### Docker Compose

1. Copy the environment file:
   
```bash
   cp .env.example .env
```

2. Build and run:
   
```bash
   docker compose up --build
```

3. Access points:
   - API: **http://localhost:5123**
   - Swagger UI: **http://localhost:5123/swagger**

## API

Base path is `/api`. Full documentation is available in Swagger UI.

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/rooms` | Create a conference room |
| `PUT` | `/api/rooms/{roomId}` | Update a room |
| `DELETE` | `/api/rooms/{roomId}` | Delete a room |
| `GET` | `/api/rooms` | List rooms with date, time, capacity filters, and pagination |
| `POST` | `/api/rooms/{roomId}/bookings` | Book a room and get final price |
| `GET` | `/api/services` | List available services |
| `GET` | `/analytics/revenue` | Revenue report by rooms for a date range |
| `GET` | `/analytics/service-revenue` | Usage and revenue report for extra services |

## Error Handling and Validation

- Incoming data is validated by FluentValidation. Validation errors return 400 Bad Request as ValidationProblemDetails.
- Expected business errors return respective HTTP status codes via the Result pattern.
- Unhandled exceptions are caught by GlobalExceptionHandler and return 500 Internal Server Error without exposing implementation details.


## Project Structure

```text
RoomReserve/
├─ docker-compose.yml / .override.yml  
├─ .env.example                         
└─ src/RoomReserve.API/
   ├─ Program.cs                        
   ├─ Extensions/                       # DI, Swagger, services registration
   ├─ Models/                           
   ├─ Features/
   │  ├─ Rooms/{Commands,Queries,Dtos}
   │  ├─ Bookings/{Commands,Dtos}
   │  ├─ Services/{Queries,Dtos}
   │  └─ Analytics/{Queries,Dtos}
   ├─ Services/PricingCalculator.cs     # pricing service 
   ├─ Infrastructure/
   │  ├─ Persistence/                   # DbContext, configurations, behavior
   │  └─ Behaviors/ValidationBehavior.cs
   └─ Middlewares/   
```
