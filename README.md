# ABC - Product Rating Calculator

## Tech Stack
- .NET 9, EF 9, PostgreSQL 17, RabbitMQ 4
- Clean Architecture with CQRS (MediatR)
- MassTransit for message broker abstraction
- FluentResults, FluentValidation
- xUnit

## Architecture

- Clean Architecture with clear separation of concerns
- CQRS Pattern using MediatR for command/query segregation
- Event-Driven Processing with RabbitMQ for asynchronous operations
- Repository Pattern with Unit of Work
- Docker Containerization for all services
- Worker Service for background processing
- Migration Project for database migrations

## Prerequisites
- Docker

## Running the Application

```bash
docker compose up -d --build
```

Initial image downloads may take some time minutes depending on network speed.

API available at `http://localhost:8080` once started.

## Testing

- Interactive UI: Navigate to `http://localhost:8080/scalar/v1` for Scalar API documentation and testing
- HTTP Files: Use `showcase.http` with your HTTP client

**Setup**:
- IDs for Entities are generated at runtime - copy the Product ID from the first GET request response to use in subsequent requests
- Database is pre-seeded with 3 products, 5 sentiment terms, and sample feedbacks
- `.env` file is included to make the process as straightforward as possible. Won't be there in the real project.

## Implementation Decisions

### API Protection:

- Sentiment term management endpoints (`GET/POST /api/v1/sentiment-terms`) require `X-Api-Key` header
- Manual recalculation endpoint (`POST /api/v1/product-ratings/recalculate-outdated`) requires `X-Api-Key` header
- API key for testing: `79ee2c60-0703-46ca-a638-24cfa63477ba` (hardcoded for demo purposes)

### Rating Calculation Optimization
When feedback is submitted:
1. First calculates if feedback contains any sentiment terms
2. If no terms found - stores feedback but skips rating recalculation
3. If terms found - recalculates product rating from ALL feedbacks in the Worker service

### Sentiment Term Matching
- Longest match first algorithm: "very bad" takes precedence over "bad"
- No double-counting: once text is matched, it can't be matched again (e.g., "very bad" prevents inner "bad" from being counted)
- Whole-word, case-insensitive matching, allows punctuation as part of the sentiment term

### Rating Invalidation
When new sentiment term is added:
1. Database query finds all products with at least one feedback containing that term
2. Marks those ratings as `IsOutdated = true`
3. The global recalculation of outdated ratings happens in two scenarios (both use the same handler that performs the updates in configurable batches):
    - **Scheduled job**: Daily at 2 AM UTC (configurable in Worker's `appsettings.json`)
    - **Manual trigger**: Admin endpoint `/api/v1/product-ratings/recalculate-outdated`
    

### Worker Service
Separate background service consuming RabbitMQ events:
- Processes new feedback ratings asynchronously
- Marks ratings outdated when sentiment terms added
- Runs scheduled recalculation of outdated ratings
- Batch processing with configurable size (default: 50)

## Notes
- Optimistic concurrency on ProductRating prevents race conditions
- Sentiment terms cached in memory (60 min) for performance