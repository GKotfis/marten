using System.Linq;
using Marten.Exceptions;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;
using Xunit;

namespace Marten.Testing.Exceptions
{
    public class known_exception_causes_dueto_pg9: IntegratedFixture
    {
        [PgVersionTargetedFact(MaximumVersion = "10.0")]
        public void can_map_jsonb_FTS_not_supported()
        {
            var e = Assert.Throws<MartenCommandNotSupportedException>(() =>
            {
                using (var session = theStore.OpenSession())
                {
                    session.Query<User>().Where(x => x.PlainTextSearch("throw")).ToList();
                }
            });

            e.Reason.ShouldBe(NotSupportedReason.FullTextSearchNeedsAtLeastPostgresVersion10);
            e.Message.ShouldContain(KnownNotSupportedExceptionCause.ToTsvectorOnJsonb.Description);
        }

        [PgVersionTargetedFact(MaximumVersion = "10.0")]
        public void can_totsvector_other_than_jsonb_without_FTS_exception()
        {
            var e = Assert.Throws<MartenCommandException>(() =>
            {
                using (var session = theStore.OpenSession())
                {
                    session.Query<User>("to_tsvector(?)", 0).ToList();
                }
            });
            e.ShouldNotBeOfType<MartenCommandNotSupportedException>();
        }
    }
}
