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
        const string StrResult = "NonRegisterClass";
        var f1 = ServiceResolver.Resolve<IGetClassNameNonRegisterd>();
        var f2 = ServiceResolver.Resolve<IGetClassName1>(ServiceKey.NonRegister);
        var f3 = ServiceResolver.Resolve<IGetParentNameNonRegister>();
        var f4 = ServiceResolver.Resolve<IGetParentName>(ServiceKey.NonRegister);
        var f5 = ServiceResolver.Resolve<PreDefineVoidFunction>(ServiceKey.NonRegister);
        var f6 = ServiceResolver.Resolve<ILastResult>(ServiceKey.NonRegister);
        var f7 = ServiceResolver.Resolve<ILastResult2>(ServiceKey.NonRegister2);
        var f8 = ServiceResolver.Resolve<PreDefineIntFunction>(ServiceKey.NonRegister);
        var f9 = ServiceResolver.Resolve<ILastResult>(ServiceKey.NonRegister);
        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);
        Assert.AreEqual(StrResult, f1());
        Assert.AreEqual(StrResult, f2());
        Assert.AreEqual(StrResult, f3());
        Assert.AreEqual(StrResult, f4());
        Assert.AreEqual(0, f6());
        f5o(1, 2);
        Assert.AreEqual((1+1+2)*2, f7());
        Assert.AreEqual(2+3+3, f8(2,3,3));
        Assert.AreEqual(1+2+8, f8o(8));
        Assert.AreEqual(1+1+2, f9());
    }

    [TestMethod]
    public void SingletonClassFunctions()
    {
        const string StrResult = "SingletonClass";
        var f1 = ServiceResolver.Resolve<IGetClassNameSingletond>();
        var f2 = ServiceResolver.Resolve<IGetClassName1>(ServiceKey.Singleton);
        var f3 = ServiceResolver.Resolve<IGetParentNameSingleton>();
        var f4 = ServiceResolver.Resolve<IGetParentName>(ServiceKey.Singleton);
        var f5 = ServiceResolver.Resolve<PreDefineVoidFunction>(ServiceKey.Singleton);
        var f6 = ServiceResolver.Resolve<ILastResult>(ServiceKey.Singleton);
        var f7 = ServiceResolver.Resolve<ILastResult2>(ServiceKey.Singleton2);
        var f8 = ServiceResolver.Resolve<PreDefineIntFunction>(ServiceKey.Singleton);
        var f9 = ServiceResolver.Resolve<ILastResult>(ServiceKey.Singleton);
        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);
        Assert.AreEqual(StrResult, f1());
        Assert.AreEqual(StrResult, f2());
        Assert.AreEqual(StrResult, f3());
        Assert.AreEqual(StrResult, f4());
        Assert.AreEqual(0, f6());
        f5o(1, 2);
        Assert.AreEqual((1+1+2-2)*2, f7());
        Assert.AreEqual(2+3+3-1, f8(2,3,3));
        Assert.AreEqual(1+2+8-1, f8o(8));
        Assert.AreEqual(1+1+2-2, f9());
    }

    [TestMethod]
    public void TransientClassFunctions()
    {
        const string StrResult = "TransientClass";
        var f1 = ServiceResolver.Resolve<IGetClassNameTransientd>();
        var f2 = ServiceResolver.Resolve<IGetClassName1>(ServiceKey.Transient);
        var f3 = ServiceResolver.Resolve<IGetParentNameTransient>();
        var f4 = ServiceResolver.Resolve<IGetParentName>(ServiceKey.Transient);
        var f5 = ServiceResolver.Resolve<PreDefineVoidFunction>(ServiceKey.Transient);
        var f6 = ServiceResolver.Resolve<ILastResult>(ServiceKey.Transient);
        var f7 = ServiceResolver.Resolve<ILastResult2>(ServiceKey.Transient2);
        var f8 = ServiceResolver.Resolve<PreDefineIntFunction>(ServiceKey.Transient);
        var f9 = ServiceResolver.Resolve<ILastResult>(ServiceKey.Transient);

        var (f10, f11) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult>(ServiceKey.Transient, ServiceKey.Transient);
        var (f12, f13, f14) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult, ILastResult2>(ServiceKey.Transient, ServiceKey.Transient, ServiceKey.Transient2);

        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);
        Assert.AreEqual(StrResult, f1());
        Assert.AreEqual(StrResult, f2());
        Assert.AreEqual(StrResult, f3());
        Assert.AreEqual(StrResult, f4());
        Assert.AreEqual(0, f6());
        f5o(1, 2);
        Assert.AreEqual(0, f7());
        Assert.AreEqual(2+3+3-1, f8(2,3,3));
        Assert.AreEqual(1+2+8-1, f8o(8));
        Assert.AreEqual(0, f9());

        f10(4, 5, 6);
        Assert.AreEqual(0, f11());
        Assert.AreEqual(0, f7());
        f12(5, 6, 7);
        Assert.AreEqual(0, f13());
        Assert.AreEqual(0, f14());
        Assert.AreEqual(0, f11());
        Assert.AreEqual(0, f7());
    }

    [TestMethod]
    public void ScopedClassFunctions()
    {
        const string StrResult = "ScopedClass";
        var f1 = ServiceResolver.Resolve<IGetClassNameScoped>();
        var f2 = ServiceResolver.Resolve<IGetClassName1>(ServiceKey.Scoped);
        var f3 = ServiceResolver.Resolve<IGetParentNameScoped>();
        var f4 = ServiceResolver.Resolve<IGetParentName>(ServiceKey.Scoped);
        var f5 = ServiceResolver.Resolve<PreDefineVoidFunction>(ServiceKey.Scoped);
        var f6 = ServiceResolver.Resolve<ILastResult>(ServiceKey.Scoped);
        var f7 = ServiceResolver.Resolve<ILastResult2>(ServiceKey.Scoped2);
        var f8 = ServiceResolver.Resolve<PreDefineIntFunction>(ServiceKey.Scoped);
        var f9 = ServiceResolver.Resolve<ILastResult>(ServiceKey.Scoped);

        var (f10, f11) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult>(ServiceKey.Scoped, ServiceKey.Scoped);
        var (f12, f13, f14) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult, ILastResult2>(ServiceKey.Scoped, ServiceKey.Scoped, ServiceKey.Scoped2);

        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);
        Assert.AreEqual(StrResult, f1());
        Assert.AreEqual(StrResult, f2());
        Assert.AreEqual(StrResult, f3());
        Assert.AreEqual(StrResult, f4());
        Assert.AreEqual(0, f6());
        f5o(1, 2);
        Assert.AreEqual(0, f7());
        Assert.AreEqual(2+3+3-1, f8(2,3,3));
        Assert.AreEqual(1+2+8-1, f8o(8));
        Assert.AreEqual(0, f9());
        f10(4, 5, 6);
        Assert.AreEqual(4+5+6-2, f11());
        Assert.AreEqual(0, f7());
        f12(5, 6, 7);
        Assert.AreEqual(5+6+7-2, f13());
        Assert.AreEqual((5+6+7-2)*2, f14());
        Assert.AreEqual(4+5+6-2, f11());
        Assert.AreEqual(0, f7());
    }

    [TestMethod]
    public void ComplexFunction()
    {
        var (f1, f2) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult>(ServiceKey.Scoped, ServiceKey.Transient);
        var (f3, f4, f5) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult, ILastResult2>(ServiceKey.Scoped, ServiceKey.Transient, ServiceKey.Scoped2);
        f1(4, 5, 6);
        Assert.AreEqual(0, f2());
        f3(5, 6, 7);
        Assert.AreEqual(0, f4());
        Assert.AreEqual((5+6+7-2)*2, f5());
        Assert.AreEqual(0, f2());
    }
}
