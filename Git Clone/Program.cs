namespace Git_Clone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Repository repo = new Repository("Test Repo");
            //repo.PrintAllFilesInRepository();
            GitInterface branchTester = new GitInterface();
            branchTester.Begin();
        }
    }
}