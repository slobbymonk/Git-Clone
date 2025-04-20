using Newtonsoft.Json;

namespace Git_Clone
{
    // TODO: Fix bug where for some reason it thinks things that don't, already exist
    // TODO: Add checking out of previous commits
    // TODO: Add saving commits in file


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
                Console.WriteLine(file);
            }
        }

        public void ListAllChanges()
        {
            Dictionary<string, FileChangeStatus> changedFiles = Index.GetAllChangedFiles();

            Console.WriteLine("Staged files:");
            foreach (var file in StagedFiles)
            {
                FileChangeStatus status = changedFiles[file];

                switch (status)
                {
                    case FileChangeStatus.Added:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case FileChangeStatus.Modified:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case FileChangeStatus.Deleted:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
                Console.WriteLine($"File {file} has been {status}.");
                Console.ResetColor();

                changedFiles.Remove(file);
            }

            Console.WriteLine("UnStaged files:");
            foreach (var file in changedFiles.Keys)
            {
                FileChangeStatus status = changedFiles[file];

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"File {file} has been {status}.");
                Console.ResetColor();
            }
        }

        #region File Management

        public void EditFile(string fileName)
        {
            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            string fullPath = WorkingDirectory.GetFullPathFromRepositoryPath(fileName);
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
        public void CreateFile(string fileName)
        {
            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            if (File.Exists(WorkingDirectory.GetFullPathFromRepositoryPath(fileName)))
            {
                Console.WriteLine($"File {fileName} already exists.");
                return;
            }

            File.Create($"{WorkingDirectory.GetFullPathFromRepositoryPath(fileName)}");
            Console.WriteLine($"File {fileName} was added.");
        }
        public void DeleteFile(string fileName)
        {
            if(!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            string fullPath = WorkingDirectory.GetFullPathFromName(fileName);

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"File {fileName} doesn't exist.");
                return;
            }

            File.Delete(fullPath);
            Console.WriteLine($"File {fileName} was deleted.");
        }

        #endregion

        public void StageFileWithName(string fileName)
        {
            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            if (StagedFiles.Contains(fileName))
            {
                Console.WriteLine($"{fileName} was already staged.");
                return;
            }

            string? fileInLocalPath = WorkingDirectory.GetLocalPathName(fileName);

            if(fileInLocalPath == null)
            {
                Console.WriteLine("Local path could not be found, this is StageFileWithName inside of Repository.");
                return;
            }

            StageFileInternal(fileName, fileInLocalPath);
        }

        public void StageFileByLocalPath(string fileInLocalPath)
        {
            StageFileInternal(null, fileInLocalPath);
        }

        private void StageFileInternal(string? fileName, string fileInLocalPath)
        {
            if (StagedFiles.Contains(fileInLocalPath))
                return;

            string fileInDirectory = WorkingDirectory.GetFullPathFromRepositoryPath(fileInLocalPath);

            if (!File.Exists(fileInDirectory))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File {fileInLocalPath} does not exist inside the repo.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"File {fileName ?? fileInLocalPath} was staged.");
            Console.ResetColor();

            StagedFiles.Add(fileInLocalPath);
        }

        public void StageAllChanges()
        {
            Dictionary<string, FileChangeStatus> changedFilesInLocalPath = Index.GetAllChangedFiles();

            foreach (var fileInLocalPath in changedFilesInLocalPath.Keys)
            {
                StageFileByLocalPath(fileInLocalPath);
            }
        }

        public Tree PrepareCommit()
        {
            FolderNode root = new FolderNode("Root");
            Tree commitTree = new Tree(root);

            Dictionary<string, FileChangeStatus> changedFiles = Index.GetAllChangedFiles();

            Branch currentBranch = BranchManager.GetCurrentBranch();
            Commit? head = currentBranch.GetHead();
            Tree? _headTree = head?.CommitTree;

            List<string> allFiles = WorkingDirectory.GetFilesInLocalPath().ToList();

            foreach (string file in allFiles)
            {
                string[] pathParts = file.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

                FolderNode currentFolder = root;

                // Iterate through all folders (exclude the last part aka the file name)
                string currentFolderPath = string.Empty;
                int loopStartingValue = 0;
                for (int i = loopStartingValue; i < pathParts.Length - 1; i++)
                {
                    string folderName = pathParts[i];

                    if (!currentFolder.Children.TryGetValue(folderName, out INode? existingNode))
                    {
                        FolderNode newFolder = new FolderNode(folderName);
                        newFolder.Parent = currentFolder;
                        currentFolder.Children.Add(folderName, newFolder);
                        currentFolder = newFolder;
                    }
                    else
                    {
                        currentFolder = (FolderNode)existingNode;
                    }
                    currentFolderPath += pathParts[i] + Path.DirectorySeparatorChar;
                    FolderNode parentFolder = (i == loopStartingValue) ? root : 
                        (FolderNode)commitTree.Nodes[pathParts[i - 1] + Path.DirectorySeparatorChar];
                    commitTree.AddNode(currentFolderPath, currentFolder, parentFolder);
                }

                FileNode? newFileNode = null;

                if (changedFiles.ContainsKey(file)) // Did file change?
                {
                    if (!StagedFiles.Contains(file) || changedFiles[file] == FileChangeStatus.Deleted) // Ok but did you stage it huh?
                        continue;

                    // Oh you did, alright then let's make a new node
                    string fullPath = WorkingDirectory.GetFullPathFromRepositoryPath(file);
                    string content = File.ReadAllText(fullPath);

                    newFileNode = new FileNode(file, content);
                    commitTree.AddNode(file, newFileNode, currentFolder);
                }
                else
                {
                    if(_headTree != null && _headTree.Nodes.TryGetValue(file, out INode? existingNode))
                    {
                        commitTree.AddNode(file, existingNode, currentFolder);
                    }
                }

                if (newFileNode == null)
                    continue;
                
                newFileNode.Parent = currentFolder;

                if (!currentFolder.Children.ContainsKey(file))
                {
                    currentFolder.Children.Add(file, newFileNode);
                }
                else
                {
                    Console.WriteLine($"Warning: {file} already exists in {currentFolder.Name}.");
                }

                Console.WriteLine($"Adding to tree: {file}");

                commitTree.AddNode(file, newFileNode, currentFolder);
            }

            StagedFiles.Clear();

            string json = JsonConvert.SerializeObject(commitTree.RootNode, Formatting.Indented);
            File.WriteAllText("C:\\Games" + "\\" + "tree.json", json);


            return commitTree;
        }

        public void GetFullPath(string fileName, out string fullPath)
        {
            fullPath = Path.Combine(WorkingDirectory.RepositoryRootFolder, $"{fileName}.txt");
        }
    }
    /// <summary>
    /// Currently only holds the repository directory. Represents the current state of the project.
    /// </summary>
    public class WorkingDirectory : BranchState
    {
        // TODO: Make this path something you can set up
        public string RepositoryRootFolder { get; set; } =
            "C:\\Projects\\Coding\\GitClone\\Git Clone\\Repos\\TestRepository\\";

        /// <summary>
        /// Gets all the files in the directory inside of its path relative to the directory root folder.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFilesInLocalPath()
        {
            return Directory.EnumerateFiles(RepositoryRootFolder, "*.*", SearchOption.AllDirectories)
                                           .Select(path => Path.GetRelativePath(RepositoryRootFolder, path));
        }
        public string? GetLocalPathName(string fileName)
        {
            // Search for any file that has the given file name (case-insensitive)
            string? fullPath = Directory.EnumerateFiles(RepositoryRootFolder, "*.*", SearchOption.AllDirectories)
                .FirstOrDefault(path => Path.GetFileName(path).Equals(fileName, StringComparison.OrdinalIgnoreCase));

            if (fullPath == null)
            {
                // File not found in the repo
                return null;
            }

            // Return the path relative to the repository root
            return Path.GetRelativePath(RepositoryRootFolder, fullPath);
        }
        public string GetFullPathFromRepositoryPath(string fileName)
        {
            return Path.Combine(RepositoryRootFolder, fileName);
        }
        public string GetFullPathFromName(string fileName)
        {
            if(fileName == null)
            {
                Console.WriteLine("File name cannot be null.");
                return string.Empty;
            }
            string localPathName = GetLocalPathName(fileName);
            string value = GetFullPathFromRepositoryPath(localPathName);
            return value;
        }

    }

    public enum FileChangeStatus
    {
        Added,
        Deleted,
        Modified
    }
}