# Alphatech — Event-Driven Microservices Sample (.NET)

A working microservices reference implementation demonstrating an API Gateway, two independent backing services communicating asynchronously over RabbitMQ, and an MVC front-end — the kind of architecture used for enterprise order/inventory systems.

## Architecture

```
                    ┌─────────────────┐
                    │  OrderManagement │  (ASP.NET MVC front-end)
                    │        UI        │
                    └────────┬─────────┘
                             │ REST
                    ┌────────▼─────────┐
                    │   APIGateway      │  (Ocelot)
                    └───┬───────────┬───┘
                        │           │
              ┌─────────▼──┐   ┌────▼────────┐
              │  OrderAPI   │   │  ProductAPI  │
              │ (EF Core)   │   │  (EF Core)   │
              └──────┬──────┘   └──────┬───────┘
                     │   RabbitMQ      │
                     └───── pub/sub ───┘
        ProductAPI publishes product/order events →
        OrderAPI consumes them (ProductOrderConsumerService)
```

## Services

| Service | Responsibility | Key Details |
|---|---|---|
| **APIGateway** | Single entry point for all client traffic | Built on **Ocelot**, routes requests to OrderAPI / ProductAPI |
| **Alphatech.Services.OrderAPI** | Order management | EF Core (code-first), repository pattern, consumes RabbitMQ events via `ProductOrderConsumerService` |
| **Alphatech.Services.ProductAPI** | Product catalog | EF Core with migrations, repository pattern, publishes events via `ProductOrderPublisherService` |
| **OrderManagementUI** | Customer-facing front-end | ASP.NET Core MVC, consumes the gateway's REST APIs |

Both services also include a `DynamicDtoGenerator` / `DynamicClassGenerator` helper — runtime DTO generation to avoid hand-writing boilerplate mapping classes for common CRUD shapes.

## Tech Stack
- .NET 8, ASP.NET Core Web API & MVC
- Entity Framework Core (code-first + migrations)
- Ocelot API Gateway
- RabbitMQ (publish/subscribe between services)
- NLog

## Running Locally
1. Start a RabbitMQ broker (see [RabbitMQ](https://github.com/shahrozkhan63/RabbitMQ) repo for the same pattern in isolation): `docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:3-management`
2. Update the connection strings in each service's `appsettings.json`, then run `dotnet ef database update` in `Alphatech.Services.OrderAPI` and `Alphatech.Services.ProductAPI`.
3. Open `AlphaTech.sln` and start `APIGateway`, `Alphatech.Services.OrderAPI`, `Alphatech.Services.ProductAPI` and `OrderManagementUI` (multiple startup projects).
4. Browse the UI — creating an order triggers the RabbitMQ event flow between ProductAPI and OrderAPI end to end.
