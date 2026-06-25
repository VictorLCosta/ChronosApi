using Domain.Common;

using NetArchTest.Rules;

namespace ArchitectureTests;

internal class DomainTests
{
    [Fact]
    public void DomainShouldNotHaveDependencyOnApplication()
    {
        var result = Types.InAssembly(typeof(BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("ClassifiedAds.Application")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DomainShouldNotHaveDependencyOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("ClassifiedAds.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}