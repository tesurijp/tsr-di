using System.IO;
internal static class TemplateReader
{
    private static readonly string TemplateFolder = $"{typeof(TemplateReader).Assembly.GetName().Name.Replace("-", "_")}.template";
    public static readonly string AttributeCS = GetStreamText($"{TemplateFolder}.Attributes.cs");
    public static readonly string ResolveMethodCS = GetStreamText($"{TemplateFolder}.ResolverMethod.cs");
    public static readonly string InnerResolverCS = GetStreamText($"{TemplateFolder}.InnerResolver.cs");
    public static readonly string FieldStoreCS = GetStreamText($"{TemplateFolder}.FieldStore.cs");
    public static readonly string DelegatesCS= GetStreamText($"{TemplateFolder}.Delegates.cs");

    public static readonly string ServiceTypeEnumCS = GetStreamText($"{TemplateFolder}.ServiceTypeEnum.cs");

    static string GetStreamText(string name)
    {
        using var st = typeof(TemplateReader).Assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(st);
        return reader.ReadToEnd();
    }
}
