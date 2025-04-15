namespace Git_Clone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //FolderNode root = new FolderNode("Root");
            //FolderNode srcFolder = new FolderNode("src");
            //FileNode mainFile = new FileNode("main.cs", "Content");
            //srcFolder.Children.Add(mainFile.Name, mainFile);
            //root.Children.Add(srcFolder.Name, srcFolder);
            //
            //// Create a commit that takes this tree snapshot:
            //Commit commit = new Commit("Initial commit", new Tree(root));
            //
            //// Display the commit tree:
            //commit.DisplayTree(root);


            GitInterface branchTester = new GitInterface();
            branchTester.Begin();
        }
    }
}