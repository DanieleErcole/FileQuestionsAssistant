using System.Text.Json.Serialization;

namespace Core.Utils;

[method: JsonConstructor]
public class MemoryFile(string name, byte[] data) {
    public string Name { get; } = name;
    public byte[] Data { get; } = data;

    public override bool Equals(object? obj) => Equals(obj as MemoryFile);
    
    private bool Equals(MemoryFile? other) {
        if (other is null) return false;
        return ReferenceEquals(this, other) || Data.SequenceEqual(other.Data);
    }

    public override int GetHashCode() => HashCode.Combine(Data);
    
}