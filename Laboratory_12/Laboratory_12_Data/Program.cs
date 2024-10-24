namespace Laboratory_12_Data;

[Serializable]
public class Data
{
    public int InputA { get; set; }
    public int InputB { get; set; }
    public long? Result { get; set; }
    public string Content { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"['InputA': {InputA}, 'InputB': {InputB}, 'Result': {Result}, 'Content': {Content}]";
    }
}

public static class DataSerializer
{
    public static string Serialize(Data data)
    {
        return System.Text.Json.JsonSerializer.Serialize(data);
    }

    public static Data Deserialize(string dataString)
    {
        return System.Text.Json.JsonSerializer.Deserialize<Data>(dataString);
    }
}
