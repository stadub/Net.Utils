## *<a name="servicelocatormapper"> ServiceLocator/ObjectMapper</a>*- service locator implementation with type mapping capabilities.


* ### [Service locator.](#servicelocator)

* ### [Type Mapper.](#typemapper)

## <a name="servicelocator"></a> *Service Locator:* ##
#### *Supported capabilities:*

Type mapping registry as a single point of mapping registration:
```C#
    var registry = new TypeMapperRegistry();
    var source = new ClassW2Properties {Prop = 1, Prop2 = 2};

    var mapper = new TypeMapper<ClassW2Properties, ClassW4Properties>();
    registry.Register<ClassW2Properties, ClassW4Properties>(mapper);

    var dest = registry.Resolve<ClassW4Properties>(source);

    Assert.IsTrue(dest.Prop == 1);
    Assert.IsTrue(dest.Prop2 == 2);
```

Property injection support via embed ServiceLocator resolver:
```C#
    var registry = new TypeMapperRegistry();
    var source = new ClassW2Properties { Prop = 1, Prop2 = 2 };

    var propertiesMapper=registry.Register<ClassW2Properties, ClassW4Properties>();
    propertiesMapper.InjectPropertyValue(properties => properties.Prop3,3);

    ClassW4Properties dest = registry.Resolve(source, typeof(ClassW4Properties)) as ClassW4Properties;

    Assert.IsTrue(dest.Prop == 1);
    Assert.IsTrue(dest.Prop3 == 3);
```

Allow to specify correct decedant resolving:
```C#
    var registry = new TypeMapperRegistry();
    var source = new ClassW2Properties { Prop = 1, Prop2 = 2 };

    var mapper = new TypeMapper<ClassW2Properties, ClassW4PropertiesDescendant1>();
    registry.Register<ClassW2Properties, ClassW4PropertiesDescendant1>(mapper);
    registry.Register<ClassW2Properties, ClassW4PropertiesDescendant2>();
    registry.Register<ClassW2Properties, ClassW4Properties>();

    var dest = registry.ResolveDescendants<ClassW4Properties>(source);

    Assert.IsTrue(dest.Count()==3);
    Assert.IsTrue(dest.All(properties => properties.Prop2 == 2));
    Assert.IsTrue(dest.All(properties => properties.Prop == 1));
```

## *<a name="typemapper"> Type Mapper:</a>* ##
#### *Supported capabilities:*

Colletions mapping support:
```
var arr = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
var arrMapper = new ArrayTypeMapper<int, string>(new MappingFunc<int, string>(i => i.ToString()));
var dest=arrMapper.Map(arr);
```

Support identical named property/ctor param mapping:
```C#
    class ClassWithSourceProp
    {
        public ClassWithSourceProp(int prop2)
        {
            Prop2 = prop2;
        }

        public int Prop2 { get; private set; }
    }

    var mapper = new TypeMapper<ClassW2Properties, ClassWithSourceProp>();
    var dest = mapper.Map(new ClassW2Properties { Prop = 1, Prop2 = 2 });
    Assert.IsTrue(dest.Prop2 == 2);
```

Type to constructor mapping:
```C#

    class ClassWithSourceCtor
    {
        public ClassWithSourceCtor(ClassW2Properties source)
        {
            Source = source;
        }
        public ClassW2Properties Source { get; private set; }
    }
    class ClassW2Properties
    {
            public int Prop { get; set; }
            public int Prop2 { get; set; }
    }

    var mapper = new TypeMapper<ClassW2Properties, ClassWithSourceCtor>();
    var source = new ClassW2Properties();
    var dest = mapper.Map(source);

    Assert.AreEqual(dest.Source, source);
```

Property value injectoin support:
```C#
    var mapper = new TypeMapper<ClassW2Properties, ClassW4Properties>();
    mapper.RegistrationInfo.InjectPropertyValue(cl => cl.Prop3,3);
    var source = new ClassW2Properties();
    var dest = mapper.Map(source);
    Assert.AreEqual(3, dest.Prop3);
```

Allow to add property to ignore list:
```C#
    var mapper = new TypeMapper<ClassW2Properties, ClassW4Properties>();

    mapper.RegistrationInfo.IgnoreProperty(cl => cl.Prop);
    var source = new ClassW2Properties{ Prop = 1,Prop2 = 2};
    var dest = mapper.Map(source);
    Assert.IsNotNull(dest);
    Assert.AreEqual(0, dest.Prop);
    Assert.AreEqual(2, dest.Prop2);
```

Rules to resolve properties and add to ignore list:
```C#
        class ClassWIntListProperty : ITestClass
        {
            public IList<int> Prop { get; set; }
        } 

        var locator = new ServiceLocator();
        //inject property rules
        locator
            .RegisterType<ITestClass, ClassWIntListProperty>()
            .InjectProperty(classWProp => classWProp.Prop);

        //instance resolver
        var list = new List<int>();
        locator.RegisterInstance<IList<int>, List<int>>(list);

        var mapper = new TypeMapper<ClassW2Properties, ClassW4Properties>(locator);

        //ignore property list
        mapper.RegistrationInfo.IgnoreProperty(cl => cl.Prop);
        mapper.LocatorMappingInfo.InjectProperty(properties => properties.Prop4);

        var source = new ClassW2Properties { Prop2 = 2 };
        var dest = mapper.Map(source);

        Assert.AreEqual(2, dest.Prop2);

        var instance = dest.Prop4 as TestClassWIntListProperty;

        Assert.IsTrue(ReferenceEquals(instance.Prop, list));
```

Property name mapping attributes:
```C#
    class ClassWSourcePropertyMapAttribute
    {
        [MapSourceProperty(Name="Prop2")]
        public int Prop { get; set; }
        public int Prop2 { get; set; }
    } 

    var mapper = new TypeMapper<ClassW2Properties, ClassWSourcePropertyMapAttribute>();
    var source = new ClassW2Properties {Prop = 1, Prop2 = 2};
    var dest = mapper.Map(source);
    Assert.AreEqual(2, dest.Prop);
    Assert.AreEqual(2, dest.Prop2);
```

Propery path mapping attributes:
```C#
    class ClassWSourcePropertyPathAttribute
    {
        [MapSourceProperty(Path = "Source.Prop2")]
        public int Prop { get; set; }
    }
    class ClassW2Properties
    {
            public int Prop { get; set; }
            public int Prop2 { get; set; }
    }
    class ClassWithSourceCtor
    {
        public ClassWithSourceCtor(ClassW2Properties source)
        {
            Source = source;
        }
        public ClassW2Properties Source { get; private set; }
    }

    var mapper = new TypeMapper<ClassWithSourceCtor, ClassWSourcePropertyPathAttribute>();
    var source = new ClassWithSourceCtor(new ClassW2Properties {Prop = 1, Prop2 = 2});
    var dest = mapper.Map(source);
    Assert.IsNotNull(dest);
    Assert.AreEqual(2, dest.Prop);
```

Initalize method support:
```C#
    class ClassWSourcePropertyInitalizer
    {
        [UseInitalizer(Name = "InitProp")]
        public int Prop { get; private set; }

        public void InitProp(int value)
        {
            Prop = value;
        }
    }

    var mapper = new TypeMapper<ClassW2Properties, ClassWSourcePropertyInitalizer>();
    var source = new ClassW2Properties { Prop = 1, Prop2 = 2 };
    var dest = mapper.Map(source);
    Assert.IsNotNull(dest);
    Assert.AreEqual(1, dest.Prop);
```

Initalizer method support property path usage:
```C#
    class ClassWSourcePropertyInitalizerAndPropertyPath
    {
        [MapSourceProperty(Path = "Source.Prop2")] [UseInitalizer(Name = "InitProp")]
        public int Prop { get; private set; }

        public void InitProp(int value)
        {
            Prop = value;
        }
    }

    var mapper = new TypeMapper<ClassWithSourceCtor, ClassWSourcePropertyInitalizerAndPropertyPath>();
    var source = new ClassWithSourceCtor(new ClassW2Properties {Prop = 1, Prop2 = 2});
    var dest = mapper.Map(source);
    Assert.IsNotNull(dest);
    Assert.IsTrue(dest.Prop == 2);
```

Array property mapping support via ArrayTypeMapper mapper:
```C#
    class ClassWIntListProperty : ITestClass
    {
        public IList<int> Prop { get; set; }
    } 
    class ClassWStringListProperty : ITestClass
    {
        public IList<string> Prop { get; set; }
    }

    var mapper = new TypeMapper<ClassWIntListProperty, ClassWStringListProperty>();
    var arrMapper = new ArrayTypeMapper<int, string>(new MappingFunc<int, string>(i => i.ToString()));

    mapper.PropertyMappingInfo.MapProperty((property, ints) => property.Prop,arrMapper);

    var arr = new[] {1, 2, 3, 4, 5, 6, 7, 8};

    var source = new ClassWIntListProperty { Prop = new List<int>(arr) };

    var dest = mapper.Map(source);
```

Mapping converters for primitive types:
```C#
    public class ClassWProperty
    {
        public Uri Prop { get; set; }
    }
    public class ClassWStringProperty
    {
        public string Prop { get; set; }
    }

    var source = new ClassWStringProperty();
    source.Prop = "http://test.link/";
    var mapper = new TypeMapper<ClassWStringProperty, ClassWProperty>();
    var dest = mapper.Map(source);
    Assert.AreEqual(source.Prop,dest.Prop.AbsoluteUri);
```

Dictionary<=>type mapping:
```C#
    var source = new Dictionary<string, int>();

    source.Add("Prop",1);
    source.Add("Prop2", 1);
    source.Add("Prop3", 1);
    var mapper = new DictionaryMapper<int, ClassW4Properties>();

    var dest = mapper.Map(source);
    Assert.IsNotNull(dest);
    Assert.AreEqual(source["Prop"], dest.Prop);
    Assert.AreEqual(source["Prop2"], dest.Prop2);
    Assert.AreEqual(source["Prop3"], dest.Prop3);
```


Numeric formatting support:
```C#
    public class ClassWFormatedProperty
    {

        [FormatedNumeric("D9")]
        public long Prop { get; set; }
    }
    public class ClassWStringProperty
    {
        public string Prop { get; set; }
    }

    var source = new ClassWFormatedProperty();

    source.Prop = 1;
    var mapper = new TypeMapper<ClassWFormatedProperty, ClassWStringProperty>();
    var dest = mapper.Map(source);
    Assert.AreEqual("000000001", dest.Prop);
```

String formatter `magic`:
```C#
    string TextHtmlData = @"Version:1.0
StartHTML:000000194
EndHTML:000001170
StartFragment:000000493
EndFragment:000001112
StartSelection:000000507
EndSelection:000001108
SourceURL:res://iesetup.dll/HardAdmin.htm";

    var data = new HtmlClipboardFormatData
    {
        Version = "1.0",
        StartHTML = 000000194,
        EndHTML = 000001170,
        StartFragment = 000000493,
        EndFragment = 000001112,
        StartSelection = 000000507,
        EndSelection = 000001108,
        SourceURL = new Uri("res://iesetup.dll/HardAdmin.htm"),
    };
    var mapper = new StringFormatter<HtmlClipboardFormatData>();

    var dest = mapper.Map(data).Trim();
    Assert.AreEqual(TextHtmlData, dest);
```

#### Advanced 
Custom Type builders support with resolve override possibilities: 
```C#

    class TypeBuilder<TSource, TDest> : MappingTypeBuilder<TSource, TDest>
    {
        public PropertyInfo PropertyToResolve
        {
            get { return injectResolver.PropertyToResolve; }
            set { injectResolver.PropertyToResolve = value; }
        }

        public object ProertyValue
        {
            get { return injectResolver.ProertyValue; }
            set { injectResolver.ProertyValue=value; }
        }

        private InjectValueResolver injectResolver;

        public TypeBuilder(bool priorResolver=false): base()
        {
            injectResolver = new TestInjectValueResolver();
            if (priorResolver)
                RegisterPriorSourceResolver(injectResolver);
            else
                RegisterSourceResolver(injectResolver);
        }

        public override void CreateBuildingContext()
        {
            base.Context= new TypeBuilerContext<TSource, TDest>();
        }

        public override void InitBuildingContext()
        {
            
        }
    }
    class TypeMapper<TSource, TDest> : TypeMapper<TSource, TDest>
    {
        private readonly Func<TestTypeBuilder<TSource, TDest>> createBuilderFunc;

        public TypeMapper(Func<TestTypeBuilder<TSource, TDest>> createBuilderFunc)
        {
            this.createBuilderFunc = createBuilderFunc;
        }

        protected override MappingTypeBuilder<TSource, TDest> CreateTypeBuilder()
        {
            return createBuilderFunc();
        }
    }

    class TypeBuilerContext<TSource, TDest> : TypeMapperContext<TSource, TDest> 
    {
        public TypeBuilerContext(): base(new Dictionary<PropertyInfo, ITypeMapper>()){}
    }

    var destType = typeof (ClassW4Properties);
    //Custom property resolve rule
    var testBuilder = new TypeBuilder<ClassW2Properties,ClassW4Properties>();
    testBuilder.PropertyToResolve = destType.GetProperty("Prop3");
    testBuilder.ProertyValue = 3;

    var mapper = new TypeMapper<ClassW2Properties, ClassW4Properties>(() => testBuilder);
    var source = new ClassW2Properties();
    var dest = mapper.Map(source);

    Assert.AreEqual(3, dest.Prop3);
```
___
