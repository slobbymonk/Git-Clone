using my.utils;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

        public void StageFile(string fileName)
        {

            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            string fileInLocalPath = WorkingDirectory.GetLocalPathName(fileName);
            string fileInDirectory = WorkingDirectory.GetFullPathFromRepositoryPath(fileInLocalPath);

            if (StagedFiles.Contains(fileInLocalPath))
            {
                Console.WriteLine($"File {fileName} is already staged.");
                return;
            }

            if (!File.Exists(fileInDirectory))
            {
                Console.WriteLine($"File {fileInLocalPath} does not exist inside the repo.");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"File {fileName} was staged.");
            Console.ResetColor();
            StagedFiles.Add(fileInLocalPath);
        }
        
        public Tree PrepareCommit()
        {
            FolderNode root = new FolderNode("Root");
            Tree commitTree = new Tree(root);

            foreach (string file in StagedFiles)
            {
                string[] pathParts = file.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

                FolderNode currentFolder = root;

                // Iterate through all folders (exclude the last part — the file name)
                for (int i = 0; i < pathParts.Length - 1; i++)
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
                }

                // The last part is the actual file
                string fileName = pathParts[^1];

                string fullPath = WorkingDirectory.GetFullPathFromRepositoryPath(file);
                string content = File.ReadAllText(fullPath);
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

                Console.WriteLine($"Adding to tree: {file}");

                commitTree.AddNode(file, newFileNode, currentFolder);
            }

            StagedFiles.Clear();
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