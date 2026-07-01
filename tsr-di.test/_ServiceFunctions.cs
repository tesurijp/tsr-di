namespace tsr_di.test;

public interface IFunctionPlaceHolder;

public delegate void PreDefineVoidFunction(int x, int y, int z);
public delegate int PreDefineIntFunction(int x,int y, int z);

public class NonRegisterdClass
{
    private static string GetClassName() => "NonRegisterClass";

    [ServiceFunction]
    public static string GetClassNameNonRegisterd() => GetClassName();

    [ServiceFunction(Name="NonRegister")]
    public static string GetClassName1() => GetClassName();

    [ServiceFunction(ServiceName="GetParentNameNonRegister")]
    public static string GetClassName2() => GetClassName();

    [ServiceFunction(ServiceName="GetParentName", Name="NonRegister")]
    public static string GetClassName3() => GetClassName();

    private static int lastResult = 0;
    [ServiceFunction(ServiceType=typeof(PreDefineVoidFunction), Name="NonRegister")]
    public static void Func1(int x, int y, int z) => lastResult = x+y+z;

    [ServiceFunction(Name="NonRegister")]
    public static int LastResult() => lastResult;

    [ServiceFunction(Name="NonRegister2")]
    public static int LastResult2() => lastResult * 2;

    [ServiceFunction(ServiceType=typeof(PreDefineIntFunction), Name="NonRegister")]
    public static int Func2(int x, int y, int z) => x+y+z;

}
[ServiceClass(LifeTime=LifeTime.Singleton, Name ="Singleton")]
public class RegisterdClassSingleton : IFunctionPlaceHolder
{
    private string GetClassName() => "SingletonClass";

    [ServiceFunction]
    public string GetClassNameSingletond() => GetClassName();

    [ServiceFunction(Name="Singleton")]
    public string GetClassName1() => GetClassName();

    [ServiceFunction(ServiceName="GetParentNameSingleton")]
    public string GetClassName2() => GetClassName();

    [ServiceFunction(ServiceName="GetParentName", Name="Singleton")]
    public string GetClassName3() => GetClassName();

    private int lastResult = 0;
    [ServiceFunction(ServiceType=typeof(PreDefineVoidFunction), Name="Singleton")]
    public void Func1(int x, int y, int z) => lastResult = x+y+z-2;

    [ServiceFunction(Name="Singleton")]
    public int LastResult() => lastResult;

    [ServiceFunction(Name="Singleton2")]
    public int LastResult2() => lastResult * 2;

    [ServiceFunction(ServiceType=typeof(PreDefineIntFunction), Name="Singleton")]
    public int Func2(int x, int y, int z) => x+y+z-1;

}
[ServiceClass(LifeTime=LifeTime.Transient,Name ="Transient")]
public class RegisterdClassTransient : IFunctionPlaceHolder
{
    private string GetClassName() => "TransientClass";

    [ServiceFunction]
    public string GetClassNameTransientd() => GetClassName();

    [ServiceFunction(Name="Transient")]
    public string GetClassName1() => GetClassName();

    [ServiceFunction(ServiceName="GetParentNameTransient")]
    public string GetClassName2() => GetClassName();

    [ServiceFunction(ServiceName="GetParentName", Name="Transient")]
    public string GetClassName3() => GetClassName();

    private int lastResult = 0;
    [ServiceFunction(ServiceType=typeof(PreDefineVoidFunction), Name="Transient")]
    public void Func1(int x, int y, int z) => lastResult = x+y+z-2;

    [ServiceFunction(Name="Transient")]
    public int LastResult() => lastResult;

    [ServiceFunction(Name="Transient2")]
    public int LastResult2() => lastResult * 2;

    [ServiceFunction(ServiceType=typeof(PreDefineIntFunction), Name="Transient")]
    public int Func2(int x, int y, int z) => x+y+z-1;
}
[ServiceClass(LifeTime=LifeTime.Scoped, Name ="Scoped")]
public class RegisterdClassScoped : IFunctionPlaceHolder
{
    private string GetClassName() => "ScopedClass";

    [ServiceFunction]
    public string GetClassNameScoped() => GetClassName();

    [ServiceFunction(Name="Scoped")]
    public string GetClassName1() => GetClassName();

    [ServiceFunction(ServiceName="GetParentNameScoped")]
    public string GetClassName2() => GetClassName();

    [ServiceFunction(ServiceName="GetParentName", Name="Scoped")]
    public string GetClassName3() => GetClassName();

    private int lastResult = 0;
    [ServiceFunction(ServiceType=typeof(PreDefineVoidFunction), Name="Scoped")]
    public void Func1(int x, int y, int z) => lastResult = x+y+z-2;

    [ServiceFunction(Name="Scoped")]
    public int LastResult() => lastResult;

    [ServiceFunction(Name="Scoped2")]
    public int LastResult2() => lastResult * 2;

    [ServiceFunction(ServiceType=typeof(PreDefineIntFunction), Name="Scoped")]
    public int Func2(int x, int y, int z) => x+y+z-1;

}
