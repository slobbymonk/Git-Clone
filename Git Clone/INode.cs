namespace Git_Clone
{
    public interface INode
    {
        string Name { get; set; }
        public INode? Parent { get; set; }
    }
    public class FileNode : INode
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public INode? Parent { get; set; }

        public FileNode(string folderName, string content)
        { 
            Name = folderName; 
            Content = content;
        }
    }
    public class FolderNode : INode
    {
        public string Name { get; set; }
        public INode? Parent { get; set; }
        public Dictionary<string, INode> Children { get; set; } = new();
        public FolderNode(string folderName) { Name = folderName; }
    }
}