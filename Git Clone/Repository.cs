using Newtonsoft.Json;

namespace Git_Clone
{
    // TODO: Fix bug where for some reason it thinks things that don't, already exist
    // TODO: Add checking out of previous commits
    // TODO: Add saving commits in localFilePath


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
            BranchManager.LoadAllSavedBranchCommits(WorkingDirectory);
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
                //Console.WriteLine("Local path could not be found, this is StageFileWithName inside of Repository.");
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
                StageFileWithName(fileInLocalPath);
            }
        }

        public Tree PrepareCommit(string commitMessage = "")
        {
            FolderNode root = FolderNode.CreateRoot("Root");
            Tree commitTree = new Tree(root);

            Dictionary<string, FileChangeStatus> changedFiles = Index.GetAllChangedFiles();

            Branch currentBranch = BranchManager.GetCurrentBranch();
            Commit? head = currentBranch.GetHead();
            Tree? _headTree = head?.CommitTree;

            List<string> allFiles = WorkingDirectory.GetFilesInLocalPath().ToList();

            foreach (string localFilePath in allFiles)
            {
                string[] pathParts = localFilePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

                FolderNode currentFolder = root;

                // Iterate through all folders (excluding the last part which is the localFilePath name)
                for (int i = 0; i < pathParts.Length - 1; i++)
                {
                    string folderName = pathParts[i];

                    // Check if this folder path already exists in the tree
                    if (!currentFolder.Children.ContainsKey(folderName))
                    {
                        FolderNode newFolder = FolderNode.Create(folderName, currentFolder);
                        currentFolder.Children.Add(folderName, newFolder);
                        currentFolder = newFolder;
                    }
                    else
                    {
                        currentFolder = (FolderNode)currentFolder.Children[folderName];
                    }

                    // Ensure folder nodes are added correctly to the tree
                    commitTree.AddNode(folderName, currentFolder, root);
                }

                // Handle the localFilePath node
                string fileName = pathParts.Last();
                FileNode? newFileNode = null;

                if (changedFiles.ContainsKey(fileName))  // Did the localFilePath change?
                {
                    if (!StagedFiles.Contains(localFilePath) || changedFiles[fileName] == FileChangeStatus.Deleted)
                        continue;

                    // File is staged and changed, read its content
                    string fullPath = WorkingDirectory.GetFullPathFromRepositoryPath(localFilePath);
                    string content = File.ReadAllText(fullPath);

                    newFileNode = new FileNode(fileName, content, currentFolder, localFilePath);
                    commitTree.AddNode(fileName, newFileNode, currentFolder);
                }
                else
                {
                    if (_headTree != null && _headTree.Nodes.TryGetValue(localFilePath, out INode? existingNode))
                    {
                        commitTree.AddNode(fileName, existingNode, currentFolder);
                    }
                }

                // If the localFilePath was added, make sure it's in the current folder
                if (newFileNode != null && !currentFolder.Children.ContainsKey(fileName))
                {
                    currentFolder.Children.Add(fileName, newFileNode);
                }

                //Console.WriteLine($"Adding to tree: {fileName}");
            }

            // Clear staged files after committing
            StagedFiles.Clear();

            // Build the correct path for saving the commit tree JSON localFilePath
            string jsonSavingPlace = Path.Combine(WorkingDirectory.RepositorySavingFolder, 
                BranchManager.GetCurrentBranch().BranchName + " Branch Commit History", commitMessage + ".json");

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(jsonSavingPlace));

            // Serialize the commit tree to JSON format
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All, // 🔥 key fix here
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // Serializing:
            string json = JsonConvert.SerializeObject(commitTree, settings);

            // Write the JSON to the localFilePath
            File.WriteAllText(jsonSavingPlace, json);

            Console.WriteLine($"Commit done!");

            return commitTree;
        }


        public void GetFullPath(string fileName, out string fullPath)
        {
            fullPath = Path.Combine(WorkingDirectory.RepositoryRootFolder, $"{fileName}.txt");
        }
    }

    public enum FileChangeStatus
    {
        Added,
        Deleted,
        Modified
    }
}