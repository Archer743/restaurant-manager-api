using Restaurant_API.Tests.Common;
using Restaurant_API.Tests.Integration.Fixtures;

namespace Restaurant_API.Tests.Integration
{
    [Trait(TestCategories.Key, TestCategories.Integration)]
    public class ExampleTests : IntegrationTestBase
    {
        public ExampleTests(PostgresFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Test2() {}
    }
}
