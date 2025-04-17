using my.utils;
using System.Text.RegularExpressions;

namespace Git_Clone
{
    public class Repository
    {
        public string RepositoryName { get; set; }

        public BranchManager BranchManager { get; set; } = new BranchManager();

        // WorkingDirectory
        public WorkingDirectory WorkingDirectory { get; set; } = new WorkingDirectory();

        // Index
        public List<string> StagedFiles { get; set; } = new List<string>();

        public Index Index { get; set; }

        public Repository(string repositoryName)
        {
            RepositoryName = repositoryName;

            Index = new Index(this);

            BranchManager.Initialize();
        }
        public void ListAllFilesInRepository()
        {
            List<string> files = WorkingDirectory.GetFilesInLocalPath().ToList();

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

            if (File.Exists(fileName))
            {
                Console.WriteLine("Enter the new content for the file:");
                string newContent = Console.ReadLine();
                File.WriteAllText(fileName, newContent);
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

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} does not exist inside the repo.");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"File {fileName} was staged.");
            Console.ResetColor();
            StagedFiles.Add(fileName);
        }
        public List<FileSnapShot> GetAllStagedFiles()
        {
            List<FileSnapShot> changedFiles = Index.CurrentFileSnapShots;
            
            for (int i = 0; i < changedFiles.Count; i++)
            {
                if (StagedFiles.Contains(changedFiles[i].GetFileName()))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.WriteLine(changedFiles[i].GetFileName());
                Console.ResetColor();
            }

            return Index.CurrentFileSnapShots;
        }

        public Tree PrepareCommit()
        {;
            FolderNode root = new FolderNode("Root");
            Tree commitTree = new Tree(root);

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

                Console.WriteLine($"Adding to tree: {relativePath}");

                string absolutePath = Path.Combine(WorkingDirectory.RepositoryDirectory, relativePath);
                commitTree.AddNode(absolutePath, newFileNode, currentFolder);
            }

            return commitTree;
        }
        
        public Dictionary<string, FileStatus> GetAllFileChanges()
        {
            Dictionary<string, FileStatus> _fileChanges = new Dictionary<string, FileStatus>();

            string[] files = WorkingDirectory.GetFilesInLocalPath().ToArray();

            foreach (var file in files)
            {
                bool doesExistsInHead = BranchManager.GetCurrentBranch().GetHead().CommitTree.Nodes.ContainsKey(file);

                if (doesExistsInHead)
                {
                    if (!File.Exists(file))
                    {
                        _fileChanges.Add(file, FileStatus.Deleted);
                        continue;
                    }

                    Diff diff = new Diff();
                    FileNode fileNode = BranchManager.GetCurrentBranch().GetHead().CommitTree.Nodes[file] as FileNode;
                    var diffResult = diff.DiffText(fileNode.Content, File.ReadAllText(file));

                    if (diffResult.Length > 0)
                    {
                        _fileChanges.Add(file, FileStatus.Modified);
                    }
                    else
                    {
                        _fileChanges.Add(file, FileStatus.Unchanged);
                    }
                }
                else
                {
                    _fileChanges.Add(file, FileStatus.Added);
                }
            }

            return _fileChanges;
        }


        public void GetFullPath(string fileName, out string fullPath)
        {
            fullPath = Path.Combine(WorkingDirectory.RepositoryDirectory, $"{fileName}.txt");
        }
    }
    /// <summary>
    /// Currently only holds the repository directory. Represents the current state of the project.
    /// </summary>
    public class WorkingDirectory : BranchState
    {
        // TODO: Make this path something you can set up
        public string RepositoryDirectory { get; set; } =
            "C:\\Projects\\Coding\\GitClone\\Git Clone\\Repos\\TestRepository\\";

        /// <summary>
        /// Gets all the files in the directory inside of its path relative to the directory root folder.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFilesInLocalPath()
        {
            return Directory.EnumerateFiles(RepositoryDirectory, "*.*", SearchOption.AllDirectories)
                                           .Select(path => Path.GetRelativePath(RepositoryDirectory, path));
        }
    }

    public enum FileStatus
    {
        Added,
        Deleted,
        Modified,
        Unchanged
    }
}