namespace Git_Clone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //GitInterface branchTester = new GitInterface();
            //branchTester.Begin();

            TreeTester treeBuilder = new TreeTester();
            treeBuilder.Begin();
        }
    }
}