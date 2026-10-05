namespace Restaurant_API.Tests.Integration.Fixtures
{
    /// <summary>
    /// Groups every database-backed test into one xUnit collection, so the container is started once
    /// and the classes inside it do not race each other over CREATE DATABASE.
    ///
    /// xUnit creates collection fixtures lazily, which is what keeps `dotnet test --filter Category!=Integration`
    /// from needing Docker at all.
    /// </summary>
    [CollectionDefinition(Name)]
    public sealed class DatabaseCollection : ICollectionFixture<PostgresFixture>
    {
        public const string Name = "Postgres database";
    }
}
