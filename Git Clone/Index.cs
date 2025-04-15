namespace Git_Clone
{
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
            string[] files = Directory.GetFiles(WorkingDirectory.RepositoryDirectory, "*.*", SearchOption.AllDirectories);

            Tree headCommitTree = Repository.BranchManager.GetCurrentBranch().GetHead().CommitTree;

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string content = File.ReadAllText(file);
                FileSnapShot fileSnapShot = new FileSnapShot(fileName, content);
                CurrentFileSnapShots.Add(fileSnapShot);
            }
        }
    }
}