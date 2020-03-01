namespace Ultimate.DI
{
    public interface IContainer
    {
        void AddTransient<TImplementation>();
        void AddTransient<TService, TImplementation>();
        void AddInstance<TService>(TService instance);
        void AddSingleton<TImplementation>();
        void AddSingleton<TService, TImplementation>();
        T Resolve<T>();
        IContainer GetNestedContainer();
    }
}