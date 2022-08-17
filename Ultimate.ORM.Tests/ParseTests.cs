using Xunit;
using System;

namespace Ultimate.ORM.Tests
{
    public class ParseTests
    {
        [Fact]
        public void ParseGuid()
        {
            var guidString = "f09f60e12407492784d2d3e1b4c63e21"; // such as might be retrieved by mysql
            var objectMapper = new ObjectMapper();
            var guid = objectMapper.ConvertValue(guidString, typeof(Guid));
            Console.WriteLine(guid.GetType().ToString());
            Console.WriteLine(guid.ToString());
            Assert.IsType<Guid>(guid);
            Assert.Equal(guidString, ((Guid)guid).ToString("n"));
        }
    }
}
