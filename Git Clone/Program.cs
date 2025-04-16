using my.utils;
using static my.utils.Diff;

namespace Git_Clone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GitInterface branchTester = new GitInterface();
            branchTester.RunTest();

            //ChangesTest();
        }

        public static void ChangesTest()
        {

            Diff diff = new Diff();

            string file1 = "Hello World";
            string file2 = "Well Hello World!";

            string[] linesA = file1.Replace("\r", "").Split('\n');
            string[] linesB = file2.Replace("\r", "").Split('\n');

            Item[] lines = diff.DiffText(file1, file2);

            bool hasChange = lines.Length > 0;
            Console.WriteLine($"Has change: {hasChange}");

            foreach (var line in lines)
            {
                Console.WriteLine($"Change at old A[{line.StartA}] (deleted {line.deletedA}) and new B[{line.StartB}] (inserted {line.insertedB})");

                if (line.deletedA > 0)
                {
                    Console.WriteLine("Deleted lines:");
                    for (int i = 0; i < line.deletedA; i++)
                    {
                        Console.WriteLine($"- {linesA[line.StartA + i]}");
                    }
                }

                if (line.insertedB > 0)
                {
                    Console.WriteLine("Inserted lines:");
                    for (int i = 0; i < line.insertedB; i++)
                    {
                        Console.WriteLine($"+ {linesB[line.StartB + i]}");
                    }
                }
            }
        }
    }
}