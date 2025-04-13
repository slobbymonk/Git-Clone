namespace Git_Clone
{
    public class Commit
    {
        public string Message { get; set; }
        public DateTime CommitDate { get; set; } = DateTime.Now;
        public List<FileSnapShot> FileSnapShots { get; set; } = new List<FileSnapShot>();

        public Commit(string commitTag, List<FileSnapShot> fileSnapShots)
        {
            Message = commitTag;
            FileSnapShots = fileSnapShots;
        }
    }
}