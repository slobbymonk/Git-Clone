using Git_Clone;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Git_Clone
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface INode
    {
        string Name { get; set; }
        public INode? Parent { get; set; }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class FileNode : INode
    {
        [JsonProperty] public string Name { get; set; }
        [JsonProperty]public string Content { get; set; }
        [JsonIgnore] public INode? Parent { get; set; }

        public FileNode(string folderName, string content)
        { 
            Name = folderName; 
            Content = content;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class FolderNode : INode
    {
        [JsonProperty] public string Name { get; set; }
        [JsonProperty(ItemConverterType = typeof(NodeConverter))] public Dictionary<string, INode> Children { get; set; } = new();
        [JsonIgnore] public INode? Parent { get; set; }
        public FolderNode(string folderName)
        { 
            Name = folderName; 
        }
    }
}

public class NodeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(INode).IsAssignableFrom(objectType);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        if (obj["Content"] != null)
        {
            return obj.ToObject<FileNode>(serializer);
        }
        else if (obj["Children"] != null)
        {
            return obj.ToObject<FolderNode>(serializer);
        }

        throw new JsonSerializationException("Unknown INode type.");
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}