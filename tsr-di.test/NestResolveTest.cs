using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tsr_di.test;

[TestClass]
public sealed class NestResolveTest
{
    [TestMethod]
    public void LifetimeDefault()
    {
        var p1 = ServiceResolver.Resolve<INestedDefault>();
        var p2 = ServiceResolver.Resolve<INestedDefault>();
        var ap1 = p1 as NestedDefault;
        var ap2 = p2 as NestedDefault;
        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.AreNotSame(ap1.S1, ap1.S2);
        Assert.AreNotSame(ap1.S1, ap2.S1);
        Assert.AreNotSame(ap1.S1, ap2.S2);
        Assert.AreNotSame(ap1.S2, ap2.S1);
        Assert.AreNotSame(ap1.S2, ap2.S2);
        Assert.AreNotSame(ap2.S1, ap2.S2);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap1.S1);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap1.S2);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap2.S1);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap2.S2);
    }

    [TestMethod]
    public void LifetimeSingleton()
    {
        var p1 = ServiceResolver.Resolve<INestedSingleton>();
        var p2 = ServiceResolver.Resolve<INestedSingleton>();
        var ap1 = p1 as NestedSingleton;
        var ap2 = p2 as NestedSingleton;
        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.AreSame(ap1.S1, ap1.S2);
        Assert.AreSame(ap1.S1, ap2.S1);
        Assert.AreSame(ap1.S1, ap2.S2);
        Assert.AreSame(ap1.S2, ap2.S1);
        Assert.AreSame(ap1.S2, ap2.S2);
        Assert.AreSame(ap2.S1, ap2.S2);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap1.S1);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap1.S2);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap2.S1);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap2.S2);
    }

    [TestMethod]
    public void LifetimeTransient()
    {
        var p1 = ServiceResolver.Resolve<INestedTransient>();
        var p2 = ServiceResolver.Resolve<INestedTransient>();
        var ap1 = p1 as NestedTransient;
        var ap2 = p2 as NestedTransient;
        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.AreNotSame(ap1.S1, ap1.S2);
        Assert.AreNotSame(ap1.S1, ap2.S1);
        Assert.AreNotSame(ap1.S1, ap2.S2);
        Assert.AreNotSame(ap1.S2, ap2.S1);
        Assert.AreNotSame(ap1.S2, ap2.S2);
        Assert.AreNotSame(ap2.S1, ap2.S2);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap1.S1);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap1.S2);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap2.S1);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap2.S2);
    }

    [TestMethod]
    public void LifetimeScoped()
    {
        var p1 = ServiceResolver.Resolve<INestedScoped>();
        var p2 = ServiceResolver.Resolve<INestedScoped>();
        var ap1 = p1 as NestedScoped;
        var ap2 = p2 as NestedScoped;
        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.AreSame(ap1.S1, ap1.S2);
        Assert.AreSame(ap2.S1, ap2.S2);
        Assert.AreNotSame(ap1.S1, ap2.S1);
        Assert.AreNotSame(ap1.S1, ap2.S2);
        Assert.AreNotSame(ap1.S2, ap2.S1);
        Assert.AreNotSame(ap1.S2, ap2.S2);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap1.S1);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap1.S2);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap2.S1);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap2.S2);
    }
}
