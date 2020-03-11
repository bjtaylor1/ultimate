using System.Data.SQLite;
using System.Threading.Tasks;
using Xunit;

namespace Ultimate.ORM.Tests
{
    public class FloatOrDouble
    {
        private readonly SQLiteConnection connection;

        public FloatOrDouble(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        [Fact]
        public async Task CanParseToFloat()
        {
            await using var command = new SQLiteCommand(@"
                select FloatVal
                from TestORMData
                where Id = 1", connection);
            var objectMapper = new ObjectMapper();
            var obj = await objectMapper.ToSingleObject<FloatObj>(command);
            Assert.Equal(123.45f, obj.FloatVal);
        }

        [Fact]
        public async Task CanParseToDouble()
        {
            await using var command = new SQLiteCommand(@"
                select FloatVal
                from TestORMData
                where Id = 1", connection);
            var objectMapper = new ObjectMapper();
            var obj = await objectMapper.ToSingleObject<DoubleObj>(command);
            Assert.Equal(123.45d, obj.FloatVal);
        }
    }

    public class FloatObj
    {
        public float FloatVal { get; set; }
    }
    public class DoubleObj
    {
        public double FloatVal { get; set; }
    }
}
