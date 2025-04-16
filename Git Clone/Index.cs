namespace Git_Clone
{
    using Microsoft.VisualBasic;
    using my.utils;
    using static my.utils.Diff;

    /// <summary>
    /// Also known as the staging area. Represents the state of the files that are staged for commit.
    /// </summary>
    public class Index : BranchState
    {
        public List<FileSnapShot> CurrentFileSnapShots { get; set; } = new List<FileSnapShot>();

        public Repository Repository { get; set; }

        public WorkingDirectory WorkingDirectory { get; set; } = new WorkingDirectory();

        public Index(Repository repository)
        {
            Repository = repository;
        }

        public void TrackChanges()
        {
            // Get all the files from the working directory.
            string[] files = Directory.GetFiles(WorkingDirectory.RepositoryDirectory, "*.*", SearchOption.AllDirectories);

            // Obtain the HEAD commit tree from your repository's current branch.
            Tree headCommitTree = Repository.BranchManager.GetCurrentBranch().GetHead()?.CommitTree;

            if (headCommitTree == null)
                return;

            Diff diff = new Diff();

            foreach (string file in files)
            {
                // Read the new content from the working directory file.
                string newContent = File.ReadAllText(file);

                // Retrieve the corresponding HEAD content.
                // This assumes you have a mechanism to locate the FileNode in the commit tree.
                // For example, if your commit tree maintains a map of file paths to FileNodes,
                // you would do something like:
                string relativePath = Path.GetRelativePath(WorkingDirectory.RepositoryDirectory, file);

                if (!headCommitTree.TryGetFileNode(file, out FileNode headFileNode))
                {
                    Console.WriteLine($"File {relativePath} is new (not present in HEAD).");
                    continue;
                }

                string headContent = headFileNode.Content;  // Assuming FileNode has a 'Content' property.



                FindChanges(diff, newContent, headContent);


                /*
                // Optionally, store a new FileSnapShot if needed.
                string fileName = Path.GetFileName(file);
                FileSnapShot fileSnapShot = new FileSnapShot(fileName, newContent);
                CurrentFileSnapShots.Add(fileSnapShot);

                Console.WriteLine($"Looking up in tree: {relativePath}");*/
            }
        }

        private static void FindChanges(Diff diff, string newContent, string headContent)
        {
            string[] linesA = headContent.Replace("\r", "").Split('\n');
            string[] linesB = newContent.Replace("\r", "").Split('\n');

            Item[] lines = diff.DiffText(headContent, newContent);

            bool hasChange = lines.Length > 0;
            if(!hasChange)  Console.WriteLine($"Has change: {hasChange}");

            foreach (var line in lines)
            {
                if (line.deletedA > 0)
                {
                    Console.WriteLine("Deleted lines:");
                    for (int i = 0; i < line.deletedA; i++)
                    {
                        Console.WriteLine($"- {linesA[line.StartA + i]}");
                    }
                }

                if (line.insertedB > 0)
                {
                    Console.WriteLine("Inserted lines:");
                    for (int i = 0; i < line.insertedB; i++)
                    {
                        Console.WriteLine($"+ {linesB[line.StartB + i]}");
                    }
                }
            }
        }
    }
}