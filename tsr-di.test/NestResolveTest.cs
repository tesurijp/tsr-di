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
        var p3 = ServiceResolver.Services.tsr_di.test.INestedDefault;
        var p4 = ServiceResolver.Services.tsr_di.test.INestedDefault;
        var ap1 = p1 as NestedDefault;
        var ap2 = p2 as NestedDefault;
        var ap3 = p3 as NestedDefault;
        var ap4 = p4 as NestedDefault;

        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.IsNotNull(ap3);
        Assert.IsNotNull(ap4);

        Assert.AreNotSame(ap1.S1, ap1.S2);
        Assert.AreNotSame(ap1.S1, ap2.S1);
        Assert.AreNotSame(ap1.S1, ap2.S2);
        Assert.AreNotSame(ap1.S1, ap3.S1);
        Assert.AreNotSame(ap1.S1, ap3.S2);
        Assert.AreNotSame(ap1.S1, ap4.S1);
        Assert.AreNotSame(ap1.S1, ap4.S2);
        Assert.AreNotSame(ap1.S2, ap2.S1);
        Assert.AreNotSame(ap1.S2, ap2.S2);
        Assert.AreNotSame(ap1.S2, ap3.S1);
        Assert.AreNotSame(ap1.S2, ap3.S2);
        Assert.AreNotSame(ap1.S2, ap4.S1);
        Assert.AreNotSame(ap1.S2, ap4.S2);
        Assert.AreNotSame(ap2.S1, ap2.S2);
        Assert.AreNotSame(ap2.S1, ap3.S1);
        Assert.AreNotSame(ap2.S1, ap3.S2);
        Assert.AreNotSame(ap2.S1, ap4.S1);
        Assert.AreNotSame(ap2.S1, ap4.S2);
        Assert.AreNotSame(ap2.S2, ap3.S1);
        Assert.AreNotSame(ap2.S2, ap3.S2);
        Assert.AreNotSame(ap2.S2, ap4.S1);
        Assert.AreNotSame(ap2.S2, ap4.S2);
        Assert.AreNotSame(ap3.S1, ap3.S2);
        Assert.AreNotSame(ap3.S1, ap4.S1);
        Assert.AreNotSame(ap3.S1, ap4.S2);
        Assert.AreNotSame(ap3.S2, ap4.S1);
        Assert.AreNotSame(ap3.S2, ap4.S2);
        Assert.AreNotSame(ap4.S1, ap4.S2);

        Assert.IsInstanceOfType<SimpleResolveDefault>(ap1.S1);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap1.S2);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap2.S1);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap2.S2);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap3.S1);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap3.S2);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap4.S1);
        Assert.IsInstanceOfType<SimpleResolveDefault>(ap4.S2);


    }

    [TestMethod]
    public void LifetimeSingleton()
    {
        var p1 = ServiceResolver.Resolve<INestedSingleton>();
        var p2 = ServiceResolver.Resolve<INestedSingleton>();
        var p3 = ServiceResolver.Services.tsr_di.test.INestedSingleton;
        var p4 = ServiceResolver.Services.tsr_di.test.INestedSingleton;
        var ap1 = p1 as NestedSingleton;
        var ap2 = p2 as NestedSingleton;
        var ap3 = p3 as NestedSingleton;
        var ap4 = p4 as NestedSingleton;

        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.IsNotNull(ap3);
        Assert.IsNotNull(ap4);

        Assert.AreSame(ap1.S1, ap1.S2);
        Assert.AreSame(ap1.S1, ap2.S1);
        Assert.AreSame(ap1.S1, ap2.S2);
        Assert.AreSame(ap1.S1, ap3.S1);
        Assert.AreSame(ap1.S1, ap3.S2);
        Assert.AreSame(ap1.S1, ap4.S1);
        Assert.AreSame(ap1.S1, ap4.S2);

        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap1.S1);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap1.S2);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap2.S1);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap2.S2);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap3.S1);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap3.S2);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap4.S1);
        Assert.IsInstanceOfType<SimpleResolveSingleton>(ap4.S2);

    }

    [TestMethod]
    public void LifetimeTransient()
    {
        var p1 = ServiceResolver.Resolve<INestedTransient>();
        var p2 = ServiceResolver.Resolve<INestedTransient>();
        var p3 = ServiceResolver.Services.tsr_di.test.INestedTransient;
        var p4 = ServiceResolver.Services.tsr_di.test.INestedTransient;
        var ap1 = p1 as NestedTransient;
        var ap2 = p2 as NestedTransient;
        var ap3 = p3 as NestedTransient;
        var ap4 = p4 as NestedTransient;

        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.IsNotNull(ap3);
        Assert.IsNotNull(ap4);

        Assert.AreNotSame(ap1.S1, ap1.S2);
        Assert.AreNotSame(ap1.S1, ap2.S1);
        Assert.AreNotSame(ap1.S1, ap2.S2);
        Assert.AreNotSame(ap1.S1, ap3.S1);
        Assert.AreNotSame(ap1.S1, ap3.S2);
        Assert.AreNotSame(ap1.S1, ap4.S1);
        Assert.AreNotSame(ap1.S1, ap4.S2);
        Assert.AreNotSame(ap1.S2, ap2.S1);
        Assert.AreNotSame(ap1.S2, ap2.S2);
        Assert.AreNotSame(ap1.S2, ap3.S1);
        Assert.AreNotSame(ap1.S2, ap3.S2);
        Assert.AreNotSame(ap1.S2, ap4.S1);
        Assert.AreNotSame(ap1.S2, ap4.S2);
        Assert.AreNotSame(ap2.S1, ap2.S2);
        Assert.AreNotSame(ap2.S1, ap3.S1);
        Assert.AreNotSame(ap2.S1, ap3.S2);
        Assert.AreNotSame(ap2.S1, ap4.S1);
        Assert.AreNotSame(ap2.S1, ap4.S2);
        Assert.AreNotSame(ap2.S2, ap3.S1);
        Assert.AreNotSame(ap2.S2, ap3.S2);
        Assert.AreNotSame(ap2.S2, ap4.S1);
        Assert.AreNotSame(ap2.S2, ap4.S2);
        Assert.AreNotSame(ap3.S1, ap3.S2);
        Assert.AreNotSame(ap3.S1, ap4.S1);
        Assert.AreNotSame(ap3.S1, ap4.S2);
        Assert.AreNotSame(ap3.S2, ap4.S1);
        Assert.AreNotSame(ap3.S2, ap4.S2);
        Assert.AreNotSame(ap4.S1, ap4.S2);

        Assert.IsInstanceOfType<SimpleResolveTransient>(ap1.S1);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap1.S2);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap2.S1);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap2.S2);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap3.S1);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap3.S2);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap4.S1);
        Assert.IsInstanceOfType<SimpleResolveTransient>(ap4.S2);
    }

    [TestMethod]
    public void LifetimeScoped()
    {
        var p1 = ServiceResolver.Resolve<INestedScoped>();
        var p2 = ServiceResolver.Resolve<INestedScoped>();
        var p3 = ServiceResolver.Services.tsr_di.test.INestedScoped;
        var p4 = ServiceResolver.Services.tsr_di.test.INestedScoped;
        var ap1 = p1 as NestedScoped;
        var ap2 = p2 as NestedScoped;
        var ap3 = p3 as NestedScoped;
        var ap4 = p4 as NestedScoped;

        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.IsNotNull(ap3);
        Assert.IsNotNull(ap4);

        Assert.AreSame(ap1.S1, ap1.S2);
        Assert.AreNotSame(ap1.S1, ap2.S1);
        Assert.AreNotSame(ap1.S1, ap2.S2);
        Assert.AreNotSame(ap1.S1, ap3.S1);
        Assert.AreNotSame(ap1.S1, ap3.S2);
        Assert.AreNotSame(ap1.S1, ap4.S1);
        Assert.AreNotSame(ap1.S1, ap4.S2);
        Assert.AreNotSame(ap1.S2, ap2.S1);
        Assert.AreNotSame(ap1.S2, ap2.S2);
        Assert.AreNotSame(ap1.S2, ap3.S1);
        Assert.AreNotSame(ap1.S2, ap3.S2);
        Assert.AreNotSame(ap1.S2, ap4.S1);
        Assert.AreNotSame(ap1.S2, ap4.S2);
        Assert.AreSame(ap2.S1, ap2.S2);
        Assert.AreNotSame(ap2.S1, ap3.S1);
        Assert.AreNotSame(ap2.S1, ap3.S2);
        Assert.AreNotSame(ap2.S1, ap4.S1);
        Assert.AreNotSame(ap2.S1, ap4.S2);
        Assert.AreNotSame(ap2.S2, ap3.S1);
        Assert.AreNotSame(ap2.S2, ap3.S2);
        Assert.AreNotSame(ap2.S2, ap4.S1);
        Assert.AreNotSame(ap2.S2, ap4.S2);
        Assert.AreSame(ap3.S1, ap3.S2);
        Assert.AreNotSame(ap3.S1, ap4.S1);
        Assert.AreNotSame(ap3.S1, ap4.S2);
        Assert.AreNotSame(ap3.S2, ap4.S1);
        Assert.AreNotSame(ap3.S2, ap4.S2);
        Assert.AreSame(ap4.S1, ap4.S2);

        Assert.IsInstanceOfType<SimpleResolveScoped>(ap1.S1);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap1.S2);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap2.S1);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap2.S2);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap3.S1);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap3.S2);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap4.S1);
        Assert.IsInstanceOfType<SimpleResolveScoped>(ap4.S2);
    }
}
