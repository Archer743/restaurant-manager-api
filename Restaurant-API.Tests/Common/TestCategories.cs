namespace Restaurant_API.Tests.Common
{
    /// <summary>
    /// Trait values used to split the fast tests from the ones that need a Docker daemon.
    /// Run only the fast ones with: dotnet test --filter Category!=Integration
    /// </summary>
    public static class TestCategories
    {
        public const string Key = "Category";

        public const string Unit = "Unit";
        public const string Integration = "Integration";
    }
}
