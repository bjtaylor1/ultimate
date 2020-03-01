using Xunit;

namespace Ultimate.DI.Tests
{
    public class LifetimeTests
    {
        [Fact]
        public void ResolveTransient()
        {
            var container = new Container();
            container.AddTransient<IDependency, Dependency>();

            var s1 = container.Resolve<IDependency>();
            var s2 = container.Resolve<IDependency>();
            Assert.NotSame(s1, s2);
        }

        [Fact]
        public void ResolveSingleton()
        {
            var container = new Container();
            container.AddSingleton<ISingleton, Singleton>();

            var s1 = container.Resolve<ISingleton>();
            var s2 = container.Resolve<ISingleton>();
            Assert.Same(s1, s2);
        }

        [Fact]
        public void ResolveSingletonDependency()
        {
            var container = new Container();
            container.AddSingleton<ISingleton, Singleton>();
            container.AddTransient<IDependsOnSingleton, DependsOnSingleton>();

            var s1 = container.Resolve<IDependsOnSingleton>();
            var s2 = container.Resolve<IDependsOnSingleton>();
            Assert.NotSame(s1, s2);
            Assert.Same(s1.Singleton, s2.Singleton);
        }

        [Fact]
        public void ResolveInstance()
        {
            var container = new Container();
            var instance = new SingletonWithDependency(new Dependency());
            container.AddInstance<ISingleton>(instance);
            var s = container.Resolve<ISingleton>();
            Assert.Same(instance, s);
        }
    }

    public class DependsOnSingleton : IDependsOnSingleton
    {
        public ISingleton Singleton { get; }

        public DependsOnSingleton(ISingleton singleton)
        {
            Singleton = singleton;
        }
    }

    public interface IDependsOnSingleton
    {
        ISingleton Singleton { get; }
    }

    public class Singleton : ISingleton
    {
    }
    public class SingletonWithDependency : ISingleton
    {
        public IDependency Dependency { get; }

        public SingletonWithDependency(IDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public interface ISingleton
    {
    }
}
