using Newtonsoft.Json;

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

        public void LoadAllSavedBranchCommits(WorkingDirectory workingDirectory)
        {
            foreach (var branch in Branches.Values)
            {
                // Define the path to the commit history folder for this branch
                string branchCommitHistoryPath = Path.Combine(workingDirectory.RepositorySavingFolder,
                    branch.BranchName + " Branch Commit History");

                // Check if the directory exists
                if (Directory.Exists(branchCommitHistoryPath))
                {
                    // List all JSON files in the directory
                    var commitFiles = Directory.GetFiles(branchCommitHistoryPath, "*.json");

                    // Sort the commit files based on filename or another order to get the correct sequence (optional)
                    var orderedCommitFiles = commitFiles.OrderBy(f => f).ToList(); // Sorting by filename (commit message)

                    foreach (var commitFile in orderedCommitFiles)
                    {
                        // Read the content of the commit JSON file
                        string jsonContent = File.ReadAllText(commitFile);

                        // Deserialize the JSON content to a commit tree
                        var settings = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All, // 🔥 key fix here
                            Formatting = Formatting.Indented,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        };

                        // Deserializing:
                        Tree commitTree = JsonConvert.DeserializeObject<Tree>(jsonContent, settings);

                        if (commitTree != null)
                        {
                            // Add the commit to the branch (you may want to include additional commit metadata)
                            branch.AddCommit(new Commit(commitFile, commitTree));
                        }
                        else
                        {
                            Console.WriteLine($"Failed to deserialize commit tree for file {commitFile}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"No commit history folder found for branch {branch.BranchName}");
                }
            }

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