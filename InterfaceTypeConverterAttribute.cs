namespace Somerled.Json;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class InterfaceTypeConverterAttribute : JsonConverterAttribute
{
    public InterfaceTypeConverterAttribute(Type converterType) : base(converterType)
    {
    }

}