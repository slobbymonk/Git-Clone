namespace Git_Clone
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
            string input = Console.ReadLine();

            string[] command = input.Split(' ');

            int commandIndex = 0;

            if (command[commandIndex] != "git")
                return;

            commandIndex++;

            if (command[commandIndex] == BranchCommands.BranchCommandsIdentifier)
            {
                commandIndex++;

                if(command.Length <= commandIndex)
                {
                    Console.WriteLine($"You didn't give a command for the {BranchCommands.BranchCommandsIdentifier}. That's fucked up.");
                }
                else
                {
                    HandleBranchCommands(command, commandIndex);
                }
            }
            if (command[commandIndex] == CommitCommands.CommitCommandsIdentifier)
            {
                commandIndex++;

                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"You didn't give a command for the {CommitCommands.CommitCommandsIdentifier}." +
                        $" How am I supposed to do anything now huh.");
                }
                else
                {
                    HandleCommitCommands(command, commandIndex);
                }

            }
            if(command[commandIndex] == RepositoryCommands.Identifier)
            {
                commandIndex++;

                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"You didn't give a command for the {RepositoryCommands.Identifier}. Repo these balls.");
                }
                else
                {
                    HandleBranchCommands(command, commandIndex);
                }
            }

            Console.WriteLine($"");
            Console.WriteLine($"---------new recursive----------");
            Console.WriteLine($"");

            RunGit();
        }
        void HandleRepositoryCommands(string[] command, int commandIndex)
        {
            if (command[commandIndex] == RepositoryCommands.Create)
            {
                commandIndex++;

                if(command.Length <= commandIndex)
                {
                    repo.CreateFile(null);
                    return;
                }
                else
                {
                    repo.CreateFile(command[commandIndex]);
                }
            }else if (command[commandIndex] == RepositoryCommands.Edit)
            {
                commandIndex++;

                if (command.Length <= commandIndex)
                {
                    repo.EditFile(null);
                    return;
                }
                else
                {
                    repo.EditFile(command[commandIndex]);
                }
            }
            else if (command[commandIndex] == RepositoryCommands.Delete)
            {
                commandIndex++;

                if (command.Length <= commandIndex)
                {
                    repo.DeleteFile(null);
                    return;
                }
                else
                {
                    repo.DeleteFile(command[commandIndex]);
                }
            }
            else if (command[commandIndex] == RepositoryCommands.List)
            {
                commandIndex++;

                repo.ListAllFilesInRepository();
            }
            else if (command[commandIndex] == RepositoryCommands.StageFile)
            {
                commandIndex++;
                if (command.Length <= commandIndex)
                {
                    repo.StageFile(null);
                    return;
                }
                else
                {
                    repo.StageFile(command[commandIndex]);
                }
            }
        }
        void HandleCommitCommands(string[] command, int commandIndex)
        {
            if(command.Length <= commandIndex)
            {
                branchManager.GetCurrentBranch()
                    .AddCommit(new Commit("No Message was added.", repo.GetAllStagedFiles()));
                return;
            }

            if (command[commandIndex] == CommitCommands.AddMessage)
            {
                commandIndex++;
                branchManager.GetCurrentBranch().AddCommit(new Commit(command[commandIndex], repo.GetAllStagedFiles()));
            }
            else if (command[commandIndex] == CommitCommands.Help)
            {
                Console.WriteLine("Available commands:");
                Console.WriteLine("list");
                Console.WriteLine("help");
            }
            else if (command[commandIndex] == CommitCommands.ListCommits)
            {
                Branch currentBranch = branchManager.GetCurrentBranch();

                if(currentBranch == null)
                {
                    Console.WriteLine("No current branch found.");
                    return;
                }

                foreach (var commit in currentBranch.BranchCommits)
                {
                    Console.WriteLine($"Commit Message: {commit.Message}");
                    Console.WriteLine($"Commit Date: {commit.CommitDate}");
                    Console.WriteLine($"Commit Information: {commit.FileSnapShots}");
                }
            }
        }
        void HandleBranchCommands(string[] command, int commandIndex)
        {
            if (command[commandIndex] == BranchCommands.CreateBranch)
            {
                commandIndex++;

                if(command.Length <= commandIndex)
                {
                    Console.WriteLine($"You didn't give a name for the branch. Aborting {BranchCommands.CreateBranch}.");
                    return;
                }

                branchManager.CreateBranch(command[commandIndex], "0.0.1");
            }
            else if (command[commandIndex] == BranchCommands.DeleteBranch)
            {
                commandIndex++;

                if (command.Length <= commandIndex)
                {
                    Console.WriteLine($"You didn't give a name for the branch. Aborting {BranchCommands.DeleteBranch}.");
                    return;
                }

                if (!int.TryParse(command[commandIndex], out int idToDelete))
                {
                    Console.WriteLine($"You didn't give a valid integer bruhhh I'm gonna have to cancel the deletion now.");
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
                    Console.WriteLine($"You didn't give a name for the branch. " +
                        $"Aborting {BranchCommands.CloneBranch}, the sheep are gonna be mad.");
                    return;
                }

                if (!int.TryParse(command[commandIndex], out int branchId))
                {
                    Console.WriteLine($"You didn't give an integer *crash*");
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
                    Console.WriteLine($"You didn't give a name for the branch. Aborting {BranchCommands.CheckoutBranch}.");
                    return;
                }

                branchManager.CheckoutBranch(int.Parse(command[commandIndex]));
            }
            else if (command[commandIndex] == BranchCommands.Help)
            {
                Console.WriteLine("Available commands:");
                Console.WriteLine("create [name]");
                Console.WriteLine("delete [id]");
                Console.WriteLine("clone [id]");
                Console.WriteLine("list");
                Console.WriteLine("help");
            }
        }
        public void DisplayBranchInformation(Branch branch)
        {
            Console.WriteLine($"Branch Name: {branch.BranchName}");
            Console.WriteLine($"Branch Tag: {branch.BranchTag}");
        }
    }
}