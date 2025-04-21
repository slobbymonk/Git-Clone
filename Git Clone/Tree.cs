using Newtonsoft.Json;

namespace Git_Clone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tree
    {
        [JsonProperty] public FolderNode RootNode { get; set; }
        [JsonProperty] public Dictionary<string, INode> Nodes { get; set; } = new();

        public Tree(FolderNode rootNode)
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