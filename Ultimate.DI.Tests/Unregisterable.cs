using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ultimate.DI.Tests
{
    public class Unregisterable
    {
        [Fact]
        public void NoPublicConstructor()
        {
            var container = new Container();
            Assert.Throws<RegistrationException>(() => container.AddTransient<IDependency, NoPublicConstructor>());
            Assert.Throws<RegistrationException>(() => container.AddTransient<NoPublicConstructor>());
        }

        [Fact]
        public void TooManyConstructors()
        {
            var container = new Container();
            Assert.Throws<RegistrationException>(() => container.AddTransient<IDependency, TooManyConstructors>());
            Assert.Throws<RegistrationException>(() => container.AddTransient<TooManyConstructors>());
        }

        [Fact]
        public void OneConstructor()
        {
            var container = new Container();
            container.AddTransient<OneConstructor>()
        }
    }

    public class NoPublicConstructor : IDependency
    {
        private NoPublicConstructor()
        { }
    }

    public class TooManyConstructors : IDependency
    {
        public TooManyConstructors(C1 c1)
        { }
        public TooManyConstructors(C2 c2)
        { }
    }

    public class OneConstructor // but another non-public one, which shouldn't matter
    {
        public OneConstructor(IDependency dependency)
        {

        }

        private OneConstructor(ISingleton singleton)
        {

        }
    }
}
