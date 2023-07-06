using System;
using System.Linq;
using Marten.Testing.Documents;
using Marten.Testing.Harness;

namespace LinqTests.Operators;

public class last_operator: IntegrationContext
{
    [Fact]
    public void last_throws_an_exception()
    {
        theSession.Store(new Target { Number = 1 });
        theSession.Store(new Target { Number = 2 });
        theSession.Store(new Target { Number = 3 });
        theSession.Store(new Target { Number = 4 });
        theSession.SaveChanges();

        Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
        {
            theSession.Query<Target>().Last(x => x.Number == 3)
                .ShouldNotBeNull();
        });
    }

    public last_operator(DefaultStoreFixture fixture) : base(fixture)
    {
    }
}
