namespace Git_Clone
{
    public class Branch
    {
        public string BranchName { get; set; }
        public string BranchTag { get; set; }
        public List<Commit> BranchCommits { get; set; } = new List<Commit>();

        public Branch(string branchName, string branchTag)
        {
            BranchName = branchName;
            BranchTag = branchTag;
        }

        public void AddCommit(Commit commit)
        {
            BranchCommits.Add(commit);
        }
        public void GetHead()
        {
            if (BranchCommits.Count == 0)
            {
                Console.WriteLine("No commits found.");
                return;
            }
            Commit latestCommit = BranchCommits.Last();
            Console.WriteLine($"Latest commit: {latestCommit.Message}");
        }
    }
    public class BranchState
    {

    }
    public class Head : BranchState
    {
        // Pointer to latest commit
        public Commit LatestCommit;

        public List<FileSnapShot> GetLatestFileSnapShots()
        {
            if (LatestCommit == null)
            {
                Console.WriteLine("No commit found.");
                return new List<FileSnapShot>();
            }
            return LatestCommit.FileSnapShots;
        }
    }
    public class Staging : BranchState
    {
        public List<FileSnapShot> FileSnapShots { get; set; } = new List<FileSnapShot>();
    }
    public class WorkingDirectory : BranchState
    {
        public string RepositoryDirectory { get; set; } =
            "C:\\Users\\Séba\\source\\repos\\Git Clone\\Git Clone\\Repos\\TestRepository\\";
    }
}