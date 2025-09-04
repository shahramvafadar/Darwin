using System.Threading;
using System.Threading.Tasks;
using Darwin.Domain.Entities.Settings;
using Darwin.Domain.Entities.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace Darwin.Infrastructure.Persistence.Seed
{
    public sealed class DataSeeder
    {
        private readonly DbContext _db;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(DbContext db, ILogger<DataSeeder> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Starting data seeding…");

            // SiteSetting
            if (!await _db.Set<SiteSetting>().AnyAsync(ct))
            {
                _db.Add(new SiteSetting
                {
                    Title = "Darwin",
                    DefaultCulture = "de-DE",
                    DefaultCountry = "DE",
                    DefaultCurrency = "EUR",
                    TimeZone = "Europe/Berlin",
                    MeasurementSystem = "Metric",
                    DisplayWeightUnit = "kg",
                    DisplayLengthUnit = "cm",
                    EnableCanonical = true,
                    HreflangEnabled = true
                });
                await _db.SaveChangesAsync(ct);
            }

            // Tax categories (DE 19% & 7%)
            if (!await _db.Set<TaxCategory>().AnyAsync(ct))
            {
                _db.Add(new TaxCategory { Name = "Standard", VatRate = 0.19m, EffectiveFromUtc = DateTime.UtcNow });
                _db.Add(new TaxCategory { Name = "Reduced", VatRate = 0.07m, EffectiveFromUtc = DateTime.UtcNow });
                await _db.SaveChangesAsync(ct);
            }

            // TODO: seed Menus, Pages (Home/Privacy/Impressum), sample Catalog categories/brands if needed

            _logger.LogInformation("Data seeding completed.");
        }
    }
}
