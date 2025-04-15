namespace Git_Clone
{
    public class Branch
    {
        public string BranchName { get; set; }
        public string BranchTag { get; set; }
        public List<Commit> BranchCommits { get; set; } = new List<Commit>();

        private Head Head { get; set; } = new Head();

        public Branch(string branchName, string branchTag)
        {
            BranchName = branchName;
            BranchTag = branchTag;
        }

        public void AddCommit(Commit commit)
        {
            BranchCommits.Add(commit);
            Head.LatestCommit = commit;
        }
        public Commit GetHead()
        {
            if (Head == null)
            {
                Console.WriteLine("No HEAD found. Commit before being able to get the HEAD.");
                return null;
            }
            Console.WriteLine($"Latest commit returned: {Head.LatestCommit.Message}");
            return Head.LatestCommit;
        }
    }
    public class BranchState{ }
}