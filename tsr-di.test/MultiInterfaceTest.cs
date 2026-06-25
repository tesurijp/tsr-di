using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using tsr_di.test.AutoDefined;

namespace tsr_di.test;

[TestClass]
public sealed class MultiInterfaceTest
{
    [TestMethod]
    public void SharedDefault()
    {
        var s1 = ServiceResolver.Resolve<IMultiInterface1>(ServiceKey.Def);
        var s2 = ServiceResolver.Resolve<IMultiInterface2>(ServiceKey.Def);

        var as1 = s1 as MultiInterfacDef;
        var as2 = s2 as MultiInterfacDef;
        Assert.IsNotNull(as1);
        Assert.IsNotNull(as2);
        Assert.AreSame(as1, as2);
    }

    [TestMethod]
    public void Shared()
    {
        var s1 = ServiceResolver.Resolve<IMultiInterface1>(ServiceKey.Shared);
        var s2 = ServiceResolver.Resolve<IMultiInterface2>(ServiceKey.Shared);

        var as1 = s1 as MultiInterfaceShared;
        var as2 = s2 as MultiInterfaceShared;
        Assert.IsNotNull(as1);
        Assert.IsNotNull(as2);
        Assert.AreSame(as1, as2);
    }

    [TestMethod]
    public void Isolated()
    {
        var s1 = ServiceResolver.Resolve<IMultiInterface1>(ServiceKey.Isolate);
        var s2 = ServiceResolver.Resolve<IMultiInterface2>(ServiceKey.Isolate);

        var as1 = s1 as MultiInterfaceIsolate;
        var as2 = s2 as MultiInterfaceIsolate;
        Assert.IsNotNull(as1);
        Assert.IsNotNull(as2);
        Assert.AreNotSame(as1, as2);
    }

    [TestMethod]
    public void ResolveAll()
    {
        var a1 = ServiceResolver.ResolveAll<IMultiInterface1>();
        var a2 = ServiceResolver.ResolveAll<IMultiInterface2>();

        var a1d = a1.OfType<MultiInterfacDef>().FirstOrDefault();
        var a1s = a1.OfType<MultiInterfaceShared>().FirstOrDefault();
        var a1i = a1.OfType<MultiInterfaceIsolate>().FirstOrDefault();
        var a2d = a2.OfType<MultiInterfacDef>().FirstOrDefault();
        var a2s = a2.OfType<MultiInterfaceShared>().FirstOrDefault();
        var a2i = a2.OfType<MultiInterfaceIsolate>().FirstOrDefault();

        Assert.IsNotNull(a1d);
        Assert.IsNotNull(a1s);
        Assert.IsNotNull(a1i);
        Assert.IsNotNull(a2d);
        Assert.IsNotNull(a2s);
        Assert.IsNotNull(a2i);
        Assert.AreSame(a1d, a2d);
        Assert.AreSame(a1s, a2s);
        Assert.AreNotSame(a1i, a2i);
    }
}

