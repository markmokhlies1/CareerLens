using CareerLens.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CareerLens.Infrastructure.BackgroundJobs
{
    public sealed class PurgeExpiredNotificationsJob(IServiceScopeFactory scopeFactory, ILogger<PurgeExpiredNotificationsJob> logger, TimeProvider dateTime) 
        : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<PurgeExpiredNotificationsJob> _logger = logger;
        private readonly TimeProvider _dateTime = dateTime;

        private const int BatchSize = 100;
        private const int NotificationExpiryDays = 2;
        private static readonly TimeSpan Interval = TimeSpan.FromDays(2);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation(
                    "PurgeExpiredNotificationsJob started at {Now}",
                    _dateTime.GetUtcNow());

                try
                {
                    await PurgeExpiredNotificationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred while purging expired notifications.");
                }
            }
        }

        private async Task PurgeExpiredNotificationsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var expiryDate = _dateTime.GetUtcNow()
                .AddDays(-NotificationExpiryDays);

            var totalDeleted = 0;

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(
                        "PurgeExpiredNotificationsJob cancellation requested, stopping.");
                    break;
                }

                var batch = await db.Notifications
                    .Where(n => n.CreatedAtUtc <= expiryDate)
                    .Take(BatchSize)
                    .ToListAsync(stoppingToken);

                if (batch.Count == 0) break;

                db.Notifications.RemoveRange(batch);
                await db.SaveChangesAsync(stoppingToken);

                totalDeleted += batch.Count;

                _logger.LogInformation(
                    "Deleted batch of {Count} expired notifications. Total deleted so far: {Total}",
                    batch.Count,
                    totalDeleted);
            }

            if (totalDeleted > 0)
            {
                _logger.LogInformation(
                    "PurgeExpiredNotificationsJob completed. Total deleted: {Total} expired notifications at {Now}",
                    totalDeleted,
                    _dateTime.GetUtcNow());
            }
            else
            {
                _logger.LogInformation(
                    "PurgeExpiredNotificationsJob completed. No expired notifications found at {Now}",
                    _dateTime.GetUtcNow());
            }
        }

    }

}
