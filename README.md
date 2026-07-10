# CaseSimulator API

A backend for a CS2-inspired case opening simulator built with **.NET 10** and **Clean Architecture**. Created as a portfolio project to demonstrate real-world architectural patterns beyond typical tutorial CRUD apps.

---
## What makes this interesting

Most .NET pet projects are TaskManagers or StudentRegistries with a flat structure. This one solves actual engineering problems:
- **Weighted random drop system** — items have individual drop probabilities that must sum to exactly 1.0. Selection uses cumulative distribution over a cryptographic random value.
- **Provably Fair RNG** — every case opening is independently verifiable. The server hashes `serverSeed` with SHA-256 and provides the hash _before_ the opening. The full record (`serverSeed + clientSeed + nonce`) is stored in `ProvablyFairRounds` — the user can recalculate and verify the result themselves.
- **Financial consistency** — balance changes use a `Money` Value Object that prevents negative amounts at the domain level. Every operation creates an immutable `Transaction` with before/after balance snapshot.
- **EF Core + DDD collections** — private backing fields (`_transactions`, `_inventoryItems`) exposed only as `IReadOnlyCollection`, with EF Core configured via `UsePropertyAccessMode(PropertyAccessMode.Field)` to correctly track additions without breaking encapsulation.

---
## Architecture
Clean Architecture with **physical project separation** — not just folder convention. The compiler enforces layer boundaries.

```
src/
├── Domain/          # Entities, Value Objects, Domain Exceptions — zero external dependencies
├── Application/     # CQRS commands/queries via MediatR, FluentValidation pipeline behavior
├── Infrastructure/  # EF Core + PostgreSQL, JWT, BCrypt, CaseOpeningService
└── Api/             # ASP.NET Core controllers, middleware, DI composition
```

**Domain** references nothing. **Application** references only Domain. **Infrastructure** implements Application interfaces. **Api** wires everything together.

---
## Tech Stack

|Area|Technology|
|---|---|
|Language|C# / .NET 10|
|API|ASP.NET Core Web API|
|Architecture|Clean Architecture, CQRS|
|Mediator|MediatR 14|
|Validation|FluentValidation 12 with pipeline behavior|
|ORM|Entity Framework Core 10|
|Database|PostgreSQL 16|
|Auth|JWT Bearer + Refresh Tokens, BCrypt password hashing|
|API Docs|Scalar (OpenAPI)|
|Containerization|Docker Compose|
|OS|Arch Linux (Hyprland), JetBrains Rider|

---
## Domain Model

```
User
├── Balance: Money              ← Value Object, enforces non-negative amount
├── ClientSeed / ServerSeed / ServerSeedHash / Nonce   ← Provably Fair state
├── Transactions: IReadOnlyCollection<Transaction>
├── InventoryItems: IReadOnlyCollection<InventoryItem>
└── SaleHistory: IReadOnlyCollection<SaleItem>

Case
├── Price: Money
└── CaseContent: IReadOnlyCollection<CaseContent>
        └── CaseItem + DropChance (0 < x ≤ 1, total must equal 1.0)

ProvablyFairRound    ← immutable record of every case opening
RefreshToken         ← stored in DB, rotated on every use
Transaction          ← immutable financial log entry (amount, before/after balance)
```

---
## API Endpoints

### Auth — `/api/auth`

|Method|Path|Auth|Description|
|---|---|---|---|
|POST|`/register`|—|Register with `clientSeed`. Server generates `serverSeed`. Starting balance: 100 credits|
|POST|`/login`|—|Returns `accessToken` + `refreshToken`|
|POST|`/refresh`|—|Rotate refresh token, get new access token|

### Cases — `/api/cases`

|Method|Path|Auth|Description|
|---|---|---|---|
|GET|`/`|—|List all cases|
|GET|`/{id}`|—|Case details with full item list and drop chances|
|POST|`/{id}/open`|✓|Open a case — deducts balance, runs provably fair RNG, returns dropped item ID|

### Users — `/api/users`

|Method|Path|Auth|Description|
|---|---|---|---|
|GET|`/balance`|✓|Current balance|
|GET|`/inventory`|✓|All owned items|
|POST|`/inventory/{id}/sell`|✓|Sell item for its market price|
|GET|`/transactions`|✓|Full transaction history|
|GET|`/sales`|✓|Sale history|

### Admin — `/api/admin`
Secured via `X-Admin-Key` header (set in `appsettings`).

|Method|Path|Description|
|---|---|---|
|POST|`/items`|Create a case item (name, rarity, price, image)|
|POST|`/cases`|Create a case (name, price, image)|
|POST|`/cases/{id}/items`|Add item to case with drop chance|
|DELETE|`/items/{id}`|Delete item (cascades CaseContents)|
|DELETE|`/cases/{id}`|Delete case|
|DELETE|`/users/{id}`|Delete user|

---

## Getting Started

**Prerequisites:** Docker, .NET 10 SDK
```bash
git clone https://github.com/dzhbbv/CaseSimulator_API.git
cd CaseSimulator_API
```

Create `src/Api/appsettings.Development.json` (not tracked in git):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=casesimulator;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "Key": "YourVeryLongSecretKeyAtLeast32Chars",
    "Issuer": "CaseSimulator",
    "Audience": "CaseSimulatorClient",
    "ExpirationMinutes": 1440
  },
  "AdminSettings": {
    "SecretKey": "your-admin-secret"
  }
}
```

Start the database:
```bash
docker compose up -d
```

Apply migrations:
```bash
dotnet ef database update \
  -p src/Infrastructure/CaseSimulator.Infrastructure.csproj \
  -s src/Api/CaseSimulator.Api.csproj
```

Run:
```bash
dotnet run --project src/Api/CaseSimulator.Api.csproj
```

Open Scalar UI at `http://localhost:5051/scalar` to explore and test the API interactively.

---
## Roadmap
- [ ] Unit tests — Money value object, weighted RNG distribution
- [ ] Integration tests — Testcontainers + WebApplicationFactory
- [ ] React client
- [ ] Client-side seed rotation endpoint (`POST /users/rotate-seed`)
- [ ] GitHub Actions CI pipeline
---
## Author

**dzhbbv** — self-taught backend developer, 18 y.o.
Aspiring Software Engineer from KCHR 🇷🇺
- Backend specialist in training
- Linux enthusiast (Arch / Hyprland)