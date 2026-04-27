using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Notifications.Commands.DeleteNotification;
using CareerLens.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using CareerLens.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Application.Features.Notifications.Queries.GetNotifications;
using CareerLens.Application.Features.Notifications.Queries.GetUnreadNotificationsCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerLens.Api.Controllers
{
    [Authorize]
    [Route("api/v1/notifications")]
    public sealed class NotificationsController : ApiController
    {
        private readonly ISender _sender;

        public NotificationsController(ISender sender)
        {
            _sender = sender;
        }

        #region Get All

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<NotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isRead = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetNotificationsQuery(
                Page: page,
                PageSize: pageSize,
                IsRead: isRead
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

        #region Get total count of unread

        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetUnreadNotificationsCount(
            CancellationToken cancellationToken)
        {
            var query = new GetUnreadNotificationsCountQuery();

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }

        #endregion

        #region Mark a single notification as read

        [HttpPatch("{notificationId:guid}/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> MarkNotificationAsRead(
            [FromRoute] Guid notificationId,
            CancellationToken cancellationToken)
        {
            var command = new MarkNotificationAsReadCommand(notificationId);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }

        #endregion

        #region Mark all unread notifications as read

        [HttpPatch("read-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> MarkAllNotificationsAsRead(
            CancellationToken cancellationToken)
        {
            var command = new MarkAllNotificationsAsReadCommand();

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }

        #endregion

        #region Delete 

        [HttpDelete("{notificationId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteNotification(
            [FromRoute] Guid notificationId,
            CancellationToken cancellationToken)
        {
            var command = new DeleteNotificationCommand(notificationId);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : NoContent();
        }

        #endregion
    }
}
