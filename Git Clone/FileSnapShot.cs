namespace Git_Clone
{
    public class FileSnapShot
    {
        Dictionary<string, string> fileSnapshot = new();
        
        public FileSnapShot(string fileName, string content)
        {
            fileSnapshot.Add(fileName, content);
        }
    }
}