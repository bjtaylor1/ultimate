using Xunit;

namespace Ultimate.DI.Tests
{
    public class ResolutionTests
    {
        [Fact]
        public void SimpleResolution()
        {
            var container = new Container();
            container.AddTransient<IDependency, Dependency>();
            var d = container.Resolve<IDependency>();
            Assert.NotNull(d);
            Assert.IsType<Dependency>(d);
        }

        [Fact]
        public void DependentResolution()
        {
            var container = new Container();
            container.AddTransient<IConsumer, Consumer>();
            container.AddTransient<IDependency, Dependency>();
            var c = container.Resolve<IConsumer>();
            Assert.NotNull(c);
            var cTyped = Assert.IsType<Consumer>(c);
            Assert.NotNull(cTyped.Dependency);
            Assert.IsType<Dependency>(cTyped.Dependency);
        }
    }

    public class Dependency : IDependency
    {

    }

    public interface IDependency
    {
    }

    public class Consumer : IConsumer
    {
        public IDependency Dependency { get; }

        public Consumer(IDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public interface IConsumer
    {
    }
}
