using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Common.Errors
{ 
    public static class ApplicationErrors
    {
        public static Error CompanyNotFound =>
            Error.NotFound("ApplicationErrors.Company.NotFound", "Company does not exist.");
        public static Error JobNotFound =>
            Error.NotFound("ApplicationErrors.Job.NotFound", "Job does not exist.");
        public static Error SalaryNotFound =>
           Error.NotFound("ApplicationErrors.Salary.NotFound", "Salary does not exist.");

        public static Error InterviewNotFound =>
            Error.NotFound("ApplicationErrors.Company.NotFound", "Interview does not exist.");

        public static Error ClaimRequestNotFound =>
            Error.NotFound("ApplicationErrors.CompanyClaimRequest.NotFound", "CompanyRequest does not exist.");

        public static Error Unauthenticated => 
            Error.Unauthorized("User.Unauthenticated", "User is not authenticated.");
        public static  Error AlreadyHasPendingRequest =>
            Error.Conflict("CompanyClaimRequest.AlreadyHasPendingRequest", "User already has a pending claim request for this company with the same role.");

        public static Error AlreadyMember =>
            Error.Conflict("CompanyClaimRequest.AlreadyMember", "User is already a member of this company with the specified role.");

        public static Error AlreadyReviewed =>
            Error.Conflict("Review.AlreadyReviewed", "You have already reviewed this company.");

        public static Error ReviewNotFound =>
            Error.NotFound("Review.NotFound", "Review was not found.");

        public static Error NameAlreadyExists =>
            Error.Conflict("Company.AlreadyExcited", "Company Already Excited.");

        public static readonly Error Forbidden =
        Error.Forbidden("General.Forbidden", "You are not authorized.");

        // Ownership errors — descriptive!
        public static readonly Error NotClaimRequestOwner =
            Error.Forbidden(
                "ClaimRequest.NotOwner",
                "You can only update your own claim requests.");

        public static readonly Error NotInterviewOwner =
            Error.Forbidden(
                "Interview.NotOwner",
                "You can only modify your own interviews.");

        public static readonly Error NotReviewOwner =
            Error.Forbidden(
                "Review.NotOwner",
                "You can only modify your own reviews.");

        public static readonly Error NotSalaryOwner =
            Error.Forbidden(
                "Salary.NotOwner",
                "You can only modify your own salary records.");

        // Company membership errors
        public static readonly Error NotCompanyMember =
            Error.Forbidden(
                "Company.NotMember",
                "You are not a member of this company.");

        public static readonly Error NotCompanyModerator =
            Error.Forbidden(
                "Company.NotModerator",
                "Only company moderators can perform this action.");

        public static readonly Error NotCompanyHr =
            Error.Forbidden(
                "Company.NotHr",
                "Only company HR members can perform this action.");


        public static Error InvalidRefreshToken =>
            Error.Validation(
        "RefreshToken.Expiry.Invalid",
        "Expiry must be in the future.");

        public static readonly Error ExpiredAccessTokenInvalid = Error.Conflict(
             code: "Auth.ExpiredAccessToken.Invalid",
             description: "Expired access token is not valid.");

        public static readonly Error UserIdClaimInvalid = Error.Conflict(
            code: "Auth.UserIdClaim.Invalid",
            description: "Invalid userId claim.");

        public static readonly Error RefreshTokenExpired = Error.Conflict(
            code: "Auth.RefreshToken.Expired",
            description: "Refresh token is invalid or has expired.");

        public static readonly Error UserNotFound = Error.NotFound(
            code: "Auth.User.NotFound",
            description: "User not found.");

        public static readonly Error TokenGenerationFailed = Error.Failure(
            code: "Auth.TokenGeneration.Failed",
            description: "Failed to generate new JWT token.");

        public static readonly Error NotificationNotFound
            = Error.NotFound("ApplicationErrors.Notification.NotFound", "Notification does not exist.");

        public static readonly Error NotNotificationOwner =
            Error.Forbidden(
                "NotNotification.NotOwner",
                "You can only modify your own NotNotifications.");

    }
} 
