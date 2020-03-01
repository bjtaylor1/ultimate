namespace Ultimate.DI
{
    public interface IContainer
    {
        void AddTransient<T1>();
        void AddTransient<T, T1>();
        T Resolve<T>();
        void AddSingleton<T, T1>();
        bool AutoResolveConcreteTypes { get; set; }
        IContainer GetNestedContainer();
    }
}