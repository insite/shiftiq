namespace Shift.Test
{
    [CollectionDefinition(Default)]
    public class TestFixtures : ICollectionFixture<TestFixture>
    {
        public const string Default = "Default";
    }
}
