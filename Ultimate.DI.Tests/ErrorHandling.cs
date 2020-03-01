using System;
using Xunit;
using Xunit.Abstractions;

namespace Ultimate.DI.Tests
{
    public class ErrorHandling
    {
        private readonly ITestOutputHelper output;

        public ErrorHandling(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ConstructorException()
        {
            var container = new Container();
            container.AddTransient<C1>();
            container.AddTransient<C2>();
            container.AddTransient<ExceptionInConstructor>();
            var ex = Assert.Throws<ResolutionException>(() => container.Resolve<C1>());
            output.WriteLine(ex.Message);
        }

        [Fact]
        public void CircularReference()
        {
            var container = new Container();
            container.AddTransient<Circular1>();
            container.AddTransient<Circular2>();
            container.AddTransient<Circular3>();
            var ex = Assert.Throws<ResolutionException>(() => container.Resolve<Circular1>());
            output.WriteLine(ex.Message);
        }

        [Fact]
        public void NotCircularButComplex()
        {
            // CxA---CxB  
            //  | \ /  \
            //  | CxC--CxD
            //  |   \  /
            //  |----CxE
            var container = new Container();
            container.AddTransient<CxA>();
            container.AddTransient<CxB>();
            container.AddTransient<CxC>();
            container.AddTransient<CxD>();
            container.AddTransient<CxE>();
            var a = container.Resolve<CxA>();
            Assert.NotNull(a);
            Assert.IsType<CxA>(a);
        }
    }

    public class CxA { public CxA(CxB b, CxC c, CxE e){}}
    public class CxB { public CxB(CxC c, CxD d){}}
    public class CxC { public CxC(CxD d, CxE e){}}
    public class CxD { public CxD(CxE e) {}}
    public class CxE { }

    public class C1
    {
        public C1(C2 c2)
        {

        }
    }

    public class C2
    {
        public C2(ExceptionInConstructor exceptionInConstructor)
        {

        }
    }

    public class ExceptionInConstructor
    {
        public ExceptionInConstructor()
        {
            throw new Exception("An error has occurred with ExceptionInConstructor");
        }
    }

    public class Circular1
    {
        public Circular1(Circular2 c2)
        {
        }
    }

    public class Circular2
    {
        public Circular2(Circular3 c3)
        { 
        }
    }

    public class Circular3
    {
        public Circular3(Circular1 c1)
        {
        }
    }
}
