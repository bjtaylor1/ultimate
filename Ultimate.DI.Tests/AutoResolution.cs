using Xunit;

namespace Ultimate.DI.Tests
{
    public class AutoResolution
    {
        [Fact]
        public void DefaultNoAutoResolve()
        {
            var container = new Container();

            Assert.Throws<ResolutionException>(() => container.Resolve<Dependency>());
        }

        [Fact]
        public void AutoResolve()
        {
            var container = new Container {AutoResolveConcreteTypes = true};
            var d = container.Resolve<Dependency>();
            Assert.NotNull(d);
            Assert.IsType<Dependency>(d);
        }
    }
}
