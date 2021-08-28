namespace Brimborium.Registrator.Test {
    public interface IA { }
    public interface IB { }

    public class DummyImpl1 : IA{ }

    [Brimborium.Registrator.Singleton()]
    public class DummyImpl1AttributeSingleton : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.Auto, ServiceType = typeof(IA))]
    public class DummyImpl1IAAttributeSingletonAutoIA : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeSelf)]
    public class DummyImpl1AttributeSingletonIncludeSelf : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClasses)]
    public class DummyImpl1AttributeSingletonIncludeClasses : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClasses, ServiceType = typeof(DummyImpl1))]
    public class DummyImpl1AttributeSingletonIncludeClassesServiceType : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClasses, ServiceType = typeof(IA))]
    public class DummyImpl1AttributeSingletonIncludeClassesServiceTypeIA : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeInterfaces)]
    public class DummyImpl1AttributeSingletonIncludeInterfaces : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeInterfaces, ServiceType = typeof(IA))]
    public class DummyImpl1AttributeSingletonIncludeInterfacesServiceType : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClassesAndInterfaces)]
    public class DummyImpl1AttributeSingletonIncludeClassesAndInterfaces : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClassesAndInterfaces, ServiceType = typeof(IA))]
    public class DummyImpl1AttributeSingletonIncludeClassesAndInterfacesServiceType : DummyImpl1, IA { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeSelfAndInterfaces)]
    public class DummyImpl1AttributeSingletonIncludeSelfAndInterfaces : DummyImpl1, IA { }


    public class DummyImpl2 : IA, IB { }
    
    [Brimborium.Registrator.Singleton()]
    public class DummyImpl2AttributeSingleton : IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.Auto, ServiceType = typeof(IA))]
    public class DummyImpl2IAAttributeSingletonAutoIA : IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeSelf)]
    public class DummyImpl2AttributeSingletonIncludeSelf : DummyImpl2, IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClasses)]
    public class DummyImpl2AttributeSingletonIncludeClasses : DummyImpl2, IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClasses, ServiceType = typeof(DummyImpl2))]
    public class DummyImpl2AttributeSingletonIncludeClassesServiceType : DummyImpl2, IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClasses, ServiceType = typeof(IA))]
    public class DummyImpl2AttributeSingletonIncludeClassesServiceTypeIA : DummyImpl2, IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeInterfaces)]
    public class DummyImpl2AttributeSingletonIncludeInterfaces : DummyImpl2, IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeInterfaces, ServiceType = typeof(IA))]
    public class DummyImpl2AttributeSingletonIncludeInterfacesServiceType : DummyImpl2, IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClassesAndInterfaces)]
    public class DummyImpl2AttributeSingletonIncludeClassesAndInterfaces : DummyImpl2, IA, IB { }

    [Brimborium.Registrator.Singleton(Mode = ServiceTypeMode.IncludeClassesAndInterfaces, ServiceType = typeof(IA))]
    public class DummyImpl2AttributeSingletonIncludeClassesAndInterfacesServiceType : DummyImpl2, IA, IB { }


    [Brimborium.Registrator.Singleton()]
    [Brimborium.Registrator.Transient()]
    public class DummyImplAttributeSingletonTransient : IA, IB { }

}
