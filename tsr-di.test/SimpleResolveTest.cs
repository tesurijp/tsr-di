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

        Assert.AreNotSame(s1, s2);
        Assert.IsInstanceOfType<SimpleResolveDefault>(s1);
        Assert.IsInstanceOfType<SimpleResolveDefault>(s2);
        Assert.IsInstanceOfType<SimpleResolveDefaultNamed>(s3);
        Assert.IsInstanceOfType<SimpleResolveDefaultNamedAlt>(s4);
    }

    [TestMethod]
    public void Singleton()
    {
        var s1 = ServiceResolver.Resolve<ISimpleResolveSingleton>();
        var s2 = ServiceResolver.Resolve<ISimpleResolveSingleton>();
        var s3 = ServiceResolver.Resolve<ISimpleResolveSingleton>(ServiceKey.Single);
        var s4 = ServiceResolver.Resolve<ISimpleResolveSingleton>(ServiceKey.Single2);
        Assert.AreSame(s1, s2);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(s1);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(s2);
        Assert.IsInstanceOfType<SimpleResolveSingletonNamed>(s3);
        Assert.IsInstanceOfType<SimpleResolveSingletonNamedAlt>(s4);
    }

    [TestMethod]
    public void Transient()
    {
        var s1 = ServiceResolver.Resolve<ISimpleResolveTransient>();
        var s2 = ServiceResolver.Resolve<ISimpleResolveTransient>();
        var s3 = ServiceResolver.Resolve<ISimpleResolveTransient>(ServiceKey.Tran);
        var s4 = ServiceResolver.Resolve<ISimpleResolveTransient>(ServiceKey.Tran2);
        Assert.AreNotSame(s1, s2);
        Assert.IsInstanceOfType<SimpleResolveTransient>(s1);
        Assert.IsInstanceOfType<SimpleResolveTransient>(s2);
        Assert.IsInstanceOfType<SimpleResolveTransientNamed>(s3);
        Assert.IsInstanceOfType<SimpleResolveTransientNamedAlt>(s4);
    }

    [TestMethod]
    public void Scoped()
    {
        var s1 = ServiceResolver.Resolve<ISimpleResolveScoped>();
        var s2 = ServiceResolver.Resolve<ISimpleResolveScoped>();
        var s3 = ServiceResolver.Resolve<ISimpleResolveScoped>(ServiceKey.Scope);
        var s4 = ServiceResolver.Resolve<ISimpleResolveScoped>(ServiceKey.Scope2);
        Assert.AreNotSame(s1, s2);
        Assert.IsInstanceOfType<SimpleResolveScoped>(s1);
        Assert.IsInstanceOfType<SimpleResolveScoped>(s2);
        Assert.IsInstanceOfType<SimpleResolveScopedNamed>(s3);
        Assert.IsInstanceOfType<SimpleResolveScopedNamedAlt>(s4);
    }

    [TestMethod]
    public void LifetimeDefaultAll()
    {
        var a1 = ServiceResolver.ResolveAll<ISimpleResolveDefault>();
        var names = a1.Select(i => i.GetType().FullName).Order().ToArray();
        Assert.HasCount(3, names);
        Assert.IsTrue(names.Contains(typeof(SimpleResolveDefault).FullName));
        Assert.IsTrue(names.Contains(typeof(SimpleResolveDefaultNamed).FullName));
        Assert.IsTrue(names.Contains(typeof(SimpleResolveDefaultNamedAlt).FullName));
    }

    [TestMethod]
    public void SingletonAll()
    {
        var a1 = ServiceResolver.ResolveAll<ISimpleResolveSingleton>();
        var names = a1.Select(i => i.GetType().FullName).Order().ToArray();
        Assert.HasCount(3, names);
        Assert.IsTrue(names.Contains(typeof(SimpleResolveSingleton).FullName));
        Assert.IsTrue(names.Contains(typeof(SimpleResolveSingletonNamed).FullName));
        Assert.IsTrue(names.Contains(typeof(SimpleResolveSingletonNamedAlt).FullName));
    }

    [TestMethod]
    public void TransientAll()
    {
        var a1 = ServiceResolver.ResolveAll<ISimpleResolveTransient>();
        var names = a1.Select(i => i.GetType().FullName).Order().ToArray();
        Assert.HasCount(3, names);
        Assert.IsTrue(names.Contains(typeof(SimpleResolveTransient).FullName));
        Assert.IsTrue(names.Contains(typeof(SimpleResolveTransientNamed).FullName));
        Assert.IsTrue(names.Contains(typeof(SimpleResolveTransientNamedAlt).FullName));
    }

    [TestMethod]
    public void ScopedAll()
    {
        var a1 = ServiceResolver.ResolveAll<ISimpleResolveScoped>();
        var names = a1.Select(i => i.GetType().FullName).Order().ToArray();
        Assert.HasCount(3, names);
        Assert.IsTrue(names.Contains(typeof(SimpleResolveScoped).FullName));
        Assert.IsTrue(names.Contains(typeof(SimpleResolveScopedNamed).FullName));
        Assert.IsTrue(names.Contains(typeof(SimpleResolveScopedNamedAlt).FullName));
    }
}
