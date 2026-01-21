# Ecommerce Microservices demo (.NET 8 + YARP + Docker)

## Overview
This repository is a simple microservices demo:

- ProductService: .NET 8 Minimal API (in-memory product list)
- OrderService: .NET 8 Minimal API (in-memory orders list)
- ApiGateway: YARP (Yet Another Reverse Proxy) acting as an API Gateway routing requests to services

## Architecture (high level)
- Client -> ApiGateway (YARP) -> ProductService / OrderService

### Gateway routes

- `api/products/{**catch-all}` -> `ProductService` -> `/products/{**catch-all}`
- `api/orders/{**catch-all}`  -> `OrderService` -> `/orders/{**catch-all}`

## Run (Docker Compose)

### Prereqs
- Docker

### From the repo root
```bash
docker compose up --build
```

## Ports

- ApiGateway: http://localhost:5000
- ProductService (direct): http://localhost:5003
- OrderService (direct): http://localhost:5002

## Quick test (via gateway)

### Gateway root
- GET http://localhost:5000

### Products
- GET http://localhost:5000/api/products/
- GET http://localhost:5000/api/products/1

### Orders
- GET http://localhost:5000/api/orders/
- GET http://localhost:5000/api/orders/1

### Create a product (via gateway)
- POST http://localhost:5000/api/products/

### Body:
```json
{
"id": 4,
"name": "Monitor",
"price": 12000,
"stock": 20
}
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

## Notes
- ASPNETCORE_ENVIRONMENT=Development is set in docker-compose.yml
- Services are discovered by DNS names (e.g., http://productservice:8080/) inside the Compose network