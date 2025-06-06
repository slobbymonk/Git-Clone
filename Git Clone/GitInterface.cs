﻿namespace Git_Clone
{
    public class GitInterface
    {
        private Repository repo;

        public void Begin()
        {
            repo = new Repository("TestingRepo");

            RunGit();
        }
        public void RunTest()
        {
            var git = new GitInterfaceTestable();
            git.Begin();

            // Create and modify the repository files
            Console.WriteLine("Creating files...");
            git.TestCommand("git repo create file1.txt");
            git.TestCommand("git repo create file2.txt");
            git.TestCommand("git repo create file3.txt");
            git.TestCommand("git repo create file4.txt");

            // Modify file2 (but don't stage it)
            Console.WriteLine("Editing file2.txt...");
            git.TestCommand("git repo edit file2.txt");

            // Stage file1
            Console.WriteLine("Staging file1.txt...");
            git.TestCommand("git repo stage file1.txt");

            // Delete file4
            Console.WriteLine("Deleting file4.txt...");
            git.TestCommand("git repo delete file4.txt");


            // Check what files are staged and the commit tree state
            Console.WriteLine("Checking repository state BEFORE commit...");
            git.TestCommand("git repo status"); // List all files in the repository after commit

            // Commit changes
            Console.WriteLine("Committing changes...");
            git.TestCommand("git commit thiswillbeheadlol");

            // Check what files are staged and the commit tree state
            Console.WriteLine("Checking repository state AFTER commit...");
            git.TestCommand("git repo status"); // List all files in the repository after commit
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
                    DisplayCommandNotFound(command, commandIndex);
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
            Console.WriteLine($"  {CommitCommands.ListCommits}              - Lists all commits in the current branch.");
            Console.WriteLine($"  {CommitCommands.Help}                     - Displays this help for commit commands.");
        }

        void DisplayRepositoryHelp()
        {
            Console.WriteLine("Repository Commands Help:");
            Console.WriteLine($"  {RepositoryCommands.Create} [filename]    - Creates a new file in the repository.");
            Console.WriteLine($"  {RepositoryCommands.Edit} [filename]      - Edits an existing file in the repository.");
            Console.WriteLine($"  {RepositoryCommands.Remove} [filename]    - Deletes a file from the repository.");
            Console.WriteLine($"  {RepositoryCommands.ListAllFiles}                - Lists all files in the repository.");
            Console.WriteLine($"  {RepositoryCommands.StageFile} [filename] - Stages the specified file for commit.");
            Console.WriteLine($"  {RepositoryCommands.Help}                - Displays this help for repository commands.");
        }

        #endregion

        void HandleRepositoryCommands(string[] command, int commandIndex)
        {
            if (command[commandIndex] == RepositoryCommands.Create)
            {
                commandIndex++;

                repo.CreateFile(command[commandIndex]);
            }
            else if (command[commandIndex] == RepositoryCommands.ListAllFiles)
            {
                repo.ListAllFilesInRepository();
            }
            else if (command[commandIndex] == RepositoryCommands.Edit)
            {
                commandIndex++;
                repo.EditFile(command[commandIndex]);
            }
            else if (command[commandIndex] == RepositoryCommands.Remove)
            {
                commandIndex++;

                repo.DeleteFile(command[commandIndex]);
            }
            else if (command[commandIndex] == RepositoryCommands.RepoStatus)
            {
                repo.ListAllChanges();
            }
            else if (command[commandIndex] == RepositoryCommands.StageFile)
            {
                commandIndex++;

                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"No files to sage given. Add files to stage or use all to stage all changes.");
                    return;
                }

                if (command[commandIndex] == RepositoryCommands.StageAllFiles)
                    repo.StageAllChanges();
                else
                    repo.StageFileWithName(command[commandIndex]);
            }
            else
            {
                DisplayCommandNotFound(command, commandIndex);
            }
        }

        void HandleCommitCommands(string[] command, int commandIndex)
        {
            if (command[commandIndex] == CommitCommands.ListCommits)
            {
                Branch currentBranch = repo.BranchManager.GetCurrentBranch();
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
                }
            }
            else
            {
                string commitMessage = "";

                for (int i = commandIndex; i < command.Length; i++)
                {
                    if (i == commandIndex)
                    {
                        commitMessage = command[i];
                    }
                    else
                    {
                        commitMessage += " " + command[i];
                    }
                }

                Tree commitTree = repo.PrepareCommit(commitMessage);
                repo.BranchManager.GetCurrentBranch().AddCommit(new Commit($"'{commitMessage}'", commitTree));
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
                repo.BranchManager.CreateBranch(command[commandIndex], "0.0.1");
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
                repo.BranchManager.DeleteBranch(idToDelete);
            }
            else if (command[commandIndex] == BranchCommands.ListBranches)
            {
                repo.BranchManager.ListAllBranchNames();
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
                repo.BranchManager.CloneBranch(branchId);
            }
            else if (command[commandIndex] == BranchCommands.CurrentBranch)
            {
                Console.WriteLine($"You're currently on branch: {repo.BranchManager.CurrentBranchId}");
            }
            else if (command[commandIndex] == BranchCommands.CheckoutBranch)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"No branch id provided. Aborting {BranchCommands.CheckoutBranch}.");
                    return;
                }
                repo.BranchManager.CheckoutBranch(int.Parse(command[commandIndex]));
            }
            else
            {
                DisplayCommandNotFound(command, commandIndex);
            }
        }
        void DisplayCommandNotFound(string[] command, int commandIndex)
        {
            Console.WriteLine($"Command {command[commandIndex - 1]} not recognized. Type 'git help' for a list of available commands.");
            /*if(commandIndex > 1)
            {
                Console.WriteLine($"Command {command[commandIndex]} not recognized as part of " +
                    $"{command[commandIndex -1]}. Type 'git help' for a list of available commands.");
            }
            else
            {
                Console.WriteLine($"Command {command[commandIndex-1]} not recognized. Type 'git help' for a list of available commands.");
            }*/
        }
    }
}