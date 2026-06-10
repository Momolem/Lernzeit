using Xunit;
using Lernzeit_Backend.IntegrationTests.TestHost;

namespace Lernzeit_Backend.IntegrationTests;

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<LernzeitWaf>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
