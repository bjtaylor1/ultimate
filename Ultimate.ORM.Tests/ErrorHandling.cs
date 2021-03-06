﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ultimate.ORM.Tests
{
    public class ErrorHandling
    {
        private readonly SQLiteConnection connection;

        public ErrorHandling(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        [Fact]
        public async Task SomePropertiesUnsatisfied()
        {
            await using var command = new SQLiteCommand(@"
                select IntVal, BigIntVal
                from TestORMData
                where Id = 1", connection);
            var objectMapper = new ObjectMapper();
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await objectMapper.ToSingleObject<FullObject>(command));
            Assert.Equal(@"The following properties were not satisfied:
StringVal
EnumVal
FloatVal
DecimalVal
DateTimeVal", ex.Message);
        }
    }
}
