using System.Text;
using System.Text.Json;

namespace Lab2.Tools.Tools;

public class PageTokenSerializer
{
    private readonly byte[] _saltLeft;
    private readonly byte[] _saltRight;

    public PageTokenSerializer()
    {
        var guid = Guid.NewGuid();
        byte[] bytes = guid.ToByteArray();

        int pivot = Random.Shared.Next(1, bytes.Length - 1);

        _saltLeft = bytes[..pivot];
        _saltRight = bytes[pivot..];
    }

    public T? TryDeserialize<T>(string? value, T defaultValue)
    {
        if (value is null)
            return defaultValue;

        try
        {
            byte[] bytes = Convert.FromBase64String(value);
            bytes = bytes[_saltLeft.Length..^_saltRight.Length];

            value = Encoding.Default.GetString(bytes);

            return JsonSerializer.Deserialize<T>(value);
        }
        catch
        {
            return default;
        }
    }

    public string Serialize<T>(T value)
    {
        string serialized = JsonSerializer.Serialize(value);

        byte[] bytes = Encoding.Default.GetBytes(serialized);
        bytes = [.._saltLeft, ..bytes, .._saltRight];

        return Convert.ToBase64String(bytes);
    }
}