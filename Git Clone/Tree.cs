namespace Git_Clone
{
    public class Tree
    {
        public INode RootNode { get; set; }
        public Tree(INode rootNode)
        {
            RootNode = rootNode;
        }

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
}