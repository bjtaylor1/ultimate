﻿using System.Data.SQLite;
using System.Threading.Tasks;
using Xunit;

namespace Ultimate.ORM.Tests
{
    public class Nulls
    {
        private readonly SQLiteConnection connection;

        public Nulls(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        [Fact]
        public async Task GetObjectWithNulls()
        {
            await using var command = new SQLiteCommand(@"
                select Id, IntVal, BigIntVal, StringVal, EnumStringVal as EnumVal, FloatVal, DecimalVal, DateTimeVal
                from TestORMData
                where id  = 3", connection);
            var objectMapper = new ObjectMapper();
            var obj = await objectMapper.ToSingleObject<FullObjectNullables>(command);
            Assert.NotNull(obj);
            Assert.Equal(3, obj.Id);
            Assert.Equal(12, obj.IntVal);
            Assert.Null(obj.BigIntVal);
            Assert.Null(obj.StringVal);
            Assert.Null(obj.FloatVal);
            Assert.Null(obj.DecimalVal);
            Assert.Null(obj.DateTimeVal);
        }
    }
}
