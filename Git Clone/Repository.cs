using System.Text.RegularExpressions;

namespace Git_Clone
{
    public class Repository
    {
        public string RepositoryName { get; set; }
        public List<Branch> Branches { get; set; } = new List<Branch>();

        // WorkingDirectory
        public WorkingDirectory WorkingDirectory { get; set; } = new WorkingDirectory();

        // Index
        public List<string> StagedFiles { get; set; } = new List<string>();

        public Index Index { get; set; } = new Index();

        // Maybe use this repository for a string diff algorithm that runs on O(nD) time complexity
        // https://github.com/kpdecker/jsdiff.git

        public Repository(string repositoryName)
        {
            RepositoryName = repositoryName;
        }
        public string RemoveWhitespace(string input)
        {
            return new string(input
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        public void ListAllFilesInRepository()
        {
            string[] files = Directory.GetFiles(WorkingDirectory.RepositoryDirectory, "*.*", SearchOption.AllDirectories);
            files[0] = RemoveWhitespace(files[0]);
            foreach (string file in files)
            {
                if (StagedFiles.Contains(file))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.WriteLine(file);
                Console.ResetColor();
            }
        }

        #region File Management

        public void EditFile(string? fileName = null)
        {
            if(fileName == null)
            {
                Console.WriteLine("Enter the file name:");
                fileName = Console.ReadLine();
            }

            GetFullPath(fileName,out string fullPath);

            if (File.Exists(fullPath))
            {
                Console.WriteLine("Enter the new content for the file:");
                string newContent = Console.ReadLine();
                File.WriteAllText(fullPath, newContent);
                Console.WriteLine($"File {fileName} has been edited.");
            }
            else
            {
                Console.WriteLine($"File {fileName} does not exist inside the repo.");
            }
        }
        public void CreateFile(string? fileName = null)
        {
            if(fileName == null)
            {
                Console.WriteLine("Enter the file name:");
                fileName = Console.ReadLine();
            }

            if (File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} already exists.");
                return;
            }

            File.Create($"{WorkingDirectory}{fileName}.txt");
        }
        public void DeleteFile(string? fileName = null)
        {
            if (fileName == null)
            {
                Console.WriteLine("Enter the file name:");
                fileName = Console.ReadLine();
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} doesn't exist.");
                return;
            }

            File.Delete($"{WorkingDirectory}{fileName}.txt");
        }

        #endregion

        public void StageFile(string? fileName = null)
        {
            if (fileName == null)
            {
                Console.WriteLine("Enter the file name:");
                fileName = Console.ReadLine();
            }
            if(StagedFiles.Contains(fileName))
            {
                Console.WriteLine($"File {fileName} is already staged.");
                return;
            }

            if(!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} does not exist inside the repo.");
                return;
            }

            StagedFiles.Add(fileName);
        }
        public List<FileSnapShot> GetAllStagedFiles()
        {
            List<FileSnapShot> fileSnapShots = new List<FileSnapShot>();

            foreach (string file in StagedFiles)
            {
                string content = File.ReadAllText(file);
                fileSnapShots.Add(new FileSnapShot(file, content));
            }

            return fileSnapShots;
        }

        public Tree PrepareCommit()
        {
            FolderNode root = new FolderNode("Root");

            string[] files = Directory.GetFiles(WorkingDirectory.RepositoryDirectory, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                if (!StagedFiles.Contains(file))
                    continue;

                // Calculate the relative path from the repository root
                string relativePath = Path.GetRelativePath(WorkingDirectory.RepositoryDirectory, file);
                // Use the OS-specific separator (Path.DirectorySeparatorChar)
                string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

                FolderNode currentFolder = root;

                // Walk through the relative path to build the folder hierarchy
                for (int i = 0; i < pathParts.Length - 1; i++)
                {
                    string folderName = pathParts[i];

                    if (!currentFolder.Children.ContainsKey(folderName))
                    {
                        FolderNode newFolder = new FolderNode(folderName);
                        newFolder.Parent = currentFolder;
                        currentFolder.Children.Add(folderName, newFolder);
                        currentFolder = newFolder;
                    }
                    else
                    {
                        currentFolder = (FolderNode)currentFolder.Children[folderName];
                    }
                }

                // The last segment is the file name
                string fileName = pathParts[^1];
                // Read file content (or just a hash, etc.)
                string content = File.ReadAllText(file);
                FileNode newFileNode = new FileNode(fileName, content);
                newFileNode.Parent = currentFolder;

                if (!currentFolder.Children.ContainsKey(fileName))
                {
                    currentFolder.Children.Add(fileName, newFileNode);
                }
                else
                {
                    Console.WriteLine($"Warning: {fileName} already exists in {currentFolder.Name}.");
                }
            }

            return new Tree(root);
        }


            public void GetFullPath(string fileName, out string fullPath)
        {
            fullPath = Path.Combine(WorkingDirectory.RepositoryDirectory, $"{fileName}.txt");
        }
    }

    /// <summary>
    /// Also known as the staging area. Represents the state of the files that are staged for commit.
    /// </summary>
    public class Index : BranchState
    {
        public List<FileSnapShot> CurrentFileSnapShots { get; set; } = new List<FileSnapShot>();
    }
    /// <summary>
    /// Currently only holds the repository directory. Represents the current state of the project.
    /// </summary>
    public class WorkingDirectory : BranchState
    {
        // TODO: Make this path something you can set up
        public string RepositoryDirectory { get; set; } =
            "C:\\Projects\\Coding\\GitClone\\Git Clone\\Repos\\TestRepository\\";
    }
}