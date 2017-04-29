## Service locator

Singleton registration
```C#
    var locator = new ServiceLocator();
    var list = new List<int>();
    locator.RegisterInstance<IList<int>,List<int>>(list);
    
    var reslut = locator.Resolve<IList<int>>();
```

Type registration
```C#
    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, TestClass>();

    var reslut = locator.Resolve<ITestClass>();
```

Constructor usage samples

Simple constructor sample
```C#

    class ClassWDefaultCtor : ITestClass
    {
        public ClassWDefaultCtor() { }
    }
    
    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWDefaultCtor>(); 
    
    var reslut = locator.Resolve<ITestClass>();
```

Constructor with arguments
```C#
    class TestClassWCtor : ITestClass
    {
        public TestClassWCtor(int a=2) { }
    }

    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, TestClassWCtor>();
    var reslut = locator.Resolve<ITestClass>();
```

Several constructors with arguments 
```C#
    class ClassWUseCtorAttribute : ITestClass
    {
        public ClassWUseCtorAttribute(object a) { }

        public object b;
        [UseConstructor()]
        public ClassWUseCtorAttribute(int a = 2) { b = a; }
    }

    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWUseCtorAttribute>();//will use constructor marked as default

    var reslut = locator.Resolve<ITestClass>();
```

Constructor arguments can be injected
```C#
    class ClassWSimpleInjectionCtor : ITestClass
    {
        public object b;
        public ClassWSimpleInjectionCtor([InjectValue("2")]int a){ b = a; }
    }

    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWSimpleInjectionCtor>();
    var reslut = locator.Resolve<ITestClass>();
```

Generic arguments injection
```C#
    class ClassWGenericInjectionCtor : ITestClass
    {
        public object b;
        public ClassWGenericInjectionCtor(IList<int> a ) { b = a; }
    }

    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWGenericInjectionCtor>();
    var list = new List<int>();
    locator.RegisterInstance<IList<int>, List<int>>(list);

    var reslut = locator.Resolve<ITestClass>();
```

Generic arguments injection into constructor with `ShoudlInject` arguments
```C#
    class ClassWInjectionCtor : ITestClass
    {
        public ClassWInjectionCtor([ShoudlInject()]IList<int> a) { }
    }
    
    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWInjectionCtor>();
    var list = new List<int>();
    locator.RegisterInstance<IList<int>, List<int>>(list);

    var reslut = locator.Resolve<ITestClass>();
```

### Properties injection

Simple properties injection
```C#
    class ClassWPropertyInjection : ITestClass
    {
        [InjectInstance()]
        public IList<int> Prop{get;set;}
    }
	
    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWPropertyInjection>();
    var list = new List<int>();
    locator.RegisterInstance<IList<int>, List<int>>(list);

    var reslut = locator.Resolve<ITestClass>();
```

Named instances properties injection
```C#
    class ClassWNamedPropertyInjection : ITestClass
    {
        [InjectInstance("test")]
        public ITestClass Prop { get; set; }
    }

    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWNamedPropertyInjection>();
    locator.RegisterType<ITestClass, ClassWDispose>("test");

    var reslut = locator.Resolve<ITestClass>();//will inject 
	
	var instance = reslut as ClassWNamedPropertyInjection;

     Assert.IsTrue(instance.Prop is ClassWDispose);
```

Named instances properties injection for singletons
```C#
    class ClassWNamedPropertyInjection : ITestClass
    {
        [InjectInstance("test")]
        public ITestClass Prop { get; set; }
    }

    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWNamedPropertyInjection>();
    locator.RegisterInstance<ITestClass, ClassWDispose>(new ClassWDispose(),"test");

    var reslut = locator.Resolve<ITestClass>();

    Assert.IsNotNull(reslut);
    var instance = reslut as ClassWNamedPropertyInjection;

    Assert.IsTrue(instance.Prop is TestClassWDispose);
```

Premitive type value injection
```C#
    class TestClassWPropertyValueInjection : ITestClass
    {
        [InjectValue("2")]
        public int Prop { get; set; }
    }


    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWPropertyValueInjection>();

    var reslut = locator.Resolve<ITestClass>();

    var instance = reslut as ClassWPropertyValueInjection;

    Assert.IsTrue(instance.Prop== 2);
```

Destroy IDisposable types on `ServiceLocator` destruction
```C#
    class ClassWDispose : ITestClass,IDisposable
    {
        public bool Disposed{get;set;}
        
        public void Dispose()
        {
            Disposed=true;
        }
    }


    var locator = new ServiceLocator();
    var disbposble=new ClassWDispose();
    locator.RegisterInstance<ITestClass,ClassWDispose>(disbposble);
    var reslut = locator.Resolve<ITestClass>();

    locator.Dispose();

    Assert.IsTrue(disbposble.Disposed);
        
```

Type initalizers support
```C#
    class ClassWDispose : ITestClass,IDisposable
    {
        public bool Disposed{get;set;}
        
        public void Dispose()
        {
            Disposed=true;
        }
    }

    var locator = new ServiceLocator();
    locator.RegisterInitializer<ITestClass>(() => new ClassWDispose());
    var reslut = locator.Resolve<ITestClass>();
```

Injection methos extensions provide more handy configuration

Property injection
```C#


    var locator = new ServiceLocator();
    locator
        .RegisterType<ITestClass, ClassWProperty>()
        .InjectProperty(classWProp => classWProp.Prop);
    var list = new List<int>();
    locator.RegisterInstance<IList<int>, List<int>>(list);

    var reslut = locator.Resolve<ITestClass>();

    var instance = reslut as ClassWProperty;

    Assert.IsTrue(ReferenceEquals(instance.Prop, list));
```    

Named property injection
```C#
    var locator = new ServiceLocator();
    locator
        .RegisterType<ITestClass, ClassWTestClassProperty>()
        .InjectNamedProperty(classWProp => classWProp.Prop,"TestProp");
    locator.RegisterType<ITestClass, ClassWDispose>("TestProp");

    var reslut = locator.Resolve<ITestClass>();

    Assert.IsNotNull(reslut);
    var instance = reslut as ClassWTestClassProperty;

    Assert.IsNotNull(instance);
    Assert.IsTrue(instance.Prop is ClassWDispose);
```

Property value injection
```C#
    var list = new List<int>();
    var locator = new ServiceLocator();
    locator
        .RegisterType<ITestClass, ClassWProperty>()
        .InjectPropertyValue(classWProp => classWProp.Prop, list);
            
    var reslut = locator.Resolve<ITestClass>();

    Assert.IsNotNull(reslut);
    var instance = reslut as ClassWProperty;

    ReferenceEquals(instance.Prop, list);
```

Property named type injection
```C#

    var locator = new ServiceLocator();
    locator.RegisterType<ITestClass, ClassWProperty>("class1");
    var reslut = locator.ResolveType<ITestClass>("class1");
    Assert.IsNotNull(reslut);
```

Complex example of usage Named and default type resolvers
```C#

    var locator = new ServiceLocator();
    locator
        .RegisterType<ITestClass, TestClassWProperty>()
        .InjectProperty(classWProp => classWProp.Prop);
    var list = new List<int>();
    locator.RegisterInstance<IList<int>, List<int>>(list);
    locator.RegisterType<ITestClass, TestClassWProperty>("class1");

    var reslut = locator.ResolveType<ITestClass>();
    var reslut2 = locator.ResolveType<ITestClass>("class1");

    var instance = reslut as TestClassWProperty;
    var instance2 = reslut2 as TestClassWProperty;

    ReferenceEquals(instance.Prop, list);

    Assert.IsNull(instance2.Prop);
```

Primetive types resolver:
```C#

    var locator = new ServiceLocator();
    locator.RegisterType<int, int>();

    var defaultValue=locator.ResolveType<int>();

    Assert.IsTrue((int)defaultValue == 0);
```