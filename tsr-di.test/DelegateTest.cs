using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using tsr_di.test.AutoDefined;

namespace tsr_di.test;

[TestClass]
public sealed class DelegateTest
{
    [TestMethod]
    public void NonRegisterdClassFunctions()
    {
        var f1 = ServiceResolver.Resolve<IGetParentName>();
        var f2 = ServiceResolver.Resolve<PreDefineIntFunction>();
        var f3 = ServiceResolver.Resolve<PreDefineVoidFunction>();
        var f4 = ServiceResolver.Resolve<ILastResult>();
        var f2b = f2.Bind(10);
        var f3b = f3.Bind(10, 11);
        Assert.AreEqual("NonRegisterdClass static", f1());
        Assert.AreEqual((1 + 2 + 3).ToString(), f2(1, 2, 3));
        Assert.AreEqual((10 + 2 + 3).ToString(), f2b(2, 3));
        f3(3, 4, 5);
        Assert.AreEqual((3 + 4 + 5), f4());
        f3b(3);
        Assert.AreEqual((10 + 11 + 3), f4());
    }
}
