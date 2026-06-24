using Application.Common.Mediator;

using NetArchTest.Rules;

namespace ArchitectureTests;

public class ApplicationTests
{   
    [Fact]
    public void ApplicationShouldNotHaveDependencyOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(ICommand).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
