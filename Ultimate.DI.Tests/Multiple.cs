using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ultimate.DI.Tests
{
    public class Multiple
    {
        [Fact]
        public void ResolveMultiple()
        {
            var container = new Container();
            container.AddTransient<IService, Service1>();
            container.AddTransient<IService, Service2>();

            var s = container.Resolve<IEnumerable<IService>>();
            Assert.NotNull(s);
            var sa = s.ToArray();
            Assert.Contains(sa, e => e is Service1);
            Assert.Contains(sa, e => e is Service2);
        }
    }

    public interface IService
    {

    }

    public class Service1 : IService
    {

    }

    public class Service2 : IService
    { }
}
