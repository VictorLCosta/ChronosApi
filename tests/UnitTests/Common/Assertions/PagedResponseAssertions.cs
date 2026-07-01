using CrossCutting.Models;

namespace UnitTests.Common.Assertions;

internal static class PagedResponseAssertions
{
    public static void ShouldHavePagination<T>(
        this PagedResponse<T> response,
        int pageNumber,
        int pageSize,
        long totalCount,
        int totalPages)
    {
        response.PageNumber.ShouldBe(pageNumber);
        response.PageSize.ShouldBe(pageSize);
        response.TotalCount.ShouldBe(totalCount);
        response.TotalPages.ShouldBe(totalPages);
    }

    public static IReadOnlyCollection<T> ShouldHaveSingleItem<T>(this PagedResponse<T> response)
    {
        response.Items.Count.ShouldBe(1);
        return response.Items;
    }

    public static IReadOnlyCollection<T> ShouldBeEmptyPage<T>(
        this PagedResponse<T> response,
        int pageNumber = 1,
        int pageSize = 10)
    {
        response.Items.ShouldBeEmpty();
        response.PageNumber.ShouldBe(pageNumber);
        response.PageSize.ShouldBe(pageSize);
        response.TotalCount.ShouldBe(0);
        response.TotalPages.ShouldBe(0);
        response.HasNext.ShouldBeFalse();
        response.HasPrevious.ShouldBeFalse();

        return response.Items;
    }

    public static IReadOnlyCollection<T> ShouldContainItemsInOrder<T>(
        this PagedResponse<T> response,
        params T[] expectedItems)
    {
        response.Items.ShouldBe(expectedItems);
        return response.Items;
    }
}