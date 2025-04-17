namespace Git_Clone
{
    using Microsoft.VisualBasic;
    using my.utils;
    using static my.utils.Diff;

    /// <summary>
    /// Also known as the staging area. Represents the state of the changedFiles that are staged for commit.
    /// </summary>
    public class Index : BranchState
    {
        public List<FileSnapShot> ChangedFiles { get; set; } = new List<FileSnapShot>();

        public Repository Repository { get; set; }

        public WorkingDirectory WorkingDirectory { get; set; } = new WorkingDirectory();

        public Index(Repository repository)
        {
            Repository = repository;
        }

        public void ListAllChanges()
        {
            Dictionary<string, FileChangeStatus> changedFiles = GetAllChangedFiles();

            foreach (var file in changedFiles.Keys)
            {
                FileChangeStatus status = changedFiles[file];

                switch (status)
                {
                    case FileChangeStatus.Added:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"File {file} has been added.");
                        break;
                    case FileChangeStatus.Modified:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"File {file} has been modified.");
                        break;
                    case FileChangeStatus.Deleted:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"File {file} has been deleted.");
                        break;
                }
                Console.ResetColor();
            }
        }

        public Dictionary<string, FileChangeStatus> GetAllChangedFiles()
        {
            Dictionary<string, FileChangeStatus> _fileChanges = new Dictionary<string, FileChangeStatus>();
            Diff diff = new Diff();

            string[] files = WorkingDirectory.GetFilesInLocalPath().ToArray();
            Commit HEAD = Repository.BranchManager.GetCurrentBranch().GetHead();

            HashSet<string> _headFiles = new HashSet<string>(HEAD.CommitTree.Nodes.Keys);

            if (HEAD == null)
            {
                Console.WriteLine("No HEAD found. Commit before being able to get the HEAD.");
                return _fileChanges;
            }

            foreach (var file in files)
            {
                bool doesExistsInHead = HEAD.CommitTree.Nodes.ContainsKey(file);

                if (doesExistsInHead)
                {
                    string fullPath = WorkingDirectory.GetFullPathFromRepositoryPath(file);

                    FileNode fileNode = (FileNode)HEAD.CommitTree.Nodes[file];
                    string currentFileContent = File.ReadAllText(fullPath);
                    var diffResult = diff.DiffText(fileNode.Content, currentFileContent);

                    if (diffResult.Length > 0)
                    {
                        _fileChanges.Add(file, FileChangeStatus.Modified);
                    }
                }
                else
                {
                    _fileChanges.Add(file, FileChangeStatus.Added);
                }
                if (_headFiles.Contains(file))
                {
                    _headFiles.Remove(file);
                }
            }
            foreach (var deletedChange in _headFiles)
            {
                _fileChanges.Add(deletedChange, FileChangeStatus.Deleted);
            }

            return _fileChanges;
        }
    }
}