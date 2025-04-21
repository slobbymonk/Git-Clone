namespace Git_Clone
{
    using my.utils;

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

        public Dictionary<string, FileChangeStatus> GetAllChangedFiles()
        {
            var fileChanges = new Dictionary<string, FileChangeStatus>();
            var diff = new Diff();

            var workingFiles = WorkingDirectory.GetFilesInfo().ToArray();

            Commit? HEAD = Repository.BranchManager.GetCurrentBranch().GetHead();

            

            // if no HEAD yet, everything is added amigo and this is a clean repo (that's short for repository btw,
            // I'm just using slang)
            if (HEAD == null)
            {
                foreach (var workingFile in workingFiles)
                    fileChanges[workingFile.Name] = FileChangeStatus.Added;
                return fileChanges;
            }

            var headFiles = new HashSet<string>(
    HEAD.CommitTree
        .Nodes
        .Where(kvp => kvp.Value is FileNode)
        .Select(kvp => kvp.Key)
);

            // compare working dir vs HEAD
            foreach (var workingFile in workingFiles)
            {
                if (headFiles.Contains(workingFile.Name))
                {
                    // tracked in HEAD
                    var fileNode = (FileNode)HEAD.CommitTree.Nodes[workingFile.Name];
                    var currentContent = File.ReadAllText(
                      WorkingDirectory.GetFullPathFromRepositoryPath(workingFile.RelativePath)
                    );
                    var d = diff.DiffText(fileNode.Content, currentContent);
                    if (d.Length > 0)
                        fileChanges[workingFile.Name] = FileChangeStatus.Modified;
                }
                else
                {
                    fileChanges[workingFile.Name] = FileChangeStatus.Added;
                }

                // remove from headFiles so leftover = deleted
                headFiles.Remove(workingFile.Name);
            }

            // anything still in headFiles must have been deleted
            foreach (var deleted in headFiles)
                fileChanges[deleted] = FileChangeStatus.Deleted;

            return fileChanges;
        }

    }
}