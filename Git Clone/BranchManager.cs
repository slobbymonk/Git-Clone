namespace Git_Clone
{
    public class BranchManager
    {
        public Dictionary<int, Branch> Branches { get; set; } = new Dictionary<int, Branch>();

        private const int startingBranchId = 1;
        private int _currentBranchIdIndex = startingBranchId;

        private int _currentBranchId;
        public int CurrentBranchId
        {
            get { return _currentBranchId; }
            set { _currentBranchId = value; }
        }

        public void Initialize()
        {
            CurrentBranchId = _currentBranchIdIndex;
            CreateBranch("main", "0.0.1");
        }

        public void CheckoutBranch(int branchId)
        {
            if (Branches.ContainsKey(branchId))
            {
                CurrentBranchId = branchId;
                Console.WriteLine($"Current branch set to {branchId}.");
            }
            else
            {
                Console.WriteLine($"Branch with ID {branchId} not found.");
            }
        }

        #region Branch Existance Functions

        public void CreateBranch(string branchName, string branchTag)
        {
            int branchId = GetBranchId();
            Branch newBranch = new Branch(branchName, branchTag);
            Branches.Add(branchId, newBranch);
            Console.WriteLine($"Branch {newBranch.BranchName} with tag {newBranch.BranchTag} created.");
        }
        public void DeleteBranch(int branchId)
        {
            if(branchId == CurrentBranchId)
            {
                Console.WriteLine($"You cannot delete the current branch {CurrentBranchId}.");
                return;
            }

            if (Branches.TryGetValue(branchId, out Branch branchToDelete))
            {
                Branches.Remove(branchId);
                Console.WriteLine($"Branch {branchId} deleted.");
            }
            else
            {
                Console.WriteLine($"Branch {branchId} not found.");
            }
        }
        public void CloneBranch(int branchId)
        {
            Branch branch = Branches[branchId];
            Branch newBranch = new Branch(branch.BranchName, branch.BranchTag);
            Branches.Add(GetBranchId(), newBranch);
            Console.WriteLine($"Branch {newBranch.BranchName} with tag {newBranch.BranchTag} created.");
        }

        #endregion

        #region Helper Functions

        public void ListAllBranches()
        {
            foreach (var branch in Branches)
            {
                Console.WriteLine($"Branch ID: {branch.Key}");
                Console.WriteLine($"Branch Name: {branch.Value.BranchName}");
                Console.WriteLine($"Branch Tag: {branch.Value.BranchTag}");
            }
        }
        public void ListAllBranchNames()
        {
            Console.WriteLine($"Here's all the existing branches:");
            foreach (var branch in Branches)
            {
                Console.WriteLine($"{branch.Key}; {branch.Value.BranchName}");
            }
        }

        #endregion

        #region Getter Functions

        public Branch GetBranchUsingID(int branchId)
        {
            if (Branches.ContainsKey(branchId))
            {
                return Branches[branchId];
            }
            else
            {
                Console.WriteLine($"Branch with ID {branchId} not found.");
                return null;
            }
        }

        public Branch GetCurrentBranch()
        {
            return GetBranchUsingID(CurrentBranchId);
        }

        #endregion

        public int GetBranchId()
        {
            return _currentBranchIdIndex++;
        }
    }
}