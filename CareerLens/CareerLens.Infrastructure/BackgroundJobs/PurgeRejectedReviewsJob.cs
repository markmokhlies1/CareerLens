using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Reviews.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CareerLens.Infrastructure.BackgroundJobs
{
    public sealed class PurgeRejectedReviewsJob(IServiceScopeFactory scopeFactory, ILogger<PurgeRejectedReviewsJob> logger, TimeProvider dateTime)
        : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<PurgeRejectedReviewsJob> _logger = logger;
        private readonly TimeProvider _dateTime = dateTime;

        private const int BatchSize = 100;
        private const int RejectedReviewExpiryDays = 7;
        private static readonly TimeSpan Interval = TimeSpan.FromDays(7);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation(
                    "PurgeRejectedReviewsJob started at {Now}",
                    _dateTime.GetUtcNow());

                try
                {
                    await PurgeRejectedReviewsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred while purging rejected reviews.");
                }
            }
        }

        private async Task PurgeRejectedReviewsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var expiryDate = _dateTime.GetUtcNow()
                .AddDays(-RejectedReviewExpiryDays);

            var totalDeleted = 0;

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(
                        "PurgeRejectedReviewsJob cancellation requested, stopping.");
                    break;
                }

                var batch = await db.Reviews
                    .Where(r => r.Status == ReviewStatus.Rejected
                             && r.LastModifiedUtc <= expiryDate)
                    .Take(BatchSize)
                    .ToListAsync(stoppingToken);

                if (batch.Count == 0) break;

                db.Reviews.RemoveRange(batch);
                await db.SaveChangesAsync(stoppingToken);

                totalDeleted += batch.Count;

                _logger.LogInformation(
                    "Deleted batch of {Count} rejected reviews. Total deleted so far: {Total}",
                    batch.Count,
                    totalDeleted);
            }

            if (totalDeleted > 0)
            {
                _logger.LogInformation(
                    "PurgeRejectedReviewsJob completed. Total deleted: {Total} rejected reviews at {Now}",
                    totalDeleted,
                    _dateTime.GetUtcNow());
            }
            else
            {
                _logger.LogInformation(
                    "PurgeRejectedReviewsJob completed. No rejected reviews found at {Now}",
                    _dateTime.GetUtcNow());
            }
        }

    }

}
