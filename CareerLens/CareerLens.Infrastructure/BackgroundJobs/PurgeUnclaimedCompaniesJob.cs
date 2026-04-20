using CareerLens.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Infrastructure.BackgroundJobs
{
    public sealed class PurgeUnclaimedCompaniesJob(IServiceScopeFactory scopeFactory, ILogger<PurgeUnclaimedCompaniesJob> logger, TimeProvider dateTime) 
        : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<PurgeUnclaimedCompaniesJob> _logger = logger;
        private readonly TimeProvider _dateTime = dateTime;

        private const int BatchSize = 100;
        private static readonly TimeSpan Interval = TimeSpan.FromDays(15);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation(
                    "PurgeUnclaimedCompaniesJob started at {Now}",
                    _dateTime.GetUtcNow());

                try
                {
                    await PurgeUnclaimedCompaniesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred while purging unclaimed companies.");
                }
            }
        }
        private async Task PurgeUnclaimedCompaniesAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var totalDeleted = 0;

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(
                        "PurgeUnclaimedCompaniesJob cancellation requested, stopping.");
                    break;
                }

                var batch = await db.Companies
                    .Where(c => !c.IsClaimed)
                    .Take(BatchSize)
                    .ToListAsync(stoppingToken);

                if (batch.Count == 0) break;

                db.Companies.RemoveRange(batch);
                await db.SaveChangesAsync(stoppingToken);

                totalDeleted += batch.Count;

                _logger.LogInformation(
                    "Deleted batch of {Count} unclaimed companies. Total deleted so far: {Total}",
                    batch.Count,
                    totalDeleted);
            }

            if (totalDeleted > 0)
            {
                _logger.LogInformation(
                    "PurgeUnclaimedCompaniesJob completed. Total deleted: {Total} unclaimed companies at {Now}",
                    totalDeleted,
                    _dateTime.GetUtcNow());
            }
            else
            {
                _logger.LogInformation(
                    "PurgeUnclaimedCompaniesJob completed. No unclaimed companies found at {Now}",
                    _dateTime.GetUtcNow());
            }
        }
    }

}
