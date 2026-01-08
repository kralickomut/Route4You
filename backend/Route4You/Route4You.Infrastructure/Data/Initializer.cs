using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Route4You.Domain.Areas;
using Route4You.Domain.Ascents;
using Route4You.Domain.Routes;
using Route4You.Domain.Users;

namespace Route4You.Infrastructure.Data;

public sealed class Initializer(
    MongoContext context,
    ILogger<Initializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 1) Ping
            await context.Db.RunCommandAsync(
                (Command<BsonDocument>)"{ ping: 1 }",
                cancellationToken: cancellationToken
            );

            logger.LogInformation("✅ Connected to MongoDB '{Db}'.",
                context.Db.DatabaseNamespace.DatabaseName);

            // 2) Seed demo data (idempotent)
            await SeedDemoAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed during MongoDB initialization.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task SeedDemoAsync(CancellationToken ct)
    {
        // Collections
        var usersCol = context.GetCollection<AppUser>("users");
        var areasCol = context.GetCollection<Area>("areas");
        var routesCol = context.GetCollection<Route>("routes");
        var ascentsCol = context.GetCollection<Ascent>("ascents");

        // If we already have areas, assume seeded
        var alreadySeeded = await areasCol.Find(_ => true).AnyAsync(ct);
        if (alreadySeeded)
        {
            logger.LogInformation("ℹ️ Demo data already present. Skipping seed.");
            return;
        }

        logger.LogInformation("🌱 Seeding demo data...");

        // ----- Users -----
        var uFranta = AppUser.Create("u_franta", "František", "franta@route4you.cz", null, null);
        var uPepa   = AppUser.Create("u_pepa", "Pepa", "pepa@route4you.cz", null, null);
        var uKlara  = AppUser.Create("u_klara", "Klára", "klara@route4you.cz", null, null);

        await usersCol.InsertManyAsync(new[] { uFranta, uPepa, uKlara }, cancellationToken: ct);

        // ----- Areas -----
        // Root country
        var cz = Area.Create(
            name: "Czech Republic",
            type: AreaType.Country,
            parentId: null,
            pathIds: null,
            pathNames: null,
            slug: "czech-republic",
            lat: 49.8175,
            lng: 15.4730
        );

        // Region under CZ
        var mk = Area.Create(
            name: "Moravský kras",
            type: AreaType.Region,
            parentId: cz.Id,
            pathIds: new[] { cz.Id },
            pathNames: new[] { cz.Name },
            slug: "moravsky-kras",
            lat: 49.3731,
            lng: 16.7350
        );

        // Crag under MK
        var ts = Area.Create(
            name: "Tančící skála",
            type: AreaType.Crag,
            parentId: mk.Id,
            pathIds: new[] { cz.Id, mk.Id },
            pathNames: new[] { cz.Name, mk.Name },
            slug: "tancici-skala",
            lat: 49.3521,
            lng: 16.7223
        );

        // Denormalized counts (optional but nice)
        cz.ChildrenCount = 1;
        mk.ChildrenCount = 1;
        ts.ChildrenCount = 0;

        await areasCol.InsertManyAsync(new[] { cz, mk, ts }, cancellationToken: ct);

        // ----- Routes -----
        var pathNames = new List<string> { cz.Name, mk.Name, ts.Name };

        var rohova = Route.Create(
            id: Guid.NewGuid().ToString(),
            name: "Rohová",
            grade: "6c",
            areaId: ts.Id,
            pathNames: pathNames,
            pitches: 1,
            lengthMeters: 22,
            style: RouteStyle.Sport,
            tags: new[] { "overhang", "technical" },
            createdByUserId: uFranta.Id
        );

        var hrana = Route.Create(
            id: Guid.NewGuid().ToString(),
            name: "Hrana",
            grade: "6a",
            areaId: ts.Id,
            pathNames: pathNames,
            pitches: 1,
            lengthMeters: 18,
            style: RouteStyle.Sport,
            tags: new[] { "slab" },
            createdByUserId: uPepa.Id
        );

        // Set some demo counters
        rohova.AscentsCount = 0;
        rohova.RatingsCount = 0;

        await routesCol.InsertManyAsync(new[] { rohova, hrana }, cancellationToken: ct);

        // ----- Ascents -----
        // Create ascents + update route counters like your domain methods do
        var a1 = Ascent.Create(
            userId: uFranta.Id,
            routeId: rohova.Id,
            dateClimbed: DateTime.UtcNow.AddDays(-3),
            style: AscentStyle.Redpoint,
            rating: 4,
            notes: "Crux at the roof"
        );

        var a2 = Ascent.Create(
            userId: uPepa.Id,
            routeId: rohova.Id,
            dateClimbed: DateTime.UtcNow.AddDays(-2),
            style: AscentStyle.Flash,
            rating: 5,
            notes: "Nice warmup"
        );

        await ascentsCol.InsertManyAsync(new[] { a1, a2 }, cancellationToken: ct);

        // Update route counters to match demo ascents
        rohova.IncrementAscents();
        rohova.ApplyNewRating(4);
        rohova.IncrementAscents();
        rohova.ApplyNewRating(5);

        await routesCol.ReplaceOneAsync(r => r.Id == rohova.Id, rohova, cancellationToken: ct);

        // Update area route counts (optional)
        ts.RoutesCount = 2;
        mk.RoutesCount = 2;
        cz.RoutesCount = 2;

        await areasCol.ReplaceOneAsync(a => a.Id == ts.Id, ts, cancellationToken: ct);
        await areasCol.ReplaceOneAsync(a => a.Id == mk.Id, mk, cancellationToken: ct);
        await areasCol.ReplaceOneAsync(a => a.Id == cz.Id, cz, cancellationToken: ct);

        logger.LogInformation("✅ Demo seed finished.");
    }
}