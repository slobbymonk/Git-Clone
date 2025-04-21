namespace Git_Clone
{
    /// <summary>
    /// Currently only holds the repository directory. Represents the current state of the project.
    /// </summary>
    public class WorkingDirectory : BranchState
    {
        // TODO: Make this path something you can set up
        public string RepositoryRootFolder { get; set; } =
            "C:\\Projects\\Coding\\GitClone\\Git Clone\\Repos\\TestRepository\\";
        public string RepositorySavingFolder { get; set; } =
            "C:\\Projects\\Coding\\GitClone\\Git Clone\\Repos\\Saves\\";

        /// <summary>
        /// Gets all the files in the directory inside of its path relative to the directory root folder.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFilesInLocalPath()
        {
            return Directory.EnumerateFiles(RepositoryRootFolder, "*.*", SearchOption.AllDirectories)
                                           .Select(path => Path.GetRelativePath(RepositoryRootFolder, path));
        }
        public IEnumerable<string> GetFileNamesInLocalPath()
        {
            return Directory.EnumerateFiles(RepositoryRootFolder, "*.*", SearchOption.AllDirectories)
                            .Select(path => Path.GetFileName(path));
        }

        public IEnumerable<(string Name, string RelativePath)> GetFilesInfo()
        {
            return Directory
              .EnumerateFiles(RepositoryRootFolder, "*.*", SearchOption.AllDirectories)
              .Select(path => (
                 Name: Path.GetFileName(path),
                 RelativePath: Path.GetRelativePath(RepositoryRootFolder, path)
              ));
        }
        public string? GetLocalPathName(string fileName)
        {
            // Search for any localFilePath that has the given localFilePath name (case-insensitive)
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
}