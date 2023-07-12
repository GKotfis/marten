using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperFx.Core.Reflection;
using Marten;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Npgsql;
using NpgsqlTypes;
using NSubstitute.Core;
using Shouldly;
using Weasel.Core;
using Weasel.Postgresql;
using Weasel.Postgresql.SqlGeneration;
using Xunit.Abstractions;

namespace LinqTests;

public class playing : IntegrationContext
{
    private readonly ITestOutputHelper _output;

    public playing(DefaultStoreFixture fixture, ITestOutputHelper output) : base(fixture)
    {
        _output = output;
    }

    protected override Task fixtureSetup()
    {
        var targets = Target.GenerateRandomData(10000).ToArray();
        targets[5].Number = 5;
        targets[55].Number = 5;
        targets[56].Number = 5;
        targets[57].Number = 5;
        targets[58].Number = 5;

        targets[5].Inner.AnotherNumber = 10;
        targets[5].TagsArray = new[] { "a", "b", "c" };

        var deep = targets.FirstOrDefault(x => x.Children.Any());
        var child = deep.Children.Last();

        child.Number = 11;

        return theStore.BulkInsertDocumentsAsync(targets);
    }

    // [Fact]
    // public async Task try_stuff()
    // {
    //
    // }

    [Fact]
    public async Task try_linq()
    {
        theSession.Logger = new TestOutputMartenLogger(_output);

        var targets = await theSession.Query<Target>()
            .OrderBy(x => x.Double)
            .Where(x => x.Number == 6)
            .Take(5)
            .ToListAsync();



        /*
Method is Take
Method is SelectMany
Method is Where
Method is OrderBy
Method is Equals
         */
    }

    [Fact]
    public async Task play_with_serializer()
    {
        StoreOptions(opts => opts.UseDefaultSerialization(enumStorage: EnumStorage.AsString));

        var targets = await theSession.Query<Target>().Where(x => x.TagsArray.Contains("a")).ToListAsync();


        var builder = new CommandBuilder();
        builder.Append("select count(*) from mt_doc_target where ");

        var fragment = new ContainmentWhereFragment("data", theStore.Serializer);
        // fragment.AddShallow(nameof(Target.Number), 5);
        //
        // // If array
        // fragment.AddShallow(nameof(Target.TagsArray), new []{"b"});
        //
        // // if deep
        // fragment.AddDeep(new []{nameof(Target.Inner), nameof(Target.AnotherNumber)}, 10);

        var data = new Dictionary<string, object> { [nameof(Target.Number)] = 11 };

        fragment.AddShallow(nameof(Target.Children), new[]{data});

        fragment.Apply(builder);


        var command = builder.Compile();
        using var conn = new NpgsqlConnection(ConnectionSource.ConnectionString);
        await conn.OpenAsync();
        command.Connection = conn;

        var count = (long)(await command.ExecuteScalarAsync());

        count.ShouldBeGreaterThan(0);
    }
}

public class ContainmentWhereFragment : ISqlFragment
{
    private readonly string _locator;
    private readonly ISerializer _serializer;
    private readonly Dictionary<string, object> _data = new();

    public ContainmentWhereFragment(string locator, ISerializer serializer)
    {
        _locator = locator;
        _serializer = serializer;
    }

    public void AddShallow(string field, object value)
    {
        _data[field] = value;
    }

    public void AddDeep(string[] fields, object value)
    {
        var dict = _data;
        for (int i = 0; i < fields.Length - 1; i++)
        {
            if (dict.TryGetValue(fields[i], out var raw))
            {
                dict = (Dictionary<string, object>)raw;
            }
            else
            {
                var next = new Dictionary<string, object>();
                dict[fields[i]] = next;
                dict = next;
            }

        }

        dict[fields[^1]] = value;
    }

    public void Apply(CommandBuilder builder)
    {
        var json = _serializer.ToCleanJson(_data);

        builder.Append($"{_locator} @> ");
        builder.AppendParameter(json, NpgsqlDbType.Jsonb);
    }

    public bool Contains(string sqlText)
    {
        return false;
    }
}


