using System;

namespace Ultimate.DI
{
    public class Container : IContainer
    {
        public void AddTransient<T1>() => AddTransient<T1, T1>();
        public void AddTransient<T, T1>()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }

        public void AddSingleton<T, T1>()
        {
            throw new NotImplementedException();
        }

        public bool AutoResolveConcreteTypes { get; set; }

        public IContainer GetNestedContainer()
        {
            throw new NotImplementedException();
        }
    }
}
