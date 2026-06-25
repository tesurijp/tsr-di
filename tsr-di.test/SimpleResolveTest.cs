using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tsr_di.test.AutoDefined;

namespace tsr_di.test;

[TestClass]
public sealed class SimpleResolveTest
{
    [TestMethod]
    public void LifetimeDefault()
    {
        var s1 = ServiceResolver.Resolve<ISimpleResolveDefault>();
        var s2 = ServiceResolver.Resolve<ISimpleResolveDefault>();
        var s3 =  ServiceResolver.Resolve<ISimpleResolveDefault>(ServiceKey.Def);
        var s4 =  ServiceResolver.Resolve<ISimpleResolveDefault>(ServiceKey.Def2);

        var (s5,s6,s7,s8) = ServiceResolver.Resolve<ISimpleResolveDefault, ISimpleResolveDefault, ISimpleResolveDefault, ISimpleResolveDefault>(ServiceKey.None, ServiceKey.None,ServiceKey.Def, ServiceKey.Def2);

        Assert.AreNotSame(s1, s2);
        Assert.IsInstanceOfType<SimpleResolveDefault>(s1);
        Assert.IsInstanceOfType<SimpleResolveDefault>(s2);
        Assert.IsInstanceOfType<SimpleResolveDefaultNamed>(s3);
        Assert.IsInstanceOfType<SimpleResolveDefaultNamedAlt>(s4);

        Assert.AreNotSame(s5, s6);
        Assert.IsInstanceOfType<SimpleResolveDefault>(s5);
        Assert.IsInstanceOfType<SimpleResolveDefault>(s6);
        Assert.IsInstanceOfType<SimpleResolveDefaultNamed>(s7);
        Assert.IsInstanceOfType<SimpleResolveDefaultNamedAlt>(s8);
    }

    [TestMethod]
    public void Singleton()
    {
        var s1 = ServiceResolver.Resolve<ISimpleResolveSingleton>();
        var s2 = ServiceResolver.Resolve<ISimpleResolveSingleton>();
        var s3 = ServiceResolver.Resolve<ISimpleResolveSingleton>(ServiceKey.Single);
        var s4 = ServiceResolver.Resolve<ISimpleResolveSingleton>(ServiceKey.Single2);

        var (s5,s6,s7,s8) = ServiceResolver.Resolve<ISimpleResolveSingleton, ISimpleResolveSingleton, ISimpleResolveSingleton, ISimpleResolveSingleton>(ServiceKey.None, ServiceKey.None,ServiceKey.Single, ServiceKey.Single2);

        Assert.AreSame(s1, s2);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(s1);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(s2);
        Assert.IsInstanceOfType<SimpleResolveSingletonNamed>(s3);
        Assert.IsInstanceOfType<SimpleResolveSingletonNamedAlt>(s4);

        Assert.AreSame(s5, s6);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(s5);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(s6);
        Assert.IsInstanceOfType<SimpleResolveSingletonNamed>(s7);
        Assert.IsInstanceOfType<SimpleResolveSingletonNamedAlt>(s8);
    }

    [TestMethod]
    public void Transient()
    {
        var s1 = ServiceResolver.Resolve<ISimpleResolveTransient>();
        var s2 = ServiceResolver.Resolve<ISimpleResolveTransient>();
        var s3 = ServiceResolver.Resolve<ISimpleResolveTransient>(ServiceKey.Tran);
        var s4 = ServiceResolver.Resolve<ISimpleResolveTransient>(ServiceKey.Tran2);
        var (s5,s6,s7,s8) = ServiceResolver.Resolve<ISimpleResolveTransient, ISimpleResolveTransient, ISimpleResolveTransient, ISimpleResolveTransient>(ServiceKey.None, ServiceKey.None,ServiceKey.Tran, ServiceKey.Tran2);

        Assert.AreNotSame(s1, s2);
        Assert.IsInstanceOfType<SimpleResolveTransient>(s1);
        Assert.IsInstanceOfType<SimpleResolveTransient>(s2);
        Assert.IsInstanceOfType<SimpleResolveTransientNamed>(s3);
        Assert.IsInstanceOfType<SimpleResolveTransientNamedAlt>(s4);

        Assert.AreNotSame(s5, s6);
        Assert.IsInstanceOfType<SimpleResolveTransient>(s5);
        Assert.IsInstanceOfType<SimpleResolveTransient>(s6);
        Assert.IsInstanceOfType<SimpleResolveTransientNamed>(s7);
        Assert.IsInstanceOfType<SimpleResolveTransientNamedAlt>(s8);
    }

    [TestMethod]
    public void Scoped()
    {
        var s1 = ServiceResolver.Resolve<ISimpleResolveScoped>();
        var s2 = ServiceResolver.Resolve<ISimpleResolveScoped>();
        var s3 = ServiceResolver.Resolve<ISimpleResolveScoped>(ServiceKey.Scope);
        var s4 = ServiceResolver.Resolve<ISimpleResolveScoped>(ServiceKey.Scope2);

        var (s5,s6,s7,s8) = ServiceResolver.Resolve<ISimpleResolveScoped, ISimpleResolveScoped, ISimpleResolveScoped, ISimpleResolveScoped>(ServiceKey.None, ServiceKey.None,ServiceKey.Scope, ServiceKey.Scope2);

        Assert.AreNotSame(s1, s2);
        Assert.IsInstanceOfType<SimpleResolveScoped>(s1);
        Assert.IsInstanceOfType<SimpleResolveScoped>(s2);
        Assert.IsInstanceOfType<SimpleResolveScopedNamed>(s3);
        Assert.IsInstanceOfType<SimpleResolveScopedNamedAlt>(s4);

        Assert.AreSame(s5, s6);
        Assert.IsInstanceOfType<SimpleResolveScoped>(s5);
        Assert.IsInstanceOfType<SimpleResolveScoped>(s6);
        Assert.IsInstanceOfType<SimpleResolveScopedNamed>(s7);
        Assert.IsInstanceOfType<SimpleResolveScopedNamedAlt>(s8);
    }

    [TestMethod]
    public void LifetimeDefaultAll()
    {
        var a1 = ServiceResolver.ResolveAll<ISimpleResolveDefault>();
        Assert.HasCount(3, a1);
        Assert.IsTrue(a1.Any(a => a is SimpleResolveDefault));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveDefaultNamed));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveDefaultNamedAlt));
    }

    [TestMethod]
    public void SingletonAll()
    {
        var a1 = ServiceResolver.ResolveAll<ISimpleResolveSingleton>();
        Assert.HasCount(3, a1);
        Assert.IsTrue(a1.Any(a => a is SimpleResolveSingleton));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveSingletonNamed));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveSingletonNamedAlt));
    }

    [TestMethod]
    public void TransientAll()
    {
        var a1 = ServiceResolver.ResolveAll<ISimpleResolveTransient>();
        Assert.HasCount(3, a1);
        Assert.IsTrue(a1.Any(a => a is SimpleResolveTransient));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveTransientNamed));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveTransientNamedAlt));
    }

    [TestMethod]
    public void ScopedAll()
    {
        var a1 = ServiceResolver.ResolveAll<ISimpleResolveScoped>();
        Assert.HasCount(3, a1);
        Assert.IsTrue(a1.Any(a => a is SimpleResolveScoped));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveScopedNamed));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveScopedNamedAlt));
    }

    [TestMethod]
    public void All()
    {
        var (a1, a2, a3, a4) = ServiceResolver.ResolveAll<ISimpleResolveDefault, ISimpleResolveSingleton, ISimpleResolveTransient, ISimpleResolveScoped>();
        Assert.HasCount(3, a1);
        Assert.IsTrue(a1.Any(a => a is SimpleResolveDefault));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveDefaultNamed));
        Assert.IsTrue(a1.Any(a => a is SimpleResolveDefaultNamedAlt));

        Assert.HasCount(3, a2);
        Assert.IsTrue(a2.Any(a => a is SimpleResolveSingleton));
        Assert.IsTrue(a2.Any(a => a is SimpleResolveSingletonNamed));
        Assert.IsTrue(a2.Any(a => a is SimpleResolveSingletonNamedAlt));

        Assert.HasCount(3, a3);
        Assert.IsTrue(a3.Any(a => a is SimpleResolveTransient));
        Assert.IsTrue(a3.Any(a => a is SimpleResolveTransientNamed));
        Assert.IsTrue(a3.Any(a => a is SimpleResolveTransientNamedAlt));

        Assert.HasCount(3, a4);
        Assert.IsTrue(a4.Any(a => a is SimpleResolveScoped));
        Assert.IsTrue(a4.Any(a => a is SimpleResolveScopedNamed));
        Assert.IsTrue(a4.Any(a => a is SimpleResolveScopedNamedAlt));
    }
}
