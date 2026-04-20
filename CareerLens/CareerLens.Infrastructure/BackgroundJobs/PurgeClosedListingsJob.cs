using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Jobs.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CareerLens.Infrastructure.BackgroundJobs
{
    public sealed class PurgeClosedListingsJob(IServiceScopeFactory scopeFactory, ILogger<PurgeClosedListingsJob> logger, TimeProvider dateTime)
        : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<PurgeClosedListingsJob> _logger = logger;
        private readonly TimeProvider _dateTime = dateTime;

        private const int BatchSize = 100;
        private const int ClosedJobExpiryDays = 7;
        private static readonly TimeSpan Interval = TimeSpan.FromDays(7);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation(
                    "PurgeClosedListingsJob started at {Now}",
                    _dateTime.GetUtcNow());

                try
                {
                    await PurgeClosedListingsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred while purging closed job listings.");
                }
            }
        }

        private async Task PurgeClosedListingsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var expiryDate = _dateTime.GetUtcNow()
                .AddDays(-ClosedJobExpiryDays);

            var totalDeleted = 0;

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(
                        "PurgeClosedListingsJob cancellation requested, stopping.");
                    break;
                }

                var batch = await db.Jobs
                    .Where(j => j.Status == JobStatus.Closed
                             && j.LastModifiedUtc <= expiryDate)
                    .Take(BatchSize)
                    .ToListAsync(stoppingToken);

                if (batch.Count == 0) break;

                db.Jobs.RemoveRange(batch);
                await db.SaveChangesAsync(stoppingToken);

                totalDeleted += batch.Count;

                _logger.LogInformation(
                    "Deleted batch of {Count} closed job listings. Total deleted so far: {Total}",
                    batch.Count,
                    totalDeleted);
            }

            if (totalDeleted > 0)
            {
                _logger.LogInformation(
                    "PurgeClosedListingsJob completed. Total deleted: {Total} closed job listings at {Now}",
                    totalDeleted,
                    _dateTime.GetUtcNow());
            }
            else
            {
                _logger.LogInformation(
                    "PurgeClosedListingsJob completed. No closed job listings found at {Now}",
                    _dateTime.GetUtcNow());
            }
        }
    }

}
