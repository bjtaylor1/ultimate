using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ultimate.ORM.Tests
{
    public class MultipleObjects
    {
        private readonly SqlConnection connection;

        public MultipleObjects(SqlConnection connection)
        {
            this.connection = connection;
        }

        [Fact]
        public async Task GetMultipleObjects()
        {
            await using var command = new SqlCommand(@"
                select IntVal, BigIntVal, StringVal, EnumStringVal as EnumVal, FloatVal, DecimalVal, DateTimeVal
                from TestORMData
                where id in (1,2)", connection);
            var objectMapper = new ObjectMapper();
            var objs = await objectMapper.ToMultipleObjects<FullObject>(command);
            Assert.Equal(2, objs.Count);
            Assert.Contains(objs, o => o.IntVal == 42);
            Assert.Contains(objs, o => o.IntVal == 715);
        }
    }
}
