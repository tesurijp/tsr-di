using System.Collections.Generic;

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
[ServiceClass(Lifetime=Lifetime.Singleton)] public class SimpleResolveSingleton : ISimpleResolveSingleton;
[ServiceClass(Lifetime = Lifetime.Transient)] public class SimpleResolveTransient : ISimpleResolveTransient;
[ServiceClass(Lifetime = Lifetime.Scoped)] public class SimpleResolveScoped : ISimpleResolveScoped;

[ServiceClass(Name ="Def")] public class SimpleResolveDefaultNamed : ISimpleResolveDefault;
[ServiceClass(Lifetime=Lifetime.Singleton, Name ="Single")] public class SimpleResolveSingletonNamed : ISimpleResolveSingleton;
[ServiceClass(Lifetime = Lifetime.Transient, Name ="Tran")] public class SimpleResolveTransientNamed : ISimpleResolveTransient;
[ServiceClass(Lifetime = Lifetime.Scoped, Name ="Scope")] public class SimpleResolveScopedNamed : ISimpleResolveScoped;

[ServiceClass(Name ="Def2")] public class SimpleResolveDefaultNamedAlt : ISimpleResolveDefault;
[ServiceClass(Lifetime=Lifetime.Singleton, Name ="Single2")] public class SimpleResolveSingletonNamedAlt : ISimpleResolveSingleton;
[ServiceClass(Lifetime = Lifetime.Transient, Name ="Tran2")] public class SimpleResolveTransientNamedAlt : ISimpleResolveTransient;
[ServiceClass(Lifetime = Lifetime.Scoped, Name ="Scope2")] public class SimpleResolveScopedNamedAlt : ISimpleResolveScoped;


[ServiceClass] public record class NestedDefault(ISimpleResolveDefault S1, ISimpleResolveDefault S2) : INestedDefault;
[ServiceClass] public record class NestedSingleton(ISimpleResolveSingleton S1, ISimpleResolveSingleton S2) : INestedSingleton;
[ServiceClass] public record class NestedTransient(ISimpleResolveTransient S1, ISimpleResolveTransient S2) : INestedTransient;
[ServiceClass] public record class NestedScoped(ISimpleResolveScoped S1, ISimpleResolveScoped S2) : INestedScoped;

[ServiceClass(Name ="Def", Lifetime =Lifetime.Singleton)] public class MultiInterfacDef : IMultiInterface1, IMultiInterface2;
[ServiceClass(Name="Shared", SharingMode = SharingMode.Shared, Lifetime =Lifetime.Singleton)] public class MultiInterfaceShared : IMultiInterface1, IMultiInterface2;
[ServiceClass(Name="Isolate", SharingMode = SharingMode.IsolatedPerService, Lifetime =Lifetime.Singleton)] public class MultiInterfaceIsolate: IMultiInterface1, IMultiInterface2;

public interface INestedNamedSingleton;
public interface INestedNamedScoped;
public interface INestedNamedTransient;

[ServiceClass(Lifetime = Lifetime.Singleton)] public record class NestedNamedSingleton([FromNamed("Single")]ISimpleResolveSingleton S1, [FromNamed("Single")]ISimpleResolveSingleton S2, [FromNamed("Single")] [FromNewScope] ISimpleResolveSingleton S3) : INestedNamedSingleton;
[ServiceClass(Lifetime = Lifetime.Scoped)] public record class NestedNamedScoped([FromNamed("Scope")]ISimpleResolveScoped S1, [FromNamed("Scope")]ISimpleResolveScoped S2, [FromNamed("Scope")] [FromNewScope] ISimpleResolveScoped S3) : INestedNamedScoped;
[ServiceClass(Lifetime = Lifetime.Transient)] public record class NestedNamedTransient([FromNamed("Tran")]ISimpleResolveTransient S1, [FromNamed("Tran")]ISimpleResolveTransient S2, [FromNamed("Tran")] [FromNewScope] ISimpleResolveTransient S3) : INestedNamedTransient;

[ServiceClass(Lifetime = Lifetime.Singleton,Name ="Array")] public record class NestedNoNamedSingletonArray(List<ISimpleResolveSingleton> S1, List<ISimpleResolveSingleton> S2, [FromNewScope]List<ISimpleResolveSingleton> S3 ) : INestedNamedSingleton;
[ServiceClass(Lifetime = Lifetime.Scoped,Name ="Array")] public record class NestedNoNamedScopedArray(List<ISimpleResolveScoped> S1, List<ISimpleResolveScoped> S2, [FromNewScope]List<ISimpleResolveScoped> S3) : INestedNamedScoped;
[ServiceClass(Lifetime = Lifetime.Transient, Name ="Array")] public record class NestedNoNamedTransientArray(List<ISimpleResolveTransient> S1, List<ISimpleResolveTransient> S2, [FromNewScope]List<ISimpleResolveTransient> S3) : INestedNamedTransient;
