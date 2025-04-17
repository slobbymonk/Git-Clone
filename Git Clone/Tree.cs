using System.Xml.Linq;

namespace Git_Clone
{
    public class Tree
    {
        public INode RootNode { get; set; }
        public Dictionary<string, INode> Nodes { get; set; } = new();

        public Tree(INode rootNode)
        {
            RootNode = rootNode;
        }

        public void AddNode(string filepath, INode node, FolderNode parentNode)
        {
            if(!Nodes.ContainsKey(filepath))
            {
                Console.WriteLine($"A node with path {filepath} already exists in the tree.");
                Nodes.Add(filepath, node);
            }

            if (!parentNode.Children.ContainsKey(filepath))
            {
                parentNode.Children.Add(filepath, node);
            }
            node.Parent = parentNode;
        }
        public void RemoveNode(string filePath, INode node, FolderNode parentNode)
        {
            if (!parentNode.Children.ContainsKey(filePath))
            {
                Console.WriteLine($"The inputted node does not exist in {parentNode.Name}.");
                return;
            }

            Nodes.Remove(filePath);
            parentNode.Children.Remove(filePath);
            node.Parent = null;
        }
        public bool TryGetFileNode(string filePath, out FileNode headFileNode)
        {
            if (Nodes.ContainsKey(filePath))
            {
                
                if (Nodes[filePath] is FileNode fileNode)
                {
                    headFileNode = fileNode;
                    return true;
                }
                else
                {
                    Console.WriteLine($"The node at {filePath} is not a file.");
                    headFileNode = null;
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"No node found at {filePath}.");
                headFileNode = null;
                return false;
            }
        }
    }
}