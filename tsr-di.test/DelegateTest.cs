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
        var f1 = ServiceResolver.Resolve<GetClassNameNonRegisterdFunc>();
        var f2 = ServiceResolver.Resolve<GetClassName1Func>(ServiceKey.NonRegister);
        var f3 = ServiceResolver.Resolve<GetParentNameNonRegister>();
        var f4 = ServiceResolver.Resolve<GetParentName>(ServiceKey.NonRegister);
        var f5 = ServiceResolver.Resolve<PreDefineVoidFunction>(ServiceKey.NonRegister);
        var f6 = ServiceResolver.Resolve<LastResultFunc>(ServiceKey.NonRegister);
        var f7 = ServiceResolver.Resolve<LastResult2Func>(ServiceKey.NonRegister2);
        var f8 = ServiceResolver.Resolve<PreDefineIntFunction>(ServiceKey.NonRegister);
        var f9 = ServiceResolver.Resolve<LastResultFunc>(ServiceKey.NonRegister);
        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);

        var f11 = ServiceResolver.Services.GetClassNameNonRegisterdFunc;
        var f12 = ServiceResolver.Services.GetClassName1Func_NonRegister;
        var f13 = ServiceResolver.Services.GetParentNameNonRegister;
        var f14 = ServiceResolver.Services.GetParentName_NonRegister;
        var f15 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_NonRegister;
        var f16 = ServiceResolver.Services.LastResultFunc_NonRegister;
        var f17 = ServiceResolver.Services.LastResult2Func_NonRegister2;
        var f18 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_NonRegister;
        var f19 = ServiceResolver.Services.LastResultFunc_NonRegister;
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
        var f1 = ServiceResolver.Resolve<GetClassNameSingletondFunc>();
        var f2 = ServiceResolver.Resolve<GetClassName1Func>(ServiceKey.Singleton);
        var f3 = ServiceResolver.Resolve<GetParentNameSingleton>();
        var f4 = ServiceResolver.Resolve<GetParentName>(ServiceKey.Singleton);
        var f5 = ServiceResolver.Resolve<PreDefineVoidFunction>(ServiceKey.Singleton);
        var f6 = ServiceResolver.Resolve<LastResultFunc>(ServiceKey.Singleton);
        var f7 = ServiceResolver.Resolve<LastResult2Func>(ServiceKey.Singleton2);
        var f8 = ServiceResolver.Resolve<PreDefineIntFunction>(ServiceKey.Singleton);
        var f9 = ServiceResolver.Resolve<LastResultFunc>(ServiceKey.Singleton);
        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);

        var f11 = ServiceResolver.Services.GetClassNameSingletondFunc;
        var f12 = ServiceResolver.Services.GetClassName1Func_Singleton;
        var f13 = ServiceResolver.Services.GetParentNameSingleton;
        var f14 = ServiceResolver.Services.GetParentName_Singleton;
        var f15 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Singleton;
        var f16 = ServiceResolver.Services.LastResultFunc_Singleton;
        var f17 = ServiceResolver.Services.LastResult2Func_Singleton2;
        var f18 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Singleton;
        var f19 = ServiceResolver.Services.LastResultFunc_Singleton;
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
        var f1 = ServiceResolver.Resolve<GetClassNameTransientdFunc>();
        var f2 = ServiceResolver.Resolve<GetClassName1Func>(ServiceKey.Transient);
        var f3 = ServiceResolver.Resolve<GetParentNameTransient>();
        var f4 = ServiceResolver.Resolve<GetParentName>(ServiceKey.Transient);
        var f5 = ServiceResolver.Resolve<PreDefineVoidFunction>(ServiceKey.Transient);
        var f6 = ServiceResolver.Resolve<LastResultFunc>(ServiceKey.Transient);
        var f7 = ServiceResolver.Resolve<LastResult2Func>(ServiceKey.Transient2);
        var f8 = ServiceResolver.Resolve<PreDefineIntFunction>(ServiceKey.Transient);
        var f9 = ServiceResolver.Resolve<LastResultFunc>(ServiceKey.Transient);
        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);

        var f11 = ServiceResolver.Services.GetClassNameTransientdFunc;
        var f12 = ServiceResolver.Services.GetClassName1Func_Transient;
        var f13 = ServiceResolver.Services.GetParentNameTransient;
        var f14 = ServiceResolver.Services.GetParentName_Transient;
        var f15 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Transient;
        var f16 = ServiceResolver.Services.LastResultFunc_Transient;
        var f17 = ServiceResolver.Services.LastResult2Func_Transient2;
        var f18 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Transient;
        var f19 = ServiceResolver.Services.LastResultFunc_Transient;
        var f15o = f15.Bind(1);
        var f18o = f18.Bind(1, 2);

        var (f20, f21) = ServiceResolver.Resolve<PreDefineVoidFunction, LastResultFunc>(ServiceKey.Transient, ServiceKey.Transient);
        var (f22, f23, f24) = ServiceResolver.Resolve<PreDefineVoidFunction, LastResultFunc, LastResult2Func>(ServiceKey.Transient, ServiceKey.Transient, ServiceKey.Transient2);

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
        var f1 = ServiceResolver.Resolve<GetClassNameScopedFunc>();
        var f2 = ServiceResolver.Resolve<GetClassName1Func>(ServiceKey.Scoped);
        var f3 = ServiceResolver.Resolve<GetParentNameScoped>();
        var f4 = ServiceResolver.Resolve<GetParentName>(ServiceKey.Scoped);
        var f5 = ServiceResolver.Resolve<PreDefineVoidFunction>(ServiceKey.Scoped);
        var f6 = ServiceResolver.Resolve<LastResultFunc>(ServiceKey.Scoped);
        var f7 = ServiceResolver.Resolve<LastResult2Func>(ServiceKey.Scoped2);
        var f8 = ServiceResolver.Resolve<PreDefineIntFunction>(ServiceKey.Scoped);
        var f9 = ServiceResolver.Resolve<LastResultFunc>(ServiceKey.Scoped);
        var f5o = f5.Bind(1);
        var f8o = f8.Bind(1, 2);

        var f11 = ServiceResolver.Services.GetClassNameScopedFunc;
        var f12 = ServiceResolver.Services.GetClassName1Func_Scoped;
        var f13 = ServiceResolver.Services.GetParentNameScoped;
        var f14 = ServiceResolver.Services.GetParentName_Scoped;
        var f15 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Scoped;
        var f16 = ServiceResolver.Services.LastResultFunc_Scoped;
        var f17 = ServiceResolver.Services.LastResult2Func_Scoped2;
        var f18 = ServiceResolver.Services.tsr_di.test.PreDefineIntFunction_Scoped;
        var f19 = ServiceResolver.Services.LastResultFunc_Scoped;
        var f15o = f15.Bind(1);
        var f18o = f18.Bind(1, 2);

        var (f20, f21) = ServiceResolver.Resolve<PreDefineVoidFunction, LastResultFunc>(ServiceKey.Scoped, ServiceKey.Scoped);
        var (f22, f23, f24) = ServiceResolver.Resolve<PreDefineVoidFunction, LastResultFunc, LastResult2Func>(ServiceKey.Scoped, ServiceKey.Scoped, ServiceKey.Scoped2);

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
        var (f1, f2) = ServiceResolver.Resolve<PreDefineVoidFunction, LastResultFunc>(ServiceKey.Scoped, ServiceKey.Transient);
        var (f3, f4, f5) = ServiceResolver.Resolve<PreDefineVoidFunction, LastResultFunc, LastResult2Func>(ServiceKey.Scoped, ServiceKey.Transient, ServiceKey.Scoped2);
        f1(4, 5, 6);
        Assert.AreEqual(0, f2());
        f3(5, 6, 7);
        Assert.AreEqual(0, f4());
        Assert.AreEqual((5+6+7-2)*2, f5());
        Assert.AreEqual(0, f2());
    }
}
