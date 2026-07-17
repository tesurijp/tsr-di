using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using tsr_di.test.AutoDefined;

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

    [TestMethod]
    public void NestedNamed()
    {
        var p1 = ServiceResolver.Resolve<INestedNamedSingleton>();
        var p2 = ServiceResolver.Resolve<INestedNamedScoped>();
        var p3 = ServiceResolver.Resolve<INestedNamedTransient>();
        var p4 = ServiceResolver.Services.tsr_di.test.INestedNamedSingleton;
        var p5 = ServiceResolver.Services.tsr_di.test.INestedNamedScoped;
        var p6 = ServiceResolver.Services.tsr_di.test.INestedNamedTransient;
        var ap1 = p1 as NestedNamedSingleton;
        var ap2 = p2 as NestedNamedScoped;
        var ap3 = p3 as NestedNamedTransient;
        var ap4 = p4 as NestedNamedSingleton;
        var ap5 = p5 as NestedNamedScoped;
        var ap6 = p6 as NestedNamedTransient;
        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.IsNotNull(ap3);
        Assert.IsNotNull(ap4);
        Assert.IsNotNull(ap5);
        Assert.IsNotNull(ap6);

        Assert.AreSame(ap1.S1, ap1.S2);
        Assert.AreSame(ap1.S1, ap1.S3);

        Assert.AreSame(ap2.S1, ap2.S1);
        Assert.AreNotSame(ap2.S1, ap2.S3);

        Assert.AreNotSame(ap3.S1, ap3.S2);
        Assert.AreNotSame(ap3.S1, ap3.S3);
        Assert.AreNotSame(ap3.S2, ap3.S3);

        Assert.AreSame(ap4.S1, ap4.S2);
        Assert.AreSame(ap4.S1, ap4.S3);

        Assert.AreSame(ap5.S1, ap5.S2);
        Assert.AreNotSame(ap5.S1, ap5.S3);

        Assert.AreNotSame(ap6.S1, ap6.S2);
        Assert.AreNotSame(ap6.S1, ap6.S3);
        Assert.AreNotSame(ap6.S2, ap6.S3);
    }
    [TestMethod]
    public void NestedNamedArray()
    {
        var p1 = ServiceResolver.Resolve<INestedNamedSingleton>(ServiceKey.Array);
        var p2 = ServiceResolver.Resolve<INestedNamedScoped>(ServiceKey.Array);
        var p3 = ServiceResolver.Resolve<INestedNamedTransient>(ServiceKey.Array);
        var p4 = ServiceResolver.Services.tsr_di.test.INestedNamedSingleton_Array;
        var p5 = ServiceResolver.Services.tsr_di.test.INestedNamedScoped_Array;
        var p6 = ServiceResolver.Services.tsr_di.test.INestedNamedTransient_Array;
        var ap1 = p1 as NestedNoNamedSingletonArray;
        var ap2 = p2 as NestedNoNamedScopedArray;
        var ap3 = p3 as NestedNoNamedTransientArray;
        var ap4 = p4 as NestedNoNamedSingletonArray;
        var ap5 = p5 as NestedNoNamedScopedArray;
        var ap6 = p6 as NestedNoNamedTransientArray;

        Assert.IsNotNull(ap1);
        Assert.IsNotNull(ap2);
        Assert.IsNotNull(ap3);
        Assert.IsNotNull(ap4);
        Assert.IsNotNull(ap5);
        Assert.IsNotNull(ap6);

        Assert.IsTrue(ap1.S1.SequenceEqual(ap1.S2));
        Assert.IsTrue(ap1.S1.SequenceEqual(ap1.S3));

        Assert.IsTrue(ap2.S1.SequenceEqual(ap2.S2));
        Assert.IsFalse(ap2.S1.SequenceEqual(ap2.S3));

        Assert.IsFalse(ap3.S1.SequenceEqual(ap3.S2));
        Assert.IsFalse(ap3.S1.SequenceEqual(ap3.S3));
        Assert.IsFalse(ap3.S2.SequenceEqual(ap3.S3));

        Assert.IsTrue(ap4.S1.SequenceEqual(ap4.S2));
        Assert.IsTrue(ap4.S1.SequenceEqual(ap4.S3));
        Assert.IsTrue(ap1.S1.SequenceEqual(ap4.S1));

        Assert.IsTrue(ap5.S1.SequenceEqual(ap5.S2));
        Assert.IsFalse(ap5.S1.SequenceEqual(ap5.S3));

        Assert.IsFalse(ap6.S1.SequenceEqual(ap6.S2));
        Assert.IsFalse(ap6.S1.SequenceEqual(ap6.S3));
        Assert.IsFalse(ap6.S2.SequenceEqual(ap6.S3));

    }
}
