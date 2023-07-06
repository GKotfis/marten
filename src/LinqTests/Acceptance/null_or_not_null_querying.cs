using System.Linq;
using Marten;
using Marten.Schema;
using Marten.Testing.Documents;
using Marten.Testing.Harness;

namespace LinqTests.Acceptance;

// Change type mapping to treat "unknown" PG types as jsonb -> null checks depths at arbitrary depths don't fail due to CAST
public class multi_level_is_null_querying : IntegrationContext
{
    public class UserNested : User
    {
        public UserNested Nested { get; set; }
    }

    [Fact]
    public void CanQueryNullNotNullAtArbitraryDepth()
    {
        var user = new UserNested
        {
            Nested = new UserNested
            {
                Nested = new UserNested
                {
                    Nested = new UserNested()
                }
            }
        };

        theSession.Store(user);

        theSession.SaveChanges();

        using (var s = theStore.QuerySession())
        {
            var notNull = s.Query<UserNested>().First(x => x.Nested.Nested.Nested != null);
            var notNullAlso = s.Query<UserNested>().First(x => x.Nested.Nested.Nested.Nested.Nested == null);
            var shouldBeNull = s.Query<UserNested>().FirstOrDefault(x => x.Nested.Nested.Nested == null);

            Assert.Equal(user.Id, notNull.Id);
            Assert.Equal(user.Id, notNullAlso.Id);
            Assert.Null(shouldBeNull);
        }
    }

    [Fact]
    public void UnknownPGTypesMapToJsonb()
    {
        var mapping = new DocumentMapping<UserNested>(new StoreOptions());

        var field = mapping.FieldFor(x => x.Nested);

        Assert.Equal("CAST(d.data ->> 'Nested' as jsonb)", field.TypedLocator);
    }

    public multi_level_is_null_querying(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}
