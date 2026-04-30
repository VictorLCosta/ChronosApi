using Application.Common.Identity.Models;
using Application.Common.Identity.Services;
using Application.Common.Interfaces;
using Application.Features.Identity.Passwords;
using Application.Features.Identity.Tokens.AccessToken;
using Application.Features.Identity.Tokens.RefreshToken;
using Application.Features.Identity.TwoFactor;
using Application.Features.Identity.Users;

using Ardalis.Result.AspNetCore;

using Mediator;

using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class IdentityEndpoints
{
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/identity")
            .WithTags("Identity");

        group.MapPost("/login", async (IMediator mediator, [FromBody] GenerateAccessTokenCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/register", async (
            IMediator mediator,
            IRequestContext requestContext,
            [FromBody] RegisterUserCommand command) =>
        {
            command.Origin = requestContext.Origin ?? string.Empty;

            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/refresh", async (IMediator mediator, [FromBody] RefreshTokenCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        });

        group.MapDelete("/logout", async () =>
        {

        });

        group.MapPost("/reset-password", async (IMediator mediator, [FromBody] ResetPasswordCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .AllowAnonymous();

        group.MapPost("/change-password", async (IMediator mediator, [FromBody] ChangePasswordCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/forgot-password", async (
            IMediator mediator,
            IRequestContext requestContext,
            [FromBody] ForgotPasswordCommand command) =>
        {
            command.Origin = requestContext.Origin ?? string.Empty;

            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/2fa/setup", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTwoFactorSetupCommand());

            return result.ToMinimalApiResult();
        })
        .RequireAuthorization()
        .Produces<TwoFactorSetupResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/2fa/enable", async (IMediator mediator, [FromBody] EnableTwoFactorCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .RequireAuthorization()
        .Produces<TwoFactorEnableResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/2fa/verify", async (IMediator mediator, [FromBody] VerifyTwoFactorCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);


        group.MapPost("/2fa/verify-login", async (IMediator mediator, [FromBody] VerifyTwoFactorLoginCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .Produces<TokenResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/confirm-email", async ([FromServices] IMediator mediator, [AsParameters] ConfirmEmailCommand command) =>
        {
            var result = await mediator.Send(command);

            return result.ToMinimalApiResult();
        })
        .Produces(StatusCodes.Status200OK)
        .AllowAnonymous();

        group.MapPost("/external-login", async () =>
        {

        });

        group.MapGet("/me", async (IMediator mediator, [FromServices] ICurrentUserService currentUserService) =>
        {
            var userId = currentUserService.GetUserId();

            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var result = await mediator.Send(new GetUserByIdQuery(userId));

            return result.ToMinimalApiResult();
        });

    }
}