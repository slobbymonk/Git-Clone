namespace Git_Clone
{
    public class Commit
    {
        public string Message { get; set; }
        public DateTime CommitDate { get; set; } = DateTime.Now;
        public Tree CommitTree { get; set; }

        public Commit(string commitMessage, Tree tree)
        {
            Message = commitMessage;
            CommitTree = tree;
        }

        public void DisplayTree(FolderNode folder, string indent = "")
        {
            Console.WriteLine($"{indent}{folder.Name}/");

            foreach (var kvp in folder.Children)
            {
                if (kvp.Value is FolderNode childFolder)
                {
                    DisplayTree(childFolder, indent + "    ");
                }
                else if (kvp.Value is FileNode file)
                {
                    Console.WriteLine($"{indent}    {file.Name}");
                }
            }
        }

    }
}