using Xunit;

namespace AlGreenMES.Tests.Integration;

[Collection("Integration")]
public abstract class IntegrationTestBase : IClassFixture<AlgreenWebApplicationFactory>
{
    protected readonly AlgreenWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(AlgreenWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
}

[CollectionDefinition("Integration")]
public class IntegrationTestCollection : ICollectionFixture<AlgreenWebApplicationFactory>
{
    // Shared fixture across all integration test classes — single Postgres container for the run.
}
