namespace tsr_di.test;

public interface ISimpleResolveSingleton;
public interface ISimpleResolveTransient;
public interface ISimpleResolveScoped;
public interface ISimpleResolveDefault;
public interface INestedDefault;
public interface INestedSingleton;
public interface INestedTransient;
public interface INestedScoped;
public interface IMultiInterface1;
public interface IMultiInterface2;

[ServiceClass] public class SimpleResolveDefault: ISimpleResolveDefault;
[ServiceClass(LifeTime=LifeTime.Singleton)] public class SimpleResolveSingleton : ISimpleResolveSingleton;
[ServiceClass(LifeTime = LifeTime.Transient)] public class SimpleResolveTransient : ISimpleResolveTransient;
[ServiceClass(LifeTime = LifeTime.Scoped)] public class SimpleResolveScoped : ISimpleResolveScoped;

[ServiceClass(Name ="Def")] public class SimpleResolveDefaultNamed : ISimpleResolveDefault;
[ServiceClass(LifeTime=LifeTime.Singleton, Name ="Single")] public class SimpleResolveSingletonNamed : ISimpleResolveSingleton;
[ServiceClass(LifeTime = LifeTime.Transient, Name ="Tran")] public class SimpleResolveTransientNamed : ISimpleResolveTransient;
[ServiceClass(LifeTime = LifeTime.Scoped, Name ="Scope")] public class SimpleResolveScopedNamed : ISimpleResolveScoped;

[ServiceClass(Name ="Def2")] public class SimpleResolveDefaultNamedAlt : ISimpleResolveDefault;
[ServiceClass(LifeTime=LifeTime.Singleton, Name ="Single2")] public class SimpleResolveSingletonNamedAlt : ISimpleResolveSingleton;
[ServiceClass(LifeTime = LifeTime.Transient, Name ="Tran2")] public class SimpleResolveTransientNamedAlt : ISimpleResolveTransient;
[ServiceClass(LifeTime = LifeTime.Scoped, Name ="Scope2")] public class SimpleResolveScopedNamedAlt : ISimpleResolveScoped;


[ServiceClass] public record class NestedDefault(ISimpleResolveDefault S1, ISimpleResolveDefault S2) : INestedDefault;
[ServiceClass] public record class NestedSingleton(ISimpleResolveSingleton S1, ISimpleResolveSingleton S2) : INestedSingleton;
[ServiceClass] public record class NestedTransient(ISimpleResolveTransient S1, ISimpleResolveTransient S2) : INestedTransient;
[ServiceClass] public record class NestedScoped(ISimpleResolveScoped S1, ISimpleResolveScoped S2) : INestedScoped;

[ServiceClass(Name ="Def", LifeTime =LifeTime.Singleton)] public class MultiInterfacDef : IMultiInterface1, IMultiInterface2;
[ServiceClass(Name="Shared", SharingMode = SharingMode.Shared, LifeTime =LifeTime.Singleton)] public class MultiInterfaceShared : IMultiInterface1, IMultiInterface2;
[ServiceClass(Name="Isolate", SharingMode = SharingMode.IsolatePerService, LifeTime =LifeTime.Singleton)] public class MultiInterfaceIsolate: IMultiInterface1, IMultiInterface2;

