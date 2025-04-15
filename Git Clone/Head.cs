namespace Git_Clone
{
    public class Head : BranchState
    {
        private Commit _latestCommit;
        public Commit LatestCommit { 
            get 
            { 
                return _latestCommit; 
            }
            set
            {
                _latestCommit = value;
            }
        }

        public Tree GetHeadFileTree()
        {
            if (LatestCommit == null)
            {
                Console.WriteLine("No commit found.");
                return null;
            }
            return LatestCommit.CommitTree;
        }
    }
}