﻿namespace Git_Clone
{
    public class GitInterface
    {
        BranchManager branchManager = new BranchManager();
        Repository repo = new Repository("TestingRepo");

        public void Begin()
        {
            branchManager.Initialize();
            RunGit();
        }

        public void RunGit()
        {
            Console.Write("> "); // prompt
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                RunGit();
                return;
            }

            string[] command = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int commandIndex = 0;

            if (command[commandIndex] != "git")
                return;

            commandIndex++;

            // Global git help: "git help" prints overall commands
            if (commandIndex < command.Length && command[commandIndex] == "help")
            {
                DisplayGlobalHelp();
                RunGit();
                return;
            }

            // Process subcommands (branch, commit, repository) with improved help messages.
            // Using else-if to avoid overlapping command indices.
            if (commandIndex < command.Length && command[commandIndex] == BranchCommands.Identifier)
            {
                commandIndex++;

                if (commandIndex >= command.Length)
                {
                    Console.WriteLine($"No branch command given. Use 'git {BranchCommands.Identifier} help' for branch commands help.");
                    RunGit();
                    return;
                }
                else if (command[commandIndex] == "help")
                {
                    DisplayBranchHelp();
                }
                else
                {
                    HandleBranchCommands(command, commandIndex);
                }
            }
            else if (commandIndex < command.Length && command[commandIndex] == CommitCommands.Identifier)
            {
                commandIndex++;

                if (commandIndex >= command.Length)
                {
                    // Default commit if no further parameter is given.
                    Tree commitTree = repo.PrepareCommit();
                    branchManager.GetCurrentBranch().AddCommit(new Commit("No Message was added.", commitTree));
                    branchManager.GetCurrentBranch().GetHead().DisplayTree((FolderNode)commitTree.RootNode);
                }
                else if (command[commandIndex] == "help")
                {
                    DisplayCommitHelp();
                }
                else
                {
                    HandleCommitCommands(command, commandIndex);
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
                    DisplayRepositoryHelp();
                }
                else
                {
                    HandleRepositoryCommands(command, commandIndex);
                }
            }
            else
            {
                DisplayCommandNotFound(command, commandIndex);
            }

            Console.WriteLine();
            Console.WriteLine("---------new recursive----------");
            Console.WriteLine();

            RunGit();
        }

        #region Help Display Methods

        void DisplayGlobalHelp()
        {
            Console.WriteLine("Global git commands:");
            Console.WriteLine($"  git help                         - Displays this help message.");
            Console.WriteLine($"  git {BranchCommands.Identifier} help                  - Displays available branch commands.");
            Console.WriteLine($"  git {CommitCommands.Identifier} help                  - Displays available commit commands.");
            Console.WriteLine($"  git {RepositoryCommands.Identifier} help              - Displays available repository commands.");
        }

        void DisplayBranchHelp()
        {
            Console.WriteLine("Branch Commands Help:");
            Console.WriteLine($"  {BranchCommands.CreateBranch} [name]      - Creates a new branch with the given name.");
            Console.WriteLine($"  {BranchCommands.DeleteBranch} [id]        - Deletes the branch with the specified id.");
            Console.WriteLine($"  {BranchCommands.CloneBranch} [id]         - Clones the branch with the specified id.");
            Console.WriteLine($"  {BranchCommands.ListBranches}             - Lists all existing branches.");
            Console.WriteLine($"  {BranchCommands.CurrentBranch}            - Displays the current branch.");
            Console.WriteLine($"  {BranchCommands.CheckoutBranch} [id]      - Checks out the branch with the specified id.");
            Console.WriteLine($"  {BranchCommands.Help}                     - Displays this help for branch commands.");
        }

        void DisplayCommitHelp()
        {
            Console.WriteLine("Commit Commands Help:");
            Console.WriteLine($"  {CommitCommands.AddMessage} [message]     - Creates a new commit with the given message.");
            Console.WriteLine($"  {CommitCommands.ListCommits}              - Lists all commits in the current branch.");
            Console.WriteLine($"  {CommitCommands.Help}                     - Displays this help for commit commands.");
        }

        void DisplayRepositoryHelp()
        {
            Console.WriteLine("Repository Commands Help:");
            Console.WriteLine($"  {RepositoryCommands.Create} [filename]    - Creates a new file in the repository.");
            Console.WriteLine($"  {RepositoryCommands.Edit} [filename]      - Edits an existing file in the repository.");
            Console.WriteLine($"  {RepositoryCommands.Delete} [filename]    - Deletes a file from the repository.");
            Console.WriteLine($"  {RepositoryCommands.List}                - Lists all files in the repository.");
            Console.WriteLine($"  {RepositoryCommands.StageFile} [filename] - Stages the specified file for commit.");
            Console.WriteLine($"  {RepositoryCommands.Help}                - Displays this help for repository commands.");
        }

        #endregion

        void HandleRepositoryCommands(string[] command, int commandIndex)
        {
            if (command[commandIndex] == RepositoryCommands.Create)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                    repo.CreateFile(null);
                else
                    repo.CreateFile(command[commandIndex]);
            }
            else if (command[commandIndex] == RepositoryCommands.Edit)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                    repo.EditFile(null);
                else
                    repo.EditFile(command[commandIndex]);
            }
            else if (command[commandIndex] == RepositoryCommands.Delete)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                    repo.DeleteFile(null);
                else
                    repo.DeleteFile(command[commandIndex]);
            }
            else if (command[commandIndex] == RepositoryCommands.List)
            {
                repo.ListAllFilesInRepository();
            }
            else if (command[commandIndex] == RepositoryCommands.StageFile)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                    repo.StageFile(null);
                else
                    repo.StageFile(command[commandIndex]);
            }
            else
            {
                DisplayCommandNotFound(command, commandIndex);
            }
        }

        void HandleCommitCommands(string[] command, int commandIndex)
        {
            if (command.Length <= commandIndex)
            {
                Tree commitTree = repo.PrepareCommit();
                branchManager.GetCurrentBranch().AddCommit(new Commit("No Message was added.", commitTree));
                branchManager.GetCurrentBranch().GetHead().DisplayTree((FolderNode)commitTree.RootNode);
                return;
            }

            if (command[commandIndex] == CommitCommands.AddMessage)
            {
                commandIndex++;
                Tree commitTree = repo.PrepareCommit();
                branchManager.GetCurrentBranch().AddCommit(new Commit(command[commandIndex], commitTree));
                branchManager.GetCurrentBranch().GetHead().DisplayTree((FolderNode)commitTree.RootNode);
            }
            else if (command[commandIndex] == CommitCommands.ListCommits)
            {
                Branch currentBranch = branchManager.GetCurrentBranch();
                if (currentBranch == null)
                {
                    Console.WriteLine("No current branch found.");
                    return;
                }
                foreach (var commit in currentBranch.BranchCommits)
                {
                    Console.WriteLine($"Commit Message: {commit.Message}");
                    Console.WriteLine($"Commit Date: {commit.CommitDate}");
                    Console.WriteLine("Commit Information: ");
                    commit.DisplayTree(commit.CommitTree.RootNode as FolderNode);
                }
            }
            else
            {
                DisplayCommandNotFound(command, commandIndex);
            }
        }

        void HandleBranchCommands(string[] command, int commandIndex)
        {
            if (command[commandIndex] == BranchCommands.CreateBranch)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"No branch name provided. Aborting {BranchCommands.CreateBranch}.");
                    return;
                }
                branchManager.CreateBranch(command[commandIndex], "0.0.1");
            }
            else if (command[commandIndex] == BranchCommands.DeleteBranch)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"No branch id provided. Aborting {BranchCommands.DeleteBranch}.");
                    return;
                }
                if (!int.TryParse(command[commandIndex], out int idToDelete))
                {
                    Console.WriteLine("Invalid branch id provided. Aborting deletion.");
                    return;
                }
                branchManager.DeleteBranch(idToDelete);
            }
            else if (command[commandIndex] == BranchCommands.ListBranches)
            {
                branchManager.ListAllBranchNames();
            }
            else if (command[commandIndex] == BranchCommands.CloneBranch)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"No branch id provided. Aborting {BranchCommands.CloneBranch}.");
                    return;
                }
                if (!int.TryParse(command[commandIndex], out int branchId))
                {
                    Console.WriteLine("Invalid branch id provided. Aborting cloning.");
                    return;
                }
                branchManager.CloneBranch(branchId);
            }
            else if (command[commandIndex] == BranchCommands.CurrentBranch)
            {
                Console.WriteLine($"You're currently on branch: {branchManager.CurrentBranchId}");
            }
            else if (command[commandIndex] == BranchCommands.CheckoutBranch)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"No branch id provided. Aborting {BranchCommands.CheckoutBranch}.");
                    return;
                }
                branchManager.CheckoutBranch(int.Parse(command[commandIndex]));
            }
            else
            {
                DisplayCommandNotFound(command, commandIndex);
            }
        }
        void DisplayCommandNotFound(string[] command, int commandIndex)
        {
            if(commandIndex > 1)
            {
                Console.WriteLine($"Command {command[commandIndex]} not recognized as part of " +
                    $"{command[commandIndex -1]}. Type 'git help' for a list of available commands.");
            }
            else
            {
                Console.WriteLine($"Command {command[commandIndex-1]} not recognized. Type 'git help' for a list of available commands.");
            }
        }
    }
}