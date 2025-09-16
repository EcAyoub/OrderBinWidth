# OrderBinWidth

## Achievements

- REST API for creating orders and automatically calculating the required bin width.
- Order persistence using Entity Framework Core and PostgreSQL.
- Interactive documentation via Swagger/OpenAPI.
- Centralized error handling through middleware.
- Robust data validation for incoming requests using DataAnnotations on DTOs.
- Unit and integration tests covering main use cases.

## Project Structure

- `Domain/`: Pure business entities and domain logic.
- `Application/`: Use cases (services, business orchestration).
- `Infrastructure/`: Data persistence, repository implementations (EF Core, InMemory).
- `API/`: Contracts, controllers, configuration, middleware.
- `Tests/`: Unit and integration test scenarios for business logic, use cases.

## Architecture

- **Clean Architecture**: Strict separation between domain, application, infrastructure, and presentation layers.
- **Domain-Driven Design (DDD)**: Clear modeling of entities and business rules.
- **Dependency Injection (DI)**: All services and repositories are injected for flexibility and testability.

## Principles Used

- **SOLID**: Applied across layers for maintainable, extensible, and testable code.
- **TDD (Test Driven Development)**: Development guided by unit and integration tests.
- **DDD (Domain-Driven Design)**: Domain entities and business rules drive the architecture.
- **Business and Technical Validation**: Both at the domain level and via DataAnnotations in the API.
- **Centralized Error Handling**: Middleware transforms exceptions into consistent HTTP responses.

## Continuous Integration (CI)

This project includes a CI pipeline using **GitHub Actions**.  
On each push or pull request to `master`, the workflow will:

- Spin up a PostgreSQL database in a container.
- Restore dependencies, build the project, and run database migrations.
- Run all unit and integration tests.
- Upload the test results as artifacts.

## Docker & Docker Compose

You can run the project using Docker for easy setup and deployment.

### Usage

1. **Build and start all services (API + PostgreSQL):**
   ```bash
   docker-compose up --build

## Example - Create Order Request

```http
POST /api/orders
Content-Type: application/json

{
  "orderId": "Order-123",
  "items": [
    { "productType": "Cards", "quantity": 3 }
  ]
}
```

## Example - Get Order Request

```http
GET /api/orders/Order-123
Accept: application/json

Response:
{
  "orderId": "Order-123",
  "items": [
    { "productType": "Cards", "quantity": 3 }
  ],
  "binWidth": 120
}
```

## API Documentation

After starting the application, browse to [swagger](http://localhost:8080/index.html) for interactive API documentation.

## Testing

Unit and integration tests are located in the `Tests` folder. Run all tests with:

```bash
dotnet test
```
