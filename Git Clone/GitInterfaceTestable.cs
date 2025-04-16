namespace Git_Clone
{
    public class GitInterfaceTestable : GitInterface
    {
        public void TestCommand(string input)
        {
            Console.WriteLine($"> {input}");
            RunCommand(input);
            Console.WriteLine();
        }

        private void RunCommand(string input)
        {
            var repoField = typeof(GitInterface).GetField("repo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var branchManagerField = typeof(GitInterface).GetField("branchManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var repo = (Repository)repoField.GetValue(this);
            var branchManager = (BranchManager)branchManagerField.GetValue(this);

            if (repo == null || branchManager == null)
            {
                repo = new Repository("TestingRepo");
                branchManager = repo.BranchManager;
                repoField.SetValue(this, repo);
                branchManagerField.SetValue(this, branchManager);
            }

            // Duplicate RunGit's inner logic but using the input string instead of Console.ReadLine.
            if (string.IsNullOrWhiteSpace(input))
                return;

            string[] command = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int commandIndex = 0;

            if (command[commandIndex] != "git")
                return;

            commandIndex++;

            if (commandIndex < command.Length && command[commandIndex] == "help")
            {
                typeof(GitInterface).GetMethod("DisplayGlobalHelp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .Invoke(this, null);
                return;
            }

            if (commandIndex < command.Length && command[commandIndex] == BranchCommands.Identifier)
            {
                commandIndex++;
                if (commandIndex >= command.Length)
                {
                    Console.WriteLine($"No branch command given. Use 'git {BranchCommands.Identifier} help' for branch commands help.");
                    return;
                }
                else if (command[commandIndex] == "help")
                {
                    typeof(GitInterface).GetMethod("DisplayBranchHelp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(this, null);
                }
                else
                {
                    typeof(GitInterface).GetMethod("HandleBranchCommands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(this, new object[] { command, commandIndex });
                }
            }
            else if (commandIndex < command.Length && command[commandIndex] == CommitCommands.Identifier)
            {
                commandIndex++;
                if (commandIndex >= command.Length)
                {
                    Tree commitTree = repo.PrepareCommit();
                    branchManager.GetCurrentBranch().AddCommit(new Commit("No Message was added.", commitTree));
                    branchManager.GetCurrentBranch().GetHead().DisplayTree((FolderNode)commitTree.RootNode);
                }
                else if (command[commandIndex] == "help")
                {
                    typeof(GitInterface).GetMethod("DisplayCommitHelp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(this, null);
                }
                else
                {
                    typeof(GitInterface).GetMethod("HandleCommitCommands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(this, new object[] { command, commandIndex });
                }
            }
            else if (commandIndex < command.Length && command[commandIndex] == RepositoryCommands.Identifier)
            {
                commandIndex++;
                if (commandIndex >= command.Length)
                {
                    Console.WriteLine($"No repository command given. Use 'git {RepositoryCommands.Identifier} help' for repository commands help.");
                }
                else if (command[commandIndex] == "help")
                {
                    typeof(GitInterface).GetMethod("DisplayRepositoryHelp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(this, null);
                }
                else
                {
                    typeof(GitInterface).GetMethod("HandleRepositoryCommands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(this, new object[] { command, commandIndex });
                }
            }
            else
            {
                typeof(GitInterface).GetMethod("DisplayCommandNotFound", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .Invoke(this, new object[] { command, commandIndex });
            }
        }
    }
}