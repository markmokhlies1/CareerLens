using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Identity.Dtos;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.DomainUsers;
using CareerLens.Domain.DomainUsers.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Identity.Commands.RegisterUser
{
    public record RegisterUserCommand(string FirstName,
                                      string LastName,
                                      string Email,
                                      string Password,
                                      string ConfirmPassword,
                                      Role Role) : IRequest<Result<RegisterUserResponse>>;

    public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithErrorCode("FirstName_Required")
                .WithMessage("First name is required.")
                .MaximumLength(50).WithErrorCode("FirstName_Too_Long")
                .WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithErrorCode("LastName_Required")
                .WithMessage("Last name is required.")
                .MaximumLength(50).WithErrorCode("LastName_Too_Long")
                .WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("Email_Required")
                .WithMessage("Email is required.")
                .EmailAddress().WithErrorCode("Email_Invalid")
                .WithMessage("Email format is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode("Password_Required")
                .WithMessage("Password is required.")
                .MinimumLength(8).WithErrorCode("Password_Too_Short")
                .WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithErrorCode("Password_No_Uppercase")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithErrorCode("Password_No_Lowercase")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithErrorCode("Password_No_Digit")
                .WithMessage("Password must contain at least one digit.")
                .Matches(@"[\W_]").WithErrorCode("Password_No_Special")
                .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithErrorCode("Password_Mismatch")
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.Role)
                .IsInEnum().WithErrorCode("Role_Invalid")
                .WithMessage("Invalid role.");
        }
    }

    public class RegisterUserCommandHandler(IIdentityService identityService,
                                            IAppDbContext context,
                                            ILogger<RegisterUserCommandHandler> logger)
        : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
    {
        private readonly IIdentityService _identityService = identityService;
        private readonly IAppDbContext _context = context;
        private readonly ILogger<RegisterUserCommandHandler> _logger = logger;

        public async Task<Result<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            var emailExists = await _context.DomainUsers
                .AnyAsync(u => u.Email == request.Email, ct);

            if (emailExists)
            {
                _logger.LogWarning("Registration attempted with existing email {Email}", request.Email);
                return Error.Conflict("Email_Already_Used", $"Email '{request.Email}' is already registered.");
            }

            var userId = Guid.NewGuid();

            var domainUserResult = User.Create(
                userId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.Role);

            if (domainUserResult.IsError)
                return domainUserResult.Errors;

            IDbContextTransaction? transaction = null;

            try
            {
                transaction = await context.Database.BeginTransactionAsync(ct);

                var identityResult = await _identityService.CreateUserAsync(
                    userId,
                    request.Email,
                    request.Password,
                    request.Role);

                if (identityResult.IsError)
                {
                    await transaction.RollbackAsync(ct);
                    _logger.LogWarning("Identity creation failed for {Email}", request.Email);
                    return identityResult.Errors;
                }

                _context.DomainUsers.Add(domainUserResult.Value);
                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                _logger.LogInformation("User registered successfully {UserId} {Email}", userId, request.Email);

                return new RegisterUserResponse(userId, request.Email);
            }
            catch (Exception ex)
            {
                if (transaction is not null)
                    await transaction.RollbackAsync(ct);

                _logger.LogError(ex, "Failed to register user {Email}", request.Email);


                return Error.Failure("Registration_Failed", "An error occurred during registration.");
            }
            finally
            {
                if (transaction is not null)
                    await transaction.DisposeAsync();
            }
        }
    }
}
