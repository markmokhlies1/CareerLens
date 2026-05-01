using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Application.Features.Notifications.Mappers;
using CareerLens.Domain.Common.Results;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Notifications.Queries.GetNotifications
{
    public sealed record GetNotificationsQuery(int Page,
                                               int PageSize,
                                               bool? IsRead = null)
    : IRequest<Result<PaginatedList<NotificationDto>>>;

    public sealed class GetNotificationsQueryValidator : AbstractValidator<GetNotificationsQuery>
    {
        public GetNotificationsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");
        }
    }

    public sealed class GetNotificationsQueryHandler(IAppDbContext context, IUser currentUser)
    : IRequestHandler<GetNotificationsQuery, Result<PaginatedList<NotificationDto>>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;

        public async Task<Result<PaginatedList<NotificationDto>>> Handle(
            GetNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var baseQuery = _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId);

            if (request.IsRead.HasValue)
            {
                baseQuery = baseQuery
                    .Where(n => n.IsRead == request.IsRead.Value);
            }

            baseQuery = baseQuery.OrderByDescending(n => n.CreatedAtUtc);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PaginatedList<NotificationDto>
                {
                    PageNumber = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    Items = []
                };
            }

            var notifications = await baseQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = notifications
                .Select(n => n.ToDto())        
                .ToList()
                .AsReadOnly();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedList<NotificationDto>
            {
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items
            };
        }
    }
}
