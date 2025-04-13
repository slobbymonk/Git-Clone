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

        public Repository(string repositoryName)
        {
            RepositoryName = repositoryName;
        }

        public void ListAllFilesInRepository()
        {
            string[] files = Directory.GetFiles(WorkingDirectory.RepositoryDirectory, "*.*", SearchOption.AllDirectories);
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

            GetFullPath(fileName, out string fullPath);

            if(!File.Exists(fullPath))
            {
                Console.WriteLine($"File {fileName} does not exist inside the repo.");
                return;
            }

            StagedFiles.Add(fullPath);
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

        public void GetFullPath(string fileName, out string fullPath)
        {
            fullPath = Path.Combine(WorkingDirectory.RepositoryDirectory, $"{fileName}.txt");
        }
    }
    public class Index : BranchState
    {
        public List<FileSnapShot> CurrentFileSnapShots { get; set; } = new List<FileSnapShot>();
    }
    public class WorkingDirectory : BranchState
    {
        public string RepositoryDirectory { get; set; } =
            "C:\\Users\\Séba\\source\\repos\\Git Clone\\Git Clone\\Repos\\TestRepository\\";
    }
}