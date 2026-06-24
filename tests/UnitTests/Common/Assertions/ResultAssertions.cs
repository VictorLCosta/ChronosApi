using Ardalis.Result;

namespace UnitTests.Common.Assertions;

internal static class ResultAssertions
{
    public static T ShouldBeSuccess<T>(this Result<T> result) where T : class
    {
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull<T>();

        return result.Value;
    }

    public static void ShouldBeSuccess(this Result result)
    {
        result.Status.ShouldBe(ResultStatus.Ok);
    }

    public static void ShouldBeNotFound<T>(this Result<T> result) where T : class
    {
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Value.ShouldBeNull();
    }

    public static void ShouldBeNotFound(this Result result)
    {
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    public static void ShouldBeInvalid<T>(this Result<T> result)
    {
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldNotBeNull();
        result.ValidationErrors.ShouldNotBeEmpty();
    }

    public static void ShouldBeInvalid(this Result result)
    {
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldNotBeNull();
        result.ValidationErrors.ShouldNotBeEmpty();
    }

    public static void ShouldHaveValidationError<T>(
        this Result<T> result,
        string identifier,
        string errorMessage)
    {
        result.ShouldBeInvalid();
        result.ValidationErrors.ShouldContain(error =>
            error.Identifier == identifier &&
            error.ErrorMessage == errorMessage);
    }

    public static void ShouldHaveValidationError(
        this Result result,
        string identifier,
        string errorMessage)
    {
        result.ShouldBeInvalid();
        result.ValidationErrors.ShouldContain(error =>
            error.Identifier == identifier &&
            error.ErrorMessage == errorMessage);
    }
}
