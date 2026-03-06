
A microservices-based ecommerce backend built with ASP.NET Core 9 Minimal APIs and PostgreSQL. Four independent services communicate over HTTP to handle products, inventory, orders, and notifications.

All services share a single PostgreSQL instance and communicate via typed HTTP clients.

| Service | Port | Database | Description |
| InventoryService | 5001 | PostgreSQL | Manages stock levels; reserve/release/confirm operations |
| ProductService | 5002 | PostgreSQL | Product catalog CRUD with pagination |
| NotificationService | 5003 | None | Stateless; logs order confirmation events |
| OrderService | 5004 | PostgreSQL | Creates orders via saga orchestration |

`Ecommerce.Shared` is a class library referenced by all services. It contains the common exception hierarchy (`BaseException`, `NotFoundException`, `ConflictException`, `BadRequestException`).

When `POST /api/orders` is called, OrderService runs a saga:

1. Look up each item's price and product ID from ProductService
2. Reserve inventory for each item (rolls back all reservations on any failure)
3. Persist the order with status `Pending`
4. Confirm inventory for each item
5. Send order confirmation via NotificationService
6. Update order status to `Confirmed`

If anything in steps 4–6 fails, the order is marked `Failed`.

### Run with Docker Compose

```bash
cd src
docker compose up --build
```
This starts PostgreSQL and all four services. Note: database migrations must be applied before

With PostgreSQL running (either locally or via Docker Compose):

# From the solution root
dotnet ef database update --project src/InventoryService/InventoryService.csproj
dotnet ef database update --project src/ProductService/ProductService.csproj
dotnet ef database update --project src/OrderService/OrderService.csproj

## End-to-End Example

```bash
# 1. Create a product
curl -X POST http://localhost:5002/api/products \
  -H "Content-Type: application/json" \
  -d '{"sku":"WIDGET-001","name":"Widget","price":9.99,"category":"widgets"}'

# 2. Create inventory for that SKU
curl -X POST http://localhost:5001/api/inventories \
  -H "Content-Type: application/json" \
  -d '{"productId":"<id-from-step-1>","sku":"WIDGET-001","quantityAvailable":100,"quantityReserved":0,"warehouseLocation":"A1"}'

# 3. Place an order (triggers full saga)
curl -X POST http://localhost:5004/api/orders \
  -H "Content-Type: application/json" \
  -d '{"customerEmail":"alice@example.com","items":[{"sku":"WIDGET-001","quantity":2}]}'

# 4. Cancel the order
curl -X PUT http://localhost:5004/api/orders/<order-id>/cancel
```