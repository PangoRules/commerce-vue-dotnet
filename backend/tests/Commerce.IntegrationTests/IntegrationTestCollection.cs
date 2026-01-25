namespace Commerce.IntegrationTests;

[CollectionDefinition("Integration")]
public sealed class IntegrationTestCollection : ICollectionFixture<PostgresContainerFixture>
{
    // no code; xUnit uses this for shared fixture
}
