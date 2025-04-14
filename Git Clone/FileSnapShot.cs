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
        public INode? Parent { get; set; }
    }
    public class FileNode : INode
    {
        public string Name { get; set; }
        public INode? Parent { get; set; }

        public FileNode(string folderName) { Name = folderName; }
    }
    public class FolderNode : INode
    {
        public string Name { get; set; }
        public INode? Parent { get; set; }
        public Dictionary<string, INode> Children { get; set; } = new();
        public FolderNode(string folderName) { Name = folderName; }
    }

    public class TreeBuilder
    {
        public void AddNode(string filepath, INode node, FolderNode parentNode)
        {
            if (parentNode.Children.ContainsKey(filepath))
            {
                Console.WriteLine($"A node with path {filepath} already exists in {parentNode.Name}.");
                return;
            }
            parentNode.Children.Add(filepath, node);
            node.Parent = parentNode;
        }
        public void RemoveNode(string filePath, INode node, FolderNode parentNode)
        {
            if (!parentNode.Children.ContainsKey(filePath))
            {
                Console.WriteLine($"The inputted node does not exist in {parentNode.Name}.");
                return;
            }

            parentNode.Children.Remove(filePath);
            node.Parent = null;
        }
    }
    public class TreeTester
    {
        public TreeBuilder TreeBuilder { get; set; } = new();

        public FolderNode Root = new("Folder1");
        public FolderNode Branch = new("Folder2");
        public FileNode Node = new("File");
        public void Begin()
        {
            TreeBuilder.AddNode("File1", Node, ref Root);
            TreeBuilder.AddNode("File2", Node, ref Root);
            TreeBuilder.AddNode("File2", Node, ref Branch);

            //TreeBuilder.DisplayTree();
        }
    }
}