using Git_Clone;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Git_Clone
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface INode
    {
        string Name { get; }
        FolderNode Parent { get; }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class FileNode : INode
    {
        public FileNode() { }
        [JsonProperty] public string Name { get; private set; }
        [JsonProperty]public string Content { get; private set; }
        [JsonProperty] public string LocalPath { get; private set; }
        [JsonIgnore] public FolderNode Parent { get; private set; }

        public FileNode(string folderName, string content, FolderNode parent, string localPath)
        { 
            Name = folderName; 
            Content = content;
            Parent = parent;
            LocalPath = localPath;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class FolderNode : INode
    {
        public FolderNode() { }
        [JsonProperty] public string Name { get; private set; }
        [JsonProperty(ItemConverterType = typeof(NodeConverter))] 
        public Dictionary<string, INode> Children { get; set; } = new();
        [JsonIgnore] public FolderNode Parent { get; private set; }
        private FolderNode(string folderName, FolderNode? parent)
        {
            Name = folderName;
            Parent = parent;
        }

        public static FolderNode Create(string name, FolderNode parent) 
            => new(name, parent);
        public static FolderNode CreateRoot(string name) 
            => new(name, null!) { Parent = null! };
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