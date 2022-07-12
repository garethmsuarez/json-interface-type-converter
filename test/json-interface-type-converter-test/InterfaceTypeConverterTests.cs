using System.Text.Json;

namespace Somerled.Json;

public class InterfaceTypeConverterTests
{
    [Fact]
    public void JsonSerializationOfTestObject_returns_json_string()
    {
        var testObject = BuildTestObject();

        var json = JsonSerializer.Serialize(testObject, new JsonSerializerOptions {
            Converters = { new InterfaceTypeConverter<IElement>() }
        });
        
        Assert.Equal(GetJsonString(), json);
    
    }

    [Fact]
    public void JsonDeserializationOfTestString_returns_concrete_objects()
    {
        var json = GetJsonString();

        var testObject = JsonSerializer.Deserialize<TestObject>(json, new JsonSerializerOptions {
            Converters = { new InterfaceTypeConverter<IElement>() }
        });

        Assert.NotNull(testObject);
        Assert.Equal(typeof(ElementOne), testObject?.Elements[0].GetType());
        Assert.Equal(typeof(ElementTwo), testObject?.Elements[1].GetType());
    }
     
    private TestObject BuildTestObject()
    {
        var testObject = new TestObject
        {
            Name = "Test"
        };

        var element1 = new ElementOne
        {
            Name = "ElementOne",
            Value = "1"
        };

        var element2 = new ElementTwo
        {
            Name = "ElementTwo",
            Value = "2",
            OtherValue = "3"
        };

        testObject.Elements.Add(element1);
        testObject.Elements.Add(element2);

        return testObject;
    }

    private static string GetJsonString()
    {
        return "{\"Name\":\"Test\",\"Elements\":[{\"$concreteType\":\"Somerled.Json.ElementOne\",\"Name\":\"ElementOne\",\"Value\":\"1\"},{\"$concreteType\":\"Somerled.Json.ElementTwo\",\"Name\":\"ElementTwo\",\"Value\":\"2\",\"OtherValue\":\"3\"}]}";
    }
}



public class TestObject
{
    public string Name { get; set; } = string.Empty;
    public IList<IElement> Elements { get; set; } = new List<IElement>();
}

public interface IElement
{
    string Name { get; set; }

}

public class ElementOne : IElement
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class ElementTwo : IElement
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string OtherValue { get; set; } = string.Empty;
}