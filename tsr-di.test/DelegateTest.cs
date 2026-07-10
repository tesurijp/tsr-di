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

        var f11 = ServiceResolver.Services.IGetClassNameNonRegisterd;
        var f12 = ServiceResolver.Services.IGetClassName1_NonRegister;
        var f13 = ServiceResolver.Services.IGetParentNameNonRegister;
        var f14 = ServiceResolver.Services.IGetParentName_NonRegister;
        var f15 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_NonRegister;
        var f16 = ServiceResolver.Services.ILastResult_NonRegister;
        var f17 = ServiceResolver.Services.ILastResult2_NonRegister2;
        var f18 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_NonRegister;
        var f19 = ServiceResolver.Services.ILastResult_NonRegister;
        var f15o = f15.Bind(1);
        var f18o = f18.Bind(1, 2);

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

        Assert.AreEqual(StrResult, f11());
        Assert.AreEqual(StrResult, f12());
        Assert.AreEqual(StrResult, f13());
        Assert.AreEqual(StrResult, f14());
        //Assert.AreEqual(0, f16()); //  f1-f9 のテストで更新されているので、f16() は 0 ではない
        f15o(1, 2);
        Assert.AreEqual((1+1+2)*2, f17());
        Assert.AreEqual(2+3+3, f18(2,3,3));
        Assert.AreEqual(1+2+8, f18o(8));
        Assert.AreEqual(1+1+2, f19());
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

        var f11 = ServiceResolver.Services.IGetClassNameSingletond;
        var f12 = ServiceResolver.Services.IGetClassName1_Singleton;
        var f13 = ServiceResolver.Services.IGetParentNameSingleton;
        var f14 = ServiceResolver.Services.IGetParentName_Singleton;
        var f15 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Singleton;
        var f16 = ServiceResolver.Services.ILastResult_Singleton;
        var f17 = ServiceResolver.Services.ILastResult2_Singleton2;
        var f18 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Singleton;
        var f19 = ServiceResolver.Services.ILastResult_Singleton;
        var f15o = f15.Bind(1);
        var f18o = f18.Bind(1, 2);

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

        Assert.AreEqual(StrResult, f11());
        Assert.AreEqual(StrResult, f12());
        Assert.AreEqual(StrResult, f13());
        Assert.AreEqual(StrResult, f14());
        //Assert.AreEqual(0, f16()); //  f1-f9 のテストで更新されているので、f16() は 0 ではない
        f15o(1, 2);
        Assert.AreEqual((1+1+2-2)*2, f17());
        Assert.AreEqual(2+3+3-1, f18(2,3,3));
        Assert.AreEqual(1+2+8-1, f18o(8));
        Assert.AreEqual(1+1+2-2, f19());
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
        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);

        var f11 = ServiceResolver.Services.IGetClassNameTransientd;
        var f12 = ServiceResolver.Services.IGetClassName1_Transient;
        var f13 = ServiceResolver.Services.IGetParentNameTransient;
        var f14 = ServiceResolver.Services.IGetParentName_Transient;
        var f15 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Transient;
        var f16 = ServiceResolver.Services.ILastResult_Transient;
        var f17 = ServiceResolver.Services.ILastResult2_Transient2;
        var f18 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Transient;
        var f19 = ServiceResolver.Services.ILastResult_Transient;
        var f15o = f15.Bind(1);
        var f18o = f18.Bind(1, 2);

        var (f20, f21) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult>(ServiceKey.Transient, ServiceKey.Transient);
        var (f22, f23, f24) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult, ILastResult2>(ServiceKey.Transient, ServiceKey.Transient, ServiceKey.Transient2);

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

        Assert.AreEqual(StrResult, f11());
        Assert.AreEqual(StrResult, f12());
        Assert.AreEqual(StrResult, f13());
        Assert.AreEqual(StrResult, f14());
        Assert.AreEqual(0, f16());
        f15o(1, 2);
        Assert.AreEqual(0, f17());
        Assert.AreEqual(2+3+3-1, f18(2,3,3));
        Assert.AreEqual(1+2+8-1, f18o(8));
        Assert.AreEqual(0, f19());

        f20(4, 5, 6);
        Assert.AreEqual(0, f21());
        Assert.AreEqual(0, f7());
        f22(5, 6, 7);
        Assert.AreEqual(0, f23());
        Assert.AreEqual(0, f24());
        Assert.AreEqual(0, f21());
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
        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);

        var f11 = ServiceResolver.Services.IGetClassNameScoped;
        var f12 = ServiceResolver.Services.IGetClassName1_Scoped;
        var f13 = ServiceResolver.Services.IGetParentNameScoped;
        var f14 = ServiceResolver.Services.IGetParentName_Scoped;
        var f15 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Scoped;
        var f16 = ServiceResolver.Services.ILastResult_Scoped;
        var f17 = ServiceResolver.Services.ILastResult2_Scoped2;
        var f18 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Scoped;
        var f19 = ServiceResolver.Services.ILastResult_Scoped;
        var f15o = f15.Bind(1);
        var f18o = f18.Bind(1, 2);

        var (f20, f21) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult>(ServiceKey.Scoped, ServiceKey.Scoped);
        var (f22, f23, f24) = ServiceResolver.Resolve<PreDefineVoidFunction, ILastResult, ILastResult2>(ServiceKey.Scoped, ServiceKey.Scoped, ServiceKey.Scoped2);

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

        Assert.AreEqual(StrResult, f11());
        Assert.AreEqual(StrResult, f12());
        Assert.AreEqual(StrResult, f13());
        Assert.AreEqual(StrResult, f14());
        Assert.AreEqual(0, f16());
        f15o(1, 2);
        Assert.AreEqual(0, f17());
        Assert.AreEqual(2+3+3-1, f18(2,3,3));
        Assert.AreEqual(1+2+8-1, f18o(8));
        Assert.AreEqual(0, f19());

        f20(4, 5, 6);
        Assert.AreEqual(4+5+6-2, f21());
        Assert.AreEqual(0, f7());
        f22(5, 6, 7);
        Assert.AreEqual(5+6+7-2, f23());
        Assert.AreEqual((5+6+7-2)*2, f24());
        Assert.AreEqual(4+5+6-2, f21());
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
