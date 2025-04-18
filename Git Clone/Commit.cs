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
    }
}