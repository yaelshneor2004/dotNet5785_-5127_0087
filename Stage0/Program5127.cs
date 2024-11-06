namespace Stage0
{

    partial class Program
    {
        public static void Main(string[] args)
        {
            Welcome5127();
            Welcome0087();
            Console.ReadKey();
        }


        private static void Welcome5127()
        {
            Console.WriteLine("Enter your name");
            string name = Console.ReadLine()!;
            Console.WriteLine("{0},welcome to my first console application", name);
        }
       
        static partial void Welcome0087();
    }
}