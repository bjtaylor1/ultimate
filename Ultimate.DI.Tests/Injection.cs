using Xunit;

namespace Ultimate.DI.Tests
{
    public class Injection
    {
        [Fact]
        public void InjectTemporaryDependency()
        {
            var container = new Container();

            container.AddTransient<TemporaryConsumer>();
            container.AddTransient<IDependency, Dependency>();

            var d1 = container.Resolve<IDependency>();
            Assert.NotNull(d1);
            Assert.IsType<Dependency>(d1);
            Assert.Throws<ResolutionException>(() => container.Resolve<TemporaryConsumer>());

            var nested = container.GetNestedContainer();
            nested.AddTransient<ITemporary, Temporary>();
            var t = nested.Resolve<TemporaryConsumer>(); // should have the consumer registration from the original one
            Assert.IsType<Temporary>(t.Temporary);
            Assert.NotNull(t.Temporary);

            // should still have the original registrations
            var d2 = container.Resolve<IDependency>();
            Assert.NotNull(d2);
            Assert.IsType<Dependency>(d2);
            Assert.Throws<ResolutionException>(() => container.Resolve<TemporaryConsumer>());
        }
    }

    public class TemporaryConsumer
    {
        public ITemporary Temporary { get; }

        public TemporaryConsumer(ITemporary temporary)
        {
            Temporary = temporary;
        }
    }

    public class Temporary : ITemporary
    {

    }

    public interface ITemporary
    {
    }
}
