namespace Git_Clone
{
    public class FileSnapShot
    {
        Dictionary<string, string> fileSnapshot = new();
        
        public FileSnapShot(string fileName, string content)
        {
            fileSnapshot.Add(fileName, content);
        }
    }
    public interface INode
    {
        string Name { get; set; }
    }
    public class FileNode : INode
    {
        public string Name { get; set; }
        public FileNode(string fileName)
        {
            Name = fileName;
        }
    }
    public class FolderNode : INode
    {
        public string Name { get; set; }
        public Dictionary<string, INode> Children { get; set; } = new();

        public FolderNode(string folderName)
        {
            Name = folderName;
        }
    }

    public class TreeBuilder
    {
        public void AddFile(string fileName, object parentNode)
        {
            
        }
        public void DisplayTree()
        {
            
        }
    }
    public class TreeTester
    {
        public TreeBuilder TreeBuilder { get; set; } = new();

        public object Root = new object();
        public object Branch = new object();
        public void Begin()
        {
            TreeBuilder.AddFile("File1", Root);
            TreeBuilder.AddFile("File2", Root);
            TreeBuilder.AddFile("File2", Branch);

            TreeBuilder.DisplayTree();
        }
    }
}