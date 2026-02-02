# Ecommerce Microservices demo (.NET 8 + YARP + Docker + SQL Server)

## Overview
This repository is a simple microservices demo using Docker Compose:

- ProductService: .NET 8 Minimal API + EF Core (SQL Server) for products
- OrderService: .NET 8 Minimal API + EF Core (SQL Server) for orders (Calls ProductService to reserve stock)
- ApiGateway: YARP (Yet Another Reverse Proxy) API Gateway with HTTP resilience. (timeouts + circuit breaker)
- SQL Server: Microsoft SQL Server 2022 (Express)

## Architecture (high level)
- Client -> ApiGateway (YARP) -> ProductService / OrderService -> SQL Server

### Gateway routes

- `/api/products/{**catch-all}` -> `ProductService` -> `/products/{**catch-all}`
- `/api/orders/{**catch-all}`  -> `OrderService` -> `/orders/{**catch-all}`

## Run (Docker Compose)

### Prereqs
- Docker (Docker Desktop / Docker Engine)

### From the repo root
```bash
docker compose up --build
```
Stop everything
```
docker compose down
```
Reset databases (deletes SQL volume)
```
docker compose down -v
```

## Ports

- ApiGateway: http://localhost:5000
- ProductService (direct): http://localhost:5003
- OrderService (direct): http://localhost:5002
- SQL Server: localhost: 1433

## Database & migrations
- ProductService and OrderService run Database.Migrate() on startup, so tables + seed data are created automatically.
- Connection strings are provided via Docker Compose environment variables.

## Quick smoke test (via gateway)

### Gateway root
- GET http://localhost:5000/ -> should return: API Gateway is running!

### Products
- GET http://localhost:5000/api/products/
- GET http://localhost:5000/api/products/1

### Orders
- GET http://localhost:5000/api/orders/
- GET http://localhost:5000/api/orders/1

### Create a product (via gateway)
```
curl -i -X POST "http://localhost:5000/api/products" \
  -H "Content-Type: application/json" \
  -d '{"name":"Monitor","description":"24 inch","price":12000,"stock":20}'
```
## Reserve stock (called internally by OrderService)
OrderService calls ProductService endpoint:
- POST /products/{id}/reserve with JSON { "quantity": 1 }

## Create an order (via gateway)
```
curl -i -X POST "http://localhost:5000/api/orders" \
  -H "Content-Type: application/json" \
  -d '{"productId":1,"quantity":1,"customerName":"John Doe","customerEmail":"john@example.com","shippingAddress":"123 Street"}'
```

## Direct service checks (optional)

### Swagger
- ProductService: http://localhost:5003/swagger
- OrderService: http://localhost:5002/swagger

### Health
- ProductService: GET http://localhost:5003/health
- OrderService: GET http://localhost:5002/health

## Running in GitHub Codespaces

1. Run: docker compose up --build
2. Open the Ports tab.
3. Port 5000 -> Open in Browser / Copy URL.
4. Take the base URL you get and append paths:
- <FORWARDED_BASE_URL>/api/products/
- <FORWARDED_BASE_URL>/api/orders/

Forwarded URLs can change per session—use the Ports tab.
