namespace tsr_di.test;

public interface IFunctionPlaceHolder;

public delegate void PreDefineVoidFunction(int x, int y, int z);
public delegate string PreDefineIntFunction(int x,int y, int z);

public class NonRegisterdClass
{
    [ServiceFunction]
    public static string GetParentName() => "NonRegisterdClass static";

    [ServiceFunction(ServiceType = typeof(PreDefineIntFunction))]
    public static string GetParentName(int x, int y, int z) => (x + y + z).ToString();

    static int lastResult = 0;
    [ServiceFunction(ServiceType = typeof(PreDefineVoidFunction))]
    public static void VoidFunc(int x, int y, int z) => lastResult = x + y + z;

    [ServiceFunction(ServiceName = "LastResult")]
    public static int VoidFuncResult() => lastResult;
}
