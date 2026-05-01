using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Salaries.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CareerLens.Infrastructure.BackgroundJobs
{
    public sealed class PurgeRejectedSalariesJob(IServiceScopeFactory scopeFactory, ILogger<PurgeRejectedSalariesJob> logger, TimeProvider dateTime)
        : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<PurgeRejectedSalariesJob> _logger = logger;
        private readonly TimeProvider _dateTime = dateTime;

        private const int BatchSize = 100;
        private const int RejectedSalaryExpiryDays = 7;
        private static readonly TimeSpan Interval = TimeSpan.FromDays(7);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation(
                    "PurgeRejectedSalariesJob started at {Now}",
                    _dateTime.GetUtcNow());

                try
                {
                    await PurgeRejectedSalariesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred while purging rejected salaries.");
                }
            }
        }

        private async Task PurgeRejectedSalariesAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var expiryDate = _dateTime.GetUtcNow()
                .AddDays(-RejectedSalaryExpiryDays);

            var totalDeleted = 0;

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(
                        "PurgeRejectedSalariesJob cancellation requested, stopping.");
                    break;
                }

                var batch = await db.Salaries
                    .Where(s => s.Status == SalaryStatus.Rejected
                             && s.LastModifiedUtc <= expiryDate)
                    .Take(BatchSize)
                    .ToListAsync(stoppingToken);

                if (batch.Count == 0) break;

                db.Salaries.RemoveRange(batch);
                await db.SaveChangesAsync(stoppingToken);

                totalDeleted += batch.Count;

                _logger.LogInformation(
                    "Deleted batch of {Count} rejected salaries. Total deleted so far: {Total}",
                    batch.Count,
                    totalDeleted);
            }

            if (totalDeleted > 0)
            {
                _logger.LogInformation(
                    "PurgeRejectedSalariesJob completed. Total deleted: {Total} rejected salaries at {Now}",
                    totalDeleted,
                    _dateTime.GetUtcNow());
            }
            else
            {
                _logger.LogInformation(
                    "PurgeRejectedSalariesJob completed. No rejected salaries found at {Now}",
                    _dateTime.GetUtcNow());
            }
        }
    }

}
