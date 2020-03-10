using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace Ultimate.ORM.Tests
{
    public class Enums
    {
        private readonly SqlConnection connection;

        public Enums(SqlConnection connection)
        {
            this.connection = connection;
            if(this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
            }
        }

        [Theory]
        [InlineData("EnumStringVal")]
        [InlineData("EnumIntVal")]
        public async Task EnumFromString(string enumExpression)
        {
            await using var command = new SqlCommand($"select {enumExpression} as EnumVal from TestORMData where Id = 1", connection);
            var objectMapper = new ObjectMapper();
            var obj = await objectMapper.ToSingleObject<TestObject>(command);
            Assert.NotNull(obj);
            Assert.Equal(AnEnum.EnumVal1, obj.EnumVal);
        }
    }

    public class TestObject
    {
        public AnEnum EnumVal { get; set; }
    }

    public enum AnEnum { EnumVal0, EnumVal1, EnumVal2 }
}
