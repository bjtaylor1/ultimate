using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ultimate.ORM.Tests
{
    public class FullObjectTest
    {
        private readonly SqlConnection connection;

        public FullObjectTest(SqlConnection connection)
        {
            this.connection = connection;
        }

        [Fact]
        public async Task GetFullObject()
        {
            await using var command = new SqlCommand(@"
                select IntVal, BigIntVal, StringVal, EnumStringVal as EnumVal, FloatVal, DecimalVal, DateTimeVal
                from TestORMData
                where Id = 1", connection);
            var objectMapper = new ObjectMapper();
            var obj = await objectMapper.ToSingleObject<FullObject>(command);
            Assert.Equal(715, obj.IntVal);
            Assert.Equal(123456789123456789, obj.BigIntVal);
            Assert.Equal("hello", obj.StringVal);
            Assert.Equal(AnEnum.EnumVal1, obj.EnumVal);
            Assert.Equal(123.45d, obj.FloatVal);
            Assert.Equal(456.789m, obj.DecimalVal);
            Assert.Equal(DateTime.Parse("2020-03-10 11:45"), obj.DateTimeVal);
        }
    }
}
